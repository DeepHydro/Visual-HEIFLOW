using System;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using HUST.WREIS.Dot3D.Renderable;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Configuration;
using HUST.WREIS.Dot3D.Terrain;
using Utility;
using System.Drawing;
using HUST.WREIS.Dot3D.Camera;
using Heiflow.Core;

namespace HUST.WREIS.Dot3D
{
    /// <summary>
    ///
    /// </summary>
    public class World : RenderableObject
    {
        /// <summary>
        /// Persisted user adjustable settings.
        /// </summary>
        public static WorldSettings Settings = new WorldSettings();

        #region Private Members

        double equatorialRadius;
        // TODO: Add ellipsoid parameters to world.
        const double flattening = 6378.135;
        const double SemiMajorAxis = 6378137.0;
        const double SemiMinorAxis = 6356752.31425;
        TerrainAccessor _terrainAccessor;
        RenderableObjectList _renderableObjects;
        private System.Collections.IList onScreenMessages;
        private DateTime lastElevationUpdate = System.DateTime.Now;
        WorldSurfaceRenderer m_WorldSurfaceRenderer = null;
        private RenderableObjectList defaultLayerList = new RenderableObjectList("DefaultLayer");
        private RenderableObjectList dataLayerList ;

        public static string DefaultLayerName
        {
            get { return "DefaultLayer"; }
        }

        public System.Collections.IList OnScreenMessages
        {
            get
            {
                return this.onScreenMessages;
            }
            set
            {
                this.onScreenMessages = value;
            }
        }
        /// <summary>
        /// Holds layers that are not shown in Layer Manager
        /// </summary>
        public RenderableObjectList DefaultLayerList
        {
            get
            {
                return defaultLayerList;
            }
            set
            {
                defaultLayerList = value;
            }
        }
        /// <summary>
        /// holds layer that is a root layer or is a layer group
        /// </summary>
        public RenderableObjectList DataLayerList
        {
            get
            {
                return dataLayerList;
            }
            set
            {
                dataLayerList = value;
            }
        }

        #endregion

        #region Properties
        public WorldSurfaceRenderer WorldSurfaceRenderer
        {
            get
            {
                return m_WorldSurfaceRenderer;
            }
        }
        /*		public string DataDirectory
                {
                    get
                    {
                        return this._dataDirectory;
                    }
                    set
                    {
                        this._dataDirectory = value;
                    }
                } */

        /// <summary>
        /// Whether this world is planet Earth.
        /// </summary>
        public bool IsEarth
        {
            get
            {
                // HACK
                return this.Name == "Earth";
            }
        }
        #endregion

        ProjectedVectorRenderer m_projectedVectorRenderer = null;

        public ProjectedVectorRenderer ProjectedVectorRenderer
        {
            get { return m_projectedVectorRenderer; }
        }

