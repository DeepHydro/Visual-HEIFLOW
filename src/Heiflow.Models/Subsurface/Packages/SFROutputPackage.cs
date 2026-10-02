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
            DefaultWQVariables = new string[] { "NO3", "ON", "P", "OP", "SEDIMENT","CHL","CBOD","DO","NH3", "SO4", "FU" };
            DefaultSO4Variables = new string[] { "Storage", "Rain", "Interflow", "GW", "Point Source"};
            DefaultFUVariables = new string[] { "Storage", "Rain", "Interflow", "GW", "Point Source" };
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
        public string[] DefaultWQVariables
        {
            get;
            private set;
        }
        [Browsable(false)]
        public string[] DefaultSO4Variables
        {
            get;
            private set;
        }
        [Browsable(false)]
        public string[] DefaultFUVariables
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
                    "Overland runoff", "Stream head", "Groundwater head", "Concentration", "Load", "Concentration1", "Load1" };
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
            Message = "";

            if (_SFRPackage == null || _SFRPackage.RiverNetwork == null)
            {
                Message = "The SFR package or its river network is not available. Please load the SFR package first.";
                OnScanFailed(Message);
                return false;
            }
            if (TimeService == null || Owner == null || Owner.WorkDirectory == null)
            {
                Message = "The model time service is not initialized. Please check the time settings of the model.";
                OnScanFailed(Message);
                return false;
            }

            NumTimeStep = TimeService.GetIOTimeLength(this.Owner.WorkDirectory);
            if (NumTimeStep <= 0)
            {
                Message = string.Format("No output time step is found in \"{0}\". Please run the model first.", this.Owner.WorkDirectory);
                OnScanFailed(Message);
                return false;
            }
            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep;

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

        /// <summary>
        /// 文本文件没有变量头信息，将变量列表恢复为默认的文本输出变量集
        /// </summary>
        public void ResetVariablesToDefault()
        {
            Variables = DefaultAttachedVariables;
            if (NumTimeStep <= 0 && TimeService != null && Owner != null)
            {
                NumTimeStep = TimeService.GetIOTimeLength(Owner.WorkDirectory);
                MaxTimeStep = NumTimeStep;
            }
        }

        /// <summary>
        /// 扫描 dcx 文件中的变量名与时间步数
        /// </summary>
        /// <param name="filename">dcx 文件全路径</param>
        /// <returns>扫描成功返回 true；失败时通过 Message 给出原因</returns>
        public bool ScanVariables(string filename)
        {
            Message = "";

            if (string.IsNullOrWhiteSpace(filename))
            {
                Message = "The output file name is not specified.";
                OnScanFailed(Message);
                return false;
            }
            if (!File.Exists(filename))
            {
                Message = string.Format("The output file does not exist: {0}", filename);
                OnScanFailed(Message);
                return false;
            }

            try
            {
                DataCubeStreamReader stream = new DataCubeStreamReader(filename);
                var info = stream.GetFileInfo();
                if (info == null || info.VariableNames == null || info.VariableNames.Length == 0)
                {
                    Message = string.Format("No variable is found in \"{0}\". The file may be empty or corrupted.", filename);
                    OnScanFailed(Message);
                    return false;
                }
                Variables = info.VariableNames;
                NumTimeStep = info.TotalTimeSteps;
                if (TimeService != null)
                    _StartLoading = TimeService.Start;
                MaxTimeStep = NumTimeStep;
                return true;
            }
            catch (Exception ex)
            {
                Message = string.Format("Failed to scan variables from \"{0}\". Error message: {1}", filename, ex.Message);
                OnScanFailed(Message);
                return false;
            }
        }
        /// <summary>
        /// 读取一行并去除首尾空白；到达文件尾时返回 null，避免对 null 调用 Trim 引发异常
        /// </summary>
        private static string ReadLineSafe(StreamReader sr)
        {
            if (sr == null || sr.EndOfStream)
                return null;
            var line = sr.ReadLine();
            if (line == null)
                return null;
            return line.Trim();
        }

        public override LoadingState Load(ICancelProgressHandler progresshandler)
        {
            _ProgressHandler = progresshandler;
            Message = "";

            if (_SFRPackage == null || _SFRPackage.RiverNetwork == null)
            {
                Message = "The river network does not exist. Please load the SFR package first.";
                ShowWarning(Message, progresshandler);
                OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = LoadingState.Warning });
                return LoadingState.Warning;
            }

            var filename = LocalFileName;
            if (string.IsNullOrWhiteSpace(filename))
            {
                Message = "The output file name is not specified.";
                ShowWarning(Message, progresshandler);
                OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = LoadingState.Warning });
                return LoadingState.Warning;
            }

            var result = LoadingState.Normal;
            if (File.Exists(filename))
            {
                try
                {
                    if (OutputFormat == FileFormat.Text)
                        result = LoadAllVarsFromText(filename, progresshandler);
                    else
                        result = LoadAllVarsFromBinary(filename, progresshandler);
                }
                catch (Exception ex)
                {
                    Message = string.Format("Failed to read \"{0}\". Error message: {1}", filename, ex.Message);
                    ShowWarning(Message, progresshandler);
                    result = LoadingState.FatalError;
                }
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
            Message = "";
            var filename = LocalFileName;
            var result = LoadingState.Normal;

            if (_SFRPackage == null || _SFRPackage.RiverNetwork == null)
            {
                Message = "The river network does not exist. Please load the SFR package first.";
                result = LoadingState.Warning;
            }
            else if (string.IsNullOrWhiteSpace(filename))
            {
                Message = "The output file name is not specified.";
                result = LoadingState.Warning;
            }
            else if (!File.Exists(filename))
            {
                Message = "The file does not exist: " + filename;
                result = LoadingState.Warning;
            }
            else if (Variables == null || var_index < 0 || var_index >= Variables.Length)
            {
                Message = string.Format("The variable index {0} is out of range. Only {1} variable(s) are available in the file.",
                    var_index, Variables == null ? 0 : Variables.Length);
                result = LoadingState.Warning;
            }
            else
            {
                var network = _SFRPackage.RiverNetwork;
                RiverNetwork = network;
                try
                {
                    if (OutputFormat == FileFormat.Text)
                        result = LoadSingleVarFromText(filename, var_index, progresshandler);
                    else
                        result = LoadSingleVarFromBanary(filename, var_index, progresshandler);
                }
                catch (Exception ex)
                {
                    Message = string.Format("Failed to read \"{0}\". Error message: {1}", filename, ex.Message);
                    result = LoadingState.FatalError;
                }
            }

            if (result != LoadingState.Normal)
                ShowWarning(Message, progresshandler);

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

            int skippedSteps = SkippedSteps;
            if (IsReadSSData)
            {
                skippedSteps = Math.Max(0, SkippedSteps - 1);
            }
            for (int t = 0; t < skippedSteps * network.ReachCount + skippedSteps * 8; t++)
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
            var scale = (float)ScaleFactor;
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
                            line = ReadLineSafe(sr);
                            if (string.IsNullOrEmpty(line))
                            {
                                Message = string.Format("The output file ends at step {0} (segment {1}, reach {2}), earlier than the expected {3} step(s).",
                                    t + 1, i + 1, j + 1, nstep);
                                goto finished;
                            }
                            var temp = TypeConverterEx.SkipSplit<float>(line, 5);
                            //Values.Value[var_index][t][rch_index] = temp[var_index];
                            // DataCube.ILArrays[var_index].SetValue(temp[var_index], t, rch_index);
                            DataCube[var_index, t, rch_index] = temp[var_index] * scale;
                            rch_index++;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < network.Rivers[i].Reaches.Count - 1; j++)
                        {
                            if (ReadLineSafe(sr) == null)
                            {
                                Message = string.Format("The output file ends at step {0} (segment {1}), earlier than the expected {2} step(s).",
                                    t + 1, i + 1, nstep);
                                goto finished;
                            }
                        }
                        line = ReadLineSafe(sr);
                        if (string.IsNullOrEmpty(line))
                        {
                            Message = string.Format("The output file ends at step {0} (segment {1}), earlier than the expected {2} step(s).",
                                t + 1, i + 1, nstep);
                            goto finished;
                        }
                        var temp = TypeConverterEx.SkipSplit<float>(line, 5);
                        DataCube[var_index, t, i] = temp[var_index] * scale;
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

            if (!string.IsNullOrEmpty(Message))
            {
                ShowWarning(Message, progresshandler);
                result = LoadingState.Warning;
            }
            else
            {
                result = LoadingState.Normal;
            }

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
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

            int skippedSteps = SkippedSteps;
            if (IsReadSSData)
            {
                skippedSteps = Math.Max(0, SkippedSteps - 1);
            }
            for (int t = 0; t < skippedSteps; t++)
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
                    buf[s] = br.ReadSingle();
                    buf[s] = buf[s] * scale;
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
            //DataCube.Variables = DefaultAttachedVariables;
            //Variables = DefaultAttachedVariables;
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

                        int skippedSteps = SkippedSteps;
                        if (IsReadSSData)
                        {
                            skippedSteps = Math.Max(0, SkippedSteps - 1);
                        }
                        for (int t = 0; t < skippedSteps * network.ReachCount + skippedSteps * 8; t++)
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
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

            int skippedSteps = SkippedSteps;
            if (IsReadSSData)
            {
                skippedSteps = Math.Max(0, SkippedSteps - 1);
            }
            for (int t = 0; t < skippedSteps; t++)
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

        /// <summary>
        /// 获取河段的串行索引；未找到时返回 -1 并通过 Message 说明原因
        /// </summary>
        public int GetReachIndex(int segIndex, int rchIndex)
        {
            var buf = from ind in ReachIndex where ind.Item1 == segIndex && ind.Item2 == rchIndex select ind.Item3;
            if (buf.Any())
            {
                return buf.First();
            }
            Message = string.Format("The reach (segment index {0}, reach index {1}) is not found in the river network.", segIndex, rchIndex);
            return -1;
        }

        /// <summary>
        /// 检查指定变量的数据是否已就绪；未就绪时通过 message 返回可读的原因
        /// </summary>
        public bool IsVariableReady(int varIndex, out string message)
        {
            message = "";
            if (DataCube == null)
            {
                message = "No SFR output is loaded. Please load the data first.";
                return false;
            }
            if (varIndex < 0 || varIndex >= DataCube.Size[0])
            {
                message = string.Format("The variable index {0} is out of range. {1} variable(s) are available.", varIndex, DataCube.Size[0]);
                return false;
            }
            if (!DataCube.IsAllocated(varIndex))
            {
                message = string.Format("The selected variable (index {0}) is not loaded. Please load it first.", varIndex);
                return false;
            }
            return true;
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

            if (profile == null || profile.Count == 0)
            {
                Message = "The river profile is empty. Please select a start segment first.";
                return mat;
            }
            string msg;
            if (!IsVariableReady(varIndex, out msg))
            {
                Message = msg;
                return mat;
            }

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
                            if (index < 0)
                                continue;
                            sumlen += reach.Length;
                            mat[0, 0, i] = sumlen;
                            mat[1, 0, i] = DataCube[varIndex, startday + current, index] * scaleFactor / (reach.Length > 0 ? reach.Length : 1.0);
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
                            if (index < 0)
                                continue;
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
                mat = new DataCube<double>(2, 1, profile.Count);
                int i = 0;
                double sumlen = 0;
                if (unified)
                {
                    foreach (var r in profile)
                    {
                        int index = r.ID - 1;
                        if (index < 0 || index >= DataCube.Size[2])
                            continue;
                        sumlen += r.Length;
                        var len = r.LastReach != null ? r.LastReach.Length : r.Length;
                        mat[0, 0, i] = sumlen;
                        mat[1, 0, i] = DataCube[varIndex, startday + current, index] * scaleFactor / (len > 0 ? len : 1.0);
                        i++;
                    }
                }
                else
                {
                    foreach (var r in profile)
                    {
                        int index = r.ID - 1;
                        if (index < 0 || index >= DataCube.Size[2])
                            continue;
                        sumlen += r.Length;
                        mat[0, 0, i] = sumlen;
                        mat[1, 0, i] = DataCube[varIndex, startday + current, index] * scaleFactor;
                        i++;
                    }
                }
            }
            return mat;
        }

        public DataCube<float> GetProfileTimeSeries(List<River> profile, int varIndex, string var_name, int total_time, bool allReach, bool unified)
        {
            DataCube<float> mat = null;
            var scaleFactor = ScaleFactor;

            if (profile == null || profile.Count == 0)
            {
                Message = "The river profile is empty. Please select a start segment first.";
                return null;
            }
            string msg;
            if (!IsVariableReady(varIndex, out msg))
            {
                Message = msg;
                return null;
            }

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
                                if (index < 0)
                                    continue;
                                mat[0, t, i] = (float)(DataCube[varIndex, t, index] * scaleFactor / (reach.Length > 0 ? reach.Length : 1.0));
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
                                if (index < 0)
                                    continue;
                                mat[0, t, i] = (float)(DataCube[varIndex, t, index] * scaleFactor);
                                i++;
                            }
                        }
                    }
                }
            }
            else
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
                            if (index < 0 || index >= DataCube.Size[2])
                                continue;
                            var len = r.LastReach != null ? r.LastReach.Length : r.Length;
                            mat[0, t, i] = (float)(DataCube[varIndex, t, index] * scaleFactor / (len > 0 ? len : 1.0));
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
                            if (index < 0 || index >= DataCube.Size[2])
                                continue;
                            mat[0, t, i] = (float)(DataCube[varIndex, t, index] * scaleFactor);
                            i++;
                        }
                    }
                }
            }

            if (mat == null)
                return null;

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