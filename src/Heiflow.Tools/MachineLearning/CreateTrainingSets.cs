using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using DotSpatial.Data;
using Excel;
using GeoAPI.Geometries;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Spatial.SpatialRelation;

namespace Heiflow.Tools.MachineLearning
{
    public class CreateTrainingSets : ModelTool
    {
        public CreateTrainingSets()
        {
            Name = "Create Training Sets";
            Category = "Machine Learning";
            Description = "Create Training Sets from shapefile and raster";
            Version = "1.0.0.0";
            Author = "Yong Tian";
            InputTraningDC = "indc";
            OutputTraningDC = "outdc";
        }
        private int _NameIndex = 0;
        private string _FeatureFileName;
        private string _ExcelDataFileName;
        private IFeatureSet _FeatureSet;
        private string _NameField;
        private string _ExcelValueField;

        [Category("Output")]
        [Description("Name of the created input data cube")]
        public string InputTraningDC
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("Name of the created output data cube")]
        public string OutputTraningDC
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The shpfile name")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FeatureFileName
        {
            get
            {
                return _FeatureFileName;
            }
            set
            {
                _FeatureFileName = value;
                if (File.Exists(_FeatureFileName))
                {
                    _FeatureSet = FeatureSet.Open(_FeatureFileName);
                    var buf = from DataColumn dc in _FeatureSet.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }
        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }
        [Browsable(false)]
        public string[] ExcelFields
        {
            get;
            protected set;
        }
 
        [Category("Parameter")]
        [Description("Name of the value feiled")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("ExcelFields")]
        public string ExcelValueField
        {
            get
            {
                return _ExcelValueField;
            }
            set
            {
                _ExcelValueField = value;
            }
        }

        [Category("Parameter")]
        [Description("Name of the site name feiled")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SiteNameField
        {
            get
            {
                return _NameField;
            }
            set
            {
                _NameField = value;
                if (Fields != null)
                {
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        if (_NameField == Fields[i])
                        {
                            _NameIndex = i;
                            break;
                        }
                    }
                }
            }
        }
        [Category("Input")]
        [Description("The EXCEL file name assocated with the feature file")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ExcelDataFileName
        {
            get
            {
                return _ExcelDataFileName;
            }
            set
            {
                _ExcelDataFileName = value;
                if (File.Exists(_ExcelDataFileName))
                {
                    FileStream stream = File.Open(_ExcelDataFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    excelReader.IsFirstRowAsColumnNames = true;
                    System.Data.DataSet result = excelReader.AsDataSet();
                    var dt = result.Tables[0];
                    ExcelFields = (from DataColumn dc in dt.Columns select dc.ColumnName).ToArray();
                    stream.Close();
                    excelReader.Close();
                }
            }
        }

        [Category("Input")]
        [Description("a csv/txt file that contains training file list. Each row in the file contains an input file name")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TrainingFileList
        {
            get;
            set;
        }
        public override void Initialize()
        {
            Initialized = TypeConverterEx.IsNotNull(ExcelValueField)  && TypeConverterEx.IsNotNull(SiteNameField) && TypeConverterEx.IsNotNull(TrainingFileList);
            Initialized = File.Exists(TrainingFileList);
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            int progress = 0;
            RcIndex index1;
            Dictionary<string, float> site_value = new Dictionary<string, float>();
            Dictionary<string, float> excel_value = new Dictionary<string, float>();
            List<string> trainingfiles = new List<string>();
            List<IRaster> rasters = new List<IRaster>();
            List<Coordinate> sites = new List<Coordinate>();
            List<float> obs_values = new List<float>();
            FileStream stream = File.Open(_ExcelDataFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            excelReader.IsFirstRowAsColumnNames = true;
            System.Data.DataSet result = excelReader.AsDataSet();
            var dt = result.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                if (!excel_value.Keys.Contains(dr[0].ToString()))
                {
                    excel_value.Add(dr[0].ToString(), float.Parse(dr[ExcelValueField].ToString()));
                }
            }
            stream.Close();
            excelReader.Close();

            StreamReader sr = new StreamReader(TrainingFileList);
            var line = "";
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (TypeConverterEx.IsNotNull(line))
                {
                    trainingfiles.Add(line.Trim());
                }
            }
            sr.Close();

            int ninputvar = trainingfiles.Count;

            for (int i = 0; i < ninputvar; i++)
            {
                var ras = Raster.Open(trainingfiles[i]);
                rasters.Add(ras);
            }
            int n = 0;
            foreach (DataRow dr in _FeatureSet.DataTable.Rows)
            {
                var sname = dr[SiteNameField].ToString();
                if (excel_value.Keys.Contains(sname))
                {
                    sites.Add(_FeatureSet.Features[n].Geometry.Coordinate);
                    obs_values.Add(excel_value[sname]);
                }
                n++;
            }

            if (sites.Count > 0)
            {
                var indc = new DataCube<float>(ninputvar,1 , sites.Count)
                {
                    Name = InputTraningDC,
                    Layout= DataCubeLayout.ThreeD
                };
                var outdc = new DataCube<float>(1, 1, sites.Count)
                {
                    Name = OutputTraningDC,
                    Layout = DataCubeLayout.ThreeD
                };
                Workspace.Add(indc);
                Workspace.Add(outdc);

                for (int i = 0; i < sites.Count; i++)
                {
                    outdc[0, 0, i] = obs_values[i];
                    for (int j = 0; j < ninputvar; j++)
                    {
                        index1 = rasters[j].ProjToCell(sites[i]);
                        //if (index1.Row <= rasters[j].EndRow && index1.Column <= rasters[j].EndColumn && index1.Row > -1
                        //               && index1.Column > -1)
                            if ( index1.Row > -1&& index1.Column > -1)
                            {
                            indc[j, 0, i] = (float)(rasters[j].Value[index1.Row, index1.Column] == rasters[j].NoDataValue
                                                  ? 0
                                                  : rasters[j].Value[index1.Row, index1.Column]);
                        }
                        else
                        {
                            indc[j,0,i] = 0;
                        }
                    }
                    progress = (i + 1) / sites.Count;
                    cancelProgressHandler.Progress("Package_Tool", progress, "begin to process site " + (i + 1));
                }
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "No sites processed");
            }
            for (int i = 0; i < ninputvar; i++)
            {
                rasters[i].Close();
            }

            return true;
        }


    }
}