        static World()
        {
            // Don't load settings here - use LoadSettings explicitly
            //LoadSettings();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.World"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <param name="equatorialRadius"></param>
        /// <param name="cacheDirectory"></param>
        /// <param name="terrainAccessor"></param>
        public World(string name, Vector3 position, Quaternion orientation, double equatorialRadius,
            string cacheDirectory,
            TerrainAccessor terrainAccessor)
            : base(name, position, orientation)
        {
            this.equatorialRadius = equatorialRadius;

            this._terrainAccessor = terrainAccessor;
            this._renderableObjects = new RenderableObjectList(this.Name);
            this.MetaData.Add("CacheDirectory", cacheDirectory);

            //	this.m_WorldSurfaceRenderer = new WorldSurfaceRenderer(32, 0, this);
            this.m_projectedVectorRenderer = new ProjectedVectorRenderer(this);
       
            m_outerSphere = new AtmosphericScatteringSphere();
            AtmosphericScatteringSphere.m_fInnerRadius = (float)equatorialRadius;
            AtmosphericScatteringSphere.m_fOuterRadius = (float)equatorialRadius * 1.025f;

            m_outerSphere.Init((float)equatorialRadius * 1.025f, 75, 75);

            RenderableObjectList hotptList = new RenderableObjectList("HotPoints");
            defaultLayerList.Add(hotptList);
        }

        public AtmosphericScatteringSphere m_outerSphere = null;
        public void SetLayerOpacity(string category, string name, float opacity)
        {
            this.setLayerOpacity(this._renderableObjects, category, name, opacity);
        }

        private static string getRenderablePathString(RenderableObject renderable)
        {
            if (renderable.ParentList == null)
            {
                return renderable.Name;
            }
            else
            {
                return getRenderablePathString(renderable.ParentList) + Path.DirectorySeparatorChar + renderable.Name;
            }
        }

        private void setLayerOpacity(RenderableObject ro, string category, string name, float opacity)
        {
            foreach (string key in ro.MetaData.Keys)
            {
                if (String.Compare(key, category, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    if (ro.MetaData[key].GetType() == typeof(String))
                    {
                        string curValue = ro.MetaData[key] as string;
                        if (String.Compare(curValue, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                        {
                            ro.Opacity = (byte)(255 * opacity);
                        }
                    }
                    break;
                }
            }

            RenderableObjectList rol = ro as RenderableObjectList;
            if (rol != null)
            {
                foreach (RenderableObject childRo in rol.ChildObjects)
                    setLayerOpacity(childRo, category, name, opacity);
            }
        }

        /// <summary>
        /// Deserializes settings from default location
        /// </summary>
        public static void LoadSettings()
        {
            try
            {
                Settings = (WorldSettings)SettingsBase.Load(Settings);
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }

        /// <summary>
        /// Deserializes settings from specified location
        /// </summary>
        public static void LoadSettings(string directory)
        {
            try
            {
                Settings = (WorldSettings)SettingsBase.LoadFromPath(Settings, directory);
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }

        public TerrainAccessor TerrainAccessor
        {
            get
            {
                return this._terrainAccessor;
            }
            set
            {
                this._terrainAccessor = value;
            }
        }

        public double EquatorialRadius
        {
            get
            {
                return this.equatorialRadius;
            }
        }

        public RenderableObjectList RenderableObjects
        {
            get
            {
                return this._renderableObjects;
            }
            set
            {
                this._renderableObjects = value;
            }
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            try
            {
                if (this.isInitialized)
                    return;

                this.RenderableObjects.Initialize(drawArgs);
            }
            catch (Exception caught)
            {
                Log.DebugWrite(caught);
            }
            finally
            {
                this.isInitialized = true;
            }
        }

        private void DrawAxis(DrawArgs drawArgs)
        {
            CustomVertex.PositionColored[] axis = new CustomVertex.PositionColored[2];
            Vector3 topV = MathEngine.SphericalToCartesian(90, 0, this.EquatorialRadius + 0.15f * this.EquatorialRadius);
            axis[0].X = topV.X;
            axis[0].Y = topV.Y;
            axis[0].Z = topV.Z;

            axis[0].Color = System.Drawing.Color.Pink.ToArgb();

            Vector3 botV = MathEngine.SphericalToCartesian(-90, 0, this.EquatorialRadius + 0.15f * this.EquatorialRadius);
            axis[1].X = botV.X;
            axis[1].Y = botV.Y;
            axis[1].Z = botV.Z;
            axis[1].Color = System.Drawing.Color.Pink.ToArgb();

            drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
            drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
            drawArgs.device.Transform.World = Matrix.Translation(
                (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                );

            drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 1, axis);
            drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;

        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!this.isInitialized)
            {
                this.Initialize(drawArgs);
            }

            if (this.RenderableObjects != null)
            {
                //foreach(var layer in this.RenderableObjects.ChildObjects)
                //{
                //    if (layer.Name == "DataLayer")
                //    {
                //        foreach (var ro in layer.ChildObjects)
                //        {
                //            if (ro is RenderableObjectList)
                //            {
                //                foreach (var roo in ro.ChildObjects)
                //                {
                //                    if (roo.Name != "Landsat7 Image")
                //                        roo.Update(drawArgs);
                //                }
                //            }
                //            else
                //            {
                //                ro.Update(drawArgs);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        layer.Update(drawArgs);
                //    }
                //}
                this.RenderableObjects.Update(drawArgs);
            }
            if (this.m_WorldSurfaceRenderer != null)
            {
                this.m_WorldSurfaceRenderer.Update(drawArgs);
            }

            if (this.m_projectedVectorRenderer != null)
            {
                this.m_projectedVectorRenderer.Update(drawArgs);
            }

            if (this.TerrainAccessor != null)
            {
                if (drawArgs.WorldCamera.Altitude < 300000)
                {
                    //if (System.DateTime.Now - this.lastElevationUpdate > TimeSpan.FromMilliseconds(500))
                    //{
                        drawArgs.WorldCamera.TerrainElevation = (short)this.TerrainAccessor.GetElevationAt(drawArgs.WorldCamera.Latitude.Degrees, drawArgs.WorldCamera.Longitude.Degrees, 100.0 / drawArgs.WorldCamera.ViewRange.Degrees);
                        this.lastElevationUpdate = System.DateTime.Now;
                    //}
                }
                else
                    drawArgs.WorldCamera.TerrainElevation = 0;
            }
            else
            {
                drawArgs.WorldCamera.TerrainElevation = 0;
            }

            if (World.Settings.EnableAtmosphericScattering && m_outerSphere != null)
                m_outerSphere.Update(drawArgs);
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return this._renderableObjects.PerformSelectionAction(drawArgs);
        }

        private void RenderSun(DrawArgs drawArgs)
        {
            Point3d sunPosition = -SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);

            Point3d sunSpherical = MathEngine.CartesianToSphericalD(sunPosition.X, sunPosition.Y, sunPosition.Z);
            sunPosition = MathEngine.SphericalToCartesianD(
                Angle.FromRadians(sunSpherical.Y),
                Angle.FromRadians(sunSpherical.Z),
                150000000000);

            Vector3 sunVector = new Vector3((float)sunPosition.X, (float)sunPosition.Y, (float)sunPosition.Z);

            Frustum viewFrustum = new Frustum();

            float aspectRatio = (float)drawArgs.WorldCamera.Viewport.Width / drawArgs.WorldCamera.Viewport.Height;
            Matrix projectionMatrix = Matrix.PerspectiveFovRH((float)drawArgs.WorldCamera.Fov.Radians, aspectRatio, 1.0f, 300000000000);

            viewFrustum.Update(
                Matrix.Multiply(drawArgs.WorldCamera.AbsoluteWorldMatrix,
                Matrix.Multiply(drawArgs.WorldCamera.AbsoluteViewMatrix,
                    projectionMatrix)));

            if (!viewFrustum.ContainsPoint(sunVector))
                return;

            Vector3 translationVector = new Vector3(
                (float)(sunPosition.X - drawArgs.WorldCamera.ReferenceCenter.X),
                (float)(sunPosition.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                (float)(sunPosition.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

            Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

            if (m_sunTexture == null)
            {
                m_sunTexture = ImageHelper.LoadTexture(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Resources\\DX\\sun.dds");
                m_sunSurfaceDescription = m_sunTexture.GetLevelDescription(0);
            }

            if (m_sprite == null)
            {
                m_sprite = new Sprite(drawArgs.device);
            }

            m_sprite.Begin(SpriteFlags.AlphaBlend);

            // Render icon
            float xscale = (float)m_sunWidth / m_sunSurfaceDescription.Width;
            float yscale = (float)m_sunHeight / m_sunSurfaceDescription.Height;
            m_sprite.Transform = Matrix.Scaling(xscale, yscale, 0);

            m_sprite.Transform *= Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
            m_sprite.Draw(m_sunTexture,
                new Vector3(m_sunSurfaceDescription.Width >> 1, m_sunSurfaceDescription.Height >> 1, 0),
                Vector3.Empty,
                System.Drawing.Color.FromArgb(253, 253, 200).ToArgb());

            // Reset transform to prepare for text rendering later
            m_sprite.Transform = Matrix.Identity;
            m_sprite.End();
        }

        int m_sunWidth = 72;
        int m_sunHeight = 72;

        Sprite m_sprite = null;
        Texture m_sunTexture = null;
        SurfaceDescription m_sunSurfaceDescription;

        public override void Render(DrawArgs drawArgs)
        {
            try
            {

                if (m_WorldSurfaceRenderer != null && World.Settings.UseWorldSurfaceRenderer)
                {
                    m_WorldSurfaceRenderer.RenderSurfaceImages(drawArgs);
                }         

                RenderStars(drawArgs, RenderableObjects);

                if (drawArgs.CurrentWorld.IsEarth && World.Settings.EnableAtmosphericScattering)
                {
                    float aspectRatio = (float)drawArgs.WorldCamera.Viewport.Width / drawArgs.WorldCamera.Viewport.Height;
                    float zNear = (float)drawArgs.WorldCamera.Altitude * 0.1f;
                    double distToCenterOfPlanet = (drawArgs.WorldCamera.Altitude + equatorialRadius);
                    double tangentalDistance = Math.Sqrt(distToCenterOfPlanet * distToCenterOfPlanet - equatorialRadius * equatorialRadius);
                    double amosphereThickness = Math.Sqrt(m_outerSphere.m_radius * m_outerSphere.m_radius + equatorialRadius * equatorialRadius);
                    Matrix proj = drawArgs.device.Transform.Projection;
                    drawArgs.device.Transform.Projection = Matrix.PerspectiveFovRH((float)drawArgs.WorldCamera.Fov.Radians, aspectRatio, zNear, (float)(tangentalDistance + amosphereThickness));
                    drawArgs.device.RenderState.ZBufferEnable = false;
                    drawArgs.device.RenderState.CullMode = Cull.CounterClockwise;
                    m_outerSphere.Render(drawArgs);
                    drawArgs.device.RenderState.CullMode = Cull.Clockwise;
                    drawArgs.device.RenderState.ZBufferEnable = true;

                    drawArgs.device.Transform.Projection = proj;
                }

                //if (World.Settings.RenderCarsianGridOnly)
                //{
                //    RenderableObject ro = (DataLayerList["分布式流场"] as RenderableObjectList)["计算网格"];
                //    if (ro != null && ro is CartesianGridLayer)
                //    {
                //        ro.Render(drawArgs);
                //    }
                //}
                //else
                //{
                    if (World.Settings.EnableSunShading)
                        RenderSun(drawArgs);

                    //render SurfaceImages
                    Render(RenderableObjects, HUST.WREIS.Dot3D.Renderable.RenderPriority.TerrainMappedImages, drawArgs);

                    if (m_projectedVectorRenderer != null)
                        m_projectedVectorRenderer.Render(drawArgs);

                    //render AtmosphericImages
                    Render(RenderableObjects, HUST.WREIS.Dot3D.Renderable.RenderPriority.AtmosphericImages, drawArgs);

                    //render LinePaths
                    Render(RenderableObjects, HUST.WREIS.Dot3D.Renderable.RenderPriority.LinePaths, drawArgs);

                    //render Placenames
                    Render(RenderableObjects, HUST.WREIS.Dot3D.Renderable.RenderPriority.Placenames, drawArgs);

                    //render Icons
                    Render(RenderableObjects, HUST.WREIS.Dot3D.Renderable.RenderPriority.Icons, drawArgs);

                    //render Custom
                    Render(RenderableObjects, HUST.WREIS.Dot3D.Renderable.RenderPriority.Custom, drawArgs);

                    if (Settings.ShowPlanetAxis)
                        this.DrawAxis(drawArgs);
                //}
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void RenderStars(DrawArgs drawArgs, HUST.WREIS.Dot3D.Renderable.RenderableObject renderable)
        {
            if (renderable is RenderableObjectList)
            {
                RenderableObjectList rol = (RenderableObjectList)renderable;
                for (int i = 0; i < rol.ChildObjects.Count; i++)
                {
                    RenderStars(drawArgs, (RenderableObject)rol.ChildObjects[i]);
                }
            }
            else if (renderable.Name != null && renderable.Name.Equals("Starfield"))
            {
                try
                {
                    renderable.Render(drawArgs);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }
        }

        private void Render(HUST.WREIS.Dot3D.Renderable.RenderableObject renderable, HUST.WREIS.Dot3D.Renderable.RenderPriority priority, DrawArgs drawArgs)
        {
            if (!renderable.IsOn || (renderable.Name != null && renderable.Name.Equals("Starfield")))
                return;

            try
            {
                if (priority == HUST.WREIS.Dot3D.Renderable.RenderPriority.Icons && renderable is Icons)
                {
                    renderable.Render(drawArgs);
                }
                else if (renderable is HUST.WREIS.Dot3D.Renderable.RenderableObjectList)
                {
                    HUST.WREIS.Dot3D.Renderable.RenderableObjectList rol = (HUST.WREIS.Dot3D.Renderable.RenderableObjectList)renderable;
                    for (int i = 0; i < rol.ChildObjects.Count; i++)
                    {
                        Render((HUST.WREIS.Dot3D.Renderable.RenderableObject)rol.ChildObjects[i], priority, drawArgs);
                    }
                }
                // hack at the moment
                else if (priority == HUST.WREIS.Dot3D.Renderable.RenderPriority.TerrainMappedImages)
                {
                    if (renderable.RenderPriority == HUST.WREIS.Dot3D.Renderable.RenderPriority.SurfaceImages || renderable.RenderPriority == HUST.WREIS.Dot3D.Renderable.RenderPriority.TerrainMappedImages)
                    {
                        renderable.Render(drawArgs);
                    }
                }
                else if (renderable.RenderPriority == priority)
                {
                    renderable.Render(drawArgs);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void saveRenderableState(RenderableObject ro)
        {
            string path = getRenderablePathString(ro);
            bool found = false;
            for (int i = 0; i < World.Settings.loadedLayers.Count; i++)
            {
                string s = (string)World.Settings.loadedLayers[i];
                if (s.Equals(path))
                {
                    if (!ro.IsOn)
                    {
                        World.Settings.loadedLayers.RemoveAt(i);
                        break;

                    }
                    else
                    {
                        found = true;
                    }
                }
            }

            if (!found && ro.IsOn)
            {
                World.Settings.loadedLayers.Add(path);
            }
        }

        private void saveRenderableStates(RenderableObjectList rol)
        {
            saveRenderableState(rol);

            foreach (RenderableObject ro in rol.ChildObjects)
            {
                if (ro is RenderableObjectList)
                {
                    RenderableObjectList childRol = (RenderableObjectList)ro;
                    saveRenderableStates(childRol);
                }
                else
                {
                    saveRenderableState(ro);
                }
            }
        }

        public override void Dispose()
        {
            saveRenderableStates(RenderableObjects);

            if (this.RenderableObjects != null)
            {
                this.RenderableObjects.Dispose();
                this.RenderableObjects = null;
            }

            if (m_WorldSurfaceRenderer != null)
            {
                m_WorldSurfaceRenderer.Dispose();
            }

            if (m_outerSphere != null)
            {
                m_outerSphere.Dispose();
            }
        }

        /// <summary>
        /// Computes the great circle distance between two pairs of lat/longs.
        /// TODO: Compute distance using ellipsoid.
        /// </summary>
        public static Angle ApproxAngularDistance(Angle latA, Angle lonA, Angle latB, Angle lonB)
        {
            Angle dlon = lonB - lonA;
            Angle dlat = latB - latA;
            double k = Math.Sin(dlat.Radians * 0.5);
            double l = Math.Sin(dlon.Radians * 0.5);
            double a = k * k + Math.Cos(latA.Radians) * Math.Cos(latB.Radians) * l * l;
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            return Angle.FromRadians(c);
        }

        /// <summary>
        /// Computes the distance between two pairs of lat/longs in meters.
        /// </summary>
        public double ApproxDistance(Angle latA, Angle lonA, Angle latB, Angle lonB)
        {
            double distance = equatorialRadius * ApproxAngularDistance(latA, lonA, latB, lonB).Radians;
            return distance;
        }

        /// <summary>
        /// Intermediate points on a great circle
        /// In previous sections we have found intermediate points on a great circle given either
        /// the crossing latitude or longitude. Here we find points (lat,lon) a given fraction of the
        /// distance (d) between them. Suppose the starting point is (lat1,lon1) and the final point
        /// (lat2,lon2) and we want the point a fraction f along the great circle route. f=0 is
        /// point 1. f=1 is point 2. The two points cannot be antipodal ( i.e. lat1+lat2=0 and
        /// abs(lon1-lon2)=pi) because then the route is undefined.
        /// </summary>
        /// <param name="f">Fraction of the distance for intermediate point (0..1)</param>
        public static void IntermediateGCPoint(float f, Angle lat1, Angle lon1, Angle lat2, Angle lon2, Angle d,
            out Angle lat, out Angle lon)
        {
            double sind = Math.Sin(d.Radians);
            double cosLat1 = Math.Cos(lat1.Radians);
            double cosLat2 = Math.Cos(lat2.Radians);
            double A = Math.Sin((1 - f) * d.Radians) / sind;
            double B = Math.Sin(f * d.Radians) / sind;
            double x = A * cosLat1 * Math.Cos(lon1.Radians) + B * cosLat2 * Math.Cos(lon2.Radians);
            double y = A * cosLat1 * Math.Sin(lon1.Radians) + B * cosLat2 * Math.Sin(lon2.Radians);
            double z = A * Math.Sin(lat1.Radians) + B * Math.Sin(lat2.Radians);
            lat = Angle.FromRadians(Math.Atan2(z, Math.Sqrt(x * x + y * y)));
            lon = Angle.FromRadians(Math.Atan2(y, x));
        }

        /// <summary>
        /// Intermediate points on a great circle
        /// In previous sections we have found intermediate points on a great circle given either
        /// the crossing latitude or longitude. Here we find points (lat,lon) a given fraction of the
        /// distance (d) between them. Suppose the starting point is (lat1,lon1) and the final point
        /// (lat2,lon2) and we want the point a fraction f along the great circle route. f=0 is
        /// point 1. f=1 is point 2. The two points cannot be antipodal ( i.e. lat1+lat2=0 and
        /// abs(lon1-lon2)=pi) because then the route is undefined.
        /// </summary>
        /// <param name="f">Fraction of the distance for intermediate point (0..1)</param>
        public Vector3 IntermediateGCPoint(float f, Angle lat1, Angle lon1, Angle lat2, Angle lon2, Angle d)
        {
            double sind = Math.Sin(d.Radians);
            double cosLat1 = Math.Cos(lat1.Radians);
            double cosLat2 = Math.Cos(lat2.Radians);
            double A = Math.Sin((1 - f) * d.Radians) / sind;
            double B = Math.Sin(f * d.Radians) / sind;
            double x = A * cosLat1 * Math.Cos(lon1.Radians) + B * cosLat2 * Math.Cos(lon2.Radians);
            double y = A * cosLat1 * Math.Sin(lon1.Radians) + B * cosLat2 * Math.Sin(lon2.Radians);
            double z = A * Math.Sin(lat1.Radians) + B * Math.Sin(lat2.Radians);
            Angle lat = Angle.FromRadians(Math.Atan2(z, Math.Sqrt(x * x + y * y)));
            Angle lon = Angle.FromRadians(Math.Atan2(y, x));

            Vector3 v = MathEngine.SphericalToCartesian(lat, lon, equatorialRadius);
            return v;
        }
    }

   
}