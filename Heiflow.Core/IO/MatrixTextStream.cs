// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Heiflow.Core.Data;

namespace Heiflow.Core.IO
{
     public  class MatrixTextStream:IFileProvider
    {
         public MatrixTextStream()
         {

         }

         public My2DMat<T> Load<T>(string filename)
         {
             FileName = filename;
             My2DMat<T> matrix = null;
             if (File.Exists(filename))
             {
                 StreamReader sr = new StreamReader(filename);
                 int nline = 0;
                 int ncol = 0;
                 string line = "";
                 while(!sr.EndOfStream)
                 {
                     line = sr.ReadLine().Trim();
                     if(!line.StartsWith(StreamReaderSequence.Comment) && line != "" && line != null)
                     {
                         nline++;
                     }
                 }
                 sr.Close();

                 sr = new StreamReader(filename);
                 while (!sr.EndOfStream)
                 {
                     if (!line.StartsWith(StreamReaderSequence.Comment) && line != "" && line != null)
                     {
                         line = sr.ReadLine().Trim();
                         var temp = TypeConverterEx.Split<T>(line);
                         ncol = temp.Length;
                         break;
                     }
                 }
                 sr.Close();

                 sr = new StreamReader(filename);
                 matrix = new My2DMat<T>(nline, ncol);
                 int i = 0;
                 while (!sr.EndOfStream)
                 {
                     if (!line.StartsWith(StreamReaderSequence.Comment) && line != "" && line != null)
                     {
                         line = sr.ReadLine().Trim();
                         var temp = TypeConverterEx.Split<T>(line);
                         matrix[i] = temp;
                         i++;
                     }
                 }

                 sr.Close();
             }
             return matrix;
         }

         public void  Save<T>(string filename, My2DMat<T> matrix)
         {
            if(matrix != null)
            {

            }
         }

         public void Save<T>(string filename, T[] vector)
         {
             if (vector != null)
             {
                 StreamWriter sw = new StreamWriter(filename);
                 string line = string.Join("\n", vector);
                 sw.WriteLine(line);
                 sw.Close();
             }
         }

         public void Save<T>(string filename, T[][] matrix)
         {
             if (matrix != null)
             {
                 StreamWriter sw = new StreamWriter(filename);
                 for (int i = 0; i < matrix.Length; i++)
                 {
                     string line = string.Join("\n", matrix[i]);
                     sw.WriteLine(line);
                 }
                 sw.Close();
             }
         }

         public void SaveTo<T>(string filename, My2DMat<T> mat)
         {
             if (mat != null)
             {
                 var matrix = mat.Value;
                 StreamWriter sw = new StreamWriter(filename);
                 for (int i = 0; i < matrix.Length; i++)
                 {
                     string line = string.Join("\n", matrix[i]);
                     sw.WriteLine(line);
                 }
                 sw.Close();
             }
         }

         public  int GetLeadingNumber(string input)
         {
             char[] chars = input.ToCharArray();
             int lastValid = -1;

             for (int i = 0; i < chars.Length; i++)
             {
                 if (Char.IsDigit(chars[i]))
                 {
                     lastValid = i;
                 }
                 else
                 {
                     break;
                 }
             }

             if (lastValid >= 0)
             {
                 return int.Parse(new string(chars, 0, lastValid + 1));
             }
             else
             {
                 return -1;
             }
         }

         public static T [] ConvertValue<T>(string [] value)
         {
             T[] converted = new T[value.Length];
             for (int i = 0; i < value.Length;i++ )
             {
                 converted[i] = (T)Convert.ChangeType(value[i], typeof(T));
             }
             return converted;
         }

         public string FileTypeDescription
         {
             get 
             {
                 return "text document";
             }
         }

         public string Extension
         {
             get 
             {
                 return ".txt";
             }
         }

         public string FileName
         {
             get;
             set;
         }
    }
}
