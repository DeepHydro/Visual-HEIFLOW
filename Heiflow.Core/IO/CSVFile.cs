//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;


#if !NET_3_5
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
#endif

namespace Heiflow.Core.IO
{
    public class CsvFile : IDisposable
    {
        #region Static Members
        public static CsvDefinition DefaultCsvDefinition { get; set; }
        public static bool UseLambdas { get; set; }
        public static bool UseTasks { get; set; }
        public static bool FastIndexOfAny { get; set; }

        static CsvFile()
        {
            // Choosing default Field Separator is a hard decision
            // In theory the C of CSV means Comma separated 
            // In Windows though many people will try to open the csv with Excel which is horrid with real CSV.
            // As the target for this library is Windows we go for Semi Colon.
            DefaultCsvDefinition = new CsvDefinition
            {
                EndOfLine = "\r\n",
                FieldSeparator = ';',
                TextQualifier = '"'
            };
            UseLambdas = true;
            UseTasks = true;
            FastIndexOfAny = true;
        }

        #endregion

        internal protected Stream BaseStream;
        protected static DateTime DateTimeZero = new DateTime();


        public static IEnumerable<T> Read<T>(CsvSource csvSource) where T : new()
        {
            var csvFileReader = new CsvFileReader<T>(csvSource);
            return (IEnumerable<T>)csvFileReader;
        }

        public char FieldSeparator { get; private set; }
        public char TextQualifier { get; private set; }
        public IEnumerable<String> Columns { get; private set; }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            // overriden in derived classes
        }
    }

    public class CsvFile<T> : CsvFile
    {
        private readonly char fieldSeparator;
        private readonly string fieldSeparatorAsString;
        private readonly char[] invalidCharsInFields;
        private readonly StreamWriter streamWriter;
        private readonly char textQualifier;
        private readonly String[] columns;
        private Func<T, object>[] getters;
        readonly bool[] isInvalidCharInFields;
#if !NET_3_5
        private int linesToWrite;
        private readonly BlockingCollection<string> csvLinesToWrite = new BlockingCollection<string>(5000);
        private readonly Thread writeCsvLinesTask;
        private Task addAsyncTask;
#endif

        public CsvFile(CsvDestination csvDestination)
            : this(csvDestination, null)
        {
        }

        public CsvFile(CsvDestination csvDestination, CsvDefinition csvDefinition)
        {
            if (csvDefinition == null)
                csvDefinition = DefaultCsvDefinition;
            this.columns = (csvDefinition.Columns ?? InferColumns(typeof(T))).ToArray();
            this.fieldSeparator = csvDefinition.FieldSeparator;
            this.fieldSeparatorAsString = this.fieldSeparator.ToString(CultureInfo.InvariantCulture);
            this.textQualifier = csvDefinition.TextQualifier;
            this.streamWriter = csvDestination.StreamWriter;

            this.invalidCharsInFields = new[] { '\r', '\n', this.textQualifier, this.fieldSeparator };
            this.isInvalidCharInFields = new bool[256];

            foreach (var c in this.invalidCharsInFields)
            {
                this.isInvalidCharInFields[c] = true;
            }
            this.WriteHeader();

            this.CreateGetters();
#if !NET_3_5
            if (CsvFile.UseTasks)
            {
                writeCsvLinesTask = new Thread((o) => this.WriteCsvLines());
                writeCsvLinesTask.Start();
            }
            this.addAsyncTask = Task.Factory.StartNew(() => { });

#endif

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
#if !NET_3_5
                addAsyncTask.Wait();
                if (csvLinesToWrite != null)
                {
                    csvLinesToWrite.CompleteAdding();
                }
                if (writeCsvLinesTask != null)
                    writeCsvLinesTask.Join();
#endif
                this.streamWriter.Close();
            }
        }

        protected static IEnumerable<string> InferColumns(Type recordType)
        {
            var columns = recordType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.GetIndexParameters().Length == 0
                    && pi.GetSetMethod() != null
                    && !Attribute.IsDefined(pi, typeof(CsvIgnorePropertyAttribute)))
                .Select(pi => pi.Name)
                .Concat(recordType
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(fi => !Attribute.IsDefined(fi, typeof(CsvIgnorePropertyAttribute)))
                    .Select(fi => fi.Name))
                .ToList();
            return columns;
        }

