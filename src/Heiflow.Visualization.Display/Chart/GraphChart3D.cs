using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HUST.WREIS.Dot3D.NewWidgets;
using ZedGraph;
using System.Drawing;
using System.IO;
using Microsoft.DirectX.Direct3D;
using HUST.WREIS.Dot3D.Display.Properties;
using HUST.WREIS.Dot3D.Display.Plugins;
using System.Data;
using Heiflow.Visualization.Renderable.Grid;
using Heiflow.Core.Data;


namespace HUST.WREIS.Dot3D.Display
{
    public enum GraphType { Line,Bar,Pie,Area}

    public class GraphChart3D : Plugin3D
    {
    //    FormWidget m_form = null;
        PictureBox mPbox = null;
        private MasterPane masterPane;
        private GraphPane defaultGraphPane;
        private PointPairList mPointPairList;
        private GraphType mGraphType;
        private Graphics mGraphics;
        private Bitmap mBitmap;
        protected CurveItem mOldCurveItem;
        protected Color[] defaultColors;
        protected SymbolType[] defaultSymbols;

        public GraphChart3D(SceneWindow sw)
            : base(sw)
        {
            //DataSource = new PointPairList();
            Location = new Point(100, 100);
            ClearExsitingLine = true;
        }

        private string mTitle = "";
        private string mXAxisTitle = "Date";
        private string myAxisTitle = "Variable";

        public bool ClearExsitingLine
        {
            get;
            set;
        }

        #region Properties
        public System.Windows.Forms.ContextMenuStrip ContextMenuStrip { get; set; }

        public IPointList PointList { get; protected set; }
        
        public GraphPane GraphPane
        {
            get
            {
                return defaultGraphPane;
            }
        }
       
        public CurveItem CurrentItem
        {
            get
            {
                return mOldCurveItem;
            }
        }

        System.Windows.Forms.Control ParentControl { get; set; }

        public Point Location { get; set; }

        public string Caption { get; set; }

        public string Name { get; set; }

        public GraphType GraphType
        {
            get
            {
                return mGraphType;
            }
            set
            {
                mGraphType = value;
                if (mOldCurveItem != null)
                {
                    switch (mGraphType)
                    {
                        case Display.GraphType.Bar:
                            DrawBar(mOldCurveItem.Label.Text, mOldCurveItem.Points, mOldCurveItem.Color);
                            break;
                        case Display.GraphType.Line:
                            DrawLine(mOldCurveItem.Label.Text, 2.0f, mOldCurveItem.Points, mOldCurveItem.Color, SymbolType.Circle);
                            break;
                        case Display.GraphType.Area:
                             DrawFilledLine(mOldCurveItem.Label.Text, mOldCurveItem.Points,2.0f,45.0f, mOldCurveItem.Color, SymbolType.Circle);
                            break;
                    }
                    Refresh();
                }
            }
        }

