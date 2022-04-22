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
using DotSpatial.Data;
using Heiflow.Models.Properties;

namespace Heiflow.Models.Subsurface
{
    public class SFROutputPackage : MFDataPackage
    {
        public static string PackageName = "SFR Output";
        private SFRPackage _SFRPackage;
        private Dictionary<string, string> _defaul_var_abv;
        public SFROutputPackage(SFRPackage sfr)
        {
            Name = PackageName;
            IsReadSSData = true;
            IsLoadCompleteData = false;
#if DEBUG
            _MaxTimeStep = 100;
#else
              _MaxTimeStep = -1;
#endif
            SkippedSteps = 1;
            DefaultAttachedVariables = new string[] { "Flow into stream", "Stream loss", "Flow out of stream", "Overland runoff","Direct pricipitation", "Stream ET"
            ,"Stream head", "Stream depth","Stream width", "Stream conductance", "Flow to water table", "Change of unsat. stor.", "Groundwater head"};
            DefaultVariablesAbbrv = new string[] { "FlowIn", "FlowLoss", "FlowOut", "Runoff","RiverRain", "RiverET"
            ,"RiverHead", "RiverDepth","RiverWidth", "RivConduct", "FlowToGW", "UnsatStor", "GWHead"};
            _defaul_var_abv = new Dictionary<string, string>();
            for (int i = 0; i < DefaultAttachedVariables.Length;i++ )
            {
                _defaul_var_abv.Add(DefaultAttachedVariables[i], DefaultVariablesAbbrv[i]);
            }
                ReachIndex = new List<Tuple<int, int, int>>();
            _SFRPackage = sfr;
            _Layer3DToken = "SFR";
            Variables = DefaultAttachedVariables;
            Category = Resources.ModelOutput; 
            Offset = 0;
            OutputFormat = FileFormat.Text;
        }
        [Browsable(false)]
        public SFRPackage SFRPackage
        {
            get
            {
                return _SFRPackage;
            }
        }
        [Browsable(false)]
        public string[] DefaultAttachedVariables
        {
            get;
            private set;
        }
        [Browsable(false)]
        public string[] DefaultVariablesAbbrv
        {
            get;
            private set;
        }
        public bool IsReadSSData
        {
            get;
            set;
        }
             [Browsable(false)]
        public float Offset
        {
            get;
            set;
        }
             [Browsable(false)]
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
        public FileFormat OutputFormat
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
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            if(this.Owner.Owner != null)
                this.TimeService = Owner.Owner.TimeServiceList["Base Timeline"];
            else
                this.TimeService = Owner.TimeServiceList["Subsurface Timeline"];
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            State = ModelObjectState.Ready;
            StartOfLoading = TimeService.Start;
            EndOfLoading = TimeService.End;
            NumTimeStep = TimeService.IOTimeline.Count;
            if (this.FileName.Contains(".dcx"))
            {
                this.OutputFormat = FileFormat.Binary;
                DefaultAttachedVariables = new string[] { "Stream loss", "Flow out of stream", 
                    "Overland runoff", "Stream head", "Groundwater head" };
                Variables = DefaultAttachedVariables;
            }
            else
            {
                this.OutputFormat = FileFormat.Text;
            }
            _Initialized = true;
        }
        public override bool Scan()
        {
            
            NumTimeStep = TimeService.GetIOTimeLength(this.Owner.WorkDirectory);
            if (NumTimeStep > 0)
            {
                _StartLoading = TimeService.Start;
                MaxTimeStep = NumTimeStep;
            }
            var network = _SFRPackage.RiverNetwork;
            var index = 0;
            ReachIndex.Clear();
            for (int i = 0; i < network.RiverCount; i++)
            {
                for (int j = 0; j < network.Rivers[i].Reaches.Count; j++)
                {
                    ReachIndex.Add(Tuple.Create(i, j, index));
                    index++;
                }
            }
            return true;
        }
        public override LoadingState Load(ICancelProgressHandler progresshandler)
        {
            _ProgressHandler = progresshandler;
            _NumTimeStep = TimeService.GetIOTimeLength(ModelService.WorkDirectory);
            var filename = LocalFileName;
            var result = LoadingState.Normal;
            if (File.Exists(filename))
            {
                if(OutputFormat == FileFormat.Text)
                    result = LoadAllVarsFromText(filename, progresshandler);
                else
                    result = LoadAllVarsFromBinary(filename, progresshandler);
            }
            else
            {
                Message = "The file does not exist: " + filename;
                ShowWarning(Message, progresshandler);
                result = LoadingState.Warning;
            }

            OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
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

        private void stream_Loaded(object sender, DataCube<float> e)
        {
            DataCube = e;
            Variables = DataCube.Variables;

            if (IsLoadCompleteData)
                DataCube.Topology = _SFRPackage.ReachTopology;
            else
                DataCube.Topology = _SFRPackage.SegTopology;
            OnLoaded(_ProgressHandler,new LoadingObjectState());
        }

        public override LoadingState Load(int var_index, ICancelProgressHandler progresshandler)
        {
            _ProgressHandler = progresshandler;
            _NumTimeStep = TimeService.GetIOTimeLength(ModelService.WorkDirectory);
             var  filename =LocalFileName;
             var result = LoadingState.Normal;
            if (File.Exists(filename))
            {
                var network = _SFRPackage.RiverNetwork;
                RiverNetwork = network;
                if (network == null)
                {
                    Message = "The river network does not exist.";
                    ShowWarning(Message, progresshandler);
                    result = LoadingState.Warning;
                    OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
                    return result;
                }
                else
                {
                    if(OutputFormat == FileFormat.Text)
                    result = LoadSingleVarFromText(filename, var_index, progresshandler);
                    else
                        result = LoadSingleVarFromBanary(filename, var_index, progresshandler);
                }
            }
            else
            {
                ShowWarning("The file does not exist: " + filename, progresshandler);
                result = LoadingState.Warning;
            }

            OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }

        private LoadingState LoadSingleVarFromText(string filename, int var_index, ICancelProgressHandler progresshandler)
        {
            var network = _SFRPackage.RiverNetwork;
            int reachNum = network.ReachCount;
            int count = 1;
            int nstep = StepsToLoad;
            var result = LoadingState.Normal;

            OnLoading(0);
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            string line = "";
            int varLen = DefaultAttachedVariables.Length;
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

            OnLoading(progress);
            try
            {
                DataCube = new DataCube<float>(varLen, nstep, reachNum, true)
                {
                    Name = "SFR_Output",
                };
                DataCube.Allocate(var_index);
                DataCube.DateTimes = new DateTime[nstep];
            }
            catch (Exception ex)
            {
                Message = "Out of memory. Error message: " + ex.Message;
                ShowWarning(Message, progresshandler);
                result = LoadingState.Warning;
                OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
                return result;
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
                                //Values.Value[var_index][t][rch_index] = temp[var_index];
                                // DataCube.ILArrays[var_index].SetValue(temp[var_index], t, rch_index);
                                DataCube[var_index, t, rch_index] = temp[var_index];
                            }
                            else
                            {
                                Debug.WriteLine(String.Format("step:{0} seg:{1} reach:{2}", t, i + 1, j + 1));
                                goto finished;
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
                        DataCube[var_index, t, i] = temp[var_index];
                    }
                }
                DataCube.DateTimes[t] = TimeService.Timeline[t];
                progress = t * 100 / nstep;
                if (progress > count)
                {
                    OnLoading(progress);
                    count++;
                }
            }
        finished:
            {
                OnLoading(100);
            }
            sr.Close();
            fs.Close();
            if (IsLoadCompleteData)
                DataCube.Topology = _SFRPackage.ReachTopology;
            else
                DataCube.Topology = _SFRPackage.SegTopology;
            DataCube.Variables = DefaultAttachedVariables;
            Variables = DefaultAttachedVariables;
            result = LoadingState.Normal;

