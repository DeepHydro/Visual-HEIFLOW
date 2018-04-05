﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using System.IO;
using Heiflow.Core.IO;

namespace Heiflow.Models.IO
{
    public class MMSDataFile : BaseDataCube
    {
        private string _FileName;

        public MMSDataFile(string filename)
        {
            _FileName = filename;
        }

        public override void Scan()
        {
            NumTimeStep = 0;
            var fileStream = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fileStream, Encoding.Default);
            string line = "";
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (line == "" || line == null)
                    break;
                NumTimeStep++;
            }
            sr.Close();
            fileStream.Close();
        }

        public override My3DMat<float> Load(int var_index)
        {
            OnLoading(0);
              Scan();
            var fileStream = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fileStream, Encoding.Default);
            string line = "";
            int nfeat = 0;
            int nstep = StepsToLoad;
            int progress = 0;
            if (StepsToLoad < NumTimeStep && StepsToLoad > 0)
                nstep = StepsToLoad;

            sr.ReadLine();
            line = sr.ReadLine();
            sr.ReadLine();

            var strs = TypeConverterEx.Split<string>(line);
            nfeat = int.Parse(strs[1]);
        
            if (!Source.IsAllocated(var_index) || Source.Size[1] != nstep)
                Source.Allocate(var_index, nstep, nfeat);
            Source.DateTimes = new DateTime[nstep];

            for (int t = 0; t < nstep; t++)
            {
                line = sr.ReadLine();
                var vv = TypeConverterEx.SkipSplit<float>(line, 6);
                Source.Value[var_index][t] = vv;
                progress = Convert.ToInt32(t * 100 / nstep);
                var temp = TypeConverterEx.Split<int>(line, 6);
                Source.DateTimes[t] = new DateTime(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5]);
                if (progress % 10 == 0)
                    OnLoading(progress);
            }
            OnLoading(100);
            fileStream.Close();
            sr.Close();
            OnLoaded(Source);
            return Source;
        }

        /// <summary>
        /// load based on a mapping table
        /// </summary>
        /// <param name="mapping">[station_id,hru_id]</param>
        /// <returns>3d mat</returns>
        public My3DMat<float> Load(Dictionary<int, int> mapping, int var_index)
        {
            OnLoading(0);
             Scan();
            var fileStream = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fileStream, Encoding.Default);
            string line = "";
            int nfeat = 0;
            int nstep = StepsToLoad;
            int progress = 0;
            int nhru = mapping.Keys.Count;

            if (StepsToLoad < NumTimeStep && StepsToLoad > 0)
                nstep = StepsToLoad;

            sr.ReadLine();
            line = sr.ReadLine();
            sr.ReadLine();

            var strs = TypeConverterEx.Split<string>(line);
            nfeat = int.Parse(strs[1]);

            if (!Source.IsAllocated(var_index) || Source.Size[1] != nstep)
                Source.Allocate(var_index, nstep, nhru);
            Source.DateTimes = new DateTime[nstep];
            Source.TimeBrowsable = true;
            Source.AllowTableEdit = true;
            if (nhru == 0)
            {
                nhru = nfeat;
                for (int t = 0; t < nstep; t++)
                {
                    line = sr.ReadLine();
                    var vv = TypeConverterEx.SkipSplit<float>(line, 6);
                    for (int i = 0; i < nhru; i++)
                    {
                        Source.Value[var_index][t][i] = vv[i];
                    }
                    var temp = TypeConverterEx.Split<int>(line, 6);
                    Source.DateTimes[t] = new DateTime(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5]);
                    progress = Convert.ToInt32(t * 100 / nstep);
                    OnLoading(progress);
                }
            }
            else
            {
                for (int t = 0; t < nstep; t++)
                {
                    line = sr.ReadLine();
                    var vv = TypeConverterEx.SkipSplit<float>(line, 6);
                    for (int i = 0; i < nhru; i++)
                    {
                        Source.Value[var_index][t][i] = vv[mapping[i + 1]];
                    }
                    var temp = TypeConverterEx.Split<int>(line, 6);
                    Source.DateTimes[t] = new DateTime(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5]);
                    progress = Convert.ToInt32(t * 100 / nstep);
                    OnLoading(progress);
                }
            }

            OnLoading(100);
            fileStream.Close();
            sr.Close();
            OnLoaded(Source);
            return Source;
        }

        public void Save(My3DMat<float> mat)
        {
            StreamWriter sw = new StreamWriter(_FileName);
            string line = "Metrological File, generated by VHF on " + DateTime.Now.ToString();
            sw.WriteLine(line);
            line = mat.Variables[0] + "\t" + mat.Size[2];
            sw.WriteLine(line);
            line = "################";
            sw.WriteLine(line);
            string buf = "";
            for (int i = 0; i < mat.Size[1]; i++)
            {
                buf = string.Join("\t", mat.Value[0][i]);
                var cur = mat.DateTimes[i];
                line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", cur.Year, cur.Month, cur.Day, cur.Hour, cur.Minute, cur.Second, buf);
                sw.WriteLine(line);
            }
            sw.Close();
        }
        public override My3DMat<float> Load()
        {
            throw new NotImplementedException();
        }
    }
}