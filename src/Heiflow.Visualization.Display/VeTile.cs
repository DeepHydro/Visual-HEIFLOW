#define Offset
using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;

using System.Runtime.InteropServices;
using System.Net;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Net;
using HUST.WREIS.Dot3D.Renderable;
using HUST.WREIS.Dot3D.Terrain;
using Heiflow.Spatial;
using Heiflow.Core;


namespace HUST.WREIS.Dot3D
{
    public class VeTile : IDisposable
    {
        //these are the coordinate extents for the tile
        UV m_ul, m_ur, m_ll, m_lr;

        /// <summary>
        /// Coordinates at upper left edge of image
        /// </summary>
        public UV UL
        {
            get { return m_ul; }
            set { m_ul = value; }
        }

        /// <summary>
        /// Coordinates at upper right edge of image
        /// </summary>
        public UV UR
        {
            get { return m_ur; }
            set { m_ur = value; }
        }

        /// <summary>
        /// Coordinates at lower left edge of image
        /// </summary>
        public UV LL
        {
            get { return m_ll; }
            set { m_ll = value; }
        }

        /// <summary>
        /// Coordinates at lower right edge of image
        /// </summary>
        public UV LR
        {
            get { return m_lr; }
            set { m_lr = value; }
        }

        //store the Vertical Exaggeration for when the mesh was created
        //so when the VerticalExaggeration setting changes, it know which meshes to recreate
        private float vertEx;
        public float VertEx
        {
            get { return vertEx; }
        }

        private static Projection _proj;
        private static double _layerRadius;
        private static TerrainAccessor _terrainAccessor;
        private static System.Drawing.Font _font;
        private static Brush _brush;

        public string DatasetName
        {
            get;
            set;
        }

        public string ImageExtension
        {
            get;
            set;
        }

        public bool IsDebug
        {
            get;
            set;
        }

        public bool IsTerrainOn
        {
            get;
            set;
        }
        public SceneWindow WorldWindow
        {
            get;
            set;
        }

        public static void Init(Projection proj, TerrainAccessor terrainAccessor, double layerRadius)
        {
            _proj = proj;
            _terrainAccessor = terrainAccessor;
            _layerRadius = layerRadius;
            _font = new System.Drawing.Font("Verdana", 15, FontStyle.Bold);
            _brush = new SolidBrush(Color.Green);
        }

        //flag for if the tile should be disposed
        private bool isNeeded = true;
        public bool IsNeeded
        {
            get { return isNeeded; }
            set { isNeeded = value; }
        }

        public bool IsEqual(int row, int col, int level)
        {
            bool retVal = false;
            if (this.row == row && this.col == col && this.level == level)
            {
                retVal = true;
            }
            return retVal;
        }

        private int row;
        private int col;
        private int level;

        public VeTile(int row, int col, int level, SceneWindow ww)
        {
            this.row = row;
            this.col = col;
            this.level = level;
            DatasetName = "h";
            ImageExtension = "jpeg";
            if (DatasetName == "s")
            {
                ImageExtension = "png";
            }
            IsDebug = false;
            WorldWindow = ww;
        }

        private Texture texture = null;
        public Texture Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        private ArrayList alMetaData = new ArrayList();
        private WebDownload download;
        public float ProgressPercent;
        private string textureName;
        private DrawArgs drawArgs;
        //private Downloader downloader;