            return result;
        }

        private LoadingState LoadSingleVarFromBanary(string filename, int var_index, ICancelProgressHandler progresshandler)
        {
            var network = _SFRPackage.RiverNetwork;
            int reachNum = network.ReachCount;
            int count = 1;
            int nstep = StepsToLoad;
            var result = LoadingState.Normal;
            int feaNum = 0;
            int varnum = 0;

            OnLoading(0);
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            varnum = br.ReadInt32();
            Variables = new string[varnum];
            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }

            int progress = 0;
            int stepbyte = feaNum * 4 * varnum;
            if (!IsLoadCompleteData)
            {
                reachNum = network.RiverCount;
            }

            if (IsReadSSData)
            {
                SkippedSteps = SkippedSteps - 1;
            }
            for (int t = 0; t < SkippedSteps; t++)
            {
                br.ReadBytes(stepbyte);
            }

            OnLoading(progress);
            try
            {
                DataCube = new DataCube<float>(varnum, nstep, reachNum, true)
                {
                    Name = "SFR_Output",
                };
                DataCube.Allocate(var_index);
                DataCube.DateTimes = new DateTime[nstep];
            }
            catch (Exception ex)
            {
                Message = "Out of memory. Error message: " + ex.Message;
                ShowWarning(Message, progresshandler);
                result = LoadingState.Warning;
                OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
                return result;
            }
            var scale = (float)ScaleFactor;
            var lastreach_index_list = new int[network.RiverCount];
            for (int j = 0; j < network.RiverCount;j++ )
            {
                lastreach_index_list[j] = GetReachSerialIndex(network.Rivers[j].ID, network.Rivers[j].LastReach.SubID);
            }
            for (int t = 0; t < nstep; t++)
            {
                var buf = new float[feaNum];
                for (int s = 0; s < feaNum; s++)
                {
                    br.ReadBytes(4 * var_index);
                    buf[s] = br.ReadSingle() * scale;
                    br.ReadBytes(4 * (varnum - var_index - 1));
                }

                if (IsLoadCompleteData)
                {
                    DataCube.ILArrays[var_index][t, ":"] = buf;
                }
                else
                {
                    var last_vec = new float[network.RiverCount];
                    for (int i = 0; i < network.RiverCount; i++)
                    {
                        //rch_index = GetReachSerialIndex(network.Rivers[i].ID, network.Rivers[i].LastReach.SubID);
                        //DataCube[var_index, t, i] = buf[rch_index];
                        last_vec[i] = buf[lastreach_index_list[i]];
                    }
                    DataCube.ILArrays[var_index][t, ":"] = last_vec;
                }
                DataCube.DateTimes[t] = TimeService.Timeline[t];
                progress = t * 100 / nstep;
                if (progress > count)
                {
                    OnLoading(progress);
                    count++;
                }
            }
            br.Close();
            fs.Close();
            if (IsLoadCompleteData)
                DataCube.Topology = _SFRPackage.ReachTopology;
            else
                DataCube.Topology = _SFRPackage.SegTopology;
            DataCube.Variables = DefaultAttachedVariables;
            Variables = DefaultAttachedVariables;
            result = LoadingState.Normal;

            return result;
        }

        private LoadingState LoadAllVarsFromText(string filename,ICancelProgressHandler progresshandler)
        {
            var network = this._SFRPackage.RiverNetwork;
            RiverNetwork = network;
            int count = 1;
            var result = LoadingState.Normal;
            if (network == null)
            {
                result = LoadingState.Warning;
                Message = "The river network dose not exist.";
                OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
            }
            else
            {
                int reachNum = network.ReachCount;
                int nstep = StepsToLoad;

                if (PackageInfo.Format == FileFormat.Text)
                {
                    OnLoading(0);
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                    try
                    {
                        string line = "";
                        int varLen = DefaultAttachedVariables.Length;
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

                        OnLoading(progress);
                        try
                        {
                            DataCube = new DataCube<float>(varLen, nstep, reachNum)
                            {
                                Name = "SFR_Output",
                            };

                            DataCube.DateTimes = new DateTime[nstep];
                        }
                        catch (Exception)
                        {
                            Message = "Out of memory.";
                            result = LoadingState.Warning;
                            OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
                            return result;
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
                                                // DataCube.ILArrays[v].SetValue(temp[v], t, rch_index);
                                                //Values.Value[v][t][rch_index] = temp[v];
                                                DataCube[v, t, rch_index] = temp[v];
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
                                        DataCube[v, t, i] = temp[v];
                                    }
                                }
                            }
                            DataCube.DateTimes[t] = TimeService.Timeline[t];
                            progress = t * 100 / nstep;
                            if (progress > count)
                            {
                                OnLoading(progress);
                                count++;
                            }
                        }
                    finished:
                        {
                            OnLoading(100);
                        }

                        if (IsLoadCompleteData)
                            DataCube.Topology = _SFRPackage.ReachTopology;
                        else
                            DataCube.Topology = _SFRPackage.SegTopology;

                        DataCube.Variables = DefaultAttachedVariables;
                        Variables = DefaultAttachedVariables;
                        result = LoadingState.Normal;
                    }
                    catch (Exception ex)
                    {
                        result = LoadingState.Warning;
                        Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                        ShowWarning(Message, progresshandler);
                    }
                    finally
                    {
                        sr.Close();
                        fs.Close();
                    }
                }
                else
                {
                    if (UseSpecifiedFile)
                        FileName = SpecifiedFileName;
                    DataCubeStreamReader stream = new DataCubeStreamReader(FileName);
                    stream.Scale = (float)this.ScaleFactor;
                    stream.MaxTimeStep = this.MaxTimeStep;
                    stream.NumTimeStep = this.NumTimeStep;
                    stream.Loading += stream_LoadingProgressChanged;
                    stream.DataCubeLoaded += stream_Loaded;
                    stream.LoadDataCube();
                    result = LoadingState.Normal;
                }
            }

            return result;
        }

        private LoadingState LoadAllVarsFromBinary(string filename, ICancelProgressHandler progresshandler)
        {
            var network = _SFRPackage.RiverNetwork;
            RiverNetwork = network;
            int reachNum = network.ReachCount;
            int count = 1;
            int nstep = StepsToLoad;
            var result = LoadingState.Normal;
            int feaNum = 0;
            int varnum = 0;

            OnLoading(0);
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            varnum = br.ReadInt32();
            Variables = new string[varnum];
            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }

            int progress = 0;
            int stepbyte = feaNum * 4 * varnum;
            if (!IsLoadCompleteData)
            {
                reachNum = network.RiverCount;
            }

            if (IsReadSSData)
            {
                SkippedSteps = SkippedSteps - 1;
            }
            for (int t = 0; t < SkippedSteps; t++)
            {
                br.ReadBytes(stepbyte);
            }

            OnLoading(progress);
            try
            {
                DataCube = new DataCube<float>(varnum, nstep, reachNum)
                {
                    Name = "SFR_Output",
                };
                DataCube.DateTimes = new DateTime[nstep];
            }
            catch (Exception ex)
            {
                Message = "Out of memory. Error message: " + ex.Message;
                ShowWarning(Message, progresshandler);
                result = LoadingState.Warning;
                OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
                return result;
            }
            var scale = (float)ScaleFactor;
            var buf = new float[varnum, feaNum];
            for (int t = 0; t < nstep; t++)
            {
                int rch_index = 0;

                if (IsLoadCompleteData)
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int k = 0; k < varnum; k++)
                        {
                            DataCube[k, t, s] = br.ReadSingle() * scale;
                        }
                    }
                }
                else
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int k = 0; k < varnum; k++)
                        {
                            buf[k, s] = br.ReadSingle() * scale;
                        }
                    }
                    for (int i = 0; i < network.RiverCount; i++)
                    {
                        rch_index = GetReachSerialIndex(network.Rivers[i].ID, network.Rivers[i].LastReach.SubID);
                        for (int k = 0; k < varnum; k++)
                        {
                            DataCube[k, t, i] = buf[k, rch_index];
                        }
                    }
                }

                DataCube.DateTimes[t] = TimeService.Timeline[t];
                progress = t * 100 / nstep;
                if (progress > count)
                {
                    OnLoading(progress);
                    count++;
                }
            }
            br.Close();
            fs.Close();
            if (IsLoadCompleteData)
                DataCube.Topology = _SFRPackage.ReachTopology;
            else
                DataCube.Topology = _SFRPackage.SegTopology;
            DataCube.Variables = DefaultAttachedVariables;
            Variables = DefaultAttachedVariables;
            result = LoadingState.Normal;

            return result;
        }
        public DataCube<float> GetTimeSeries(int segIndex, int rchIndex, int varid, DateTime start)
        {
            DataCube<float> ts = null;
            if (DataCube != null)
            {
               // var scaleFactor = ScaleFactor;
                var index = GetReachIndex(segIndex, rchIndex);
                if (DataCube.IsAllocated(varid))
                {
                    var vector = DataCube.GetVector(varid, ":", index.ToString());
                    DateTime[] dates = new DateTime[DataCube.Size[1]];
                    for (int t = 0; t < DataCube.Size[1]; t++)
                    {
                        dates[t] = start.AddDays(t);
                    }
                   // MatrixOperation.Mulitple(vector, (float)scaleFactor,Offset);
                   
                    ts = new DataCube<float>(vector, dates);
                }
            }
            return ts;
        }

        public DataCube<float> GetTimeSeries(int segIndex, int varid)
        {
            DataCube<float> ts = null;
            if (DataCube != null && DataCube.IsAllocated(varid))
            {
                //var scaleFactor =  ScaleFactor;
                var vector = DataCube.GetVector(varid, ":", segIndex.ToString());
                DateTime[] dates = new DateTime[DataCube.Size[1]];
                for (int t = 0; t < DataCube.Size[1]; t++)
                {
                    dates[t] = DataCube.DateTimes[t];
                }
                //MatrixOperation.Mulitple(vector, (float)scaleFactor,Offset);
                ts = new DataCube<float>(vector, dates);
                if (TimeUnits != Core.TimeUnits.Day)
                {
                    ts = TimeSeriesAnalyzer.Derieve(ts, NumericalDataType, TimeUnits);
                }
            }
            //else if(DataCube != null)
            //{
            //    var scaleFactor = ScaleFactor;
            //    var vector = DataCube.GetVector(varid, ":",segIndex.ToString());
            //    DateTime[] dates = new DateTime[DataCube.Size[1]];
            //    for (int t = 0; t < DataCube.Size[1]; t++)
            //    {
            //        dates[t] = DataCube.DateTimes[t];
            //    }
            //    MatrixOperation.Mulitple(vector, (float)scaleFactor);
            //    ts = new DataCube<float>(vector, dates);
            //    if (TimeUnits != Core.TimeUnits.Day)
            //    {
            //        ts = TimeSeriesAnalyzer.Derieve(ts, NumericalDataType, TimeUnits);
            //    }
            //}
            return ts;
        }

        public string GetVarAbv(string sfrvar)
        {
            if(_defaul_var_abv.Keys.Contains(sfrvar))
            {
                return _defaul_var_abv[sfrvar];
            }
            else
            {
                return "para";
            }
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
        public DataCube<double> ProfileTimeSeries(List<River> profile, int varIndex, int current, bool allReach, bool unified)
        {
            DataCube<double> mat = null;
            int startday = 0;
            var scaleFactor = 1;// ScaleFactor;

            if (profile == null)
                return mat;

            if (allReach)
            {
                int count = 0;
                foreach (var river in profile)
                {
                    count += river.Reaches.Count;
                }
                mat = new DataCube<double>(2, 1, count);
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
                            mat[0, 0, i] = sumlen;
                            mat[1, 0, i] = DataCube[varIndex, startday + current, index] * scaleFactor / reach.Length;
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
                            mat[0, 0, i] = sumlen;
                            mat[1, 0, i] = DataCube[varIndex, startday + current, index] * scaleFactor;
                            i++;
                        }
                    }
                }
            }
            else
            {
                if (DataCube != null)
                {
                    mat = new DataCube<double>(2, 1, profile.Count);
                    int i = 0;
                    double sumlen = 0;
                    if (unified)
                    {
                        foreach (var r in profile)
                        {
                            int index = r.ID - 1;
                            sumlen += r.Length;
                            mat[0, 0, i] = sumlen;
                            mat[1, 0, i] = DataCube[varIndex, startday + current, index] * scaleFactor / r.LastReach.Length;
                            i++;
                        }
                    }
                    else
                    {
                        foreach (var r in profile)
                        {
                            int index = r.ID - 1;
                            sumlen += r.Length;
                            mat[0, 0, i] = sumlen;
                            mat[1, 0, i] = DataCube[varIndex, startday + current, index] * scaleFactor;
                            i++;
                        }
                    }
                }
            }
            return mat;
        }

        public DataCube<float> GetProfileTimeSeries(List<River> profile, int varIndex, string var_name, int total_time, bool allReach, bool unified)
        {
            DataCube<float> mat = null;
            var scaleFactor = ScaleFactor;

            if (allReach)
            {
                int count = 0;
                foreach (var river in profile)
                {
                    count += river.Reaches.Count;
                }
                mat = new DataCube<float>(1, total_time, count);
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
                                mat[0,t,i] = (float)(DataCube[varIndex, t, index] * scaleFactor / reach.Length);
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
                                mat[0,t,i] = (float)(DataCube[varIndex, t, index] * scaleFactor);
                                i++;
                            }
                        }
                    }
                }
            }
            else
            {
                if (DataCube != null)
                {
                    mat = new DataCube<float>(1, total_time, profile.Count);

                    if (unified)
                    {
                        for (int t = 0; t < total_time; t++)
                        {
                            int i = 0;
                            foreach (var r in profile)
                            {
                                int index = r.ID - 1;
                                mat[0,t,i] = (float)(DataCube[varIndex, t, index] * scaleFactor / r.LastReach.Length);
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
                                mat[0,t,i] = (float)(DataCube[varIndex, t, index] * scaleFactor);
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
        /// <summary>
        /// get reach serial index
        /// </summary>
        /// <param name="segid"></param>
        /// <param name="reachid"></param>
        /// <returns>serial index starting from 0</returns>
        public int GetReachSerialIndex(int segid, int reachid)
        {
            var buf = from rch in ReachIndex where rch.Item1 == (segid - 1) && rch.Item2 == (reachid - 1) select rch;
            if(buf.Any())
            {
                return buf.First().Item3;
            }
            else
            {
                return -1;
            }
        }
    }
}