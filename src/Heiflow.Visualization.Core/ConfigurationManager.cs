using System;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Camera;
using HUST.WREIS.Dot3D.Terrain;
using HUST.WREIS.Dot3D.Renderable;
using System.Globalization;
using Utility;
using HUST.WREIS.Dot3D.Configuration;
using DotSpatial.Data;
using HUST.WREIS.Dot3D.Net.Wms;
using Heiflow.Core;
using Heiflow.Spatial;
using HUST.WREIS.Dot3D.Core;
using Heiflow.Core.Drawing;

namespace HUST.WREIS.Dot3D
{
    public class ConfigurationManager
    {
        private static Engine3DSettings mEngine3DSettings;
        private static CFDSettings mCFDSettings;
        public static List<ClickableIcons> IConLayers = new List<ClickableIcons>();
        static ConfigurationManager()
        {
            //DateTime dte = new DateTime(2013, 3, 15);
            //DateTime dts = new DateTime(2012, 10, 15);
            //if (DateTime.Now > dte || DateTime.Now < dts)
            //{
            //    throw new Exception("Unknown error!");
            //}

            ////判断HKEY_LOCAL_MACHINE\Software\MySoftware下面KeyName这个键是否存在
            //if (!RegisterOperate.IsExist("WordProcessorSoftware"))
            //{
            // //   RegisterOperate.ModifyRegEditData("WordProcessorSoftware", "1");
            //    RegisterOperate.SetRegEditData("WordProcessorSoftware", "1");
            //    //修改HKEY_LOCAL_MACHINE\Software\MySoftware下面KeyName这个键的键值
            //}
            //else
            //{
            //    int i = int.Parse(RegisterOperate.GetRegistData("WordProcessorSoftware"));
            //    if (i > 100)
            //    {
            //        throw new Exception("Unknown error!");
            //    }
            //    else
            //    {
            //        RegisterOperate.SetRegEditData("WordProcessorSoftware", (i+1).ToString());
            //    }
            //}
        }

        public static Engine3DSettings Engine3DSettings
        {
            get
            {
                return mEngine3DSettings;
            }
            set
            {
                mEngine3DSettings = value;
            }
        }

        public static CFDSettings CFDSettings
        {
            get
            {
                return mCFDSettings;
            }
            set
            {
                mCFDSettings = value;
            }
        }

        public static TerrainTileService DefaultTerrainTileService { get; set; }

        public static DrawArgs DrawArgs { get; set; }

        public static string ApplicationPath { get; set; }

         public static string SettingsPath { get; set; }

        public static void LoadEngine3DSettings(string path)
        {
            mEngine3DSettings = new Dot3D.Engine3DSettings();
            if (File.Exists(path))
            {
                mEngine3DSettings = (Engine3DSettings)SettingsBase.LoadFromPath(mEngine3DSettings, path);
                DataProtector dp = new DataProtector(DataProtector.Store.USE_USER_STORE);
                if (mEngine3DSettings.ProxyUsername.Length > 0)
                    mEngine3DSettings.ProxyUsername = dp.TransparentDecrypt(mEngine3DSettings.ProxyUsername);
                if (mEngine3DSettings.ProxyPassword.Length > 0) 
                    mEngine3DSettings.ProxyPassword = dp.TransparentDecrypt(mEngine3DSettings.ProxyPassword);
            }
        }

        public static void LoadCFDSettings(string path)
        {
            mCFDSettings = new CFDSettings();
            if (File.Exists(path))
            {
                mCFDSettings = (CFDSettings)SettingsBase.Load(mCFDSettings, path);
            }
        }

        public static World Load(string filename, Cache cache, Engine3DSettings settings)
        {
            Log.Write(Log.Levels.Debug, "CONF", "Loading " + filename);
            try
            {
                XElement doc = XElement.Load(filename);
                string worldName = doc.Attribute("Name").Value;
                double equatorialRadius = double.Parse(doc.Attribute("EquatorialRadius").Value);
                World.Settings.DefalutLatitude = float.Parse(doc.Attribute("DefalutLatitude").Value);
                World.Settings.DefalutLongitude = float.Parse(doc.Attribute("DefalutLongitude").Value);

                TerrainAccessor[] terrainAccessor = GetTerrainAccessors(doc, System.IO.Path.Combine(cache.CacheDirectory, worldName));

                World newWorld = new World(
                        worldName,
                        new Microsoft.DirectX.Vector3(0, 0, 0),
                        new Microsoft.DirectX.Quaternion(0, 0, 0, 0),
                        equatorialRadius,
                        cache.CacheDirectory,
                        (terrainAccessor != null ? terrainAccessor[0] : null)//TODO: Oops, World should be able to handle an array of terrainAccessors
                        );

                RenderableObjectList dataLayerList = new RenderableObjectList("DataLayer")
                {
                    IconImagePath = Engine3DSettings.DataPath + @"\Icons\Server.png"
                };
                GetRenderablesFromLayerDirectory(doc, newWorld, cache, dataLayerList);
                newWorld.DataLayerList = dataLayerList;
                newWorld.RenderableObjects.Add(newWorld.DataLayerList);         
               
                return newWorld;
            }
            catch (Exception ex)
            {
                Log.Write(Log.Levels.Error, "CONF", "Exception caught during XML parsing: " + ex.Message);
                Log.Write(Log.Levels.Error, "CONF", "File " + filename + " was not read successfully.");
                // TODO: should pop up a message box or something.
                return null;
            }
        }

        private static WMSTerrainTileService GetWMSTerrainTileService(XElement parent, string cacheDirectory, string terrainAccessorName)
        {
            var terrainTileService = parent.Element("WMSTerrainTileService");
            if (terrainTileService != null)
            {
                WMSTerrainTileService tts = new WMSTerrainTileService(terrainTileService.Element("ServerUrl").Value,
                    terrainTileService.Element("DataSetName").Value,
                   double.Parse(terrainTileService.Element("LevelZeroTileSizeDegrees").Value),
                     int.Parse(terrainTileService.Element("SamplesPerTile").Value),
                       terrainTileService.Element("FileExtension").Value,
                     int.Parse(terrainTileService.Element("NumberLevels").Value),
                        Path.Combine(cacheDirectory, terrainAccessorName),
                         World.Settings.TerrainTileRetryInterval,
                          terrainTileService.Element("DataFormat").Value,
                           terrainTileService.Element("Version").Value,
                            terrainTileService.Element("Format").Value,
                             terrainTileService.Element("SRS").Value);
                tts.TerrainStorageService = GetSQLiteTerrainStorageService(terrainTileService);
                if (tts.TerrainStorageService == null)
                    tts.TerrainStorageService = new FileTerrainStorageService();
                return tts;
            }
            else
            {
                return null;
            }
        }