#if !NET_3_5
        private void WriteCsvLines()
        {
            int written = 0;
            foreach (var csvLine in csvLinesToWrite.GetConsumingEnumerable())
            {
                this.streamWriter.WriteLine(csvLine);
                written++;
            }
            Interlocked.Add(ref this.linesToWrite, -written);
        }
#endif


        public void Append(T record)
        {

            if (CsvFile.UseTasks)
            {
#if !NET_3_5

                var linesWaiting = Interlocked.Increment(ref this.linesToWrite);
                Action<Task> addRecord = (t) =>
                {
                    var csvLine = this.ToCsv(record);
                    this.csvLinesToWrite.Add(csvLine);
                };

                if (linesWaiting < 10000)
                    this.addAsyncTask = this.addAsyncTask.ContinueWith(addRecord);
                else
                    addRecord(null);
#else
                throw new NotImplementedException("Tasks");
#endif
            }
            else
            {
                var csvLine = this.ToCsv(record);
                this.streamWriter.WriteLine(csvLine);
            }
        }

        private static Func<T, object> FindGetter(string c, bool staticMember)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | (staticMember ? BindingFlags.Static : BindingFlags.Instance);
            Func<T, object> func = null;
            PropertyInfo pi = typeof(T).GetProperty(c, flags);
            FieldInfo fi = typeof(T).GetField(c, flags);

            if (CsvFile.UseLambdas)
            {
                Expression expr = null;
                ParameterExpression parameter = Expression.Parameter(typeof(T), "r");
                Type type = null;

                if (pi != null)
                {
                    type = pi.PropertyType;
                    expr = Expression.Property(parameter, pi.Name);
                }
                else if (fi != null)
                {
                    type = fi.FieldType;
                    expr = Expression.Field(parameter, fi.Name);
                }
                if (expr != null)
                {
                    Expression<Func<T, object>> lambda;
                    if (type.IsValueType)
                    {
                        lambda = Expression.Lambda<Func<T, object>>(Expression.TypeAs(expr, typeof(object)), parameter);
                    }
                    else
                    {
                        lambda = Expression.Lambda<Func<T, object>>(expr, parameter);
                    }
                    func = lambda.Compile();
                }
            }
            else
            {
                if (pi != null)
                    func = o => pi.GetValue(o, null);
                else if (fi != null)
                    func = o => fi.GetValue(o);
            }
            return func;
        }

        private void CreateGetters()
        {
            var list = new List<Func<T, object>>();

            foreach (var columnName in columns)
            {
                Func<T, Object> func = null;
                var propertyName = (columnName.IndexOf(' ') < 0 ? columnName : columnName.Replace(" ", ""));
                func = FindGetter(columnName, false) ?? FindGetter(columnName, true);

                list.Add(func);
            }
            this.getters = list.ToArray();
        }

        private string ToCsv(T record)
        {
            if (record == null)
                throw new ArgumentException("Cannot be null", "record");

            string[] csvStrings = new string[getters.Length];

            for (int i = 0; i < getters.Length; i++)
            {
                var getter = getters[i];
                object fieldValue = getter == null ? null : getter(record);
                csvStrings[i] = this.ToCsvString(fieldValue);
            }
            return string.Join(this.fieldSeparatorAsString, csvStrings);

        }

        private string ToCsvString(object o)
        {
            if (o != null)
            {
                string valueString = o as string ?? Convert.ToString(o, CultureInfo.CurrentUICulture);
                if (RequiresQuotes(valueString))
                {
                    var csvLine = new StringBuilder();
                    csvLine.Append(this.textQualifier);
                    foreach (char c in valueString)
                    {
                        if (c == this.textQualifier)
                            csvLine.Append(c); // double the double quotes
                        csvLine.Append(c);
                    }
                    csvLine.Append(this.textQualifier);
                    return csvLine.ToString();
                }
                else
                    return valueString;
            }
            return string.Empty;
        }

        private bool RequiresQuotes(string valueString)
        {
            if (CsvFile.FastIndexOfAny)
            {
                var len = valueString.Length;
                for (int i = 0; i < len; i++)
                {
                    char c = valueString[i];
                    if (c <= 255 && this.isInvalidCharInFields[c])
                        return true;
                }
                return false;
            }
            else
            {
                return valueString.IndexOfAny(this.invalidCharsInFields) >= 0;
            }
        }

        private void WriteHeader()
        {
            var csvLine = new StringBuilder();
            for (int i = 0; i < this.columns.Length; i++)
            {
                if (i > 0)
                    csvLine.Append(this.fieldSeparator);
                csvLine.Append(this.ToCsvString(this.columns[i]));
            }
            this.streamWriter.WriteLine(csvLine.ToString());
        }
    }

    internal class CsvFileReader<T> : CsvFile, IEnumerable<T>, IEnumerator<T>
     where T : new()
    {
        private readonly Dictionary<Type, List<Action<T, String>>> allSetters = new Dictionary<Type, List<Action<T, String>>>();
        private string[] columns;
        private char curChar;
        private int len;
        private string line;
        private int pos;
        private T record;
        private readonly char fieldSeparator;
        private readonly TextReader textReader;
        private readonly char textQualifier;
        private readonly StringBuilder parseFieldResult = new StringBuilder();

        public CsvFileReader(CsvSource csvSource)
            : this(csvSource, null)
        {
        }

        public CsvFileReader(CsvSource csvSource, CsvDefinition csvDefinition)
        {
            var streamReader = csvSource.TextReader as StreamReader;
            if (streamReader != null)
                this.BaseStream = streamReader.BaseStream;
            if (csvDefinition == null)
                csvDefinition = DefaultCsvDefinition;
            this.fieldSeparator = csvDefinition.FieldSeparator;
            this.textQualifier = csvDefinition.TextQualifier;

            this.textReader = csvSource.TextReader;// new FileStream(csvSource.TextReader, FileMode.Open);

            this.ReadHeader(csvDefinition.Header);

        }

        public T Current
        {
            get { return this.record; }
        }

        public bool Eof
        {
            get { return this.line == null; }
        }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                this.textReader.Dispose();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            this.ReadNextLine();
            if (this.line == null && (this.line = this.textReader.ReadLine()) == null)
            {
                this.record = default(T);
            }
            else
            {
                this.record = new T();
                Type recordType = typeof(T);
                List<Action<T, String>> setters;
                if (!this.allSetters.TryGetValue(recordType, out setters))
                {
                    setters = this.CreateSetters();
                    this.allSetters[recordType] = setters;
                }

                var fieldValues = new string[setters.Count];
                for (int i = 0; i < setters.Count; i++)
                {
                    fieldValues[i] = this.ParseField();
                    if (this.curChar == this.fieldSeparator)
                        this.NextChar();
                    else
                        break;
                }
                for (int i = 0; i < setters.Count; i++)
                {
                    var setter = setters[i];
                    if (setter != null)
                    {
                        setter(this.record, fieldValues[i]);
                    }
                }
            }
            return (this.record != null);
        }


        public void Reset()
        {
            throw new NotImplementedException("Cannot reset CsvFileReader enumeration.");
        }

        private static Action<T, string> EmitSetValueAction(MemberInfo mi, Func<string, object> func)
        {
            ParameterExpression paramExpObj = Expression.Parameter(typeof(object), "obj");
            ParameterExpression paramExpT = Expression.Parameter(typeof(T), "instance");

            {
                var pi = mi as PropertyInfo;
                if (pi != null)
                {
#if !NET_3_5
                    if (CsvFile.UseLambdas)
                    {
                        var callExpr = Expression.Call(
                            paramExpT,
                            pi.GetSetMethod(),
                            Expression.ConvertChecked(paramExpObj, pi.PropertyType));
                        var setter = Expression.Lambda<Action<T, object>>(
                            callExpr,
                            paramExpT,
                            paramExpObj).Compile();
                        return (o, s) => setter(o, func(s));
                    }
#endif
                    return (o, v) => pi.SetValue(o, (object)func(v), null);

                }
            }
            {
                var fi = mi as FieldInfo;
                if (fi != null)
                {
#if !NET_3_5
                    if (CsvFile.UseLambdas)
                    {
                        //ParameterExpression valueExp = Expression.Parameter(typeof(string), "value");
                        var valueExp = Expression.ConvertChecked(paramExpObj, fi.FieldType);

                        // Expression.Property can be used here as well
                        MemberExpression fieldExp = Expression.Field(paramExpT, fi);
                        BinaryExpression assignExp = Expression.Assign(fieldExp, valueExp);

                        var setter = Expression.Lambda<Action<T, object>>
                            (assignExp, paramExpT, paramExpObj).Compile();

                        return (o, s) => setter(o, func(s));
                    }
#endif
                    return ((o, v) => fi.SetValue(o, func(v)));
                }
            }
            throw new NotImplementedException();
        }

        private static Action<T, string> FindSetter(string c, bool staticMember)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | (staticMember ? BindingFlags.Static : BindingFlags.Instance);
            Action<T, string> action = null;
            PropertyInfo pi = typeof(T).GetProperty(c, flags);
            if (pi != null)
            {
                var pFunc = StringToObject(pi.PropertyType);
                action = EmitSetValueAction(pi, pFunc);
            }
            FieldInfo fi = typeof(T).GetField(c, flags);
            if (fi != null)
            {
                var fFunc = StringToObject(fi.FieldType);
                action = EmitSetValueAction(fi, fFunc);
            }
            return action;
        }

        private static Func<string, object> StringToObject(Type propertyType)
        {
            if (propertyType == typeof(string))
                return (s) => s ?? String.Empty;
            else if (propertyType == typeof(Int32))
                return (s) => String.IsNullOrEmpty(s) ? 0 : Int32.Parse(s);
            if (propertyType == typeof(DateTime))
                return (s) => String.IsNullOrEmpty(s) ? DateTimeZero : DateTime.Parse(s);
            else
                throw new NotImplementedException();
        }

        private List<Action<T, string>> CreateSetters()
        {
            var list = new List<Action<T, string>>();
            for (int i = 0; i < this.columns.Length; i++)
            {
                string columnName = this.columns[i];
                Action<T, string> action = null;
                if (columnName.IndexOf(' ') >= 0)
                    columnName = columnName.Replace(" ", "");
                action = FindSetter(columnName, false) ?? FindSetter(columnName, true);

                list.Add(action);
            }
            return list;
        }

        private void NextChar()
        {
            if (this.pos < this.len)
            {
                this.pos++;
                this.curChar = this.pos < this.len ? this.line[this.pos] : '\0';
            }
        }

        private void ParseEndOfLine()
        {
            throw new NotImplementedException();
        }


        private string ParseField()
        {
            parseFieldResult.Length = 0;
            if (this.line == null || this.pos >= this.len)
                return null;
            while (this.curChar == ' ' || this.curChar == '\t')
            {
                this.NextChar();
            }
            if (this.curChar == this.textQualifier)
            {
                this.NextChar();
                while (this.curChar != 0)
                {
                    if (this.curChar == this.textQualifier)
                    {
                        this.NextChar();
                        if (this.curChar == this.textQualifier)
                        {
                            this.NextChar();
                            parseFieldResult.Append(this.textQualifier);
                        }
                        else
                            return parseFieldResult.ToString();
                    }
                    else if (this.curChar == '\0')
                    {
                        if (this.line == null)
                            return parseFieldResult.ToString();
                        this.ReadNextLine();
                    }
                    else
                    {
                        parseFieldResult.Append(this.curChar);
                        this.NextChar();
                    }
                }
            }
            else
            {
                while (this.curChar != 0 && this.curChar != this.fieldSeparator && this.curChar != '\r' && this.curChar != '\n')
                {
                    parseFieldResult.Append(this.curChar);
                    this.NextChar();
                }
            }
            return parseFieldResult.ToString();
        }

        private void ReadHeader(string header)
        {
            if (header == null)
            {
                this.ReadNextLine();
            }
            else
            {
                // we read the first line from the given header
                this.line = header;
                this.pos = -1;
                this.len = this.line.Length;
                this.NextChar();
            }

            var readColumns = new List<string>();
            string columnName;
            while ((columnName = this.ParseField()) != null)
            {
                readColumns.Add(columnName);
                if (this.curChar == this.fieldSeparator)
                    this.NextChar();
                else
                    break;
            }
            this.columns = readColumns.ToArray();
        }

        private void ReadNextLine()
        {
            this.line = this.textReader.ReadLine();
            this.pos = -1;
            if (this.line == null)
            {
                this.len = 0;
                this.curChar = '\0';
            }
            else
            {
                this.len = this.line.Length;
                this.NextChar();
            }
        }
    }

    public class CsvDefinition
    {
        public string Header { get; set; }
        public char FieldSeparator { get; set; }
        public char TextQualifier { get; set; }
        public IEnumerable<String> Columns { get; set; }
        public string EndOfLine { get; set; }

        public CsvDefinition()
        {
            if (CsvFile.DefaultCsvDefinition != null)
            {
                FieldSeparator = CsvFile.DefaultCsvDefinition.FieldSeparator;
                TextQualifier = CsvFile.DefaultCsvDefinition.TextQualifier;
                EndOfLine = CsvFile.DefaultCsvDefinition.EndOfLine;
            }
        }
    }

    public class CsvSource
    {
        public readonly TextReader TextReader;

        public static implicit operator CsvSource(CsvFile csvFile)
        {
            return new CsvSource(csvFile);
        }

        public static implicit operator CsvSource(string path)
        {
            return new CsvSource(path);
        }

        public static implicit operator CsvSource(TextReader textReader)
        {
            return new CsvSource(textReader);
        }

        public CsvSource(TextReader textReader)
        {
            this.TextReader = textReader;
        }

        public CsvSource(Stream stream)
        {
            this.TextReader = new StreamReader(stream);
        }

        public CsvSource(string path)
        {
            this.TextReader = new StreamReader(path);
        }

        public CsvSource(CsvFile csvFile)
        {
            this.TextReader = new StreamReader(csvFile.BaseStream);
        }
    }

    public class CsvDestination
    {
        public StreamWriter StreamWriter;

        public static implicit operator CsvDestination(string path)
        {
            return new CsvDestination(path);
        }
        private CsvDestination(StreamWriter streamWriter)
        {
            this.StreamWriter = streamWriter;
        }

        private CsvDestination(Stream stream)
        {
            this.StreamWriter = new StreamWriter(stream);
        }

        public CsvDestination(string FullName)
        {
            FixCsvFileName(ref FullName);
            this.StreamWriter = new StreamWriter(FullName);
        }

        private static void FixCsvFileName(ref string FullName)
        {
            FullName = Path.GetFullPath(FullName);
            var path = Path.GetDirectoryName(FullName);
            if (path != null && !Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!String.Equals(Path.GetExtension(FullName), ".csv"))
                FullName += ".csv";
        }
    }

    public static class CsvFileLinqExtensions
    {
        public static void ToCsv<T>(this IEnumerable<T> source, CsvDestination csvDestination)
        {
            source.ToCsv(csvDestination, null);
        }

        public static void ToCsv<T>(this IEnumerable<T> source, CsvDestination csvDestination, CsvDefinition csvDefinition)
        {
            using (var csvFile = new CsvFile<T>(csvDestination, csvDefinition))
            {
                foreach (var record in source)
                {
                    csvFile.Append(record);
                }
            }
        }

    }

    public class CsvIgnorePropertyAttribute : Attribute
    {
        public override string ToString()
        {
            return "Ignore Property";
        }
    }
    // 2013-11-29 Version 1
    // 2014-01-06 Version 2: add CoryLuLu suggestions
}