using DotSpatial.Projections;
using DotSpatial.Projections.Forms;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Subsurface.MT3DMS;
using Heiflow.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Heiflow.Core.Data;

namespace Heiflow.Controls.WinForm.Project
{
    public partial class CreatHeiflowFromMFForm : Form
    {
        private ProjectionInfo _ProjectionInfo;
        private IProjectController _Controller;
        private string _disfile = "";
        private string _basfile = "";

        public CreatHeiflowFromMFForm(IProjectController contrl)
        {
            InitializeComponent();
            _Controller = contrl;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BAS file|*.bas|All files|*.*";
            if(openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbBASfile.Text = openFileDialog.FileName;
                _basfile = tbBASfile.Text;
                tbDIS.Text = _basfile.Replace(".bas", ".dis");
                _disfile = tbDIS.Text;
            }
        }
        private void btnBrowseDIS_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DIS file|*.dis|All files|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbDIS.Text = openFileDialog.FileName;
                _disfile = tbDIS.Text;
            }
        }
        private void btnSelecProjection_Click(object sender, EventArgs e)
        {
            ProjectionSelectDialog form = new ProjectionSelectDialog();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _ProjectionInfo = form.SelectedCoordinateSystem;
                tbPrjName.Text = _ProjectionInfo.Name;
            }
            else
            {
                _ProjectionInfo = KnownCoordinateSystems.Geographic.World.WGS1984;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var project = _Controller.Project;
            var hfm = project.Model as Heiflow.Models.Integration.HeiflowModel;
            var mfmodel = hfm.ModflowModel as Heiflow.Models.Subsurface.Modflow;
        
            double x = 0;
            double y = 0;
            double.TryParse(tbX.Text, out x);
            double.TryParse(tbY.Text, out y);

            BASPackage bas = new BASPackage();
            bas.FileName = _basfile;
            bas.Owner = mfmodel;
            DISPackage dis = new DISPackage();
            dis.FileName = _disfile;
            dis.Owner = mfmodel;
            dis.ReadSP = false;

            dis.GetGridInfo(mfmodel.Grid as MFGrid);
            bas.Load(null);
            dis.Load(null);

            mfmodel.Grid.Origin = new GeoAPI.Geometries.Coordinate(x,y);
            mfmodel.Grid.Projection = _ProjectionInfo;
            mfmodel.Grid.BuildTopology();

            if(hfm.Version == "1.1.0")
            {
                var prms = hfm.PRMSModel;
                prms.SoilLayerManager.Generate(3);
            }
            _Controller.Project.CreateGridFeature();
            (mfmodel.Grid as RegularGrid).RaiseUpdate();
            _Controller.ShellService.ProjectExplorer.AddProject(_Controller.Project);
            _Controller.ProjectService.RaiseProjectOpenedOrCreated(_Controller.ShellService.MapAppManager.Map, _Controller.Project);

            //var   succ = model.Load(null);
            //if (succ != LoadingState.FatalError)
            //{
            //    model.TimeService.PopulateTimelineFromSP(property.Start);
            //    //model.TimeService.PopulateIOTimelineFromSP();
            //    model.Grid.Projection = property.Projection;
            //}
            //else
            //{
            //    OnLoadFailed("Fatal error.");
            //}
        }

        private void btnMT3D_Click(object sender, EventArgs e)
        {

            var project = _Controller.Project;
            var hfm = project.Model as Heiflow.Models.Integration.HeiflowModel;
            var mfmodel = hfm.ModflowModel as Heiflow.Models.Subsurface.Modflow;

            double x = 0;
            double y = 0;
            double.TryParse(tbX.Text, out x);
            double.TryParse(tbY.Text, out y);

            BASPackage bas = new BASPackage();
            bas.FileName = _basfile;
            bas.Owner = mfmodel;
            DISPackage dis = new DISPackage();
            dis.FileName = _disfile;
            dis.Owner = mfmodel;
            dis.ReadSP = false;

            dis.GetGridInfo(mfmodel.Grid as MFGrid);
            bas.Load(null);
            dis.Load(null);

            BTNPackage btnpck = new BTNPackage();
            btnpck.NCOMP = 1;
            //btn.InitialConcentraion = new float[mfmodel.Grid.ActualLayerCount * btn.NCOMP];
            //var j=0;
            //for (int i = 0; i < mfmodel.Grid.ActualLayerCount; i++)
            //{
            //    btn.InitialConcentraion[j] = 250;
            //    j++;
            //}
            //for (int i = 0; i < mfmodel.Grid.ActualLayerCount; i++)
            //{
            //    btn.InitialConcentraion[j] = 0.5f;
            //    j++;
            //}

            string initfile = tbInitConcFile.Text;
            StreamReader sr = new StreamReader(initfile);
            string line = sr.ReadLine();
            List<float> so4 = new List<float>();
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                var buf = TypeConverterEx.Split<float>(line, 1);
                so4.Add(buf[0]);
            }

            sr.Close();

            btnpck.TimeService = mfmodel.TimeService;
            btnpck.Owner = hfm;
            btnpck.Grid = mfmodel.Grid;
            btnpck.OnGridUpdated(mfmodel.Grid);
            btnpck.OnTimeServiceUpdated(mfmodel.TimeService);

            var k=0;
            var so4list= so4.ToArray();
            for (int i = 0; i < mfmodel.Grid.ActualLayerCount; i++)
            {
                btnpck.SCONC.ILArrays[k][0, ":"] = so4list.Copy();
                k++;
            }
            btnpck.SaveAs(tbBTN.Text, null);
        }

        private void btnOpenInitConc_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "csv file|*.csv|All files|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbInitConcFile.Text = openFileDialog.FileName;
            }
        }


    }
}