        private static TerrariumTerrainTileService GetTerrariumTerrainTileService(XElement parent, string cacheDirectory, string terrainAccessorName)
        {
            var terrainTileService = parent.Element("TerrariumTerrainTileService");
            if (terrainTileService == null)
            {
                return null;
            }
            int zoomOffset = terrainTileService.Element("ZoomOffset") != null ?
                int.Parse(terrainTileService.Element("ZoomOffset").Value) : 4;
            int maxZoom = terrainTileService.Element("MaxZoom") != null ?
                int.Parse(terrainTileService.Element("MaxZoom").Value) : 15;
            TerrariumTerrainTileService tts = new TerrariumTerrainTileService(
                terrainTileService.Element("ServerUrl").Value,
                terrainTileService.Element("DataSetName").Value,
               double.Parse(terrainTileService.Element("LevelZeroTileSizeDegrees").Value),
                 int.Parse(terrainTileService.Element("SamplesPerTile").Value),
                   terrainTileService.Element("FileExtension").Value,
                 int.Parse(terrainTileService.Element("NumberLevels").Value),
                    Path.Combine(cacheDirectory, terrainAccessorName),
                     World.Settings.TerrainTileRetryInterval,
                      terrainTileService.Element("DataFormat").Value,
                       zoomOffset, maxZoom);
            tts.TerrainStorageService = GetSQLiteTerrainStorageService(terrainTileService);
            if (tts.TerrainStorageService == null)
                tts.TerrainStorageService = new FileTerrainStorageService();
            return tts;
        }

        private static TerrainTileService GetTerrainTileService(XElement parent, string cacheDirectory, string terrainAccessorName)
        {
            var terrainTileService = parent.Element("TerrainTileService");
            if (terrainTileService != null)
            {
                TerrainTileService tts = new TerrainTileService(terrainTileService.Element("ServerUrl").Value,
                    terrainTileService.Element("DataSetName").Value,
                   double.Parse(terrainTileService.Element("LevelZeroTileSizeDegrees").Value),
                     int.Parse(terrainTileService.Element("SamplesPerTile").Value),
                       terrainTileService.Element("FileExtension").Value,
                     int.Parse(terrainTileService.Element("NumberLevels").Value),
                        Path.Combine(cacheDirectory, terrainAccessorName),
                         World.Settings.TerrainTileRetryInterval,
                          terrainTileService.Element("DataFormat").Value);
                return tts;
            }
            else
            {
                return null;
            }
        }

        private static TerrainAccessor[] GetHigherResolutioAccessor(XElement doc, TerrainTileService tts)
        {
            var terrainAccessors = doc.Elements("HigherResolutionSubset");
            List<TerrainAccessor> list = new List<TerrainAccessor>();
            foreach (var terrainAccessor in terrainAccessors)
            {
                string terrainAccessorName = terrainAccessor.Attribute("Name").Value;
                BoundingBox bbox = GetBoundingBox(terrainAccessor);
                string localfile=   terrainAccessor.Element("LocalFile").Value;
                TerrainAccessor newTerrainAccessor = new TifTerrainAccessor(terrainAccessorName,
                    bbox.West, bbox.South, bbox.East, bbox.North, tts,localfile);
                list.Add(newTerrainAccessor);
            }
            return list.ToArray();
        }