        public void GetTexture(DrawArgs drawArgs, int pixelsPerTile)
        {
            this.drawArgs = drawArgs;

            string _datasetName = DatasetName;
            string _imageExtension = ImageExtension;
            string _serverUri = ".ortho.tiles.virtualearth.net/tiles/";

            string quadKey = TileToQuadKey(col, row, level);

            //TODO no clue what ?g= is
            string textureUrl = String.Concat(new object[] { "http://", _datasetName, quadKey[quadKey.Length - 1], _serverUri, _datasetName, quadKey, ".", _imageExtension, "?g=", 15 });

            if (IsDebug == true)
            {
                //generate a DEBUG tile with metadata
                MemoryStream ms;
                //debug
                Bitmap b = new Bitmap(pixelsPerTile, pixelsPerTile);
                System.Drawing.Imaging.ImageFormat imageFormat;
                //could download on my own from here and add metadata to the images before storing to cache
                //Bitmap b = DownloadImage(url);
                //string levelDir = CreateLevelDir(level);
                //string rowDir = CreateRowDir(levelDir, row);
                //alMetaData.Add("wwLevel : " + level.ToString());
                alMetaData.Add("ww rowXcol : " + row.ToString() + "x" + col.ToString());
                //alMetaData.Add("wwArcDist : " + arcDistance.ToString());
                //alMetaData.Add("tileRange : " + tileRange.ToString());
                //alMetaData.Add("latXlon : " + lat.ToString("###.###") + "x" + lon.ToString("###.###"));
                //alMetaData.Add("lat : " + lat.ToString());
                //alMetaData.Add("lon : " + lon.ToString());
                alMetaData.Add("veLevel : " + level.ToString());
                //alMetaData.Add("ve rowXcol : " + t_x.ToString() + "x" + t_y.ToString());
                //alMetaData.Add("veArcDist : " + tileDistance.ToString());
                //alMetaData.Add("sinLat : " + sinLat.ToString());
                alMetaData.Add("quadKey " + quadKey.ToString());
                imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                b = DecorateBitmap(b, _font, _brush, alMetaData);
                //SaveBitmap(b, rowDir, row, col, _imageExtension, b.RawFormat); //, System.Drawing.Imaging.ImageFormat.Jpeg
                //url = String.Empty;
                ms = new MemoryStream();
                b.Save(ms, imageFormat);
                ms.Position = 0;
                this.texture = TextureLoader.FromStream(drawArgs.device, ms);
                ms.Close();
                ms = null;
                b.Dispose();
                b = null;
            }
            else
            {
                //load a tile from file OR download it if not cached
                string levelDir = CreateLevelDir(level, WorldWindow.Cache.CacheDirectory);
                string mapTypeDir = CreateMapTypeDir(levelDir, _datasetName);
                string rowDir = CreateRowDir(mapTypeDir, row);
                textureName = String.Empty; //= GetTextureName(rowDir, row, col, "dds");
                if (_datasetName == "r")
                {
                    textureName = GetTextureName(rowDir, row, col, "png");
                }
                else
                {
                    textureName = GetTextureName(rowDir, row, col, "jpeg");
                }
                if (File.Exists(textureName) == true)
                {
                    this.texture = TextureLoader.FromFile(drawArgs.device, textureName);
                }
                else //download it
                {
                    /*
                    //use WebDownload instead
                    downloader = new Downloader();
                    downloader.drawArgs = drawArgs;
                    downloader.textureName = textureName;
                    downloader.textureUrl = textureUrl;
                    downloader.veTile = this;
                    downloader.mapType = _datasetName;

                    ThreadStart ts = new ThreadStart(downloader.DownloadThread);
                    Thread t = new Thread(ts);
                    t.IsBackground = true;
                    t.Start();
                    */

                    download = new WebDownload(textureUrl);
                    download.DownloadType = DownloadType.Unspecified;
                    download.SavedFilePath = textureName + ".tmp"; //?
                    download.ProgressCallback += new DownloadProgressHandler(UpdateProgress);
                    download.CompleteCallback += new DownloadCompleteHandler(DownloadComplete);
                    download.BackgroundDownloadFile();
                }
            }
        }

        void UpdateProgress(int pos, int total)
        {
            if (total == 0)
            {
                // When server doesn't provide content-length, 
                //use this dummy value to at least show some progress.
                total = 50 * 1024;
            }
            pos = pos % (total + 1);
            ProgressPercent = (float)pos / total;
        }

