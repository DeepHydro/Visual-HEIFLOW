using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ILTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            int nrow=2;
            int ncol=3;
            InitializeComponent();
           //this.Load += this.Form1_Load;
          // this.ilPanel1.Load += this.ilPanel1_Load;
            //Contour();
            A = ILSpecialData.sincf(nrow, ncol) * 1f;
            // the second is created off-centered
            B = ILSpecialData.sincf(nrow, ncol, 3f);
            X = ILMath.vec<float>(-10.0, 0.1, 10.0);
             Y = ILMath.vec<float>(-10.0, 0.1, 10.0);

           
             var ret =  ILMath.mean(A,0);

            // compute X and Y coordinates for every grid point
            YMat = 1; // provide YMat as output to meshgrid
            XMat = ILMath.meshgrid(X, Y, YMat); // only need mesh for 2D function here

            // preallocate data array for ILSurface: X by Y by 3
            // Note the order: 3 matrix slices of X by Y each, for Z,X,Y coordinates of every grid point
            C = ILMath.zeros<float>(Y.Length, X.Length, 3);

            // fill in Z values (replace this with your own function / data!!)
            C[":;:;0"] = ILMath.sin(XMat) * ILMath.sin(YMat) * ILMath.exp(-ILMath.abs(XMat * YMat) / 2);
            C[":;:;1"] = XMat; // X coordinates for every grid point
            C[":;:;2"] = YMat; // Y coordinates for every grid point
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ILArray<float> D = ILMath.ones<float>(nrow, ncol, 3);
         
          
            for(int i=0;i<nrow;i++)
            {
                for (int j = 0; j < ncol; j++)
                {
                    D.SetValue(10 + i + j, i, j);
                    
                }
            }
            watch.Stop();
            var ts = watch.Elapsed;
     
        }

        ILArray<float> A, B, C, X, Y, XMat,YMat;

        private void Form1_Load(object sender, EventArgs e)
        {
            var plotCube = ilPanel1.Scene.Add(
            new ILPlotCube(twoDMode: false) {
            new ILSurface(A) { 
                new ILColorbar() 
            }
        });
        }
  
   private void Contour()
        {
            var scene = new ILScene();
            // get terrain data, convert to single precision
            ILArray<float> A = ILMath.tosingle(ILSpecialData.terrain["0:100;0:100"]);
            scene.Add(
                // create plot cube 
              new ILPlotCube(twoDMode: false) {
	// create contour plot
	new ILContourPlot(A, create3D: true, 
		levels: new List<ContourLevel> {
			// configure individual contour levels
			new ContourLevel() { Text = "Coast", Value = 5, LineWidth = 3},
			new ContourLevel() { Text = "Plateau", Value = 1000, LineWidth = 3},
			new ContourLevel() { Text = "Basis 1", Value = 1500, LineWidth = 3, 
											  LineStyle = DashStyle.PointDash },
			new ContourLevel() { Text = "High", Value = 3000, LineWidth = 3},
			new ContourLevel() { Text = "Rescue", Value = 4200, LineWidth = 3, 
											  LineStyle = DashStyle.Dotted },
			new ContourLevel() { Text = "Peak", Value = 5000, LineWidth = 3},
		}), 
	// add surface with the same data
	new ILSurface(A) {
	  	// disable wireframe 
		Wireframe = { Visible = true , Markable=false},
		UseLighting = false,
        Fill = { Markable=false, Visible=false},
		Children = {
		  new ILLegend { Location = new PointF(1f,.1f) },
		  new ILColorbar { 
			  Location = new PointF(1,.4f),
			  Anchor = new PointF(1,0)
		  }
		} 
	}
});
            // combine a plot cube rotation around Z with one around X
            scene.First<ILPlotCube>().Rotation
                // note how the order is inversed 
              = Matrix4.Rotation(new Vector3(1, 0, 0), Math.PI / 5)
                // the rotation around Z is applied first !
              * Matrix4.Rotation(new Vector3(0, 0, 1), 0.2);
            ilPanel1.Scene.Add(scene);
        }
        private void ilPanel_Load(object sender, EventArgs e)
        {
            // we keep two data matrices and blend between them later

            // add a new plot cube, a surface and a colorbar
            var plotCube = ilPanel1.Scene.Add(
                new ILPlotCube(twoDMode: false) {
            new ILSurface(null) { 
                new ILColorbar() 
            }
        });
           
            // attach an event handler to be called on each frame
            ilPanel1.BeginRenderFrame += ilPanel1_BeginRenderFrame;

        }

        void ilPanel1_BeginRenderFrame(object sender, ILRenderEventArgs e)
        {
                // this gets called very frequently! Important 
                // to clean everthing up here: ILNumerics’ scoping helps: 
            using (ILScope.Enter())
            {
                // calculate time varying factor -1...1
                float time = (float)Math.Sin(e.Parameter.Time_ms / 500f);
                // blend between A and B 
                ILArray<float> D = A * time + B * (1 - time);
                // update the surface 
                var sf = (sender as ILPanel).Scene.First<ILSurface>();
                sf.UpdateColormapped(D, dataValues: D);
            }
        }
        float count = 0.5f;
        private void button2_Click(object sender, EventArgs e)
        {
            // set the colormap to the surface
            var sf = ilPanel1.Scene.First<ILSurface>();
            using (ILScope.Enter())
            {
                count += 0.5f;
                ILArray<float> D = A * count + B * (1 - count);
                // update the surface 
                sf.UpdateColormapped(D, dataValues: D);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sf = ilPanel1.Scene.First<ILSurface>();
            sf.Fill.Markable = false;
            sf.Wireframe.Markable = false; 
         //   ilPanel1.Clock.Running = true;
            sf.ColorMode = ILSurface.ColorModes.RBGA;
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ilPanel1.Clock.Running = false;
        }

        private void btn2D_Click(object sender, EventArgs e)
        {
            var cube = ilPanel1.Scene.First<ILPlotCube>() as ILPlotCube;
            cube.TwoDMode = true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            var cube = ilPanel1.Scene.First<ILPlotCube>() as ILPlotCube;
            cube.TwoDMode = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
           // var sf = ilPanel1.Scene.First<ILSurface>() as ILSurface;
            //ilPanel1.Scene = new ILScene();
            //ilPanel1.Refresh();
            var cube = ilPanel1.Scene.First<ILPlotCube>();
            cube.Children.Clear();
            ilPanel1.Refresh();
        }

        private void btnVisible_Click(object sender, EventArgs e)
        {
            var sf = ilPanel1.Scene.First<ILSurface>();
            sf.Visible = !sf.Visible;
        }

         private void ilPanel1_Load(object sender, EventArgs e) {
    ILArray<float> A = ILMath.tosingle(ILSpecialData.terrain["0:400;0:400"]);
    // derive a 'flat shaded' colormap from Jet colormap
    var cm = new ILColormap(Colormaps.Jet);
    ILArray<float> cmData = cm.Data;
    cmData.a = Computation.CreateFlatShadedColormap(cmData);
    cm.SetData(cmData); 
    // display interpolating colormap
    ilPanel1.Scene.Add(new ILPlotCube() { 
        Plots = {
            new ILSurface(A, colormap: Colormaps.Jet) {
                Children = { new ILColorbar() },
                Wireframe = { Visible = false }
            }
        }, 
        ScreenRect = new RectangleF(0,-0.05f,1,0.6f)
    }); 

    // display flat shading colormap
    ilPanel1.Scene.Add(new ILPlotCube() {
        Plots = {
            new ILSurface(A, colormap: cm) {
                Children = { new ILColorbar() },
                Wireframe = { Visible = false }
            }
        },
        ScreenRect = new RectangleF(0, 0.40f, 1, 0.6f)
    }); 

}
    
    }



    public class Computation : ILMath
    {
        public static ILRetArray<float> CreateFlatShadedColormap(ILInArray<float> cm)
        {
            using (ILScope.Enter(cm))
            {
                // create array large enough to hold new colormap
                ILArray<float> ret = zeros<float>(cm.S[0] * 2 - 1, cm.S[1]);
                // copy the original
                ret[r(0, cm.S[0] - 1), full] = cm;
                // double original keypoints, give small offset (may not even be needed?) 
                ret[r(cm.S[0], end), 0] = cm[r(1, end), 0] - epsf;
                ret[r(cm.S[0], end), r(1, end)] = cm[r(0, end - 1), r(1, end)];
                // reorder to sort keypoints in ascending order
                ILArray<int> I = 1;
                sort(ret[full, 0], Indices: I);
                return ret[I, full];
            }

        }
    }
}