        private static TerrainAccessor[] GetTerrainAccessors(XElement doc, string cacheDirectory)
        {
            var terrainAccessors = doc.Elements("TerrainAccessor");
            List<TerrainAccessor> list = new List<TerrainAccessor>();

            foreach (var terrainAccessor in terrainAccessors)
            {
                string terrainAccessorName = terrainAccessor.Attribute("Name").Value;
              //  var terrainTileService = terrainAccessor.Element("TerrainTileService");
                TerrainTileService tts = null;
                if(terrainAccessor.Element("WMSTerrainTileService") != null)
                    tts= GetWMSTerrainTileService(terrainAccessor, cacheDirectory, terrainAccessorName);
                else if (terrainAccessor.Element("TerrariumTerrainTileService") != null)
                    tts = GetTerrariumTerrainTileService(terrainAccessor, cacheDirectory, terrainAccessorName);
                else
                    tts = GetTerrainTileService(terrainAccessor, cacheDirectory, terrainAccessorName);
                DefaultTerrainTileService = tts;
                TerrainAccessor[] higerResolutionSubsets = null;
                if (terrainAccessor.Element("HigherResolutionSubsets") != null)
                {
                    higerResolutionSubsets = GetHigherResolutioAccessor(terrainAccessor.Element("HigherResolutionSubsets"),tts);
                }

                BoundingBox bbox = GetBoundingBox(terrainAccessor);
                TerrainAccessor newTerrainAccessor = new NltTerrainAccessor(terrainAccessor.Attribute("Name").Value,
                    bbox.West, bbox.South, bbox.East, bbox.North, tts, higerResolutionSubsets);
                //newTerrainAccessor.
                list.Add(newTerrainAccessor);
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            else
            {
                return null;
            }
        }

        private static void GetRenderablesFromLayerDirectory(XElement doc, World parentWorld, Cache cache, RenderableObjectList renderableCollection)
        {
            var layerSets = doc.Descendants("LayerSet");
           // RenderableObjectList renderableCollection = new RenderableObjectList(parentWorld.Name);

            if (layerSets != null)
            {
                foreach (XElement ele in layerSets)
                {
                    RenderableObjectList parentRenderable = new RenderableObjectList(ele.Attribute("Name").Value);
                    parentRenderable.ParentList = renderableCollection;
                    parentRenderable.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                    parentRenderable.ShowOnlyOneLayer = bool.Parse(ele.Attribute("ShowOnlyOneLayer").Value);
                    renderableCollection.Add(parentRenderable);
                    AddProjectedQuadTileLayers(ele, parentWorld, parentRenderable, cache);
                    AddQuadTileLayers(ele, parentWorld, parentRenderable, cache);
                    AddLineFeatureLayer(ele, parentWorld, parentRenderable);
                    AddPolygonFeatureLayer(ele, parentWorld, parentRenderable);
                
                }
            }
            var iconSets = doc.Descendants("IconSet");
            if (iconSets != null)
            {
                foreach (XElement ele in iconSets)
                {
                    Icons icons = new Icons(ele.Attribute("Name").Value);
                    icons.ParentList = renderableCollection;
                    icons.IsOn  = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                    icons.ShowOnlyOneLayer = bool.Parse(ele.Attribute("ShowOnlyOneLayer").Value);
                    renderableCollection.Add(icons);
                    AddIconFeatureLayer(ele, parentWorld, icons); 
                }
            }

            var modelSets = doc.Descendants("Model3DSet");
            if (modelSets != null)
            {
                foreach (XElement ele in modelSets)
                {
                    RenderableObjectList models = new RenderableObjectList(ele.Attribute("Name").Value);
                    models.ParentList = renderableCollection;
                    models.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                    models.ShowOnlyOneLayer = bool.Parse(ele.Attribute("ShowOnlyOneLayer").Value);
                    renderableCollection.Add(models);
                    AddModelFeatureLayer(ele, parentWorld, models);
                }
            }

            var shapeSets = doc.Descendants("ShapeFeatureSet");
            if (shapeSets != null)
            {
                foreach (XElement ele in shapeSets)
                {
                    RenderableObjectList parentRenderable = new RenderableObjectList(ele.Attribute("Name").Value);
                    parentRenderable.ParentList = renderableCollection;
                    parentRenderable.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                    parentRenderable.ShowOnlyOneLayer = bool.Parse(ele.Attribute("ShowOnlyOneLayer").Value);
                    renderableCollection.Add(parentRenderable);
                    AddShapefileLayer(ele, parentWorld, parentRenderable);
                }
            }

            var imageSets = doc.Descendants("ImageLayer");
            if (imageSets != null)
            {
                foreach (XElement ele in imageSets)
                {
                    AddImageLayers(ele, parentWorld, renderableCollection);
                }
            }
        }

        private static void AddImageLayers(XElement imageLayerset, World parentWorld, RenderableObjectList parentRenderable)
        {
            string name = imageLayerset.Attribute("Name").Value;
            double distanceAboveSurface = double.Parse(imageLayerset.Element("DistanceAboveSurface").Value);
            string texturePath = Path.Combine(Engine3DSettings.ApplicationDirectory, imageLayerset.Element("TexturePath").Value);
            byte opacity = byte.Parse(imageLayerset.Element("Opacity").Value);
            string description = imageLayerset.Element("Description") != null ? imageLayerset.Element("Description").Value : "none";

            BoundingBox bbox = GetBoundingBox(imageLayerset);

            ImageLayer im = new ImageLayer(name, parentWorld, distanceAboveSurface, texturePath,
                bbox.South, bbox.North, bbox.West, bbox.East, opacity, parentWorld.TerrainAccessor);
            im.IsOn = bool.Parse(imageLayerset.Attribute("ShowAtStartup").Value);

            parentRenderable.Add(im);
            im.ParentList = parentRenderable;

        }

        private static void AddQuadTileLayers(XElement parent, World parentWorld, RenderableObjectList parentRenderable, Cache cache)
        {
            var quadLayersets = parent.Descendants("QuadTileSet");
            if (quadLayersets != null)
            {
                foreach (XElement ele in quadLayersets)
                {
                    ImageStore imageStores = GetImageStores(ele, parentRenderable, cache);
                    BoundingBox bbox = GetBoundingBox(ele);
                    QuadTileSet qts = new QuadTileSet(
                ele.Element("Name").Value,
                parentWorld,
              double.Parse(ele.Element("DistanceAboveSurface").Value),
                bbox.North,
                bbox.South,
                bbox.West,
                bbox.East,
                bool.Parse(ele.Element("TerrainMapped").Value),
             new ImageStore[] { imageStores }
                );
                    qts.ParentList = parentRenderable;
                    qts.ColorKey = GetColor(ele, "TransparentColor").ToArgb();
                    qts.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                    parentRenderable.Add(qts);
                }
            }
        }

        private static void AddProjectedQuadTileLayers(XElement parent, World parentWorld, RenderableObjectList parentRenderable, Cache cache)
        {
            var quadLayersets = parent.Descendants("ProjectedTileSet");
            if (quadLayersets != null)
            {
                foreach (XElement ele in quadLayersets)
                {
                    ImageStore imageStores = GetImageStores(ele, parentRenderable, cache);
                    BoundingBox bbox = GetBoundingBox(ele);
                    ProjectedTileSet qts = new ProjectedTileSet(ele.Element("Name").Value, parentWorld, double.Parse(ele.Element("DistanceAboveSurface").Value),
                        bbox.North, bbox.South, bbox.West, bbox.East, bool.Parse(ele.Element("TerrainMapped").Value), new ImageStore[] { imageStores });
                    qts.StartZoomLevel = int.Parse(ele.Element("StartZoomLevel").Value);
                    qts.ParentList = parentRenderable;
                    qts.ColorKey = GetColor(ele, "TransparentColor").ToArgb();
                    qts.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);

                    if (ele.Element("IconImageName") != null)
                    {
                        qts.IconImagePath = mEngine3DSettings.IconPath + ele.Element("IconImageName").Value;
                    }
                    parentRenderable.Add(qts);
                }
            }
        }

        private static void AddLineFeatureLayer(XElement parent, World parentWorld, RenderableObjectList parentRenderable)
        {
            var lineLayersets = parent.Descendants("LineFeature");
            if (lineLayersets != null)
              {
                  foreach (XElement ele in lineLayersets)
                  {
                      string source = Engine3DSettings.DataPath + ele.Attribute("Source").Value;
                      System.Drawing.Color color = System.Drawing.Color.Blue;
                      if (File.Exists(source))
                      {
                          Point3d[] points = null;
                          if (source.EndsWith(".txt"))
                          {
                              points = ParseTXT(source);
                          }
                          color = GetColor(ele, "FeatureColor");
                          LineFeature lf = new LineFeature(ele.Attribute("Name").Value, parentWorld, points, color);
                          lf.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                          lf.Extrude = ele.Element("Extrude") != null ? bool.Parse(ele.Element("Extrude").Value) : false;
                          lf.Outline = ele.Element("Outline") != null ? bool.Parse(ele.Element("Outline").Value) : false;
                          lf.LineColor = ele.Element("OutlineColor") != null ? GetColor(ele, "OutlineColor") : color;
                          lf.LineWidth = ele.Element("LineWidth") != null ? float.Parse(ele.Element("LineWidth").Value) : 1.0f;
                          lf.Opacity = (byte)( ele.Element("Opacity") != null ? float.Parse(ele.Element("Opacity").Value) :0);
                          lf.DistanceAboveSurface = ele.Element("DistanceAboveSurface") != null ? double.Parse(ele.Element("DistanceAboveSurface").Value) : 0;
                          string altitudemodeString = ele.Element("AltitudeMode") != null ? ele.Element("AltitudeMode").Value : "";
                          if (altitudemodeString != null && altitudemodeString.Length > 0)
                          {
                              if (altitudemodeString.ToLower(System.Globalization.CultureInfo.InvariantCulture).Equals("absolute")) lf.AltitudeMode = AltitudeMode.Absolute;
                              if (altitudemodeString.ToLower(System.Globalization.CultureInfo.InvariantCulture).Equals("relativetoground")) lf.AltitudeMode = AltitudeMode.RelativeToGround;
                              if (altitudemodeString.ToLower(System.Globalization.CultureInfo.InvariantCulture).Equals("clampedtoground")) lf.AltitudeMode = AltitudeMode.ClampedToGround;
                          }
                          lf.MinimumDisplayAltitude = ele.Element("MinimumDisplayAltitude") != null ? double.Parse(ele.Element("MinimumDisplayAltitude").Value) : 0;
                          lf.MaximumDisplayAltitude = ele.Element("MaximumDisplayAltitude") != null ? double.Parse(ele.Element("MaximumDisplayAltitude").Value) : 1000000;
                          
                          parentRenderable.Add(lf);
                      }
                    
                  }
              }
        }