        private void DownloadComplete(WebDownload downloadInfo)
        {
            try
            {
                downloadInfo.Verify();

                //m_quadTile.QuadTileArgs.NumberRetries = 0;

                //TODO add back in logic to check for the no data tile?
                //the logic was to not display that tile to at least show some data for the layers beneath VE
                //or show the no data tile so they know that VE has not covered that area yet
                //then just let those tiles get periodically deleted from cache for when VE updates

                // Rename temp file to real name
                File.Delete(textureName);
                File.Move(downloadInfo.SavedFilePath, textureName);

                // Make the quad tile reload the new image
                //m_quadTile.DownloadRequest = null;
                //m_quadTile.isInitialized = false;
                this.texture = TextureLoader.FromFile(drawArgs.device, textureName);
            }
            catch (System.Net.WebException caught)
            {
                System.Net.HttpWebResponse response = caught.Response as System.Net.HttpWebResponse;
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    using (File.Create(textureName + ".txt"))
                    { }
                    return;
                }
                //m_quadTile.QuadTileArgs.NumberRetries++;
            }
            catch
            {
                using (File.Create(textureName + ".txt"))
                { }
                if (File.Exists(downloadInfo.SavedFilePath))
                    File.Delete(downloadInfo.SavedFilePath);
            }
            finally
            {
                download.IsComplete = true;
                //m_quadTile.QuadTileArgs.RemoveFromDownloadQueue(this);
                //Immediately queue next download
                //m_quadTile.QuadTileArgs.ServiceDownloadQueue();
            }


        }

        public void AddMetaData(string metadata)
        {
            alMetaData.Add(metadata);
        }

        //for generating the debug bitmap
        public Bitmap DecorateBitmap(Bitmap b, System.Drawing.Font font, Brush brush, ArrayList alMetadata)
        {
            if (alMetadata.Count > 0)
            {
                //if(b.RawFormat == System.Drawing.Imaging.ImageFormat.Png)
                if (b.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    MemoryStream ms = new MemoryStream();
                    b.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    b.Dispose();
                    b = null;
                    b = new Bitmap(256, 256);
                    b = (Bitmap)Bitmap.FromStream(ms);
                    ms.Close();
                    ms = null;
                }
                Graphics g = Graphics.FromImage(b); //fails for png files
                g.Clear(Color.White);
                g.DrawLine(Pens.Red, 0, 0, b.Width, 0);
                g.DrawLine(Pens.Red, 0, 0, 0, b.Height);
                string s = (string)alMetadata[0];
                SizeF sizeF = g.MeasureString(s, font);
                for (int i = 0; i < alMetadata.Count; i++)
                {
                    s = (string)alMetadata[i];
                    int x = 0;
                    int y = (int)(sizeF.Height * (i + 0));
                    g.DrawString(s, font, brush, x, y);
                }
                g.Dispose();
            }
            return b;
        }

        //convert VE row, col, level into key for URL
        private static string TileToQuadKey(int tx, int ty, int zl)
        {
            string quad;
            quad = "";
            for (int i = zl; i > 0; i--)
            {
                int mask = 1 << (i - 1);
                int cell = 0;
                if ((tx & mask) != 0)
                {
                    cell++;
                }
                if ((ty & mask) != 0)
                {
                    cell += 2;
                }
                quad += cell;
            }
            return quad;
        }

        public string CreateLevelDir(int level, string cacheDirectoryRoot)
        {
            string levelDir = null;
            //VirtualEarth.m_WorldWindow.Cache.CacheDirectory
            string cacheDirectory = String.Format("{0}\\Virtual Earth", cacheDirectoryRoot);
            if (Directory.Exists(cacheDirectory) == false)
            {
                Directory.CreateDirectory(cacheDirectory);
            }
            levelDir = cacheDirectory + @"\" + level.ToString();
            if (Directory.Exists(levelDir) == false)
            {
                Directory.CreateDirectory(levelDir);
            }
            return levelDir;
        }

        public string CreateMapTypeDir(string levelDir, string mapType)
        {
            string mapTypeDir = levelDir + @"\" + mapType;
            if (Directory.Exists(mapTypeDir) == false)
            {
                Directory.CreateDirectory(mapTypeDir);
            }
            return mapTypeDir;
        }

        public string CreateRowDir(string mapTypeDir, int row)
        {
            string rowDir = mapTypeDir + @"\" + row.ToString("0000");
            if (Directory.Exists(rowDir) == false)
            {
                Directory.CreateDirectory(rowDir);
            }
            return rowDir;
        }

