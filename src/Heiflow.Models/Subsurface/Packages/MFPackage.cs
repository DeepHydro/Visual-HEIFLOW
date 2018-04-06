using GSMS.Core;
using HUST.WREIS.ModelBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GSMS.External.Modflow
{
    public class MFPackage : Package,IMFPackage
    {
        public MFPackage( string name , MFGrid grid):base(name)
        {
            if (grid != null)
            {
                mfgrid = grid;
                mfprj = mfgrid.MFProject;
                if(!mfprj.Packages.Keys.Contains(name))
                mfprj.Packages.Add(name, this);
            }
            DefaultCBCVariables = new string[] { "FLOW RIGHT FACE", "FLOW FRONT FACE", "FLOW LOWER FACE", "STREAM LEAKAGE", "UZF RECHARGE", "SURFACE LEAKAGE", "GW ET" };
        }

        static MFPackage()
        {
            CBCVariable = new string[] { "CONSTANT HEAD", "FLOW RIGHT FACE", "FLOW FRONT FACE", 
                "FLOW LOWER FACE", "STREAM LEAKAGE", "UZF RECHARGE", "SURFACE LEAKAGE", "GW ET", "WELLS" };
        }
        protected MFGrid mfgrid;
        protected MFProject mfprj;
        /// <summary>
        /// 2D array [np,2]
        /// </summary>
        public int[,] SPInfo
        {
            get;
            set;
        }
        /// <summary>
        /// "FLOW RIGHT FACE", "FLOW FRONT FACE", "FLOW LOWER FACE", "STREAM LEAKAGE", "UZF RECHARGE", "SURFACE LEAKAGE", "GW ET"
        /// </summary>
        public string[] DefaultCBCVariables { get; set; }
        /// <summary>
        ///  "CONSTANT HEAD", "FLOW RIGHT FACE", "FLOW FRONT FACE", "FLOW LOWER FACE",  "STREAM LEAKAGE", "UZF RECHARGE", "SURFACE LEAKAGE", "Gw Et", "WELLS"
        /// </summary>
        public static string[] CBCVariable { get; private set; }

        #region General utilities
        public string GetInputFile(string package)
        {
            var pp = (from f in mfprj.PackageFile where f.ModuleName == package select f.FullFileName).FirstOrDefault();
             return pp;
        }

        public string GetOutputFile(int fid)
        {
            var pp = (from f in mfprj.PackageFile where f.FID == fid select f.FullFileName).FirstOrDefault();
            return pp;
        }

        public static T ReadSingleConstantValue<T>(StreamReader sr)
        {
            string line = sr.ReadLine();
            var strs = TypeConverterEx.Split<string>(line);
            return TypeConverterEx.ChangeType<T>(strs[1]);
        }
        public static void ReadInternalMatrix<T>(StreamReader sr, MatrixCube<T> matrix, int row, int col, int cLayer)
        {
            string line = sr.ReadLine().ToUpper();
            var strs = TypeConverterEx.Split<string>(line);
            // Read constant matrix
            if (strs[0].ToUpper() == "CONSTANT")
            {
                var ar = TypeConverterEx.Split<string>(line);
                T conv = TypeConverterEx.ChangeType<T>(ar[1]);
                matrix.LayeredConstantValues[cLayer] = conv;
                matrix.IsConstant[cLayer] = true;
            }
            // Read internal matrix
            else
            {
                matrix.LayeredValues[cLayer] = new T[row, col];
                matrix.IsConstant[cLayer] = false;
                T multiplier = TypeConverterEx.ChangeType<T>(strs[1]);
                line = sr.ReadLine();
                var values = TypeConverterEx.Split<T>(line);

                if (values.Length == col)
                {
                    for (int c = 0; c < col; c++)
                    {
                        matrix.LayeredValues[cLayer][0, c] = values[c];// *multiplier;
                    }
                    for (int r = 1; r < row; r++)
                    {
                        line = sr.ReadLine();
                        values = TypeConverterEx.Split<T>(line);
                        matrix.SetRowVector(values, cLayer, r, col);
                    }
                }
                else
                {
                    int colLine = (int)Math.Ceiling(col / 10.0);
                    for (int r = 0; r < row; r++)
                    {
                        int i = 0;
                        if (r == 0)
                        {
                            i = 1;
                        }
                        else
                        {
                            i = 0;
                            line = "";
                        }
                        for (; i < colLine; i++)
                        {
                            line += sr.ReadLine() + " ";
                        }

                        values = TypeConverterEx.Split<T>(line);
                        matrix.SetRowVector(values, cLayer, r, col);
                    }
                }
            }
        }
        public static void ReadSerialArray<T>(StreamReader sr, MatrixCube<T> matrix, MFGrid grid, int cLayer)
        {
            string line = sr.ReadLine().ToUpper();
            var strs = TypeConverterEx.Split<string>(line);
            // Read constant matrix
            if (strs[0].ToUpper() == "CONSTANT")
            {
                var ar = TypeConverterEx.Split<string>(line);
                T conv = TypeConverterEx.ChangeType<T>(ar[1]);
                matrix.LayeredConstantValues[cLayer] = conv;
                matrix.IsConstant[cLayer] = true;
            }
            // Read internal matrix
            else
            {
                matrix.LayeredSerialValue[cLayer] = new T[grid.ActiveCellCount];
                matrix.IsConstant[cLayer] = false;
                T multiplier = TypeConverterEx.ChangeType<T>(strs[1]);
                line = sr.ReadLine();
                var values = TypeConverterEx.Split<T>(line);
                int col = grid.ColumnCount;
                int row = grid.RowCount;

                if (values.Length == col)
                {
                    int index = 0;
                    for (int c = 0; c < col; c++)
                    {
                        if (grid.IBound.LayeredValues[0][0, c] != 0)
                        {
                            matrix.LayeredSerialValue[cLayer][index] = values[c];
                            index++;
                        }
                    }
                    for (int r = 1; r < row; r++)
                    {
                        line = sr.ReadLine();
                        values = TypeConverterEx.Split<T>(line);
                        for (int c = 0; c < col; c++)
                        {
                            if (grid.IBound.LayeredValues[0][r, c] != 0)
                            {
                                matrix.LayeredSerialValue[cLayer][index] = values[c];
                                index++;
                            }
                        }
                    }
                }
                else
                {
                    int index = 0;
                    int colLine = (int)Math.Ceiling(col / 10.0);
                    for (int r = 0; r < row; r++)
                    {
                        int i = 0;
                        if (r == 0)
                        {
                            i = 1;
                        }
                        else
                        {
                            i = 0;
                            line = "";
                        }
                        for (; i < colLine; i++)
                        {
                            line += sr.ReadLine() + " ";
                        }

                        values = TypeConverterEx.Split<T>(line);
                        for (int c = 0; c < col; c++)
                        {
                            if (grid.IBound.LayeredValues[0][r, c] != 0)
                            {
                                matrix.LayeredSerialValue[cLayer][index] = values[c];
                                index++;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// return the first occurrence of a line that is not commet
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static string ReadCommet(StreamReader sr)
        {
            string lastline = "";
            while (!sr.EndOfStream)
            {
                lastline = sr.ReadLine();
                if (!lastline.StartsWith("#"))
                    break;
            }
            return lastline;
        }
        public static void WriteDefaultComment(StreamWriter sw, string package)
        {
            string defaultcm = string.Format("#{0}: created on {1} by Visual GSFLOW", package, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            sw.WriteLine(defaultcm);
        }
        /// <summary>
        /// write matrix in the type of Constant or Matrix using defualt format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sw"></param>
        /// <param name="matrix"></param>
        /// <param name="multiplier"></param>
        /// <param name="clayer"></param>
        /// <param name="iprn"></param>
        /// <param name="comment"></param>
        public static void WriteInternalMatrix<T>(StreamWriter sw, MatrixCube<T> matrix, T multiplier, int clayer, int iprn, string comment)
        {
            if (matrix.IsConstant[clayer])
            {
                string line = string.Format("CONSTANT\t{0}\t{1}", matrix.LayeredConstantValues[clayer], comment);
                sw.WriteLine(line);
            }
            else
            {
                string line = string.Format("INTERNAL\t{0}\t(FREE)\t{1}\t{2}", multiplier, iprn, comment);
                int row = matrix.Row;
                int col = matrix.Column;

                sw.WriteLine(line);
                for (int r = 0; r < row; r++)
                {
                    line = "";
                    for (int c = 0; c < col; c++)
                    {
                        line += string.Format("{0}", matrix.LayeredValues[clayer][r, c]) + StreamReaderSequence.stab;
                    }
                    line = line.Trim(StreamReaderSequence.ctab);
                    sw.WriteLine(line);
                }
            }
        }
        /// <summary>
        /// write matrix in the type of Constant or Matrix using given format
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="matrix"></param>
        /// <param name="multiplier"></param>
        /// <param name="clayer"></param>
        /// <param name="format"></param>
        /// <param name="iprn"></param>
        /// <param name="comment"></param>
        public static void WriteInternalMatrix(StreamWriter sw, MatrixCube<float> matrix, float multiplier, int clayer, string format, int iprn, string comment)
        {
            if (matrix.IsConstant[clayer])
            {
                string line = string.Format("CONSTANT\t{0}\t{1}", matrix.LayeredConstantValues[clayer], comment);
                sw.WriteLine(line);
            }
            else
            {
                string line = string.Format("INTERNAL\t{0}\t(FREE)\t{1}\t{2}", multiplier, iprn, comment);
                int row = matrix.Row;
                int col = matrix.Column;

                sw.WriteLine(line);

                for (int r = 0; r < row; r++)
                {
                    line = "";
                    for (int c = 0; c < col; c++)
                    {
                        line += matrix.LayeredValues[clayer][r, c].ToString(format) + StreamReaderSequence.stab;
                    }
                    line = line.Trim(StreamReaderSequence.ctab);
                    sw.WriteLine(line);
                }
            }
        }

        public static void WriteSerialArray<T>(StreamWriter sw, MatrixCube<T> matrix, T multiplier, MFGrid grid, int clayer, int iprn, string comment)
        {
            if (matrix.IsConstant[clayer])
            {
                string line = string.Format("CONSTANT\t{0}\t{1}", matrix.LayeredConstantValues[clayer], comment);
                sw.WriteLine(line);
            }
            else
            {
                string line = string.Format("INTERNAL\t{0}\t(FREE)\t{1}\t{2}", multiplier, iprn, comment);
                int row = grid.RowCount;
                int col = grid.ColumnCount;

                sw.WriteLine(line);
                int index = 0;

                for (int r = 0; r < row; r++)
                {
                    line = "";
                    for (int c = 0; c < col; c++)
                    {
                        if (grid.IBound.LayeredValues[0][r, c] != 0)
                        {
                            line += matrix.LayeredSerialValue[clayer][index] + StreamReaderSequence.stab;
                            index++;
                        }
                        else
                        {
                            line += "0" + StreamReaderSequence.stab;
                        }
                    }
                    line = line.Trim(StreamReaderSequence.ctab);
                    sw.WriteLine(line);
                }

            }
        }
        #endregion

        #region Read results
        public void ReadBinaryFHD(string filename = "")
        {
            if (filename == "")
            {
                filename = GetInputFile("FHD");
            }

            if (File.Exists(filename))
            {
                var grid = mfgrid;

                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);

                List<float[]> headLst = new List<float[]>();
                long layerbyte = 32 + 4 * 3 + grid.RowCount * grid.ColumnCount * 4;
                float head = 0;
                while (fs.Position < fs.Length)
                {
                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        if (l == 0)
                        {
                            fs.Seek(32, SeekOrigin.Current);
                            var vv = br.ReadInt32();
                            vv = br.ReadInt32();
                            vv = br.ReadInt32();
                            int index = 0;
                            float[] heads = new float[grid.ActiveCellCount];

                            for (int r = 0; r < grid.RowCount; r++)
                            {
                                for (int c = 0; c < grid.ColumnCount; c++)
                                {
                                    head = br.ReadSingle();
                                    if (grid.IBound.LayeredValues[0][r, c] != 0)
                                    {
                                        heads[index] = head;
                                        index++;
                                    }
                                }
                            }
                            headLst.Add(heads);
                        }
                        else
                        {
                            fs.Seek(layerbyte, SeekOrigin.Current);
                        }
                    }
                }
                grid.ArrayCube = new MatrixCube<float>(headLst.Count, true);
                for (int i = 0; i < headLst.Count; i++)
                {
                    grid.ArrayCube.LayeredSerialValue[i] = headLst[i];
                }

                br.Close();
                fs.Close();
                headLst.Clear();
            }
        }

        public void ReadCBC(string[] varNames, string filename = "", int maxstep = -1, int layer = 0)
        {
            if (filename == "")
            {
                filename = GetInputFile("CBC");
            }

            if (File.Exists(filename))
            {
                var grid = mfgrid;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                List<string> vnLst = new List<string>();
                long layerbyte = grid.RowCount * grid.ColumnCount * 4;
                float vv = 0;
                int step = 0;
                int varLen = varNames.Length;
                List<float[]>[] cbcLst = new List<float[]>[varLen];
                for (int i = 0; i < varLen; i++)
                {
                    cbcLst[i] = new List<float[]>();
                }
                int varIndex = 0;

                while (!(fs.Position == fs.Length))
                {
                    fs.Seek(4 * 2, SeekOrigin.Current);
                    var vn = new string(br.ReadChars(16)).Trim();
                    fs.Seek(4 * 3, SeekOrigin.Current);
                    if (varNames.Contains(vn))
                    {
                        if (vnLst.Contains(vn))
                        {
                            varIndex = 0;
                            vnLst.Clear();
                            step++;
                        }
                        for (int l = 0; l < grid.ActualLayerCount; l++)
                        {
                            if (l == layer)
                            {
                                int index = 0;
                                float[] values = new float[grid.ActiveCellCount];

                                for (int r = 0; r < grid.RowCount; r++)
                                {
                                    for (int c = 0; c < grid.ColumnCount; c++)
                                    {
                                        vv = br.ReadSingle();
                                        if (grid.IBound.LayeredValues[layer][r, c] != 0)
                                        {
                                            values[index] = vv;
                                            index++;
                                        }
                                    }
                                }
                                vnLst.Add(vn);
                                cbcLst[varIndex].Add(values);
                            }
                            else
                            {
                                fs.Seek(layerbyte, SeekOrigin.Current);
                            }
                        }
                        varIndex++;
                    }
                    else
                    {
                        fs.Seek(layerbyte * grid.ActualLayerCount, SeekOrigin.Current);
                    }
                    if (maxstep > 0)
                    {
                        if (step >= maxstep)
                            break;
                    }
                }
                grid.MultipleArrayCube = new MatrixCube<float>[varLen];

                for (int i = 0; i < varLen; i++)
                {
                    grid.MultipleArrayCube[i] = new MatrixCube<float>(step, true)
                    {
                        Name = varNames[i]
                    };
                    for (int s = 0; s < step; s++)
                    {
                        if (s < cbcLst[i].Count)
                            grid.MultipleArrayCube[i].LayeredSerialValue[s] = cbcLst[i][s];
                    }
                }

                br.Close();
                fs.Close();
            }
        }

        private float[] ReadNonZeroArray(BinaryReader br, MFGrid grid, float[][] values)
        {
            float[] wt = new float[grid.ActiveCellCount];
            float vv = 0;
            for (int l = 0; l < grid.ActualLayerCount; l++)
            {
                int index = 0;
                for (int r = 0; r < grid.RowCount; r++)
                {
                    for (int c = 0; c < grid.ColumnCount; c++)
                    {
                        vv = br.ReadSingle();
                        if (grid.IBound.LayeredValues[0][r, c] != 0)
                        {
                            values[l][index] = vv;
                            index++;
                        }
                    }
                }
            }

            for (int i = 0; i < grid.ActiveCellCount; i++)
            {
                for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                {
                    if (values[ll][i] != 0)
                    {
                        wt[i] = values[ll][i];
                        break;
                    }
                }
            }
            return wt;
        }

        public void ReadCellFlow( string filename = "", int maxstep = -1)
        {
            if (filename == "")
            {
                filename = GetInputFile("CBC");
            }

            if (File.Exists(filename))
            {
                var grid = mfgrid;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                List<string> vnLst = new List<string>();
                long layerbyte = grid.RowCount * grid.ColumnCount * 4;

                int step = 0;
                int varLen =3;
                List<float[]>[] cbcLst = new List<float[]>[varLen];

                for (int i = 0; i < varLen; i++)
                {
                    cbcLst[i] = new List<float[]>();
                }

                float[][] values = new float[grid.ActualLayerCount][];
                for (int l = 0; l < grid.ActualLayerCount; l++)
                {
                    values[l] = new float[grid.ActiveCellCount];
                }

                while (!(fs.Position == fs.Length))
                {
                    fs.Seek(4 * 2, SeekOrigin.Current);
                    var vn = new string(br.ReadChars(16)).Trim();
                    fs.Seek(4 * 3, SeekOrigin.Current);
                    if (vn == CBCVariable[1])
                    {
                        var wt = ReadNonZeroArray(br, grid, values);
                        cbcLst[0].Add(wt);         
                    }
                    else if (vn == CBCVariable[2])
                    {
                        var wt = ReadNonZeroArray(br, grid, values);
                        cbcLst[1].Add(wt);
                    }
                    else if (vn == CBCVariable[3])
                    {
                        var wt = ReadNonZeroArray(br, grid, values);
                        cbcLst[2].Add(wt);
                        step++;
                        if (maxstep > 0)
                        {
                            if (step >= maxstep)
                                break;
                        }
                    }
                    else
                    {
                        fs.Seek(layerbyte * grid.ActualLayerCount, SeekOrigin.Current);
                    }
                }
                grid.MultipleArrayCube = new MatrixCube<float>[varLen];


                for (int i = 0; i < varLen; i++)
                {
                    grid.MultipleArrayCube[i] = new MatrixCube<float>(step, true)
                    {
                        Name = CBCVariable[i + 1]
                    };
                    for (int s = 0; s < step; s++)
                    {
                        if (s < cbcLst[i].Count)
                            grid.MultipleArrayCube[i].LayeredSerialValue[s] = cbcLst[i][s];
                    }
                }
                br.Close();
                fs.Close();
            }
        }

        public void ReadAverageCBC(string[] varNames, string filename = "", int layer = 0)
        {
            if (filename == "")
            {
                filename = GetInputFile("CBC");
            }

            if (File.Exists(filename))
            {
                var grid = mfgrid;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                List<string> vnLst = new List<string>();
                long layerbyte = grid.RowCount * grid.ColumnCount * 4;
                float vv = 0;
                int step = 0;
                int varLen = varNames.Length;
                List<float[]>[] cbcLst = new List<float[]>[varLen];
                for (int i = 0; i < varLen; i++)
                {
                    cbcLst[i] = new List<float[]>();
                }
                int varIndex = 0;

                while (!(fs.Position == fs.Length))
                {
                    fs.Seek(4 * 2, SeekOrigin.Current);
                    var vn = new string(br.ReadChars(16)).Trim();
                    fs.Seek(4 * 3, SeekOrigin.Current);
                    if (varNames.Contains(vn))
                    {
                        if (vnLst.Contains(vn))
                        {
                            varIndex = 0;
                            vnLst.Clear();
                            step++;
                        }
                        for (int l = 0; l < grid.ActualLayerCount; l++)
                        {
                            if (l == layer)
                            {
                                int index = 0;
                                float[] values = new float[grid.ActiveCellCount];

                                for (int r = 0; r < grid.RowCount; r++)
                                {
                                    for (int c = 0; c < grid.ColumnCount; c++)
                                    {
                                        vv = br.ReadSingle();
                                        if (grid.IBound.LayeredValues[layer][r, c] != 0)
                                        {
                                            values[index] = vv;
                                            index++;
                                        }
                                    }
                                }
                                vnLst.Add(vn);
                                cbcLst[varIndex].Add(values);
                            }
                            else
                            {
                                fs.Seek(layerbyte, SeekOrigin.Current);
                            }
                        }
                        varIndex++;
                    }
                    else
                    {
                        fs.Seek(layerbyte * grid.ActualLayerCount, SeekOrigin.Current);
                    }
                }
                grid.MultipleArrayCube = new MatrixCube<float>[varLen];

                for (int i = 0; i < varLen; i++)
                {
                    grid.MultipleArrayCube[i] = new MatrixCube<float>(1, true)
                    {
                        Name = varNames[i]
                    };
                    grid.MultipleArrayCube[i].LayeredSerialValue[0] = new float[grid.ActiveCellCount];
                    for (int s = 0; s < step; s++)
                    {
                        for (int t = 0; t < grid.ActiveCellCount; t++)
                        {
                            grid.MultipleArrayCube[i].LayeredSerialValue[0][t] += cbcLst[i][s][t];
                        }
                    }
                    for (int t = 0; t < grid.ActiveCellCount; t++)
                    {
                        grid.MultipleArrayCube[i].LayeredSerialValue[0][t] /= step;
                    }
                }

                br.Close();
                fs.Close();
            }
        }

        public void ReadYearlyBinWaterTable(string filename = "",int yrs=13, int maxstep = 30)
        {
            if (filename == "")
            {
                filename = GetInputFile("FHD");
            }

            if (File.Exists(filename))
            {
                var grid = mfgrid;

                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);

                List<float[]> headLst = new List<float[]>();
                long layerbyte = 32 + 4 * 3 + grid.RowCount * grid.ColumnCount * 4;
                float head = 0;
                float[][] heads = new float[grid.ActualLayerCount][];
                for (int l = 0; l < grid.ActualLayerCount; l++)
                {
                    heads[l] = new float[grid.ActiveCellCount];
                }
                int step = 0;
                while (!(fs.Position == fs.Length))
                {
                    float[] wt = new float[grid.ActiveCellCount];
                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        fs.Seek(32, SeekOrigin.Current);
                        var vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        int index = 0;
                        for (int r = 0; r < grid.RowCount; r++)
                        {
                            for (int c = 0; c < grid.ColumnCount; c++)
                            {
                                head = br.ReadSingle();
                                if (grid.IBound.LayeredValues[0][r, c] != 0)
                                {
                                    heads[l][index] = head;
                                    index++;
                                }
                            }
                        }

                        float[] lwt = new float[grid.ActualLayerCount];
                        for (int i = 0; i < grid.ActiveCellCount; i++)
                        {
                            for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                            {
                                lwt[ll] = heads[ll][i];
                            }
                            wt[i] = lwt.Max();
                        }
                    }

                    headLst.Add(wt);
                    step++;
                    if (maxstep > 0)
                        if (step >= maxstep)
                            break;
                }
                grid.ArrayCube = new MatrixCube<float>(yrs, true);
                int tt = 0;
                for (int i = 0; i < yrs; i++)
                {
                    float[] ff = new float[grid.ActiveCellCount];
                    for (int m = 0; m < 36; m++)
                    {
                        for (int s = 0; s < ff.Length; s++)
                        {
                            ff[s] += headLst[tt][s];
                        }
                        tt++;
                    }
                    for (int s = 0; s < ff.Length; s++)
                    {
                        ff[s] /= 36;
                    }
                    grid.ArrayCube.LayeredSerialValue[i] = ff;
                }

                heads = null;

                br.Close();
                fs.Close();
                headLst.Clear();
                GC.Collect();
            }
        }

        public void ReadBinWaterTable(string filename = "", int maxstep=30)
        {
            if (filename == "")
            {
                filename = GetInputFile("FHD");
            }

            if (File.Exists(filename))
            {
                var grid = mfgrid;

                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);

                List<float[]> headLst = new List<float[]>();
                long layerbyte = 32 + 4 * 3 + grid.RowCount * grid.ColumnCount * 4;
                float head = 0;
                float[][] heads = new float[grid.ActualLayerCount][];
                for (int l = 0; l < grid.ActualLayerCount; l++)
                {
                    heads[l] = new float[grid.ActiveCellCount];
                }
                int step = 0;
                while (!(fs.Position == fs.Length))
                {
                    float[] wt = new float[grid.ActiveCellCount];
                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        fs.Seek(32, SeekOrigin.Current);
                        var vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        int index = 0;
                        for (int r = 0; r < grid.RowCount; r++)
                        {
                            for (int c = 0; c < grid.ColumnCount; c++)
                            {
                                head = br.ReadSingle();
                                if (grid.IBound.LayeredValues[0][r, c] != 0)
                                {
                                    heads[l][index] = head;
                                    index++;
                                }
                            }
                        }

                        float[] lwt = new float[grid.ActualLayerCount];
                        for (int i = 0; i < grid.ActiveCellCount; i++)
                        {
                            for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                            {
                                lwt[ll] = heads[ll][i];
                            }
                            wt[i] = lwt.Max();
                        }
                    }

                    headLst.Add(wt);
                    step++;
                    if (maxstep > 0)
                        if (step >= maxstep)
                            break;
                }

                grid.ArrayCube = new MatrixCube<float>(headLst.Count, true);
                for (int i = 0; i < headLst.Count; i++)
                {
                    grid.ArrayCube.LayeredSerialValue[i] = headLst[i];
                }

                heads = null;

                br.Close();
                fs.Close();
                headLst.Clear();
                GC.Collect();
            }
        }

        public void ReadTxtWaterTable(string filename = "", int maxstep=100)
        {
            if (filename == "")
            {
                filename = GetInputFile("FHD");
            }

            if (File.Exists(filename))
            {
                StreamReader srFhd = new StreamReader(filename);
                string headline = srFhd.ReadLine();
                string[] strs = Regex.Split(headline.Trim(), @"[ ]+");
                int step = int.Parse(strs[0]);
                int sp = int.Parse(strs[1]);
                int col = int.Parse(strs[5]);
                int row = int.Parse(strs[6]);
                srFhd.Close();
                int nlayer = mfgrid.ActualLayerCount;
                string line = "";
                var grid = mfgrid;
                int stepIndex = 0;
                srFhd = new StreamReader(filename);

                int colLine = (int)Math.Ceiling(col / 10.0);
                float head = 0;
                float[][] heads = new float[mfgrid.ActualLayerCount][];
                List<float[]> headLst = new List<float[]>();
                for (int l = 0; l < mfgrid.ActualLayerCount; l++)
                {
                    heads[l] = new float[mfgrid.ActiveCellCount];
                }

                while (!srFhd.EndOfStream)
                {
                    float[] wt = new float[grid.ActiveCellCount];
                    for (int l = 0; l < nlayer; l++)
                    {
                        headline = srFhd.ReadLine();
                        strs = Regex.Split(headline.Trim(), @"[ ]+");
                        step = int.Parse(strs[0]);
                        sp = int.Parse(strs[1]);
                        int index = 0;
                        for (int r = 0; r < row; r++)
                        {
                            line = "";
                            for (int i = 0; i < colLine; i++)
                            {
                                line += srFhd.ReadLine() + " ";
                            }
                            strs = Regex.Split(line.Trim(), @"[ ]+");
                            for (int c = 0; c < strs.Length; c++)
                            {
                                head = float.Parse(strs[c]);
                                if (grid.IBound.LayeredValues[0][r, c] != 0)
                                {
                                    heads[l][index] = head;
                                    index++;
                                }
                            }
                        }
                        float[] lwt = new float[grid.ActualLayerCount];
                        for (int i = 0; i < grid.ActiveCellCount; i++)
                        {
                            for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                            {
                                lwt[ll] = heads[ll][i];
                            }
                            wt[i] = lwt.Max();
                        }
                    }
                    headLst.Add(wt);
                    stepIndex++;
                    if (stepIndex >= maxstep)
                        break;
                }

                grid.ArrayCube = new MatrixCube<float>(headLst.Count, true);
                for (int i = 0; i < headLst.Count; i++)
                {
                    grid.ArrayCube.LayeredSerialValue[i] = headLst[i];
                }

                srFhd.Close();
                heads = null;
                headLst.Clear();
                GC.Collect();
            }
        }

        public void ReadTxtAvWaterTable(string filename = "", int maxstep = 100)
        {
            if (filename == "")
            {
                filename = GetInputFile("FHD");
            }

            if (File.Exists(filename))
            {
                StreamReader srFhd = new StreamReader(filename);
                string headline = srFhd.ReadLine();
                string[] strs = Regex.Split(headline.Trim(), @"[ ]+");
                int step = int.Parse(strs[0]);
                int sp = int.Parse(strs[1]);
                int col = int.Parse(strs[5]);
                int row = int.Parse(strs[6]);
                srFhd.Close();
                int nlayer = mfgrid.ActualLayerCount;
                string line = "";
                var grid = mfgrid;
                int stepIndex = 0;
                srFhd = new StreamReader(filename);

                int colLine = (int)Math.Ceiling(col / 10.0);
                float head = 0;
                float[][] heads = new float[mfgrid.ActualLayerCount][];
                for (int l = 0; l < mfgrid.ActualLayerCount; l++)
                {
                    heads[l] = new float[mfgrid.ActiveCellCount];
                }
                float[] wtsum = new float[grid.ActiveCellCount];

                while (!srFhd.EndOfStream)
                {
                    float[] wt = new float[grid.ActiveCellCount];
                    for (int l = 0; l < nlayer; l++)
                    {
                        headline = srFhd.ReadLine();
                        strs = Regex.Split(headline.Trim(), @"[ ]+");
                        step = int.Parse(strs[0]);
                        sp = int.Parse(strs[1]);
                        int index = 0;
                        for (int r = 0; r < row; r++)
                        {
                            line = "";
                            for (int i = 0; i < colLine; i++)
                            {
                                line += srFhd.ReadLine() + " ";
                            }
                            strs = Regex.Split(line.Trim(), @"[ ]+");
                            for (int c = 0; c < strs.Length; c++)
                            {
                                head = float.Parse(strs[c]);
                                if (grid.IBound.LayeredValues[0][r, c] != 0)
                                {
                                    heads[l][index] = head;
                                    index++;
                                }
                            }
                        }
                        if (stepIndex > 0)
                        {
                            float[] lwt = new float[grid.ActualLayerCount];
                            for (int i = 0; i < grid.ActiveCellCount; i++)
                            {
                                for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                                {
                                    lwt[ll] = heads[ll][i];
                                }
                                wt[i] = lwt.Max();
                            }
                        }
                    }

                    var  newline = sp + "\t" + step + "\t";
                    Console.WriteLine(newline);

                    for (int i = 0; i < grid.ActiveCellCount; i++)
                    {
                        wtsum[i] += wt[i];
                    }
                    stepIndex++;
                    if (stepIndex >= maxstep)
                        break;
                }
                grid.ArrayCube = new MatrixCube<float>(1, true);
                grid.ArrayCube.LayeredSerialValue[0] = new float[grid.ActiveCellCount];
                for (int i = 0; i < grid.ActiveCellCount; i++)
                {
                    grid.ArrayCube.LayeredSerialValue[0][i] = wtsum[i] / (stepIndex - 1);
                }

                srFhd.Close();
                heads = null;
                GC.Collect();
            }
        }
        public void ReadBinAvWaterTable(string filename = "", int maxstep = 30)
        {
            if (filename == "")
            {
                filename = GetInputFile("FHD");
            }

            if (File.Exists(filename))
            {
                var grid = mfgrid;

                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);

                long layerbyte = 32 + 4 * 3 + grid.RowCount * grid.ColumnCount * 4;
                float head = 0;
                float[][] heads = new float[grid.ActualLayerCount][];
                for (int l = 0; l < grid.ActualLayerCount; l++)
                {
                    heads[l] = new float[grid.ActiveCellCount];
                }
                int step = 0;
                float[] wt = new float[grid.ActiveCellCount];
                float[] wtsum = new float[grid.ActiveCellCount];
                while (!(fs.Position == fs.Length))
                {
                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        fs.Seek(32, SeekOrigin.Current);
                        var vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        int index = 0;
                        for (int r = 0; r < grid.RowCount; r++)
                        {
                            for (int c = 0; c < grid.ColumnCount; c++)
                            {
                                head = br.ReadSingle();
                                if (grid.IBound.LayeredValues[0][r, c] != 0)
                                {
                                    heads[l][index] = head;
                                    index++;
                                }
                            }
                        }
                        if (step > 0)
                        {
                            float[] lwt = new float[grid.ActualLayerCount];
                            for (int i = 0; i < grid.ActiveCellCount; i++)
                            {
                                for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                                {
                                    lwt[ll] = heads[ll][i];
                                }
                                wt[i] = lwt.Max();
                            }
                        }
                    }
                    for (int i = 0; i < grid.ActiveCellCount; i++)
                    {
                        wtsum[i] += wt[i];
                    }
                    step++;
                    if (maxstep > 0 && step >= maxstep)
                        break;
                }

                grid.ArrayCube = new MatrixCube<float>(1, true);
                grid.ArrayCube.LayeredSerialValue[0] = new float[grid.ActiveCellCount];
                for (int i = 0; i < grid.ActiveCellCount; i++)
                {
                    grid.ArrayCube.LayeredSerialValue[0][i] = wtsum[i] / (step - 1);
                }

                heads = null;
                br.Close();
                fs.Close();
                GC.Collect();
            }
        }

        public float ExtractPointValue(HeadObservation obs,MatrixCube<float> arrayCube, int step )
        {
            float value = 0;
           int ids = mfgrid.Topology.ActiveCellIndex[obs.CellID];
           value = arrayCube.LayeredSerialValue[step][ids];
            return value;
        }

        public void ExtractMonthlyTimeSeries(HeadObservation[] wells, MatrixCube<float> arrayCube, DateTime start, int DayInteval)
        {
            int nwell = wells.Length;
            //compute monthly averaged value, thus divided by 3
            int nl = arrayCube.Layer / 3;
            int[] ids = new int[nwell];
            for (int i = 0; i < nwell; i++)
            {
                ids[i] = mfgrid.Topology.ActiveCellIndex[wells[i].CellID];
                DateTime[] dates = new DateTime[nl];
                for (int l = 0; l < nl; l++)
                {
                    //dates[l] = start.AddDays(l * DayInteval * 3);
                    dates[l] = start.AddMonths(l);
                }
                double[] values = new double[nl];
                wells[i].TimeSeries = new NumericalTimeSeries(values, dates);
            }
            for (int l = 0; l< nl; l++)
            {              
                for (int i = 0; i < nwell; i++)
                {
                    var temp = arrayCube.LayeredSerialValue[l * 3][ids[i]] + arrayCube.LayeredSerialValue[l * 3 + 1][ids[i]] + arrayCube.LayeredSerialValue[l * 3 + 2][ids[i]];
                    wells[i].TimeSeries.DataValueVector[l] = temp / 3;
                }
            }          
        }

        public void ExtractTimeSeries(HeadObservation[] wells, MatrixCube<float> arrayCube, DateTime start, int DayInteval)
        {
            int nwell = wells.Length;
            //compute monthly averaged value, thus divided by 3
            int nl = arrayCube.Layer;
            int[] ids = new int[nwell];
            for (int i = 0; i < nwell; i++)
            {
                ids[i] = mfgrid.Topology.ActiveCellIndex[wells[i].CellID];
                DateTime[] dates = new DateTime[nl];
                for (int l = 0; l < nl; l++)
                {
                    dates[l] = start.AddDays(l * DayInteval);
                }
                double[] values = new double[nl];
                wells[i].TimeSeries = new NumericalTimeSeries(values, dates);
            }
            for (int l = 0; l < nl; l++)
            {
                for (int i = 0; i < nwell; i++)
                {
                    wells[i].TimeSeries.DataValueVector[l] = arrayCube.LayeredSerialValue[l][ids[i]];
                }
            }
        }
        #endregion

        public void ReadArrayCube(string filename, int maxstep)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            List<float[]> list = new List<float[]>();
            MFGrid grid = mfgrid;

            int step = 0;
            while (fs.Position < fs.Length)
            {
                float[] data = new float[grid.ActiveCellCount];
                for (int l = 0; l < grid.ActiveCellCount; l++)
                {
                    data[l] = br.ReadSingle();
                }
                step++;
                if (step >= maxstep)
                    break;
                list.Add(data);
            }
            grid.ArrayCube = new MatrixCube<float>(list.Count, true);
            for (int i = 0; i < list.Count; i++)
            {
                grid.ArrayCube.LayeredSerialValue[i] = list[i];
            }

            br.Close();
            fs.Close();
        }
    }
}