        private static void AddPolygonFeatureLayer(XElement parent, World parentWorld, RenderableObjectList parentRenderable)
        {
            var lineLayersets = parent.Descendants("PolygonFeature");
            if (lineLayersets != null)
            {
                foreach (XElement ele in lineLayersets)
                {
                    string source = Engine3DSettings.DataPath + ele.Attribute("Source").Value;
                    System.Drawing.Color color = System.Drawing.Color.Blue;
                    if (File.Exists(source))
                    {
                        Point3d[] points = null;
                        if (source.EndsWith(".txt"))
                        {
                            points = ParseTXT(source);
                        }
                        color = GetColor(ele, "FeatureColor");
                        LinearRing outerRing = new LinearRing();
                        outerRing.Points = points;

                        PolygonFeature lf =  new PolygonFeature(ele.Attribute("Name").Value, parentWorld, outerRing,null, color);
                        lf.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                        lf.Extrude = ele.Element("Extrude") != null ? bool.Parse(ele.Element("Extrude").Value) : false;
                        lf.Outline = ele.Element("Outline") != null ? bool.Parse(ele.Element("Outline").Value) : false;
                        lf.OutlineColor = ele.Element("OutlineColor") != null ? GetColor(ele, "OutlineColor") : color;
                        lf.Opacity =  (byte)(ele.Element("Opacity") != null ? float.Parse(ele.Element("Opacity").Value) : 100);
                        lf.DistanceAboveSurface = ele.Element("DistanceAboveSurface") != null ? double.Parse(ele.Element("DistanceAboveSurface").Value) : 0;
                        string altitudemodeString = ele.Element("AltitudeMode") != null ? ele.Element("AltitudeMode").Value : "";
                        if (altitudemodeString != null && altitudemodeString.Length > 0)
                        {
                            if (altitudemodeString.ToLower(System.Globalization.CultureInfo.InvariantCulture).Equals("absolute")) lf.AltitudeMode = AltitudeMode.Absolute;
                            if (altitudemodeString.ToLower(System.Globalization.CultureInfo.InvariantCulture).Equals("relativetoground")) lf.AltitudeMode = AltitudeMode.RelativeToGround;
                            if (altitudemodeString.ToLower(System.Globalization.CultureInfo.InvariantCulture).Equals("clampedtoground")) lf.AltitudeMode = AltitudeMode.ClampedToGround;
                        }
                        lf.AltitudeMode = AltitudeMode.Absolute;
                        lf.MinimumDisplayAltitude = ele.Element("MinimumDisplayAltitude") != null ? double.Parse(ele.Element("MinimumDisplayAltitude").Value) : 0;
                        lf.MaximumDisplayAltitude = ele.Element("MaximumDisplayAltitude") != null ? double.Parse(ele.Element("MaximumDisplayAltitude").Value) : 1000000;

                        Bar3D bar = new Bar3D(lf.Name + "1", parentWorld, 0.5 * (lf.BoundingBox.North + lf.BoundingBox.South),
                            0.5 * (lf.BoundingBox.West + lf.BoundingBox.East), 10, 200,   RandomColor.CreateColorSeries(1)[0]);
                        bar.IsOn = true;
                        bar.ScalarValue = 10;
                        bar.ScaleX = 100;
                        bar.ScaleY = 100;
                        bar.Height = 1000;
                        bar.IconImagePath = Engine3DSettings.IconPath + "modis-fire.png";
                     //   parentRenderable.Add(bar);

                        WavingFlagLayer flag = new WavingFlagLayer(lf.Name + "1", parentWorld, 0.5 * (lf.BoundingBox.North + lf.BoundingBox.South),
                            0.5 * (lf.BoundingBox.West + lf.BoundingBox.East), Engine3DSettings.IconPath + "modis-fire.png");
                        flag.IsOn = true;
                        flag.Bar3D = bar;
                        flag.ScaleX = 700;
                        flag.ScaleY = 700;
                        flag.ScaleZ = 700;
                        flag.Bar3D.ScaleX = 0.3f * flag.ScaleX;
                        flag.Bar3D.ScaleY = 0.3f * flag.ScaleY;
                        flag.RenderPriority = RenderPriority.Custom;
                      
                        parentRenderable.Add(flag);

                        parentRenderable.Add(lf);
                    }
                }
            }
        }