        public string GetTextureName(string rowDir, int row, int col, string textureExtension)
        {
            string textureName = rowDir + @"\" + row.ToString("0000") + "_" + col.ToString("0000") + "." + textureExtension;
            return textureName;
        }

        public void SaveBitmap(Bitmap b, string rowDir, int row, int col, string imageExtension, System.Drawing.Imaging.ImageFormat format)
        {
            string bmpName = rowDir + @"\" + row.ToString("0000") + "_" + col.ToString("0000") + "." + imageExtension;
            b.Save(bmpName, format);
            //b.Save(bmpName); //, format
        }

        public void Reproject()
        {
            //TODO refactor from the VeLayer class
        }

        protected CustomVertex.PositionNormalTextured[] vertices;
        public CustomVertex.PositionNormalTextured[] Vertices
        {
            get { return vertices; }
        }
        protected short[] indices;
        public short[] Indices
        {
            get { return indices; }
        }
        protected int meshPointCount = 64;

        private double North;
        private double South;
        private double West;
        private double East;
        public Point3d localOrigin;
        Angle CenterLatitude;
        Angle CenterLongitude;
        //NOTE this is a mix from Mashi's Reproject and WW for terrain
        public void CreateMesh(byte opacity, float verticalExaggeration)
        {
            this.vertEx = verticalExaggeration;

            int opacityColor = System.Drawing.Color.FromArgb(opacity, 0, 0, 0).ToArgb();

            meshPointCount = 32; //64; //96 // How many vertices for each direction in mesh (total: n^2)
            //vertices = new CustomVertex.PositionColoredTextured[meshPointCount * meshPointCount];

            // Build mesh with one extra row and col around the terrain for normal computation and struts
            vertices = new CustomVertex.PositionNormalTextured[(meshPointCount + 2) * (meshPointCount + 2)];

            int upperBound = meshPointCount - 1;
            float scaleFactor = (float)1 / upperBound;
            //using(Projection proj = new Projection(m_projectionParameters))
            //{
            double uStep = (UR.U - UL.U) / upperBound;
            double vStep = (UL.V - LL.V) / upperBound;
            UV curUnprojected = new UV(UL.U - uStep, UL.V + vStep);

            // figure out latrange (for terrain detail)
            UV geoUL = _proj.Inverse(m_ul);
            UV geoLR = _proj.Inverse(m_lr);
            double latRange = (geoUL.U - geoLR.U) * 180 / Math.PI;

            North = geoUL.V * 180 / Math.PI;
            South = geoLR.V * 180 / Math.PI;
            West = geoUL.U * 180 / Math.PI;
            East = geoLR.U * 180 / Math.PI;

            CenterLatitude = Angle.FromDegrees(0.5f * (North + South));
           CenterLongitude = Angle.FromDegrees(0.5f * (West + East));
            localOrigin = MathEngine.SphericalToCartesianD(CenterLatitude, CenterLongitude, _layerRadius);
            localOrigin.X = (float)(Math.Round(localOrigin.X / 10000) * 10000);
            localOrigin.Y = (float)(Math.Round(localOrigin.Y / 10000) * 10000);
            localOrigin.Z = (float)(Math.Round(localOrigin.Z / 10000) * 10000);

            float meshBaseRadius = (float)_layerRadius;
         
            UV geo;
            Vector3 pos;
            double height = 0;
            for (int i = 0; i < meshPointCount + 2; i++)
            {
                for (int j = 0; j < meshPointCount + 2; j++)
                {
                    geo = _proj.Inverse(curUnprojected);

                    // Radians -> Degrees
                    geo.U *= 180 / Math.PI;
                    geo.V *= 180 / Math.PI;

                    if (_terrainAccessor != null)
                    {
                        if (IsTerrainOn == true)
                        {
                            //height = heightData[i, j] * verticalExaggeration;
                            //original : need to fetch altitude on a per vertex basis (in VE space) to have matching tile borders (note PM)
                            height = verticalExaggeration * _terrainAccessor.GetElevationAt(geo.V, geo.U,Math.Abs(upperBound / latRange));
                        }
                        else
                        {
                            height = 0;
                        }
                    }

                    pos = MathEngine.SphericalToCartesian(
                        geo.V,
                        geo.U,
                        _layerRadius + height);
                    int idx = i * (meshPointCount + 2) + j;
                    vertices[idx].X = pos.X;
                    vertices[idx].Y = pos.Y;
                    vertices[idx].Z = pos.Z;
#if Offset
                    vertices[idx].X = pos.X - (float)localOrigin.X;
                    vertices[idx].Y = pos.Y - (float)localOrigin.Y;
                    vertices[idx].Z = pos.Z - (float)localOrigin.Z;
#endif
                    //double sinLat = Math.Sin(geo.V);
                    //vertices[idx].Z = (float) (pos.Z * sinLat);

                    vertices[idx].Tu = (j - 1) * scaleFactor;
                    vertices[idx].Tv = (i - 1) * scaleFactor;
                    //vertices[idx].Color = opacityColor;
                    curUnprojected.U += uStep;
                }
                curUnprojected.U = UL.U - uStep;
                curUnprojected.V -= vStep;
            }
            //}

            int slices = meshPointCount + 1;
            indices = new short[2 * slices * slices * 3];
            for (int i = 0; i < slices; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    indices[(2 * 3 * i * slices) + 6 * j] = (short)(i * (meshPointCount + 2) + j);
                    indices[(2 * 3 * i * slices) + 6 * j + 1] = (short)((i + 1) * (meshPointCount + 2) + j);
                    indices[(2 * 3 * i * slices) + 6 * j + 2] = (short)(i * (meshPointCount + 2) + j + 1);

                    indices[(2 * 3 * i * slices) + 6 * j + 3] = (short)(i * (meshPointCount + 2) + j + 1);
                    indices[(2 * 3 * i * slices) + 6 * j + 4] = (short)((i + 1) * (meshPointCount + 2) + j);
                    indices[(2 * 3 * i * slices) + 6 * j + 5] = (short)((i + 1) * (meshPointCount + 2) + j + 1);
                }
            }

