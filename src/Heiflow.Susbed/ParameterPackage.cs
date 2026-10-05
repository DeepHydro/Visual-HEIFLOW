// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    [PackageItem]
    public class ParameterPackage : Package
    {
        public static string PackageName = "SusbedParameter";
        public ParameterPackage()
        {
            Name = "SusbedParameter";
            _FullName = "Parameter";
            _PackageInfo = new Generic.PackageInfo();
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".txt";
            _PackageInfo.ModuleName = "PARA";
            Description = "";
            Version = "PARA1";
            IsMandatory = true;
            Tributaries = new List<SourceInfo>();
            PointSources = new List<SourceInfo>();
            MainStream = new SourceInfo();
            LL = new List<SourceInfo>();
        }
        //＝＝＝＝＝＝＝水流参数＝＝＝＝＝＝＝
        [Category("水流参数1")]
        public int LMODE { get; set; }
        [Category("水流参数1")]
        public int MFUNC { get; set; }
        [Category("水流参数1")]
        public int PDTH { get; set; }
        [Category("水流参数1")]
        public int IDBUG { get; set; }
        [Category("水流参数1")]
        public int ENDTIME { get; set; }
        [Category("水流参数1")]
        public int DAYIDBUG { get; set; }
        [Category("水流参数2")]
        public int NPXT { get; set; }
        [Category("水流参数2")]
        public int NPJMAX { get; set; }
        [Category("水流参数2")]
        public int ZDEAD { get; set; }
        [Category("水流参数2")]
        public int ZNORMAL { get; set; }
        [Category("水流参数2")]
        public int MLEIJI { get; set; }


        [Browsable(false)]
        public int NUMBR { get; set; }
        [Browsable(false)]
        public int INODE { get; set; }
        [Browsable(false)]
        public List<SourceInfo> Tributaries { get; private set; }
        [Browsable(false)]
        public List<SourceInfo> PointSources { get; private set; }

        public SourceInfo MainStream { get; private set; }

        //＝＝＝＝＝＝＝糙率参数＝＝＝＝＝＝＝	
        [Browsable(false)]
        public int NT { get; set; }
        [Browsable(false)]
        public double[] TTD { get; set; }
        [Browsable(false)]
        public int MD { get; set; }
        [Browsable(false)]
        public List<SourceInfo> LL { get; set; }
        [Category("糙率参数")]
        public int MNN { get; set; }
        [Category("糙率参数")]
        [Description("预估平衡时间（天）")]
        public double DDDT { get; set; }
        [Browsable(false)]
        public int MT3 { get; set; }
        [Browsable(false)]
        public double[] QP3 { get; set; }
        [Browsable(false)]
        public double[,] CN3 { get; set; }
        [Browsable(false)]
        public double[,] CNP3 { get; set; }

        //＝＝＝＝＝＝泥沙参数＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝	
        [Browsable(false)]
        List<string> Sendiments { get; set; }

        public override void Initialize()
        {
            _Initialized = true;
        }

        public override LoadingState Load(ICancelProgressHandler progress)
        {
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName, Encoding.Default);
               // StreamReader sr = new StreamReader(FileName, Encoding.GetEncoding("gb2312"));
                string line = sr.ReadLine();

                line = sr.ReadLine();
                line = sr.ReadLine();
                var buf_int = TypeConverterEx.Split<int>(line, 6);
                LMODE = buf_int[0];
                MFUNC = buf_int[1];
                PDTH = buf_int[2];
                IDBUG = buf_int[3];
                ENDTIME = buf_int[4];
                DAYIDBUG = buf_int[5];

                line = sr.ReadLine();
                line = sr.ReadLine();
                buf_int = TypeConverterEx.Split<int>(line, 5);
                NPXT = buf_int[0];
                NPJMAX = buf_int[1];
                ZDEAD = buf_int[2];
                ZNORMAL = buf_int[3];
                MLEIJI = buf_int[4];

                line = sr.ReadLine();
                line = sr.ReadLine();
                buf_int = TypeConverterEx.Split<int>(line, 2);
                NUMBR = buf_int[0];
                INODE = buf_int[1];

                Tributaries.Clear();
                PointSources.Clear();
                LL.Clear();

                line = sr.ReadLine();
                for (int i = 0; i < NUMBR; i++)
                {
                    line = sr.ReadLine();
                    buf_int = TypeConverterEx.Split<int>(line, 6);
                    var trb = new SourceInfo()
                     {
                         NO = buf_int[0],
                         MFH = buf_int[1],
                         MQ = buf_int[2],
                         MS = buf_int[3],
                         LowerSectionID = buf_int[4],
                         UpperSectionID = buf_int[5]
                     };
                    var strs = TypeConverterEx.Split<string>(line, 7);
                    trb.Name = strs[6];
                    Tributaries.Add(trb);
                }

                line = sr.ReadLine();
                for (int i = 0; i < INODE; i++)
                {
                    line = sr.ReadLine();
                    buf_int = TypeConverterEx.Split<int>(line, 5);
                    var pt = new SourceInfo()
                      {
                          NO = buf_int[0],
                          MFH = buf_int[1],
                          MQ = buf_int[2],
                          MS = buf_int[3],
                          LowerSectionID = buf_int[4],                    
                      };
                    var strs = TypeConverterEx.Split<string>(line, 6);
                    pt.Name = strs[5];
                    PointSources.Add(pt);
                }

                line = sr.ReadLine();//＝＝＝＝＝＝＝糙率参数＝＝＝＝＝＝＝	
                line = sr.ReadLine();
                var str_buf = TypeConverterEx.Split<string>(line, 2);
                NT = int.Parse(str_buf[1]);
                TTD = new double[NT];
                line = sr.ReadLine();
                str_buf = TypeConverterEx.Split<string>(line, NT + 1);
                for (int i = 0; i < NT; i++)
                {
                    TTD[i] = double.Parse(str_buf[i + 1]);
                }

                line = sr.ReadLine();
                line = sr.ReadLine();
                str_buf = TypeConverterEx.Split<string>(line, 2);
                MD = int.Parse(str_buf[1]);
               
                line = sr.ReadLine();
                str_buf = TypeConverterEx.Split<string>(line, MD + 2);
                for (int i = 0; i < MD+1; i++)
                {
                    var src=new SourceInfo()
                    {
                         NO = int.Parse(str_buf[i + 1])
                    };
                    LL.Add(src);
                }

                line = sr.ReadLine();
                line = sr.ReadLine();
                str_buf = TypeConverterEx.Split<string>(line, 2);
                MNN = int.Parse(str_buf[1]);
                line = sr.ReadLine();
                str_buf = TypeConverterEx.Split<string>(line, 2);
                MT3 = int.Parse(str_buf[1]);
                line = sr.ReadLine();
                str_buf = TypeConverterEx.Split<string>(line, MT3 + 1);
                QP3 = new double[MT3];
                for (int i = 0; i < MT3; i++)
                {
                    QP3[i] = double.Parse(str_buf[i + 1]);
                }
                line = sr.ReadLine();
                str_buf = TypeConverterEx.Split<string>(line, 2);
                DDDT = double.Parse(str_buf[1]);

                line = sr.ReadLine();
                CN3 = new double[MT3, MD + 1];
                CNP3 = new double[MT3, MD + 1];
                line = sr.ReadLine();
                for (int i = 0; i < MT3; i++)
                {
                    line = sr.ReadLine();
                    var double_buf = TypeConverterEx.Split<double>(line, MD + 2);
                    for (int j = 0; j < MD + 1; j++)
                    {
                        CN3[i, j] = double_buf[j + 1];
                    }
                }
                line = sr.ReadLine();
                for (int i = 0; i < MT3; i++)
                {
                    line = sr.ReadLine();
                    var double_buf = TypeConverterEx.Split<double>(line, MD + 2);
                    for (int j = 0; j < MD + 1; j++)
                    {
                        CNP3[i, j] = double_buf[j + 1];
                    }
                }

                line = sr.ReadLine();
                line = sr.ReadLine();//＝＝＝＝＝＝泥沙参数＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
                Sendiments = new List<string>();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    Sendiments.Add(line);
                }
                sr.Close();
                State = ModelObjectState.Ready;
                return LoadingState.Normal;
            }
            else
            {
                OnLoadFailed("参数文件不存在：" + FileName);
                return LoadingState.FatalError;
            }
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            StreamWriter sw = new StreamWriter(filename);
            string line = "＝＝＝＝＝＝＝水流参数＝＝＝＝＝＝＝";
            sw.WriteLine(line);
            sw.WriteLine("LMODE	MFUNC	PDTH	IDBUG	ENDTIME	DAYIDBUG");
            sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", LMODE, MFUNC, PDTH, IDBUG, ENDTIME, DAYIDBUG));
            sw.WriteLine("NPXT	NPJMAX	ZDEAD	ZNORMAL	MLEIJI");
            sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", NPXT, NPJMAX, ZDEAD, ZNORMAL, MLEIJI));
            sw.WriteLine("NUMBR	INODE               !支流入汇总数目、节点入汇总数目");
            sw.WriteLine(string.Format("{0}\t{1}", NUMBR, INODE));
            sw.WriteLine("NO1	MFH1	MQ1	MS1	NMODE	!支流编号、分汇类型、Q形式、S形式、进出口断面编号");
            for (int i = 0; i < NUMBR; i++)
            {
                line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", Tributaries[i].NO, Tributaries[i].MFH, Tributaries[i].MQ, Tributaries[i].MS, 
                    Tributaries[i].UpperSectionID,Tributaries[i].LowerSectionID, Tributaries[i].Name);
                sw.WriteLine(line);
            }
            sw.WriteLine("NO2	MFH1	MQ1	MS1	NMODE	!支流编号、分汇类型、Q形式、S形式、下游断面编号");
            for (int i = 0; i < INODE; i++)
            {
                line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", PointSources[i].NO, PointSources[i].MFH, PointSources[i].MQ, PointSources[i].MS,
                    PointSources[i].LowerSectionID, PointSources[i].Name);
                sw.WriteLine(line);
            }
            sw.WriteLine("＝＝＝＝＝＝＝糙率参数＝＝＝＝＝＝＝");
            sw.WriteLine(string.Format("时段个数NT\t{0}", NT));
            var ttd = string.Join("\t", TTD);
            sw.WriteLine(string.Format("各时段TTD\t{0}", ttd));
            sw.WriteLine(" ");
            sw.WriteLine(string.Format("河段数MD\t{0}", MD));
            var nos = from l in LL select l.NO;
            var buf = string.Join("\t", nos);
            sw.WriteLine(string.Format("河段分界断面LL(MD+1)\t{0}", buf));
            sw.WriteLine(" ");
            sw.WriteLine(string.Format("糙率插值方法MNN\t{0}", MNN));
            sw.WriteLine(string.Format("流量级个数MT3\t{0}", MT3));
            buf = string.Join("\t", QP3);
            sw.WriteLine(string.Format("各流量级QP3(MT3)\t{0}", buf));
            sw.WriteLine(string.Format("预估平衡时间（天）DDDT\t{0}", DDDT));
            sw.WriteLine("各流量级(行)各河段（列）糙率CN3(MD3,MQ3)和CNP3(MD3,MQ3)");
            sw.WriteLine("初始糙率CN3(MT3,MD+1)");
            for (int i = 0; i < MT3; i++)
            {
                line = (i + 1).ToString();
                for (int j = 0; j < MD + 1; j++)
                {
                    line += "\t" + CN3[i, j];
                }
                sw.WriteLine(line);
            }
            sw.WriteLine("平衡时糙率CNP3(MT3,MD+1)");
            for (int i = 0; i < MT3; i++)
            {
                line = (i + 1).ToString();
                for (int j = 0; j < MD + 1; j++)
                {
                    line += "\t" + CNP3[i, j];
                }
                sw.WriteLine(line);
            }
            sw.WriteLine("");
            sw.WriteLine("＝＝＝＝＝＝泥沙参数＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝	");
            foreach (var str in Sendiments)
            {
                sw.WriteLine(str);
            }
            sw.Close();
        }
    
        public void AddSourceInfoTo(List<SourceInfo> target)
        {
            SourceInfo newinfo = new SourceInfo();
            if (target.Count > 0)
            {
                var buf = from info in target select info.NO;
                var no = buf.Max() + 1;
                newinfo.NO = no;
            }
            else
            {
                newinfo.NO = 1;
            }
            target.Add(newinfo);
        }

        public void RemoveSourceInfoFrom(SourceInfo info, List<SourceInfo> target)
        {
            if(target.Contains(info))
                target.Remove(info);
        }
    }
}