        private static void AddIconFeatureLayer(XElement parent, World parentWorld, RenderableObjectList parentRenderable)
        {
            var iconLayersets = parent.Descendants("IconFeature");
            if (iconLayersets != null)
            {
                System.Windows.Forms.MenuItem[] mitems = new System.Windows.Forms.MenuItem[1];
                mitems[0] = new System.Windows.Forms.MenuItem("属性");
                foreach (XElement ele in iconLayersets)
                {
                    double lat = ele.Element("Latitude") != null ? double.Parse(ele.Element("Latitude").Value) : 30.4;
                    double lon = ele.Element("Longitude") != null ? double.Parse(ele.Element("Longitude").Value) : 114.0;
                    string des = ele.Element("Description") != null ?ele.Element("Description").Value : "none";
                    double distanceAboveSurface = ele.Element("DistanceAboveSurface") != null ? double.Parse(ele.Element("DistanceAboveSurface").Value) : 0;
                    string textureName=  ele.Element("TextureFilePath") != null ?ele.Element("TextureFilePath").Value : "DefaultIcon.png";
                    textureName = Engine3DSettings.IconPath + textureName;
                    int width = ele.Element("IconWidthPixels") != null ? int.Parse(ele.Element("IconWidthPixels").Value) : 16;
                    int height = ele.Element("IconHeightPixels") != null ? int.Parse(ele.Element("IconHeightPixels").Value) : 16;
                    Icon ic = new Icon(ele.Attribute("Name").Value, des, lat, lon, distanceAboveSurface, parentWorld, textureName, width,height,"");
                    ic.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                    ic.isSelectable = true;
                    ic.NameAlwaysVisible = ele.Element("AlwaysVisible") != null ? bool.Parse(ele.Element("AlwaysVisible").Value) : true;

                    double onClickZoomAltString = ele.Element("OnClickZoomAltitude") != null ? double.Parse(ele.Element("OnClickZoomAltitude").Value) : 1000;
                    double onClickZoomHeadingString = ele.Element("OnClickZoomHeading") != null ? double.Parse(ele.Element("OnClickZoomHeading").Value) : 0;
                    double onClickZoomTiltString = ele.Element("OnClickZoomTilt") != null ? double.Parse(ele.Element("OnClickZoomTilt").Value) : 2.0; 					

                    ic.MinimumDisplayDistance = ele.Element("MinimumDisplayAltitude") != null ? double.Parse(ele.Element("MinimumDisplayAltitude").Value) : 0;
                    ic.MaximumDisplayDistance = ele.Element("MaximumDisplayAltitude") != null ? double.Parse(ele.Element("MaximumDisplayAltitude").Value) : 1000000;
                    ic.ParentControl = Engine3DSettings.SceneWindow;
                  
                    ic.ContextMenu = new System.Windows.Forms.ContextMenu(mitems);
         
                    parentRenderable.Add(ic);
                }
            }
        }

        private static void AddModelFeatureLayer(XElement parent, World parentWorld, RenderableObjectList parentRenderable)
        {
                var modelLayersets = parent.Descendants("ModelFeature");
                if (modelLayersets != null)
                {
                    foreach (XElement ele in modelLayersets)
                    {
                        float lat = ele.Element("Latitude") != null ? float.Parse(ele.Element("Latitude").Value) : 30.4f;
                        float lon = ele.Element("Longitude") != null ? float.Parse(ele.Element("Longitude").Value) : 114.0f;
                        float scaleFactor = ele.Element("ScaleFactor") != null ? float.Parse(ele.Element("ScaleFactor").Value) : 1.0f;
                        string des = ele.Element("Description") != null ? ele.Element("Description").Value : "none";
                        float distanceAboveSurface = ele.Element("DistanceAboveSurface") != null ? float.Parse(ele.Element("DistanceAboveSurface").Value) : 0;
                        float rotX =  ele.Element("RotationX") != null ? float.Parse(ele.Element("RotationX").Value) : 0;
                        float rotY = ele.Element("RotationY") != null ? float.Parse(ele.Element("RotationY").Value) : 0;
                        float rotZ = ele.Element("RotationZ") != null ? float.Parse(ele.Element("RotationZ").Value) : 0;

                        string meshFilePath = ele.Element("MeshFilePath") != null ? ele.Element("MeshFilePath").Value : "DefaultModel.x";
                        meshFilePath = Path.Combine(Engine3DSettings.DataPath, meshFilePath);
                        if (File.Exists(meshFilePath))
                        {
                            ModelFeature model = new ModelFeature(ele.Attribute("Name").Value, parentWorld, 
                                meshFilePath, lat, lon, distanceAboveSurface, scaleFactor, rotX, rotY, rotZ);
                            model.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                            model.RenderPriority = RenderPriority.Custom;
                            model.isElevationRelative2Ground = ele.Element("IsElevationRelativeToGround") != null ? bool.Parse(ele.Element("IsElevationRelativeToGround").Value) : true;
                            model.isVertExaggerable = ele.Element("IsVerticalExaggerable") != null ? bool.Parse(ele.Element("IsVerticalExaggerable").Value) : true;
                            parentRenderable.Add(model);
                        }
                    }
                }
        }