            // Compute normals and fold struts
            calculate_normals();
            fold_struts(false, meshBaseRadius);
        }

        // Compute mesh normals and fold struts
        private void calculate_normals()
        {
            System.Collections.ArrayList[] normal_buffer = new System.Collections.ArrayList[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                normal_buffer[i] = new System.Collections.ArrayList();
            }
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 p1 = vertices[indices[i + 0]].Position;
                Vector3 p2 = vertices[indices[i + 1]].Position;
                Vector3 p3 = vertices[indices[i + 2]].Position;

#if Offset
                p1.X = p1.X + (float)localOrigin.X;
                p1.Y = p1.Y + (float)localOrigin.Y;
                p1.Z = p1.Z + (float)localOrigin.Z;

                p2.X = p2.X + (float)localOrigin.X;
                p2.Y = p2.Y + (float)localOrigin.Y;
                p2.Z = p2.Z + (float)localOrigin.Z;

                p3.X = p3.X + (float)localOrigin.X;
                p3.Y = p3.Y + (float)localOrigin.Y;
                p3.Z = p3.Z + (float)localOrigin.Z;
#endif


                Vector3 v1 = p2 - p1;
                Vector3 v2 = p3 - p1;
                Vector3 normal = Vector3.Cross(v1, v2);

                normal.Normalize();

                // Store the face's normal for each of the vertices that make up the face.
                normal_buffer[indices[i + 0]].Add(normal);
                normal_buffer[indices[i + 1]].Add(normal);
                normal_buffer[indices[i + 2]].Add(normal);
            }

            // Now loop through each vertex vector, and avarage out all the normals stored.
            for (int i = 0; i < vertices.Length; ++i)
            {
                for (int j = 0; j < normal_buffer[i].Count; ++j)
                {
                    Vector3 curNormal = (Vector3)normal_buffer[i][j];

                    if (vertices[i].Normal == Vector3.Empty)
                        vertices[i].Normal = curNormal;
                    else
                        vertices[i].Normal += curNormal;
                }

                vertices[i].Normal.Multiply(1.0f / normal_buffer[i].Count);
            }
        }

        // Adjust/Fold struts vertices using terrain border vertices positions
        private void fold_struts(bool renderStruts, float meshBaseRadius)
        {
            short vertexDensity = (short)Math.Sqrt(vertices.Length);
            for (int i = 0; i < vertexDensity; i++)
            {
                if (i == 0 || i == vertexDensity - 1)
                {
                    for (int j = 0; j < vertexDensity; j++)
                    {
                        int offset = (i == 0) ? vertexDensity : -vertexDensity;
                        if (j == 0) offset++;
                        if (j == vertexDensity - 1) offset--;
                        Point3d p = new Point3d(vertices[i * vertexDensity + j + offset].Position.X, vertices[i * vertexDensity + j + offset].Position.Y, vertices[i * vertexDensity + j + offset].Position.Z);
                        if (renderStruts) p = ProjectOnMeshBase(p, meshBaseRadius);
                        vertices[i * vertexDensity + j].Position = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
                    }
                }
                else
                {
                    Point3d p = new Point3d(vertices[i * vertexDensity + 1].Position.X, vertices[i * vertexDensity + 1].Position.Y, vertices[i * vertexDensity + 1].Position.Z);
                    if (renderStruts) p = ProjectOnMeshBase(p, meshBaseRadius);
                    vertices[i * vertexDensity].Position = new Vector3((float)p.X, (float)p.Y, (float)p.Z);

                    p = new Point3d(vertices[i * vertexDensity + vertexDensity - 2].Position.X, vertices[i * vertexDensity + vertexDensity - 2].Position.Y, vertices[i * vertexDensity + vertexDensity - 2].Position.Z);
                    if (renderStruts) p = ProjectOnMeshBase(p, meshBaseRadius);
                    vertices[i * vertexDensity + vertexDensity - 1].Position = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
                }
            }
        }

        // Project an elevated mesh point to the mesh base
        private Point3d ProjectOnMeshBase(Point3d p, float meshBaseRadius)
        {
         
#if Offset
            p = p + this.localOrigin;
            p = p.normalize();
            p = p * meshBaseRadius - this.localOrigin;
#else
            p = p.normalize();
            p = p * meshBaseRadius;
#endif
            return p;
        }

        public void Dispose()
        {
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
            if (download != null)
            {
                download.Dispose();
                download = null;
            }
            if (vertices != null)
            {
                vertices = null;
            }
            if (indices != null)
            {
                indices = null;
            }
            if (downloadRectangle != null)
            {
                downloadRectangle = null;
            }
            GC.SuppressFinalize(this);
        }

        CustomVertex.PositionColored[] downloadRectangle = new CustomVertex.PositionColored[5];

        public static void Render(DrawArgs drawArgs, bool disableZbuffer, ArrayList alVeTiles)
        {
            try
            {
                if (alVeTiles.Count <= 0)
                    return;

                lock (alVeTiles.SyncRoot)
                {
                    //setup device to render textures
                    if (disableZbuffer)
                    {
                        if (drawArgs.device.RenderState.ZBufferEnable)
                            drawArgs.device.RenderState.ZBufferEnable = false;
                    }
                    else
                    {
                        if (!drawArgs.device.RenderState.ZBufferEnable)
                            drawArgs.device.RenderState.ZBufferEnable = true;
                    }
                    drawArgs.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                    drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
                    drawArgs.device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                    drawArgs.device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
                    drawArgs.device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;

                    // Set up for shading 
                    if (World.Settings.EnableSunShading)
                    {
                        drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                        drawArgs.device.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
                        drawArgs.device.TextureState[0].ColorArgument2 = TextureArgument.TextureColor;
                    }

                    //save index to tiles not downloaded yet
                    int notDownloadedIter = 0;
                    int[] notDownloaded = new int[alVeTiles.Count];
                    

                 

                    //render tiles that are downloaded
                    VeTile veTile;
                    for (int i = 0; i < alVeTiles.Count; i++)
                    {
                        veTile = (VeTile)alVeTiles[i];
                        if (veTile.Texture == null) //not downloaded yet
                        {
                            notDownloaded[notDownloadedIter] = i;
                            notDownloadedIter++;
                            continue;
                        }
                        else
                        {
                            //NOTE to stop ripping?
                            drawArgs.device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);

                            drawArgs.device.SetTexture(0, veTile.Texture);
#if Offset
                            DrawArgs.Device.Transform.World = Matrix.Translation(
                 (float)(veTile.localOrigin.X - drawArgs.WorldCamera.ReferenceCenter.X),
                 (float)(veTile.localOrigin.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                 (float)(veTile.localOrigin.Z - drawArgs.WorldCamera.ReferenceCenter.Z)
                 );
                            drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
                             veTile.Vertices.Length, veTile.Indices.Length / 3, veTile.Indices, true, veTile.Vertices);

                            DrawArgs.Device.Transform.World = DrawArgs.Camera.WorldMatrix;
#else
                                 drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
                             veTile.Vertices.Length, veTile.Indices.Length / 3, veTile.Indices, true, veTile.Vertices);
#endif

                        }
                    }

                    //now render the downloading tiles
                    drawArgs.device.RenderState.ZBufferEnable = false;
                    drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
                    drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;

                    int tileIndex;
                    for (int i = 0; i < notDownloadedIter; i++)
                    {
                        tileIndex = notDownloaded[i];
                        veTile = (VeTile)alVeTiles[tileIndex];
                        //TODO render progress bar indicator too?
                        veTile.RenderDownloadRectangle(drawArgs);
                    }

                    drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
                    drawArgs.device.VertexFormat = CustomVertex.PositionTextured.Format;
                    drawArgs.device.RenderState.ZBufferEnable = true;

                    // Turn back light on if needed
                    if (World.Settings.EnableSunShading)
                    {
                        drawArgs.device.RenderState.Lighting = true;
                    }

                }
            }
            catch (Exception ex)
            {
                string sex = ex.ToString();
                Log.Write(ex);
            }
            finally
            {
                if (disableZbuffer)
                    drawArgs.device.RenderState.ZBufferEnable = true;
            }
        }

        public void CreateDownloadRectangle(DrawArgs drawArgs, int color)
        {
            // Render terrain download rectangle
            Vector3 northWestV = MathEngine.SphericalToCartesian((float)North, (float)West, _layerRadius);
            Vector3 southWestV = MathEngine.SphericalToCartesian((float)South, (float)West, _layerRadius);
            Vector3 northEastV = MathEngine.SphericalToCartesian((float)North, (float)East, _layerRadius);
            Vector3 southEastV = MathEngine.SphericalToCartesian((float)South, (float)East, _layerRadius);

            downloadRectangle[0].X = northWestV.X;
            downloadRectangle[0].Y = northWestV.Y;
            downloadRectangle[0].Z = northWestV.Z;
            downloadRectangle[0].Color = color;

            downloadRectangle[1].X = southWestV.X;
            downloadRectangle[1].Y = southWestV.Y;
            downloadRectangle[1].Z = southWestV.Z;
            downloadRectangle[1].Color = color;

            downloadRectangle[2].X = southEastV.X;
            downloadRectangle[2].Y = southEastV.Y;
            downloadRectangle[2].Z = southEastV.Z;
            downloadRectangle[2].Color = color;

            downloadRectangle[3].X = northEastV.X;
            downloadRectangle[3].Y = northEastV.Y;
            downloadRectangle[3].Z = northEastV.Z;
            downloadRectangle[3].Color = color;

            downloadRectangle[4].X = downloadRectangle[0].X;
            downloadRectangle[4].Y = downloadRectangle[0].Y;
            downloadRectangle[4].Z = downloadRectangle[0].Z;
            downloadRectangle[4].Color = color;
        }

        public void RenderDownloadRectangle(DrawArgs drawArgs)
        {

            // camera jitter fix
            drawArgs.device.Transform.World = Matrix.Translation(
                   (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Z
            );

            drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, downloadRectangle);

            // camera jitter fix
            drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
        }
    }
}
