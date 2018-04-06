﻿//
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

#define DEBUG
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Models.Integration;
using Heiflow.Core.IO;
using Heiflow.Core.Data;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.Hydrology;
using Heiflow.Models.UI;
using Heiflow.Models.IO;

namespace Heiflow.Models.Subsurface
{
    public class SFROutputPackage : MFDataPackage
    {
        public static string PackageName = "SFR Output";
        private SFRPackage _SFRPackage;
        public SFROutputPackage(SFRPackage sfr)
        {
            Name = PackageName;
            IsReadSSData = false;
            IsLoadCompleteData = false;
#if DEBUG
            _MaxTimeStep = 100;
#else
              _MaxTimeStep = -1;
#endif
            SkippedSteps = 1;
            DefaultAttachedVariables = new string[] { "Flow into stream", "Stream loss", "Flow out of stream", "Overland runoff","Direct pricipitation", "Stream ET"
            ,"Stream head", "Stream depth","Stream width", "Stream conductance", "Flow to water table", "Change of unsat. stor.", "Groundwater head"};
            ReachIndex = new List<Tuple<int, int, int>>();
            _SFRPackage = sfr;
            _Layer3DToken = "SFR";
            
        }

        [Browsable(false)]
        public string[] DefaultAttachedVariables
        {
            get;
            private set;
        }

        public bool IsReadSSData
        {
            get;
            set;
        }

        public bool IsLoadCompleteData
        {
            get;
            set;
        }