        private static ImageStore GetImageStores(XElement parent, RenderableObjectList parentRenderable, Cache cache)
        {
            XElement imageAccessor = parent.Element("ImageAccessor");
            ImageStore ia = null;
            if (imageAccessor != null)
            {
                XElement permanantDirectory = imageAccessor.Element("PermanantDirectory");
                double levelZeroTileSizeDegrees = double.Parse(imageAccessor.Element("LevelZeroTileSizeDegrees").Value);
                int numberLevels = int.Parse(imageAccessor.Element("NumberLevels").Value);
                int textureSizePixels = Int32.Parse(imageAccessor.Element("TextureSizePixels").Value);
                string imageFileExtension = imageAccessor.Element("ImageFileExtension").Value;
                bool isdebug = imageAccessor.Element("IsDebug") != null ? bool.Parse(imageAccessor.Element("IsDebug").Value) : false; 
                bool isDownloadableLayer = imageAccessor.Element("IsDownloadableLayer") != null ? bool.Parse(imageAccessor.Element("IsDownloadableLayer").Value) : false; ;
                byte opacity = 255;

                // case 1 : permanent directory specified.
                if (permanantDirectory != null)
                {
                    ia = new ImageStore();
                    ia.DataDirectory = imageAccessor.Element("PermanentDirectory").Value;
                    ia.LevelZeroTileSizeDegrees = levelZeroTileSizeDegrees;
                    ia.LevelCount = numberLevels;
                    ia.ImageExtension = imageFileExtension;
                    ia.IsDebug = isdebug;
                    ia.IsDownloadableLayer = isDownloadableLayer;
                    ia.ImageService = new FileImageAccessService(ia);
                }
                // case 2: ImageTileService specified
                XElement imageTileService = imageAccessor.Element("ImageTileService");
                if (imageTileService != null)
                {
                    string mapProvider = imageTileService.Element("MapProvider").Value;
                    string serverUrl = imageTileService.Element("ServerUrl").Value;
                    string dataSetName = imageTileService.Element("DataSetName").Value;
                    string localCache = imageTileService.Element("CacheDirectory").Value;
                    string serverLogoFilePath = imageTileService.Element("ServerLogoFilePath").Value;
                    // only the providers that need a key use it, e.g. the Tianditu key
                    string token = imageTileService.Element("Token") != null ? imageTileService.Element("Token").Value : "";
                    // only the servers whose WAF rejects the shared agent need one, e.g. Tianditu
                    string userAgent = imageTileService.Element("UserAgent") != null ? imageTileService.Element("UserAgent").Value : "";
                    if (mapProvider == "NLTMapProvider")
                    {
                        ia = new NltImageStore(dataSetName, serverUrl);
                    }
                    else
                    {
                        ia = new ProjectedMapImageStore(mapProvider, token);
                    }
                    ia.UserAgent = userAgent;
                    ia.DataSetName = dataSetName;
                    if (localCache != null)
                        ia.DataDirectory = Engine3DSettings.CachePath + "\\" + localCache;
                    else
                        ia.DataDirectory = Engine3DSettings.CachePath + "\\" + dataSetName;

                    ia.LevelZeroTileSizeDegrees = levelZeroTileSizeDegrees;
                    ia.LevelCount = numberLevels;
                    ia.ImageExtension = imageFileExtension;
                    ia.CacheDirectory = ia.DataDirectory;
                    ia.ServerLogo = serverLogoFilePath;
                    ia.IsDebug = isdebug;
                    ia.IsDownloadableLayer = isDownloadableLayer;

                    ia.ImageService = GetSQLiteService(imageTileService, ia);

                    if (ia.ImageService == null)
                    {
                        ia.ImageService = new FileImageAccessService(ia);
                    }
                   
                }
                // case 3: WMSImageTileService specified
                XElement wmsImageStore = imageAccessor.Element("WMSImageTileService");
                if (wmsImageStore != null)
                {
                  
                    WmsImageStore wmsLayerStore = new WmsImageStore();
                    wmsLayerStore.ImageFormat = wmsImageStore.Element("ImageFormat").Value;
       
                    //wmsLayerAccessor.IsTransparent = ParseBool(getInnerTextFromFirstChild(wmsAccessorIter.Current.Select("UseTransparency")));
                    wmsLayerStore.ServerGetMapUrl = wmsImageStore.Element("ServerUrl").Value;
                    wmsLayerStore.Version = wmsImageStore.Element("Version").Value;
                    wmsLayerStore.WMSLayerName =wmsImageStore.Element("LayerNames").Value;
                    string serverLogoPath = wmsImageStore.Element("ServerLogoFilePath").Value;
                    string opacityString = wmsImageStore.Element("Opacity").Value;
                    string wmsStyleName = wmsImageStore.Element("LayerStyle") != null ? wmsImageStore.Element("LayerStyle").Value : "";

                    if (serverLogoPath != null && serverLogoPath.Length > 0 && !Path.IsPathRooted(serverLogoPath))
                    {
                        serverLogoPath = Path.Combine(
                            Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath),
                            serverLogoPath);
                    }
                    if (opacityString != null)
                        opacity = byte.Parse(opacityString);

                    string localCache = wmsImageStore.Element("CacheDirectory").Value;
                    if (localCache != null)
                        wmsLayerStore.DataDirectory = Engine3DSettings.CachePath + "\\" + localCache;
                    else
                        wmsLayerStore.DataDirectory = Engine3DSettings.CachePath + "\\" + wmsLayerStore.WMSLayerName;
                    wmsLayerStore.CacheDirectory = wmsLayerStore.DataDirectory;
                    if (wmsStyleName != null && wmsStyleName.Length > 0)
                        wmsLayerStore.WMSLayerStyle = wmsStyleName;
                    else
                        wmsLayerStore.WMSLayerStyle = "";

                    wmsLayerStore.ImageExtension = imageFileExtension;
                    wmsLayerStore.LevelCount = numberLevels;
                    wmsLayerStore.LevelZeroTileSizeDegrees = levelZeroTileSizeDegrees;
                    wmsLayerStore.IsDebug = isdebug;
                    wmsLayerStore.IsDownloadableLayer = isDownloadableLayer;
                    wmsLayerStore.ImageService = GetSQLiteService(wmsImageStore, wmsLayerStore);
                    if (wmsLayerStore.ImageService == null)
                    {
                        wmsLayerStore.ImageService = new FileImageAccessService(wmsLayerStore);
                    }

                    ia = wmsLayerStore;
                }

                // case 4: SQLiteService specified
                XElement sQLiteService = imageAccessor.Element("SQLiteService");
                if (sQLiteService != null)
                {
                    string dataSetName = sQLiteService.Element("DataSetName").Value;
                    ia.LevelZeroTileSizeDegrees = levelZeroTileSizeDegrees;
                    ia.LevelCount = numberLevels;
                    ia.ImageExtension = imageFileExtension;
                    ia.CacheDirectory = ia.DataDirectory;
                    ia.IsDebug = isdebug;
                    ia.IsDownloadableLayer = isDownloadableLayer;
                }
            }
            return ia;
        }

        private static SQliteTerrainStorageService GetSQLiteTerrainStorageService(XElement parent)
        {
            SQliteTerrainStorageService service = null;
            var sQLiteService = parent.Element("SQLiteService");
            if (sQLiteService != null)
            {
                string dbpath = Engine3DSettings.ApplicationDirectory + "\\" + sQLiteService.Element("DbFilePath").Value;
                int type = sQLiteService.Element("TerrainType") != null ? int.Parse(sQLiteService.Element("TerrainType").Value) : 0;
                service = new SQliteTerrainStorageService( dbpath, type);
            }
            return service;
        }

        private static SQLiteImageService GetSQLiteService(XElement parent,ImageStore image)
        {
            SQLiteImageService service = null;
            var sQLiteService = parent.Element("SQLiteService");
            if (sQLiteService != null)
            {
                string dbpath = Engine3DSettings.ApplicationDirectory + "\\" + sQLiteService.Element("DbFilePath").Value;
                int type = sQLiteService.Element("ImageType") != null ? int.Parse(sQLiteService.Element("ImageType").Value) : 0;
                service = new SQLiteImageService(image, dbpath,type);
            }
            return service;
        }

        private static BoundingBox GetBoundingBox(XElement parent)
        {
            double north = 0;
            double south = 0;
            double west = 0;
            double east = 0;
            var latLonBoundingBox = parent.Element("LatLonBoundingBox");
            if (latLonBoundingBox != null)
            {
                goto bbox;
            }
            else
            {
                latLonBoundingBox = parent.Element("BoundingBox");
                if (latLonBoundingBox != null)
                {
                    goto bbox;
                }
            }
        bbox:
            {
                double.TryParse(latLonBoundingBox.Element("West").Value, out west);
                double.TryParse(latLonBoundingBox.Element("South").Value, out south);
                double.TryParse(latLonBoundingBox.Element("East").Value, out east);
                double.TryParse(latLonBoundingBox.Element("North").Value, out north);
            }
            return new BoundingBox(south, north, west, east);
        }

