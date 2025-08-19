using DotSpatial.Data;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.IrrigationModel
{
    public class CreatIRZoneFile : ModelTool
    {
        private string _IRZoneMapFileName;

          public CreatIRZoneFile() 
        {
            Name = "Create Irigation Zone Input File";
            Category = "Irrigation Model";
            Description = "Create irrigation zone input File";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            HruArea = 1000000;
        }

        [Category("Input")]
        [Description("The irrigation zone map file. It must contains three columns: Zone_ID, Zone_Name,HRU_ID")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string IRZoneMapFileName
        {
            get
            {
                return _IRZoneMapFileName;
            }
            set
            {
                _IRZoneMapFileName = value;
                OutputFileName = _IRZoneMapFileName + ".out";
            }
        }

        public string OutputFileName
        {
            get;
            set;
        }

        public double HruArea
        {
            get;
            set;
        }

        private void ParseAndSaveIrZoneData(string inputFilePath, string outputFilePath)
        {
            StreamWriter ir_out = new StreamWriter(outputFilePath);
            // 读取CSV文件所有行
            var lines = File.ReadAllLines(inputFilePath);

            // 跳过标题行，解析数据
            var data = lines.Skip(1)
                           .Select(line => line.Split(new char[] { ',' }))
                           .Where(parts => parts.Length >= 3)
                           .Select(parts => new
                           {
                               IrZone = int.Parse( parts[0].Trim()),
                               IrName = parts[1].Trim(),
                               HruId = int.Parse(parts[2].Trim())
                           });

            // 按IR_ZONE分组
            var groupedData = data.GroupBy(item => new { item.IrZone, item.IrName }).OrderBy(a => a.Key.IrZone);
            string header = "ID,NAME,地表水比例,用水类型,DRAWDOWN,SEGMENT,REACH,HRU_NUM,HRU_LIST,HRU_AREA,CANAL_EFFICY,CANAL_RATIO,Inlet_Type,Inlet_MinFlow,Inlet_MaxFlow,Inlet_flow_ratio,SW control factor,GW control factor";
            ir_out.WriteLine(header);
            var swcontrol = "\"1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\"";
            var gwcontrol = "\"0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t1\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\"";

            foreach (var group in groupedData)
            {
                var hrus = group.Select(item => item.HruId);
                var hrusstr = "\"" + string.Join("\t", hrus) + "\"";
                var nhru = hrus.Count();
                double[] area = new double[nhru];
                for (int i = 0; i < area.Length; i++)
                {
                    area[i] = HruArea;
                }
                var zoneid = group.Key.IrZone;
                var areastr = "\"" + string.Join("\t", area) + "\"";
                var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", zoneid, group.Key.IrName, 0.8, zoneid, 1, 1, 1, nhru, hrusstr, areastr, 0.65, 0.05, 1, 0, 0, 0, swcontrol, gwcontrol);
                ir_out.WriteLine(line);
            }
            ir_out.Close();
        }
 
        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            ParseAndSaveIrZoneData(IRZoneMapFileName,OutputFileName);
            cancelProgressHandler.Progress("Package_Tool", 100, "Done");
            return true;
        }

        public override void Initialize()
        {
            this.Initialized = !string.IsNullOrEmpty(IRZoneMapFileName);
        }
    }
}