        [Browsable(false)]
        public RiverNetwork RiverNetwork
        {
            get;
            private set;
        }
        [Browsable(false)]
        public List<Tuple<int, int, int>> ReachIndex
        {
            get;
            private set;
        }
        [Browsable(false)]
        [PackageOptionalViewItem("SFR Output")]
        public override IPackageOptionalView OptionalView
        {
            get;
            set;
        }
        public override bool Scan()
        {
            Variables = DefaultAttachedVariables;
            NumTimeStep = TimeService.Timeline.Count;
            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep;
            Start = TimeService.Start;
            End = EndOfLoading;
            return true;
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.Owner.TimeServiceList["Base Timeline"];
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            State = ModelObjectState.Ready;
            _Initialized = true;
        }
        public override bool Load()
        {
            if (File.Exists(this.PackageInfo.FileName))
            {
                var network = this._SFRPackage.RiverNetwork;
                RiverNetwork = network;
                if (network == null)
                {
                    return false;
                }
                else
                {
                    ReachIndex.Clear();
                    int reachNum = network.ReachCount;
                    int nstep = StepsToLoad;

                    if (PackageInfo.Format == FileFormat.Text)
                    {
                        OnLoading(0);
                        FileStream fs = new FileStream(this.PackageInfo.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);

                        string line = "";
                        int varLen = DefaultAttachedVariables.Length;
                        // MyLazy3DMat<float> mat_buf = Values; 
                        int index = 0;

                        int progress = 0;
                        if (!IsLoadCompleteData)
                        {
                            reachNum = network.RiverCount;
                        }

                        if (IsReadSSData)
                        {
                            SkippedSteps = SkippedSteps - 1;
                        }
                        for (int t = 0; t < SkippedSteps * network.ReachCount + SkippedSteps * 8; t++)
                        {
                            if (!sr.EndOfStream)
                                line = sr.ReadLine();
                        }

                        for (int i = 0; i < network.RiverCount; i++)
                        {
                            for (int j = 0; j < network.Rivers[i].Reaches.Count; j++)
                            {
                                ReachIndex.Add(Tuple.Create(i, j, index));
                                index++;
                            }
                        }
                        OnLoading(progress);
                        try
                        {
                            if (Values == null || Values.Size[1] != nstep ||  Values.Size[2]!= reachNum)
                            {
                                Values = new MyLazy3DMat<float>(varLen, nstep, reachNum)
                                {
                                    Name = "SFR_Output",
                                    AllowTableEdit = false,
                                    TimeBrowsable = true
                                };
                                for (int i = 0; i < varLen; i++)
                                {
                                    Values.Allocate(i, nstep, reachNum);
                                }
                                Values.DateTimes = new DateTime[nstep];
                            }
                        }
                        catch (Exception)
                        {
                            Message = "Out of memory.";
                            return false;
                        }
                        for (int t = 0; t < nstep; t++)
                        {
                            for (int c = 0; c < 8; c++)
                                sr.ReadLine();
                            int rch_index = 0;
                            for (int i = 0; i < network.RiverCount; i++)
                            {
                                if (IsLoadCompleteData)
                                {
                                    for (int j = 0; j < network.Rivers[i].Reaches.Count; j++)
                                    {
                                        line = sr.ReadLine();
                                        if (TypeConverterEx.IsNotNull(line))
                                        {
                                            var temp = TypeConverterEx.SkipSplit<float>(line, 5);
                                            for (int v = 0; v < varLen; v++)
                                            {
                                                Values.Value[v][t][rch_index] = temp[v];
                                            }
                                        }
                                        else
                                        {
                                            //Debug.WriteLine(String.Format("step:{0} seg:{1} reach:{2}", t, i + 1, j + 1));
                                            goto finished;  
                                        }
                                        rch_index++;
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < network.Rivers[i].Reaches.Count - 1; j++)
                                    {
                                        line = sr.ReadLine();
                                        if (TypeConverterEx.IsNull(line))
                                            goto finished;
                                    }
                                    line = sr.ReadLine();
                                    var temp = TypeConverterEx.SkipSplit<float>(line, 5);
                                    for (int v = 0; v < varLen; v++)
                                    {
                                        Values.Value[v][t][i] = temp[v];
                                    }
                                }
                            }
                            Values.DateTimes[t] = TimeService.Timeline[t];
                            progress = t * 100 / nstep;
                            OnLoading(progress);
                        }
                    finished:
                        {
                             OnLoading(99);
                        }
                        OnLoading(100);
                        sr.Close();
                        fs.Close();
                        if(IsLoadCompleteData)
                            Values.Topology = _SFRPackage.ReachTopology;
                        else
                            Values.Topology = _SFRPackage.SegTopology;
                        
                        Values.Variables = DefaultAttachedVariables;
                        Values.TimeBrowsable = true;
                        Variables = DefaultAttachedVariables;
                        OnLoaded(Values);
                        return true;
                    }
                    else
                    {
                        if (UseSpecifiedFile)
                            FileName = SpecifiedFileName;
                        DataCubeXStreamReader stream = new DataCubeXStreamReader(FileName);
                        stream.Scale = (float)this.ScaleFactor;
                        stream.StepsToLoad = StepsToLoad;
                        stream.Loading += stream_LoadingProgressChanged;
                        stream.Loaded += stream_Loaded;
                        stream.Load();
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        public override void Clear()
        {
            if(_Initialized)
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            State = ModelObjectState.Standby;
            _Initialized = false;
        }
        private void stream_LoadingProgressChanged(object sender, int e)
        {
            OnLoading(e);
        }

        private void stream_Loaded(object sender, MyLazy3DMat<float> e)
        {
            Values = e;
            Variables = Values.Variables;

            if (IsLoadCompleteData)
                Values.Topology = _SFRPackage.ReachTopology;
            else
                Values.Topology = _SFRPackage.SegTopology;
            OnLoaded(e);
        }

        public override bool Load(int var_index)
        {  
            if (File.Exists(this.PackageInfo.FileName))
            {
                var network = _SFRPackage.RiverNetwork;
                RiverNetwork = network;
                if (network == null)
                {
                    return false;
                }
                else
                {
                    ReachIndex.Clear();
                    int reachNum = network.ReachCount;
                    int nstep = StepsToLoad;
                   // nstep = Math.Min(nmaxstep, nstep_option);
                    OnLoading(0);
                    FileStream fs = new FileStream(this.PackageInfo.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);

                    string line = "";
                    int varLen = DefaultAttachedVariables.Length;
                    int index = 0;
                    int progress = 0;
                    if (!IsLoadCompleteData)
                    {
                        reachNum = network.RiverCount;
                    }

                    if (IsReadSSData)
                    {
                        SkippedSteps = SkippedSteps - 1;
                    }
                    for (int t = 0; t < SkippedSteps * network.ReachCount + SkippedSteps * 8; t++)
                    {
                        if (!sr.EndOfStream)
                            line = sr.ReadLine();
                    }

                    for (int i = 0; i < network.RiverCount; i++)
                    {
                        for (int j = 0; j < network.Rivers[i].Reaches.Count; j++)
                        {
                            ReachIndex.Add(Tuple.Create(i, j, index));
                            index++;
                        }
                    }

                    OnLoading(progress);
                    try
                    {
                        if (Values == null)
                        {
                            Values = new MyLazy3DMat<float>(varLen, nstep, reachNum);
                            Values.Allocate(var_index, nstep, reachNum);
                            Values.DateTimes = new DateTime[nstep];
                        }

                        Values.Allocate(var_index, nstep, reachNum);
                        Values.DateTimes = new DateTime[nstep];
                        Values.TimeBrowsable = true;
                        Values.AllowTableEdit = false;
                    }
                    catch (Exception)
                    {
                        Message = "Out of memory.";
                        OnLoadFailed(Message);
                        return false;
                    }
                    for (int t = 0; t < nstep; t++)
                    {
                        for (int c = 0; c < 8; c++)
                            sr.ReadLine();
                        int rch_index = 0;
                        for (int i = 0; i < network.RiverCount; i++)
                        {
                            if (IsLoadCompleteData)
                            {
                                for (int j = 0; j < network.Rivers[i].Reaches.Count; j++)
                                {
                                    line = sr.ReadLine().Trim();
                                    if (line != "")
                                    {
                                        var temp = TypeConverterEx.SkipSplit<float>(line, 5);
                                        Values.Value[var_index][t][rch_index] = temp[var_index];
                                    }
                                    else
                                    {
                                        Debug.WriteLine(String.Format("step:{0} seg:{1} reach:{2}", t, i + 1, j + 1));
                                    }
                                    rch_index++;
                                }
                            }
                            else
                            {
                                for (int j = 0; j < network.Rivers[i].Reaches.Count - 1; j++)
                                {
                                    line = sr.ReadLine().Trim();
                                }
                                line = sr.ReadLine().Trim();
                                var temp = TypeConverterEx.SkipSplit<float>(line, 5);
                                Values.Value[var_index][t][i] = temp[var_index];
                            }
                        }
                        Values.DateTimes[t] = TimeService.Timeline[t];
                        progress = t * 100 / nstep;
                        OnLoading(progress);
                    }
                    OnLoading(100);
                    sr.Close();
                    fs.Close();
                    if (IsLoadCompleteData)
                        Values.Topology = _SFRPackage.ReachTopology;
                    else
                        Values.Topology = _SFRPackage.SegTopology;
                    Values.Variables = DefaultAttachedVariables;
                    Values.TimeBrowsable = true;
                    Variables = DefaultAttachedVariables;
                    OnLoaded(Values);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public FloatTimeSeries GetTimeSeries(int segIndex, int rchIndex, int varid, DateTime start)
        {
            FloatTimeSeries ts = null;
            if (Values != null)
            {
                var scaleFactor = ScaleFactor;
                var index = GetReachIndex(segIndex, rchIndex);
                var vector = Values.GetVector(varid, MyMath.full, index);
                DateTime[] dates = new DateTime[Values.Size[1]];
                for (int t = 0; t < Values.Size[1]; t++)
                {
                    dates[t] = start.AddDays(t);
                }
                MatrixOperation.Mulitple(vector, (float)scaleFactor);
                ts = new FloatTimeSeries()
                {
                    DateTimes = dates,
                    Value = vector
                };

            }
            return ts;
        }

        public IVectorTimeSeries<float> GetTimeSeries(int segIndex, int varid, DateTime start)
        {
            IVectorTimeSeries<float> ts = null;
            if (Values != null)
            {
                var scaleFactor = ScaleFactor;
                var vector = Values.GetVector(varid, MyMath.full, segIndex);
                DateTime[] dates = new DateTime[Values.Size[1]];
                for (int t = 0; t < Values.Size[1]; t++)
                {
                    dates[t] = Values.DateTimes[t];
                }
                MatrixOperation.Mulitple(vector, (float)scaleFactor);
                ts = new FloatTimeSeries()
                {
                    DateTimes = dates,
                    Value = vector
                };
                if (TimeUnits != Core.TimeUnits.Day)
                {
                    ts = TimeSeriesAnalyzer.Derieve(ts, NumericalDataType, TimeUnits);
                }
            }
            return ts;
        }

        public int GetReachIndex(int segIndex, int rchIndex)
        {
            var index = (from ind in ReachIndex where ind.Item1 == segIndex && ind.Item2 == rchIndex select ind.Item3).Single();
            return index;
        }

        /// <summary>
        /// return matrix [2][nrch], matrx[0] stores length,matrx[1] stores variable
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="varIndex"></param>
        /// <param name="current"></param>
        /// <param name="allReach"></param>
        /// <param name="unified"></param>
        /// <returns></returns>
        public My2DMat<double> ProfileTimeSeries(List<River> profile, int varIndex, int current, bool allReach, bool unified)
        {
            My2DMat<double> mat = null;
            int startday = 0;
            var scaleFactor = ScaleFactor;

            if (allReach)
            {
                int count = 0;
                foreach (var river in profile)
                {
                    count += river.Reaches.Count;
                }
                mat = new My2DMat<double>(2, count);
                int i = 0;
                double sumlen = 0;
                if (unified)
                {
                    foreach (var r in profile)
                    {
                        foreach (var reach in r.Reaches)
                        {
                            int index = GetReachIndex(r.ID - 1, reach.SubID - 1);
                            sumlen += reach.Length;
                            mat.Value[0][i] = sumlen;
                            mat.Value[1][i] = Values[varIndex, startday + current, index] * scaleFactor / reach.Length;
                            i++;
                        }
                    }
                }
                else
                {
                    foreach (var r in profile)
                    {
                        foreach (var reach in r.Reaches)
                        {
                            int index = GetReachIndex(r.ID - 1, reach.SubID - 1);
                            sumlen += reach.Length;
                            mat[0, i, 0] = sumlen;
                            mat[1, i, 0] = Values[varIndex, startday + current, index] * scaleFactor;
                            i++;
                        }
                    }
                }
            }
            else
            {
                if (Values != null)
                {
                    mat = new My2DMat<double>(2, profile.Count);
                    int i = 0;
                    double sumlen = 0;
                    if (unified)
                    {
                        foreach (var r in profile)
                        {
                            int index = r.ID - 1;
                            sumlen += r.Length;
                            mat[0, i, 0] = sumlen;
                            mat[1, i, 0] = Values[varIndex, startday + current, index] * scaleFactor / r.LastReach.Length;
                            i++;
                        }
                    }
                    else
                    {
                        foreach (var r in profile)
                        {
                            int index = r.ID - 1;
                            sumlen += r.Length;
                            mat[0, i, 0] = sumlen;
                            mat[1, i, 0] = Values[varIndex, startday + current, index] * scaleFactor;
                            i++;
                        }
                    }
                }
            }
            return mat;
        }

        public My3DMat<float> GetProfileTimeSeries(List<River> profile, int varIndex, string var_name,  int total_time, bool allReach, bool unified)
        {
            My3DMat<float> mat = null;
            var scaleFactor = ScaleFactor;

            if (allReach)
            {
                int count = 0;
                foreach (var river in profile)
                {
                    count += river.Reaches.Count;
                }
                mat = new My3DMat<float>(1, total_time, count);
                if (unified)
                {
                    for (int t = 0; t < total_time; t++)
                    {
                        int i = 0;
                        foreach (var r in profile)
                        {
                            foreach (var reach in r.Reaches)
                            {
                                int index = GetReachIndex(r.ID - 1, reach.SubID - 1);
                                mat.Value[0][t][i] = (float)(Values[varIndex, t, index] * scaleFactor / reach.Length);
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    for (int t = 0; t < total_time; t++)
                    {
                        int i = 0;
                        foreach (var r in profile)
                        {
                            foreach (var reach in r.Reaches)
                            {
                                int index = GetReachIndex(r.ID - 1, reach.SubID - 1);
                                mat.Value[0][t][i] =  (float)(Values[varIndex, t, index] * scaleFactor);
                                i++;
                            }
                        }
                    }
                }
            }
            else
            {
                if (Values != null)
                {
                    mat = new My3DMat<float>(1, total_time, profile.Count);

                    if (unified)
                    {
                        for (int t = 0; t < total_time; t++)
                        {
                            int i = 0;
                            foreach (var r in profile)
                            {
                                int index = r.ID - 1;
                                mat.Value[0][t][i] = (float)(Values[varIndex, t, index] * scaleFactor / r.LastReach.Length);
                                i++;
                            }
                        }
                    }
                    else
                    {
                        for (int t = 0; t < total_time; t++)
                        {
                            int i = 0;
                            foreach (var r in profile)
                            {
                                int index = r.ID - 1;
                                mat.Value[0][t][i] = (float)(Values[varIndex, t, index] * scaleFactor);
                                i++;
                            }
                        }
                    }
                }
            }
            mat.Name = var_name;
            mat.Variables = new string[] { var_name };
            return mat;
        }

        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            this.Feature = this._SFRPackage.Feature;
            this.FeatureLayer = this._SFRPackage.FeatureLayer;
        }
    }
}