        private static System.Drawing.Color GetColor(XElement parent,string tagName)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;
            byte a = 255;
            var colorele = parent.Element(tagName);
            if (colorele != null)
            {
                r = byte.Parse(colorele.Element("Red").Value);
                g = byte.Parse(colorele.Element("Green").Value);
                b = byte.Parse(colorele.Element("Blue").Value);
                a = colorele.Element("Alpha") != null ? byte.Parse(colorele.Element("Alpha").Value) : (byte)255;
            }

            return System.Drawing.Color.FromArgb(a, r, g, b);
        }

        private static Point3d [] ParseTXT(string filename)
        {
            List<Point3d> list = new List<Point3d>();
            //XElement doc = new XElement(filename);
            //var ele=  doc.Element("coordinates");
            StreamReader sr = new StreamReader(filename);
            string ele= sr.ReadToEnd().Trim();
            if (ele != null)
            {
                string [] str = ele.Split(new char [] {' '});
                //result = new Point3d[str.Length];
              
                for (int i = 0; i < str.Length; i++)
                {
                    string[] sp = str[i].Split(new char[] { ',' });
                    if (sp.Length == 3)
                        list.Add(new Point3d(double.Parse(sp[0]), double.Parse(sp[1]), double.Parse(sp[2])));
                }
            }
            return list.ToArray(); ;
        }

        private static Point3d[] FromFeatureSet(DotSpatial.Data.IFeature fa, string coorsys,int zonewide,int zoneserial)
        {
            var points = (from coor in fa.Geometry.Coordinates select new Point3d(coor.X, coor.Y, 0)).ToArray();
            if (coorsys == "WGS1984")
            {
                return points;
            }
            else
            {

                Point3d[] newpoints = new Point3d[points.Count()];
                for(int i=0;i<points.Count();i++)
                {
                    double lng = 0;
                    double lat = 0;
                    CoordinateConvertor.GaussToBL(points[i].X, points[i].Y, zonewide, ref lng, ref lat, CoordinateSystem.Xian1980, zoneserial);
                    newpoints[i] = new Point3d()
                    {
                        X = lng,
                        Y = lat,
                        Z = 0
                    };
                }
                return newpoints;
            }
        }

        public static void AddShapefileLayer(string source, World parentWorld, RenderableObjectList parentRenderable, string name, string labelField,
            System.Drawing.Color color, string CoordinateSystem = "WGS1984", int zonewide = 6, int zoneserial = 99)
        {
            if (File.Exists(source))
            {
                IFeatureSet fs = FeatureSet.Open(source);
                Point3d[] points = null;
                RenderableObjectList feature = new RenderableObjectList("name");
                feature.ParentList = parentRenderable;

                if (fs.FeatureType == FeatureType.Line)
                {
                    foreach (var f in fs.Features)
                    {
                        points = FromFeatureSet(f, CoordinateSystem, zonewide, zoneserial);
                    }
                }
                else if (fs.FeatureType == FeatureType.Line)
                {
                    foreach (var f in fs.Features)
                    {
                        TriPolygon poly = new TriPolygon(f.Geometry.Coordinates);
                        poly.Triangulate();

                        points = FromFeatureSet(f, CoordinateSystem, zonewide, zoneserial);
                        LinearRing outerRing = new LinearRing();
                        outerRing.Points = points;
                        PolygonFeature lf = new PolygonFeature(f.DataRow[labelField].ToString(), parentWorld, outerRing, null, color);
                        lf.IsOn = true;
                        lf.Extrude = true;
                        lf.Outline = true;
                        lf.Opacity = 255;
                        lf.DistanceAboveSurface = 10;
                        lf.AltitudeMode = AltitudeMode.Absolute;
                        lf.MinimumDisplayAltitude = 0;
                        lf.MaximumDisplayAltitude = 1000 * 1000;
                        feature.Add(lf);
                    }
                    parentRenderable.Add(feature);
                }
                else if (fs.FeatureType == FeatureType.Point || fs.FeatureType == FeatureType.MultiPoint)
                {
                }

            }
        }

        private static void AddShapefileLayer(XElement parent, World parentWorld, RenderableObjectList parentRenderable)
        {
            var lineLayersets = parent.Descendants("ShapeFeature");
            if (lineLayersets != null)
            {     
                foreach (XElement ele in lineLayersets)
                {
                    string source = Engine3DSettings.DataPath + ele.Attribute("Source").Value;
                    if(!File.Exists(source))
                        source = ele.Attribute("Source").Value;
                    System.Drawing.Color color = System.Drawing.Color.Blue;
                    if (File.Exists(source))
                    {
                        IFeatureSet fs = FeatureSet.Open(source);
                        Point3d[] points = null;

                        string labelField = ele.Attribute("LabelField").Value;
                        var IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                     
                        color = GetColor(ele, "FeatureColor");
                     
                        var Extrude = ele.Element("Extrude") != null ? bool.Parse(ele.Element("Extrude").Value) : false;
                        var Outline = ele.Element("Outline") != null ? bool.Parse(ele.Element("Outline").Value) : false;
                        var OutlineColor = ele.Element("OutlineColor") != null ? GetColor(ele, "OutlineColor") : color;
                        var Opacity = (byte)(ele.Element("Opacity") != null ? float.Parse(ele.Element("Opacity").Value) : 100);
                        var DistanceAboveSurface = ele.Element("DistanceAboveSurface") != null ? double.Parse(ele.Element("DistanceAboveSurface").Value) : 0;
                        string altitudemodeString = ele.Element("AltitudeMode") != null ? ele.Element("AltitudeMode").Value : "";
                        var alMode = AltitudeMode.Absolute;
                        var MinimumDisplayAltitude = ele.Element("MinimumDisplayAltitude") != null ? double.Parse(ele.Element("MinimumDisplayAltitude").Value) : 0;
                        var MaximumDisplayAltitude = ele.Element("MaximumDisplayAltitude") != null ? double.Parse(ele.Element("MaximumDisplayAltitude").Value) : 1000000;
                        var LineColor = ele.Element("OutlineColor") != null ? GetColor(ele, "OutlineColor") : color;
                        var LineWidth = ele.Element("LineWidth") != null ? float.Parse(ele.Element("LineWidth").Value) : 1.0f;

                         int zonewide = -1;
                        int zoneserial = -1;

                        var coorEle= ele.Element("CoordinateSystem");
                        string CoordinateSystem = "WGS1984";
                        if (coorEle != null)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                        {
                            CoordinateSystem = coorEle.Value;
                            if (CoordinateSystem != "WGS1984")
                            {
                                zonewide = int.Parse(coorEle.Attribute("ZoneWide").Value);
                                zoneserial = int.Parse(coorEle.Attribute("ZoneSerial").Value);
                            }
                        }

                        if (altitudemodeString != null && altitudemodeString.Length > 0)
                        {
                            if (altitudemodeString.ToLower(System.Globalization.CultureInfo.InvariantCulture).Equals("absolute")) alMode = AltitudeMode.Absolute;
                            if (altitudemodeString.ToLower(System.Globalization.CultureInfo.InvariantCulture).Equals("relativetoground")) alMode = AltitudeMode.RelativeToGround;
                            if (altitudemodeString.ToLower(System.Globalization.CultureInfo.InvariantCulture).Equals("clampedtoground")) alMode = AltitudeMode.ClampedToGround;
                        }
                        RenderableObjectList feature = new RenderableObjectList(ele.Attribute("Name").Value);
                        feature.ParentList = parentRenderable;
                        feature.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
     
                        if (fs.FeatureType == FeatureType.Line)
                        {
                            foreach (var f in fs.Features)
                            {
                                points = FromFeatureSet(f, CoordinateSystem, zonewide, zoneserial);
                                LineFeature lf = new LineFeature(f.DataRow[labelField].ToString(), parentWorld, points, color);
                                lf.IsOn = IsOn;
                                lf.Extrude = Extrude;
                                lf.Outline = Outline;
                                lf.LineColor = LineColor;
                                lf.LineWidth = LineWidth;
                                lf.Opacity = Opacity;
                                lf.DistanceAboveSurface = DistanceAboveSurface;
                                lf.AltitudeMode = alMode;
                                lf.MinimumDisplayAltitude = MinimumDisplayAltitude;
                                lf.MaximumDisplayAltitude = MaximumDisplayAltitude;
                                feature.Add(lf);
                            }
                            parentRenderable.Add(feature);
                        }
                        else if (fs.FeatureType == FeatureType.Polygon)
                        {
                            foreach (var f in fs.Features)
                            {
                                points = FromFeatureSet(f, CoordinateSystem, zonewide, zoneserial);
                                LinearRing outerRing = new LinearRing();
                                outerRing.Points = points;
                                PolygonFeature lf = new PolygonFeature(f.DataRow[labelField].ToString(), parentWorld, outerRing, null, color);
                                lf.IsOn = IsOn;
                                lf.Extrude = Extrude;
                                lf.Outline = Outline;
                                lf.Opacity = Opacity;
                                lf.DistanceAboveSurface = DistanceAboveSurface;
                                lf.AltitudeMode = alMode;
                                lf.MinimumDisplayAltitude = MinimumDisplayAltitude;
                                lf.MaximumDisplayAltitude = MaximumDisplayAltitude;
                                feature.Add(lf);
                            }
                            parentRenderable.Add(feature);
                        }
                        else if (fs.FeatureType == FeatureType.Point || fs.FeatureType == FeatureType.MultiPoint)
                        {
                            System.Windows.Forms.MenuItem[] mitems = new System.Windows.Forms.MenuItem[1];
                            mitems[0] = new System.Windows.Forms.MenuItem("Property");
                            foreach (var f in fs.Features)
                            {
                                points = FromFeatureSet(f, CoordinateSystem, zonewide, zoneserial);
                                ClickableIcons icons = new ClickableIcons(feature.Name);
                                foreach (var p in points)
                                {
                                    string textureName = ele.Element("TextureFilePath") != null ? ele.Element("TextureFilePath").Value : "DefaultIcon.png";
                                    textureName = Engine3DSettings.IconPath + textureName;
                                    int width = ele.Element("IconWidthPixels") != null ? int.Parse(ele.Element("IconWidthPixels").Value) : 16;
                                    int height = ele.Element("IconHeightPixels") != null ? int.Parse(ele.Element("IconHeightPixels").Value) : 16;
                                    string DescriptionField = ele.Element("DescriptionField") != null ? ele.Element("DescriptionField").Value : "null";

                                    string name = "";
                                    if (f.DataRow.Table.Columns.Contains(DescriptionField))
                                    { 
                                       name = f.DataRow[DescriptionField].ToString();
                                        if (name.Length > 5)
                                            name = name.Substring(0, 5);
                                    }

                                    Icon ic = new Icon(name,name, p.Y, p.X, DistanceAboveSurface, parentWorld, textureName, width, height, "");
                                    ic.IsOn = bool.Parse(ele.Attribute("ShowAtStartup").Value);
                                    ic.isSelectable = true;
                                    ic.NameAlwaysVisible = ele.Element("AlwaysVisible") != null ? bool.Parse(ele.Element("AlwaysVisible").Value) : true;
                                    //ScreenOverlay overlay = new ScreenOverlay(ic.Name, 0, 0, textureName);
                                    //ic.AddOverlay(overlay);
                                    double onClickZoomAltString = ele.Element("OnClickZoomAltitude") != null ? double.Parse(ele.Element("OnClickZoomAltitude").Value) : 1000;
                                    double onClickZoomHeadingString = ele.Element("OnClickZoomHeading") != null ? double.Parse(ele.Element("OnClickZoomHeading").Value) : 0;
                                    double onClickZoomTiltString = ele.Element("OnClickZoomTilt") != null ? double.Parse(ele.Element("OnClickZoomTilt").Value) : 2.0;

                                    ic.MinimumDisplayDistance = ele.Element("MinimumDisplayAltitude") != null ? double.Parse(ele.Element("MinimumDisplayAltitude").Value) : 0;
                                    ic.MaximumDisplayDistance = ele.Element("MaximumDisplayAltitude") != null ? double.Parse(ele.Element("MaximumDisplayAltitude").Value) : 1000000;
                                    ic.ParentControl = Engine3DSettings.SceneWindow;
                                    ic.ContextMenu = new System.Windows.Forms.ContextMenu(mitems);
                                    if(f.DataRow.Table.Columns.Contains("SiteID"))
                                        ic.Tag = f.DataRow["SiteID"].ToString();
                                    icons.Add(ic);
                                }
                                ConfigurationManager.IConLayers.Add(icons);
                                parentRenderable.Add(icons);
                            }
                            parentRenderable.Add(feature);
                        }
                    }
                }
            }
        }
    }

}