        public string Title 
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
                masterPane.Title.Text = value;
                Refresh();
            }
        }
        public string XAxisTitle
        {
            get
            {
                return mXAxisTitle;
            }
            set
            {
                defaultGraphPane.XAxis.Title.Text = value;
                mXAxisTitle = value;
                Refresh();
            }
        }

        public string YAxisTitle
        {
            get
            {
                return myAxisTitle;
            }
            set
            {
                defaultGraphPane.YAxis.Title.Text = value;
                myAxisTitle = value;
                Refresh();
            }
        }

        public new bool Visible
        {
            get
            {
                return m_form.Visible;
            }
            set
            {
                m_form.Visible = value;
            }
        }
        #endregion

        public override void Load()
        {
            defaultColors = DataColor.GetUniqueRandomColor(10);
            defaultSymbols = new SymbolType[10];

            defaultColors[0] = Color.ForestGreen;
            defaultColors[1] = Color.OrangeRed;    
            defaultColors[2] = Color.Black;         
            defaultColors[3] = Color.Blue;
            defaultColors[4] = Color.Red;

            defaultSymbols[0] = SymbolType.Circle;
            defaultSymbols[1] = SymbolType.None;
            defaultSymbols[2] = SymbolType.Square;
            defaultSymbols[3] = SymbolType.Star;

            m_form = new FormWidget("MonitorForm");
            m_form.Name = Caption;
            m_form.ClientSize = new System.Drawing.Size(400, 300);
         //   m_form.Location = new System.Drawing.Point(10, SceneWindow.Height - 320);
            m_form.Location = Location;
            m_form.BackgroundColor = World.Settings.WidgetBackgroundColor;
            m_form.AutoHideHeader = false;
            m_form.VerticalScrollbarEnabled = true;
            m_form.HorizontalScrollbarEnabled = true;
            m_form.BorderEnabled = true;
            m_form.Initialize(SceneWindow.DrawArgs);

            int width = m_form.ClientSize.Width - 4;
            int height = m_form.ClientSize.Height -m_form.HeaderHeight- 4;
          
            mGraphics = CreateGraphics(width, height);
            Rectangle rect = new Rectangle(0, 0, width, height);
            masterPane = new MasterPane(Title, rect);
            masterPane.Fill = new Fill(new SolidBrush(Color.Black));
            masterPane.Margin.All = 0;
            masterPane.Title.IsVisible = false;

            CreateDefaultGraphPane(rect);
            masterPane.Add(defaultGraphPane);

            PointPairList list = new PointPairList();
 
            for (int i = 0; i <= 50; i++)
            {
                double x = i;
                double y = Math.Sin(i);
                list.Add(x, y);
            }
           // mOldCurveItem = defaultGraphPane.AddCurve("示例数据", list, Color.Blue, SymbolType.Diamond);

             mPointPairList = list;
            defaultGraphPane.Title.Text = "";      

            m_form.OnResizeEvent += new FormWidget.ResizeHandler(m_form_OnResizeEvent);
            m_form.FormSizeChanged += new EventHandler(m_form_FormSizeChanged);
            m_form.OnVisibleChanged += new VisibleChangedHandler(OnFormVisibleChanged);
            mPbox = new PictureBox();
            mPbox.Location = new System.Drawing.Point(2, 2);
            //m_LineGraph.Font = new System.Drawing.Font("Ariel", 10.0f, System.Drawing.FontStyle.Bold);
            mPbox.ParentWidget = m_form;
            mPbox.ClientSize = new System.Drawing.Size(width,height );
            m_form.Visible = true;
            mPbox.Visible = true;
          //  mPbox.Opacity = 220;
            m_form_OnResizeEvent(m_form, m_form.WidgetSize);
            m_form.ChildWidgets.Add(mPbox);

            m_form.MouseRightButtonDown += new System.Windows.Forms.MouseEventHandler(m_form_MouseRightButtonDown);
            DrawArgs.NewRootWidget.ChildWidgets.Add(m_form);

            ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem tsi = new System.Windows.Forms.ToolStripMenuItem("图表类型", Resources.chart);
            tsi.Name = "miChartType";
            tsi.DropDownItems.Add(new System.Windows.Forms.ToolStripMenuItem("曲线图", 
                Resources.chart_curve_icon, SelectChartTypet_Click, "miLineChart"));
            tsi.DropDownItems.Add(new System.Windows.Forms.ToolStripMenuItem("柱状图", 
                Resources.ee_icon_4, SelectChartTypet_Click, "miBarChart"));
            tsi.DropDownItems.Add(new System.Windows.Forms.ToolStripMenuItem("面积图", 
                Resources.color, SelectChartTypet_Click, "miAreaChart"));
            ContextMenuStrip.Items.Add(tsi);

            tsi = new System.Windows.Forms.ToolStripMenuItem("Import...", Resources.ee_icon_4, ImpotData_Click, "miImpotData");
            ContextMenuStrip.Items.Add(tsi);

            tsi = new System.Windows.Forms.ToolStripMenuItem("Export...", Resources.page_excel, ExportData_Click, "miExportData");
            ContextMenuStrip.Items.Add(tsi);

            tsi = new System.Windows.Forms.ToolStripMenuItem("View Data...", Resources.page_white, ViewData_Click, "miView");
            ContextMenuStrip.Items.Add(tsi);


            tsi = new System.Windows.Forms.ToolStripMenuItem("Clear", Resources.windy, Clear_Click, "miClear");
            ContextMenuStrip.Items.Add(tsi);

            //tsi = new System.Windows.Forms.ToolStripMenuItem("Property...", Resources.Prop, ViewProp_Click, "miProperty");
            //ContextMenuStrip.Items.Add(tsi);

            tsi = new System.Windows.Forms.ToolStripMenuItem("Clear Exsiting Line", Resources.Prop);
            tsi.CheckOnClick = true;
            tsi.CheckedChanged += tsi_CheckedChanged;
            ContextMenuStrip.Items.Add(tsi);

            tsi = new System.Windows.Forms.ToolStripMenuItem("Default Scale", Resources.windy, SetDefaultScale, "miSetDefuScale");
            ContextMenuStrip.Items.Add(tsi);

            Refresh();
            base.Load();

        }

        private void tsi_CheckedChanged(object sender, EventArgs e)
        {
            ClearExsitingLine = (sender as System.Windows.Forms.ToolStripMenuItem).Checked;
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            defaultGraphPane.CurveList.Clear();
        }

        private void ImpotData_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog opd = new System.Windows.Forms.OpenFileDialog();
            opd.Filter = "csv files|*.csv";
            if(opd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string filename = opd.FileName;
                    //defaultGraphPane.Title.Text = Path.GetFileNameWithoutExtension(filename);
                    StreamReader sr = new StreamReader(filename);
                    string line = sr.ReadLine();
                    var varnms = TypeConverterEx.Split<string>(line);

                    PointPairList[] lists = new PointPairList[varnms.Length - 1];

                    for (int i = 0; i < varnms.Length - 1; i++)
                    {
                        lists[i] = new PointPairList();
                    }

                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        var temp = TypeConverterEx.Split<string>(line);
                        var dates = DateTime.Parse(temp[0]);
                        line = line.Replace(temp[0], "");

                        var vv = TypeConverterEx.Split<float>(line);
                        for (int i = 0; i < varnms.Length - 1; i++)
                        {
                            lists[i].Add(new XDate(dates), vv[i]);
                        }             
                    }

                    for (int i = 0; i < varnms.Length - 1; i++)
                    {
                        DrawLine("", 2.0f, lists[i], defaultColors[i], defaultSymbols[i]);
                    }
                    sr.Close();
                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to import data! Error message:" +
                        ex.Message,"Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        void m_form_MouseRightButtonDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ContextMenuStrip.Show(new Point(e.X, e.Y));
        }

        private void SelectChartTypet_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripMenuItem item = sender as System.Windows.Forms.ToolStripMenuItem;
            if (defaultGraphPane.CurveList != null && defaultGraphPane.CurveList.Count == 1)
            {
                switch (item.Name)
                {
                    case "miLineChart":
                        ClearDefaultPane();
                        GraphType = GraphType.Line;
                        break;
                    case "miBarChart":
                        ClearDefaultPane();
                        GraphType = GraphType.Bar;
                        break;
                    case "miAreaChart":
                        ClearDefaultPane();
                        GraphType = GraphType.Area;
                        break;
                }
            }
        }

        private void ExportData_Click(object sender, EventArgs e)
        {
            if (defaultGraphPane.CurveList != null)
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = "csv file | *.csv";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(sfd.FileName);
                    foreach (var ls in defaultGraphPane.CurveList)
                    {
                        for(int  i=0; i< ls.Points.Count;i++)
                        {
                            string line = ls.Points[i].X + "," + ls.Points[i].Y;
                            sw.WriteLine(line);
                        }
                    }
                    sw.Close();
                }
            }
        }
        private void ViewData_Click(object sender, EventArgs e)
        {
            if (PointList != null )
            {
                FormChartData form = new FormChartData();
                DataTable dt = new DataTable();
                dt.Columns.Add("X",Type.GetType("System.Double"));
                dt.Columns.Add("Y", Type.GetType("System.Double"));

                for (int i = 0; i < PointList.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = PointList[i].X;
                    dr[1] = PointList[i].Y;
                    dt.Rows.Add(dr);
                }
                form.BindData(dt);
                form.Show();
            }
        }
        private void ViewProp_Click(object sender, EventArgs e)
        {

        }

        private void CreateDefaultGraphPane(Rectangle rect)
        {
             defaultGraphPane = new GraphPane(rect, Title, XAxisTitle, YAxisTitle);
            defaultGraphPane.XAxis.Type = AxisType.Linear;
            defaultGraphPane.XAxis.MajorGrid.IsVisible = true;
            defaultGraphPane.XAxis.Title.FontSpec.Size = 10.0f;

            defaultGraphPane.YAxis.MajorGrid.IsVisible = true;
            defaultGraphPane.YAxis.Title.FontSpec.Size = 10.0f;
       
            defaultGraphPane.AxisChange(mGraphics);

            defaultGraphPane.XAxis.MajorGrid.Color = Color.LightGray;
            defaultGraphPane.YAxis.MajorGrid.Color = Color.LightGray;

            // Move the legend location
            defaultGraphPane.Title.FontSpec.FontColor = Color.Green;
            defaultGraphPane.Legend.Position = ZedGraph.LegendPos.Bottom;
       //     defaultGraphPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 210), -45F);
            defaultGraphPane.Chart.Fill = new Fill(Color.White);
        }

        protected override void OnFormVisibleChanged(object o, VisibleState state)
        {
            World.Settings.NotifyPropertyChanged = true;
            if (state == VisibleState.Visible)
            {
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
            base.OnFormVisibleChanged(o, state);
        }

        void m_form_FormSizeChanged(object sender, EventArgs e)
        {
            mGraphics = CreateGraphics(mPbox.ClientSize.Width, mPbox.ClientSize.Height);
            masterPane.ReSize(mGraphics, new RectangleF(0, 0, mPbox.ClientSize.Width, mPbox.ClientSize.Height));
            Refresh();
        }

        protected Graphics CreateGraphics(int width, int height)
        {
             mBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
             return Graphics.FromImage(mBitmap);
        }

        public override void Unload()
        {
            if (m_form != null)
            {
                DrawArgs.NewRootWidget.ChildWidgets.Remove(m_form);
                m_form.Dispose();

                m_form = null;
            }
            base.Unload();
        }

        public override void Hide()
        {
            if (m_form != null)
                m_form.Hide();
            this.Visible = false;
        }

        public override void Show()
        {
            if (m_form != null)
                m_form.Show();
            this.Visible = true;
        }

        public void Refresh()
        {
            DisplayGraphImage();
        }

        public void ClearDefaultPane()
        {
            defaultGraphPane.CurveList.Clear();
        }

        private void DisplayGraphImage()
        {
            if (defaultGraphPane != null)
            {
                defaultGraphPane.AxisChange(mGraphics);
                masterPane.Draw(mGraphics);
                //  MemoryStream ms = new MemoryStream();
                // StreamWriter sw = new StreamWriter("image.png");
                //     mBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                //mBitmap.Save("image.png", System.Drawing.Imaging.ImageFormat.Png);
                //must copy original stream into new stream, if not, error occurs, not sure why
                //    Stream imageStream = new MemoryStream(ms.GetBuffer());
                // Texture texture = TextureLoader.FromStream(DrawArgs.Device, imageStream, 0, 0,
                //         1, Usage.None, Format.Dxt5, Pool.Managed, Filter.Box, Filter.Box, 0);
                // mPbox.ImageSource = imageStream;
                mPbox.ImageTexture = new Texture(DrawArgs.Device, mBitmap, Usage.None, Pool.Managed);
            }
        }

        public void DrawFilledLine(string label, IPointList list, float linewidth,float angle, Color color, SymbolType stype)
        {
            PointList = list;
            mOldCurveItem = defaultGraphPane.AddCurve(label, list, color, stype);
            LineItem myCurve = mOldCurveItem as LineItem;
            myCurve.Line.Fill = new Fill(Color.White, color, angle);
            myCurve.Line.Width = 2.0f;
            myCurve.Symbol.Size = linewidth;
            myCurve.Symbol.Fill = new Fill(Color.White);
            DisplayGraphImage();
        }

        public void DrawLine(string label, float linewidth, IPointList list, Color color, SymbolType stype)
        {
            if (ClearExsitingLine)
                ClearDefaultPane();
            PointList = list;
            mOldCurveItem = defaultGraphPane.AddCurve(label, list, color, stype);
            LineItem myCurve = mOldCurveItem as LineItem;
            myCurve.Line.Width = linewidth;
        
            myCurve.Symbol.Size = 4.0F;
            myCurve.Symbol.Fill = new Fill(Color.White);
            DisplayGraphImage();
            SetDefaultScale(null, null);
            m_form_OnResizeEvent(m_form, m_form.WidgetSize);
        }

        public void DrawBar(string label, IPointList list, Color color)
        {
            PointList = list;
            BarItem myBar = defaultGraphPane.AddBar(label, list, color);
            myBar.Bar.Fill = new Fill(Color.Red, Color.White,Color.Red);
            mOldCurveItem= myBar;
            DisplayGraphImage();
        }

        public void DrawPieGraph(string label, IPointList list, Color color)
        {
            DisplayGraphImage();
        }

        private void SetDefaultScale(object sender, EventArgs e)
        {
            if (defaultGraphPane.CurveList.Count == 0)
                return;
            int len=defaultGraphPane.CurveList[0].Points.Count;
            double [] xx= new double[len];
            for (int i = 0; i < len; i++)
            {
                xx[i]= defaultGraphPane.CurveList[0].Points[i].X;
            }

            defaultGraphPane.XAxis.Scale.Min = xx.Min();
            defaultGraphPane.XAxis.Scale.Max = xx.Max();
        }

      private  void m_form_OnResizeEvent(object IWidget, System.Drawing.Size size)
        {
            if (m_form != null && mPbox != null)
            {
                int height = size.Height - m_form.HeaderHeight - 4;
                if (height < 0)
                    height = 0;
                
                int width = size.Width - 4;
                if (width < 0)
                    width = 0;             
                if (height >= 0 && width >= 0)
                {
                    mPbox.ClientSize = new System.Drawing.Size(width, height);
                }
            }
        }
    }
}
