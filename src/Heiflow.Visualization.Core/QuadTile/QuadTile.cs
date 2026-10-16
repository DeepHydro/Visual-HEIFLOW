//#define LOD

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using HUST.WREIS.Dot3D.Terrain;
using HUST.WREIS.Dot3D.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using HUST.WREIS.Dot3D.Net;
using Utility;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using Heiflow.Core;
using Heiflow.Core.MyMath;
using Heiflow.Spatial.Geography;

namespace HUST.WREIS.Dot3D.Renderable
{

	public class QuadTile : IDisposable
	{
       
		/// <summary>
		/// Child tile location
		/// </summary>
		public enum ChildLocation
		{
			NorthWest,
			SouthWest,
			NorthEast,
			SouthEast
		}

		public QuadTileSet QuadTileSet;
		public double West;
		public double East;
		public double North;
		public double South;
		public Angle CenterLatitude;
		public Angle CenterLongitude;
		public double LatitudeSpan;
		public double LongitudeSpan;
		public int Level;
		public int Row;
		public int Col;

		public bool isInitialized;
		public BoundingBox BoundingBox;
		public GeoImageDownloadRequest DownloadRequest;

		protected Texture[] textures;

		/// <summary>
		/// Number of points in child flat mesh grid (times 2)
		/// </summary>
		protected static int vertexCount = 40;

		/// <summary>
		/// Number of points in child terrain mesh grid (times 2)
		/// </summary>
		protected static int vertexCountElevated = 40;

		protected QuadTile northWestChild;
		protected QuadTile southWestChild;
		protected QuadTile northEastChild;
		protected QuadTile southEastChild;

		protected CustomVertex.PositionNormalTextured[] northWestVertices;
		protected CustomVertex.PositionNormalTextured[] southWestVertices;
		protected CustomVertex.PositionNormalTextured[] northEastVertices;
		protected CustomVertex.PositionNormalTextured[] southEastVertices;
		protected short[] vertexIndexes;

		/// <summary>
		/// GPU copy of the tile mesh. All four quadrants go into one buffer, one after the other,
		/// and the index buffer holds the four ranges that address them, so a tile that has no
		/// child covering any of its quadrants is a single draw instead of four. Drawing used to
		/// go through DrawIndexedUserPrimitives, which handed these arrays to the driver again on
		/// every frame; the buffers are uploaded once and then only referenced.
		/// </summary>
		protected VertexBuffer tileVertexBuffer;
		protected IndexBuffer indexBuffer;

		/// <summary>
		/// How many vertices and indices one quadrant has. The four blocks in the buffers above
		/// are this big, which is what lets a draw name a quadrant by its range.
		/// </summary>
		protected int m_QuadrantVertexCount;
		protected int m_QuadrantIndexCount;

		/// <summary>
		/// The mesh arrays above were rebuilt and the buffers have not been filled from them yet.
		/// Same split as m_TexturePending: the update thread builds the arrays, only Render may
		/// touch the device.
		/// </summary>
		protected bool m_MeshPending = false;

        protected Point3d localOrigin; // Add this offset to get world coordinates

        protected bool m_isResetingCache;

        public string ImageFilePath = null;
        static Effect grayscaleEffect = null;
		/// <summary>
		/// The vertical exaggeration the tile mesh was computed for
		/// </summary>
        protected float verticalExaggeration;

        protected bool isDownloadingTerrain;


        public string Name { get; set; }
		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.QuadTile"/> class.
		/// </summary>
		/// <param name="south"></param>
		/// <param name="north"></param>
		/// <param name="west"></param>
		/// <param name="east"></param>
		/// <param name="level"></param>
		/// <param name="quadTileSet"></param>
		public QuadTile(double south, double north, double west, double east, int level, QuadTileSet quadTileSet)
		{
			this.South = south;
			this.North = north;
			this.West = west;
			this.East = east;
			CenterLatitude = Angle.FromDegrees(0.5f * (North + South));
			CenterLongitude = Angle.FromDegrees(0.5f * (West + East));
			LatitudeSpan = Math.Abs(North - South);
			LongitudeSpan = Math.Abs(East - West);

			this.Level = level;
			this.QuadTileSet = quadTileSet;

			BoundingBox = new BoundingBox((float)south, (float)north, (float)west, (float)east,
								(float)quadTileSet.LayerRadius, (float)quadTileSet.LayerRadius + 300000f);
			//localOrigin = BoundingBox.CalculateCenter();
			localOrigin = MathEngine.SphericalToCartesianD(CenterLatitude, CenterLongitude, quadTileSet.LayerRadius);

			// To avoid gaps between neighbouring tiles truncate the origin to 
			// a number that doesn't get rounded. (nearest 10km)
			localOrigin.X = (float)(Math.Round(localOrigin.X / 100000) * 100000);
			localOrigin.Y = (float)(Math.Round(localOrigin.Y / 100000) * 100000);
			localOrigin.Z = (float)(Math.Round(localOrigin.Z / 100000) * 100000);

			Row = MathEngine.GetRowFromLatitude(South, North - South);
			Col = MathEngine.GetColFromLongitude(West, North - South);
            Name = string.Format("{0}_{1}_{2}", level, Row, Col);
		}

		/// <summary>
		/// How far the sphere drops between the middle of the tile and the plane through its four
		/// corners. The bounding box is the box spanned by those corners, so a box with no slack
		/// would sit below the middle of a wide tile and the frustum test would throw the tile
		/// away even though it fills the screen.
		/// </summary>
		protected static double ChordSag(double layerRadius, double latitudeSpan, double longitudeSpan)
		{
			double maxSpan = Math.Max(latitudeSpan, longitudeSpan) * Math.PI / 180.0;
			return layerRadius * (1.0 - Math.Cos(0.5 * maxSpan));
		}

		public virtual void ResetCache()
		{
			try
			{
				m_isResetingCache = true;
				this.isInitialized = false;

				if (northEastChild != null)
				{
					northEastChild.ResetCache();
				}

				if (northWestChild != null)
				{
					northWestChild.ResetCache();
				}

				if (southEastChild != null)
				{
					southEastChild.ResetCache();
				}

				if (southWestChild != null)
				{
					southWestChild.ResetCache();
				}

				this.Dispose();

                //for (int i = 0; i < QuadTileSet.ImageStores.Length; i++)
                //{
                //    if((QuadTileSet.ImageStores[i] != null) && QuadTileSet.ImageStores[i].IsDownloadableLayer)
                //        QuadTileSet.ImageStores[i].DeleteLocalCopy(this);
                //}

				m_isResetingCache = false;
			}
            catch (Exception caught)
            {
                Log.Write(caught);
            }
		}

		/// <summary>
		/// Returns the QuadTile for specified location if available.
		/// Tries to queue a download if not available.
		/// </summary>
		/// <returns>Initialized QuadTile if available locally, else null.</returns>
		protected virtual QuadTile ComputeChild(double childSouth, double childNorth, double childWest, double childEast)
		{
			QuadTile child = new QuadTile(
				childSouth,
				childNorth,
				childWest,
				childEast,
				this.Level + 1,
				QuadTileSet);
            //child.ImageFilePath=
			return child;
		}

		public virtual void ComputeChildren(DrawArgs drawArgs)
		{
			if (Level + 1 >= QuadTileSet.ImageStores[0].LevelCount)
				return;

			double CenterLat = 0.5f * (South + North);
			double CenterLon = 0.5f * (East + West);
			if (northWestChild == null)
				northWestChild = ComputeChild(CenterLat, North, West, CenterLon);

			if (northEastChild == null)
				northEastChild = ComputeChild(CenterLat, North, CenterLon, East);

			if (southWestChild == null)
				southWestChild = ComputeChild(South, CenterLat, West, CenterLon);

			if (southEastChild == null)
				southEastChild = ComputeChild(South, CenterLat, CenterLon, East);
		}

		public virtual void Dispose()
		{
			try
			{
				isInitialized = false;
                if (textures != null)
                {
                    for (int i = 0; i < textures.Length; i++)
                    {
                        if (textures[i] != null && !textures[i].Disposed)
                        {
                            textures[i].Dispose();
                            textures[i] = null;
                        }
                    }
                }
                textures = null;
				DisposeMeshBuffers();
				if (northWestChild != null)
				{
					northWestChild.Dispose();
					northWestChild = null;
				}
				if (southWestChild != null)
				{
					southWestChild.Dispose();
					southWestChild = null;
				}
				if (northEastChild != null)
				{
					northEastChild.Dispose();
					northEastChild = null;
				}
				if (southEastChild != null)
				{
					southEastChild.Dispose();
					southEastChild = null;
				}
				if(DownloadRequest != null)
				{
					QuadTileSet.RemoveFromDownloadQueue(DownloadRequest);
					DownloadRequest.Dispose();
					DownloadRequest = null;
				}
			}
			catch
			{
			}
		}

		public bool WaitingForDownload = false;
		public bool IsDownloadingImage = false;

        /// <summary>
        /// How many tiles may be built during one update sweep. Every tile costs a jpeg read, a
        /// texture upload and a vertex buffer, all of which go to the device from the update thread
        /// and hold up the render thread while they are being marshalled. Without a budget one sweep
        /// built 60+ tiles at once and the globe stopped for many seconds.
        /// 0 or less switches the limit off.
        /// </summary>
        public static int MaxTileInitsPerSweep = 16;

        /// <summary>
        /// Vertical slack added above and below a tile's bounding box, on top of the sag of the
        /// chord across it, see ChordSag. The box has to hold the mesh, and the camera can sit
        /// inside it, and a box made only as thick as the tile turned out to be too tight: from
        /// about 110 km up the layer stopped being drawn altogether. This is deliberately
        /// generous where the old fixed height was absurd - 30 km here against 30000000000
        /// metres there - so the box still culls, it just no longer shaves the ground off.
        ///
        /// Do not shrink this to be tidy. The tile mesh is built at a fixed LayerRadius, a
        /// sphere, while the ground is an ellipsoid whose equatorial radius exceeds its polar
        /// radius by 21.4 km; the gap between the two is widest at low latitudes. At 1 km the
        /// real position of a tile below about 28 degrees north fell outside its own bounding
        /// box, and looking straight down - where that gap lies along the view direction and
        /// decides whether the box meets the frustum at all - the tile was culled, leaving a
        /// blank patch that came back the moment the camera was tilted: the oblique view
        /// crosses the box instead of meeting it end on. 30 km is past the flattening, and is
        /// still 0.5% of the earth's radius, so the box keeps culling.
        /// </summary>
        public static double BoundingBoxSlack = 30000;

        static int tileInitsThisSweep = 0;

        /// <summary>Call once per update sweep, before the tiles are visited.</summary>
        public static void BeginUpdateSweep()
        {
            tileInitsThisSweep = 0;
        }

        /// <summary>
        /// How many tile textures may be built during one rendered frame. Building one is a jpeg
        /// decode plus a DXT compression plus an upload, measured at 10.84 ms here, and it is done
        /// on the render thread, so it comes straight off the frame time. Two was too slow to keep
        /// up with a zoom in: the tree refines every frame, the new children wait for textures that
        /// arrive two a frame, and until one does the parent draws its own coarser image - which a
        /// few tens of kilometres up is a level 1 tile stretched over the screen, and reads as no
        /// image at all. Eight, with the time budget below still doing the real limiting.
        /// 0 or less switches the limit off.
        /// </summary>
        public static int MaxTextureCreationsPerFrame = 8;

        /// <summary>
        /// How much of one frame may go into tile textures, in milliseconds. Counting textures is
        /// not enough because one already costs 15 to 25 ms, so two of them make a 40 ms frame no
        /// matter how fast the rest is. 12 let one texture through a frame, which is what made a
        /// zoom in look like the layer had stopped drawing, see MaxTextureCreationsPerFrame. 60
        /// makes room for about three, so a screen fills in a few seconds instead of tens, and it
        /// only bites while tiles are actually arriving. 0 or less switches it off, then
        /// MaxTextureCreationsPerFrame is the only limit.
        /// </summary>
        public static float MaxTextureMillisecondsPerFrame = 30;

        static int textureCreationsThisFrame = 0;
        static long textureBudgetStartTicks = 0;

        /// <summary>Call once per rendered frame, before the tiles are drawn.</summary>
        public static void BeginRenderFrame()
        {
            textureCreationsThisFrame = 0;
            textureBudgetStartTicks = 0;
        }

        /// <summary>
        /// The image is on disk but its texture has not been built yet. Set by the update thread,
        /// which is not allowed to touch the device, and picked up by Render.
        /// </summary>
        bool m_TexturePending = false;

        /// <summary>
        /// Builds the textures of this tile. Only ever called from the render thread, see
        /// MaxTextureCreationsPerFrame.
        /// </summary>
        void CreatePendingTextures()
        {
            m_TexturePending = false;

            if (textures == null || textures.Length == 0)
                return;

            if (MaxTextureCreationsPerFrame > 0 && textureCreationsThisFrame >= MaxTextureCreationsPerFrame)
            {
                // Out of budget for this frame, try again in the next one.
                m_TexturePending = true;
                return;
            }

            long budgetNow = Stopwatch.GetTimestamp();
            if (textureCreationsThisFrame > 0 && MaxTextureMillisecondsPerFrame > 0 &&
                1000.0f * (budgetNow - textureBudgetStartTicks) / Stopwatch.Frequency > MaxTextureMillisecondsPerFrame)
            {
                // This frame has already spent its budget on textures, leave the rest for the
                // next one, see MaxTextureMillisecondsPerFrame.
                m_TexturePending = true;
                return;
            }

            if (textureCreationsThisFrame == 0)
                textureBudgetStartTicks = budgetNow;

            textureCreationsThisFrame++;

            // Read into a local first, Dispose sets the slots to null too.
            int storeCount = textures.Length;
            for (int i = 0; i < storeCount; i++)
            {
                Texture newTexture = QuadTileSet.ImageStores[i].LoadFile(this);

                if (textures == null)
                {
                    // The tile is already gone, there is nothing to put the image in and it has
                    // to be released here or it stays on the GPU for the rest of the run.
                    if (newTexture != null && !newTexture.Disposed)
                        newTexture.Dispose();
                    return;
                }

                Texture oldTexture = textures[i];
                if (oldTexture != null && !oldTexture.Disposed)
                    oldTexture.Dispose();
                textures[i] = newTexture;
            }
        }

		public virtual void Initialize()
		{
			if (m_isResetingCache)
				return;

            //try
            //{
				if (DownloadRequest != null)
				{
					// Waiting for download
					return;
				}
                if (textures == null)
                {
                    textures = new Texture[QuadTileSet.ImageStores.Length];

                   //  not strictly necessary
                    //for (int i = 0; i < textures.Length; i++)
                    //    textures[i] = null;
                  
                }

                // assume we're finished.
				WaitingForDownload = false;

                // check for missing textures.
                if (textures != null && textures.Length > 0)
                {
                    // A finished download calls Initialize from the download thread, while the render
                    // thread disposes the tile, and with it the array, as soon as it leaves the view.
                    // The length is cached and the array is tested again before every write, because
                    // testing it once above is not enough: Dispose can run during LoadFile, and the
                    // write below was then the first thing to touch the null and crash the layer.
                    int storeCount = textures.Length;
                    for (int i = 0; i < storeCount; i++)
                    {
                        // This runs on the update thread and must not touch the device: every
                        // device call from here is serviced by the thread that owns the device,
                        // which is the thread the render loop runs on, and while tiles were
                        // arriving that blocked rendering for hundreds of ms at a time. So this
                        // only asks whether the image is there - and queues a download when it is
                        // not - and leaves the texture itself to CreatePendingTextures, which
                        // Render calls on the render thread.
                        if (QuadTileSet.ImageStores[i].IsImageAvailable(this))
                            m_TexturePending = true;
                        else
                        {
                            // At least one texture missing, wait for download
                            WaitingForDownload = false;
                        }

                        if (textures == null)
                            return;
                    }
                }
                if (WaitingForDownload)
                    return;

				IsDownloadingImage = false;
				CreateTileMesh();
                //isInitialized = true;
			//}	
            //catch(Exception ex)
            //{
            //    if (textures != null)
            //    {
            //        Exception newex = new Exception(ex.Message + "; current textures length:" + textures.Length);
            //        Log.Write(newex);
            //    }
            //    else
            //    {
            //        Log.Write(ex);
            //    }
            //    // Texture load failed.
            //}
            //finally
            //{
                isInitialized = true;
            //}
		}
		/// <summary>
		/// Updates this layer (background)
		/// </summary>
		public virtual void Update(DrawArgs drawArgs)
		{			
			if (m_isResetingCache)
				return;

			double tileSize = North - South;

				if (!isInitialized)
				{
    //                if (DrawArgs.Camera.ViewRange * 0.5f < Angle.FromDegrees(QuadTileSet.TileDrawDistance * tileSize)
    //&& MathEngine.SphericalDistance(CenterLatitude, CenterLongitude,
    //                        DrawArgs.Camera.Latitude, DrawArgs.Camera.Longitude) < Angle.FromDegrees(QuadTileSet.TileDrawSpread * tileSize * 1.25f)
    //&& DrawArgs.Camera.ViewFrustum.Intersects(BoundingBox)
    //                    )
                    // Only a few tiles per sweep, the rest wait for the next one, so that the
                    // render thread is not blocked by a burst of texture uploads. See MaxTileInitsPerSweep.
                    if (MaxTileInitsPerSweep > 0 && tileInitsThisSweep >= MaxTileInitsPerSweep)
                        return;

                    tileInitsThisSweep++;
						Initialize();
				}

				if (isInitialized && World.Settings.VerticalExaggeration != verticalExaggeration || m_CurrentOpacity != QuadTileSet.Opacity ||
					QuadTileSet.RenderStruts != renderStruts)
				{
					CreateTileMesh();
				}

                if (isInitialized)
                {
                    int t=0;
                    if (Level >= 15)
                        t++;
                 //   var alt = DrawArgs.Camera.Altitude;
                   // DrawArgs.Camera.Altitude = DrawArgs.Camera.Distance;
                    Angle vr = Angle.FromDegrees(QuadTileSet.TileDrawDistance * tileSize);
                    Angle sd = MathEngine.SphericalDistance(CenterLatitude, CenterLongitude, DrawArgs.Camera.Latitude, DrawArgs.Camera.Longitude);
                    Angle fd = Angle.FromDegrees(QuadTileSet.TileDrawSpread * tileSize);
                    Angle cvr = DrawArgs.Camera.ViewRange;
                    //DrawArgs.Camera.Altitude = alt;

                    if (cvr < vr && sd < fd && DrawArgs.Camera.ViewFrustum.Intersects(BoundingBox))
                    {
                        if (northEastChild == null || northWestChild == null || southEastChild == null || southWestChild == null)
                        {
                            ComputeChildren(drawArgs);
                        }

                        if (northEastChild != null)
                        {
                            northEastChild.Update(drawArgs);
                        }

                        if (northWestChild != null)
                        {
                            northWestChild.Update(drawArgs);
                        }

                        if (southEastChild != null)
                        {
                            southEastChild.Update(drawArgs);
                        }

                        if (southWestChild != null)
                        {
                            southWestChild.Update(drawArgs);
                        }
                    }
                    else
                    {
                        //if (northWestChild != null)
                        //{
                        //    northWestChild.Dispose();
                        //    northWestChild = null;
                        //}

                        //if (northEastChild != null)
                        //{
                        //    northEastChild.Dispose();
                        //    northEastChild = null;
                        //}

                        //if (southEastChild != null)
                        //{
                        //    southEastChild.Dispose();
                        //    southEastChild = null;
                        //}

                        //if (southWestChild != null)
                        //{
                        //    southWestChild.Dispose();
                        //    southWestChild = null;
                        //}
                    }
                }

				if (isInitialized)
				{
					if (DrawArgs.Camera.ViewRange / 2 > Angle.FromDegrees(QuadTileSet.TileDrawDistance * tileSize * 1.5f)
							|| MathEngine.SphericalDistance(CenterLatitude, CenterLongitude, DrawArgs.Camera.Latitude, DrawArgs.Camera.Longitude) > Angle.FromDegrees(QuadTileSet.TileDrawSpread * tileSize * 1.5f))
					{
						if (Level != 0 || (Level == 0 && !QuadTileSet.AlwaysRenderBaseTiles))
							this.Dispose();
					}
				}

		}


		bool renderStruts = true;

		/// <summary>
		/// Builds flat or terrain mesh for current tile
		/// </summary>
		public virtual void CreateTileMesh()
		{
			verticalExaggeration = World.Settings.VerticalExaggeration;
			m_CurrentOpacity = QuadTileSet.Opacity;
			renderStruts = QuadTileSet.RenderStruts;

			if (QuadTileSet.TerrainMapped && Math.Abs(verticalExaggeration) > 1e-3)
			{
				CreateElevatedMesh();
			}
			else
			{
				CreateFlatMesh();
			}

			// The arrays are filled in, but this runs on the update thread and the device belongs
			// to the render thread, so the buffers are uploaded by Render instead, see m_MeshPending.
			m_MeshPending = true;
		}

		/// <summary>
		/// Uploads the mesh arrays into vertex and index buffers. Only ever called from Render,
		/// see m_MeshPending.
		/// </summary>
		protected void CreatePendingMesh()
		{
			try
			{
				Device device = DrawArgs.Device;
				if (device == null || northWestVertices == null || vertexIndexes == null)
					return;

				// Cleared only once the buffers really exist, so a tile whose first draw happens
				// before its mesh is finished asks again on the next frame instead of never.
				m_MeshPending = false;

				// Drop the previous buffers first: the mesh was rebuilt, so they hold stale data.
				DisposeMeshBuffers();

				m_QuadrantVertexCount = northWestVertices.Length;
				m_QuadrantIndexCount = vertexIndexes.Length;

				// The four quadrants are laid out one after the other, in the order the draws
				// name them: north west, south west, north east, south east.
				CustomVertex.PositionNormalTextured[] vertices =
					new CustomVertex.PositionNormalTextured[4 * m_QuadrantVertexCount];
				Array.Copy(northWestVertices, 0, vertices, 0, m_QuadrantVertexCount);
				Array.Copy(southWestVertices, 0, vertices, m_QuadrantVertexCount, m_QuadrantVertexCount);
				Array.Copy(northEastVertices, 0, vertices, 2 * m_QuadrantVertexCount, m_QuadrantVertexCount);
				Array.Copy(southEastVertices, 0, vertices, 3 * m_QuadrantVertexCount, m_QuadrantVertexCount);

				// One index range per quadrant, each shifted onto its own block of vertices, so
				// drawing a quadrant is a matter of naming the range rather than of binding
				// another buffer.
				short[] indices = new short[4 * m_QuadrantIndexCount];
				for (int quadrant = 0; quadrant < 4; quadrant++)
				{
					int indexOffset = quadrant * m_QuadrantIndexCount;
					int vertexOffset = quadrant * m_QuadrantVertexCount;
					for (int i = 0; i < m_QuadrantIndexCount; i++)
						indices[indexOffset + i] = (short)(vertexIndexes[i] + vertexOffset);
				}

				tileVertexBuffer = CreateVertexBuffer(device, vertices);
				indexBuffer = CreateIndexBuffer(device, indices);
			}
			catch (Exception caught)
			{
				// Leave the buffers null, Render then falls back to the arrays for this tile.
				DisposeMeshBuffers();
				Log.Write(caught);
			}
		}

		static VertexBuffer CreateVertexBuffer(Device device, CustomVertex.PositionNormalTextured[] vertices)
		{
			VertexBuffer buffer = new VertexBuffer(
				typeof(CustomVertex.PositionNormalTextured),
				vertices.Length,
				device,
				Usage.None,
				CustomVertex.PositionNormalTextured.Format,
				Pool.Managed);

			buffer.SetData(vertices, 0, LockFlags.None);
			return buffer;
		}

		static IndexBuffer CreateIndexBuffer(Device device, short[] indices)
		{
			IndexBuffer buffer = new IndexBuffer(
				typeof(short),
				indices.Length,
				device,
				Usage.None,
				Pool.Managed);

			buffer.SetData(indices, 0, LockFlags.None);
			return buffer;
		}

		protected void DisposeMeshBuffers()
		{
			if (tileVertexBuffer != null)
			{
				if (!tileVertexBuffer.Disposed)
					tileVertexBuffer.Dispose();
				tileVertexBuffer = null;
			}
			if (indexBuffer != null)
			{
				if (!indexBuffer.Disposed)
					indexBuffer.Dispose();
				indexBuffer = null;
			}
		}

        /// <summary>
        /// Builds a flat mesh (no terrain)
        /// </summary>
        /// <summary>
        /// Where a row of the mesh falls in the tile image, v running from 0 at the north edge
        /// to 1 at the south edge. Evenly in latitude by default, which is what an image in
        /// plate carree wants. A layer whose tiles are in web mercator overrides it, see
        /// ProjectedQuadTile: there the rows of the image are evenly spaced in projected
        /// metres, and spreading them evenly in latitude instead drags the image polewards.
        /// Only the texture coordinate is changed, never the position of the vertex, so the
        /// terrain samples underneath stay where they were sampled.
        /// </summary>
        protected virtual double TextureVForLatitude(double latitude)
        {
            return (North - latitude) / LatitudeSpan;
        }

        protected virtual void CreateFlatMesh()
        {
            double layerRadius = (double)QuadTileSet.LayerRadius;
            double scaleFactor = 1.0 / (double)vertexCount;
            int thisVertexCount = vertexCount / 2 + (vertexCount % 2);
            int thisVertexCountPlus1 = thisVertexCount + 1;

            int totalVertexCount = thisVertexCountPlus1 * thisVertexCountPlus1;
            northWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            southWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            northEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            southEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];

            const double Degrees2Radians = System.Math.PI / 180.0;

            // Cache western sin/cos of longitude values
            double[] sinLon = new double[thisVertexCountPlus1];
            double[] cosLon = new double[thisVertexCountPlus1];

            int baseIndex;
            double angle = West * Degrees2Radians;
            double deltaAngle = scaleFactor * LongitudeSpan * Degrees2Radians;

            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = West * Degrees2Radians + i * deltaAngle;
                sinLon[i] = Math.Sin(angle);
                cosLon[i] = Math.Cos(angle);
                //angle += deltaAngle;

            }

            baseIndex = 0;
            angle = North * Degrees2Radians;
            deltaAngle = -scaleFactor * LatitudeSpan * Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = North * Degrees2Radians + i * deltaAngle;
                double sinLat = Math.Sin(angle);
                double radCosLat = Math.Cos(angle) * layerRadius;

                for (int j = 0; j < thisVertexCountPlus1; j++)
                {
                    northWestVertices[baseIndex].X = (float)(radCosLat * cosLon[j] - localOrigin.X);
                    northWestVertices[baseIndex].Y = (float)(radCosLat * sinLon[j] - localOrigin.Y);
                    northWestVertices[baseIndex].Z = (float)(layerRadius * sinLat - localOrigin.Z);
                    northWestVertices[baseIndex].Tu = (float)(j * scaleFactor);
                    northWestVertices[baseIndex].Tv = (float)TextureVForLatitude(angle / Degrees2Radians);
                    northWestVertices[baseIndex].Normal = new Vector3(northWestVertices[baseIndex].X + (float)localOrigin.X, northWestVertices[baseIndex].Y + (float)localOrigin.Y, northWestVertices[baseIndex].Z + (float)localOrigin.Z);
                    northWestVertices[baseIndex].Normal.Normalize();

                    baseIndex += 1;
                }
                //angle += deltaAngle;
            }

            baseIndex = 0;
            angle = 0.5 * (North + South) * Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = 0.5 * (North + South) * Degrees2Radians + i * deltaAngle;
                double sinLat = Math.Sin(angle);
                double radCosLat = Math.Cos(angle) * layerRadius;

                for (int j = 0; j < thisVertexCountPlus1; j++)
                {
                    southWestVertices[baseIndex].X = (float)(radCosLat * cosLon[j] - localOrigin.X);
                    southWestVertices[baseIndex].Y = (float)(radCosLat * sinLon[j] - localOrigin.Y);
                    southWestVertices[baseIndex].Z = (float)(layerRadius * sinLat - localOrigin.Z);
                    southWestVertices[baseIndex].Tu = (float)(j * scaleFactor);
                    southWestVertices[baseIndex].Tv = (float)TextureVForLatitude(angle / Degrees2Radians);
                    southWestVertices[baseIndex].Normal = new Vector3(southWestVertices[baseIndex].X + (float)localOrigin.X, southWestVertices[baseIndex].Y + (float)localOrigin.Y, southWestVertices[baseIndex].Z + (float)localOrigin.Z);
                    southWestVertices[baseIndex].Normal.Normalize();

                    baseIndex += 1;
                }
                //angle += deltaAngle;
            }

            // Cache eastern sin/cos of longitude values
            angle = 0.5 * (West + East) * Degrees2Radians;
            deltaAngle = scaleFactor * LongitudeSpan * Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = 0.5 * (West + East) * Degrees2Radians + i * deltaAngle;
                sinLon[i] = Math.Sin(angle);
                cosLon[i] = Math.Cos(angle);
                //angle += deltaAngle;
            }

            baseIndex = 0;
            angle = North * Degrees2Radians;
            deltaAngle = -scaleFactor * LatitudeSpan * Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {
                angle = North * Degrees2Radians + i * deltaAngle;
                double sinLat = Math.Sin(angle);
                double radCosLat = Math.Cos(angle) * layerRadius;

                for (int j = 0; j < thisVertexCountPlus1; j++)
                {
                    northEastVertices[baseIndex].X = (float)(radCosLat * cosLon[j] - localOrigin.X);
                    northEastVertices[baseIndex].Y = (float)(radCosLat * sinLon[j] - localOrigin.Y);
                    northEastVertices[baseIndex].Z = (float)(layerRadius * sinLat - localOrigin.Z);
                    northEastVertices[baseIndex].Tu = (float)((j + thisVertexCount) * scaleFactor);
                    northEastVertices[baseIndex].Tv = (float)TextureVForLatitude(angle / Degrees2Radians);
                    northEastVertices[baseIndex].Normal = new Vector3(northEastVertices[baseIndex].X + (float)localOrigin.X, northEastVertices[baseIndex].Y + (float)localOrigin.Y, northEastVertices[baseIndex].Z + (float)localOrigin.Z);
                    northEastVertices[baseIndex].Normal.Normalize();

                    baseIndex += 1;
                }
                //angle += deltaAngle;
            }

            baseIndex = 0;
            angle = 0.5f * (North + South) * Degrees2Radians;
            for (int i = 0; i < thisVertexCountPlus1; i++)
            {

                angle = 0.5 * (North + South) * Degrees2Radians + i * deltaAngle;
                double sinLat = Math.Sin(angle);
                double radCosLat = Math.Cos(angle) * layerRadius;

                for (int j = 0; j < thisVertexCountPlus1; j++)
                {
                    southEastVertices[baseIndex].X = (float)(radCosLat * cosLon[j] - localOrigin.X);
                    southEastVertices[baseIndex].Y = (float)(radCosLat * sinLon[j] - localOrigin.Y);
                    southEastVertices[baseIndex].Z = (float)(layerRadius * sinLat - localOrigin.Z);
                    southEastVertices[baseIndex].Tu = (float)((j + thisVertexCount) * scaleFactor);
                    southEastVertices[baseIndex].Tv = (float)TextureVForLatitude(angle / Degrees2Radians);
                    southEastVertices[baseIndex].Normal = new Vector3(southEastVertices[baseIndex].X + (float)localOrigin.X, southEastVertices[baseIndex].Y + (float)localOrigin.Y, southEastVertices[baseIndex].Z + (float)localOrigin.Z);
                    southEastVertices[baseIndex].Normal.Normalize();

                    baseIndex += 1;
                }
                //angle += deltaAngle;
            }

            vertexIndexes = new short[2 * thisVertexCount * thisVertexCount * 3];

            for (int i = 0; i < thisVertexCount; i++)
            {
                baseIndex = (2 * 3 * i * thisVertexCount);

                for (int j = 0; j < thisVertexCount; j++)
                {
                    vertexIndexes[baseIndex] = (short)(i * thisVertexCountPlus1 + j);
                    vertexIndexes[baseIndex + 1] = (short)((i + 1) * thisVertexCountPlus1 + j);
                    vertexIndexes[baseIndex + 2] = (short)(i * thisVertexCountPlus1 + j + 1);

                    vertexIndexes[baseIndex + 3] = (short)(i * thisVertexCountPlus1 + j + 1);
                    vertexIndexes[baseIndex + 4] = (short)((i + 1) * thisVertexCountPlus1 + j);
                    vertexIndexes[baseIndex + 5] = (short)((i + 1) * thisVertexCountPlus1 + j + 1);

                    baseIndex += 6;
                }
            }
        }

        double meshBaseRadius = 0;

        /// <summary>
        /// Build the elevated terrain mesh
        /// </summary>
        protected virtual void CreateElevatedMesh()
        {
            isDownloadingTerrain = true;
            // Get height data with one extra sample around the tile
            double degreePerSample = LatitudeSpan / vertexCountElevated;
            TerrainTile tile = QuadTileSet.World.TerrainAccessor.GetElevationArray(this,North + degreePerSample, South - degreePerSample, West - degreePerSample, East + degreePerSample, vertexCountElevated + 3);
            float[,] heightData = tile.ElevationData;

            int vertexCountElevatedPlus3 = vertexCountElevated / 2 + 3;
            int totalVertexCount = vertexCountElevatedPlus3 * vertexCountElevatedPlus3;
            northWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            southWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            northEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            southEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
            double layerRadius =QuadTileSet.LayerRadius;

            // Calculate mesh base radius (bottom vertices)
            // Find minimum elevation to account for possible bathymetry
            float minimumElevation = float.MaxValue;
            float maximumElevation = float.MinValue;
            foreach (float height in heightData)
            {
                if (height < minimumElevation)
                    minimumElevation = height;
                if (height > maximumElevation)
                    maximumElevation = height;
            }
            minimumElevation *= verticalExaggeration;
            maximumElevation *= verticalExaggeration;

            if (minimumElevation > maximumElevation)
            {
                // Compensate for negative vertical exaggeration
                minimumElevation = maximumElevation;
                maximumElevation = minimumElevation;
            }

            double overlap = 500 * verticalExaggeration; // 500m high tiles

            // Radius of mesh bottom grid
            meshBaseRadius = layerRadius + minimumElevation - overlap;

            CreateElevatedMesh(ChildLocation.NorthWest, northWestVertices, meshBaseRadius, heightData);
            CreateElevatedMesh(ChildLocation.SouthWest, southWestVertices, meshBaseRadius, heightData);
            CreateElevatedMesh(ChildLocation.NorthEast, northEastVertices, meshBaseRadius, heightData);
            CreateElevatedMesh(ChildLocation.SouthEast, southEastVertices, meshBaseRadius, heightData);

            //BoundingBox = new BoundingBox((float)South, (float)North, (float)West, (float)East,
            //    (float)layerRadius, (float)layerRadius + 10000 * this.verticalExaggeration);

            // From the bottom of the struts to the highest sample of this tile, plus the sag of
            // the chord across it, see ChordSag. This replaces a height of 30000000000 times the
            // vertical exaggeration, which stretched the box far past the whole scene, so that
            // every tile in the tree answered the frustum test with yes - which is why a view
            // near the ground loaded so many of them.
            double chordSag = ChordSag(layerRadius, LatitudeSpan, LongitudeSpan);

            BoundingBox = new BoundingBox((float)South, (float)North, (float)West, (float)East,
             (float)(meshBaseRadius - BoundingBoxSlack), (float)(layerRadius + maximumElevation + chordSag + BoundingBoxSlack));

            QuadTileSet.IsDownloadingElevation = false;

            // Build common set of indexes for the 4 child meshes	
            int vertexCountElevatedPlus2 = vertexCountElevated / 2 + 2;
            vertexIndexes = new short[2 * vertexCountElevatedPlus2 * vertexCountElevatedPlus2 * 3];

            int elevated_idx = 0;
            for (int i = 0; i < vertexCountElevatedPlus2; i++)
            {
                for (int j = 0; j < vertexCountElevatedPlus2; j++)
                {
                    vertexIndexes[elevated_idx++] = (short)(i * vertexCountElevatedPlus3 + j);
                    vertexIndexes[elevated_idx++] = (short)((i + 1) * vertexCountElevatedPlus3 + j);
                    vertexIndexes[elevated_idx++] = (short)(i * vertexCountElevatedPlus3 + j + 1);

                    vertexIndexes[elevated_idx++] = (short)(i * vertexCountElevatedPlus3 + j + 1);
                    vertexIndexes[elevated_idx++] = (short)((i + 1) * vertexCountElevatedPlus3 + j);
                    vertexIndexes[elevated_idx++] = (short)((i + 1) * vertexCountElevatedPlus3 + j + 1);
                }
            }

            calculate_normals(ref northWestVertices, vertexIndexes);
            calculate_normals(ref southWestVertices, vertexIndexes);
            calculate_normals(ref northEastVertices, vertexIndexes);
            calculate_normals(ref southEastVertices, vertexIndexes);

            isDownloadingTerrain = false;
        }

        protected byte m_CurrentOpacity = 255;

        public float GetTvFromAltitude(float alt, string fileName, float mapMaxAlt, float mapMinAlt,float mean,float sd)
        {
            float Tv = 0f;
          
            float newMin=  mean - World.Settings.StandardDeviationNumber * sd;
            float newMax = mean + World.Settings.StandardDeviationNumber * sd;
            if (World.Settings.TerrainScaleMethod == ScaleMethod.None)
            {
                newMin = mapMinAlt;
                newMax = mapMaxAlt;
            }
            if (alt < newMin)
            {
                Tv = 0f;
            }
            else if (alt > newMax)
            {
                Tv = 0.98f;
            }
            else
            {
                Tv = (alt - newMin) / (newMax - newMin);
            }
            if (!World.Settings.InversePseudoColor)
            {
                Tv = 1.0f - Tv;
            }
            //if (fileName.IndexOf("Water") != -1)
            //{
            //    Tv = 0.5f - ((float)alt / (float)Math.Max(Math.Abs(mapMaxAlt), Math.Abs(mapMinAlt)) / 2f);
            //}
            return Tv;
        }
        /// <summary>
        /// Create child tile terrain mesh
        /// Build the mesh with one extra vertice all around for proper normals calculations later on.
        /// Use the struts vertices to that effect. Struts are properly folded after normals calculations.
        /// </summary>
        protected void CreateElevatedMesh(ChildLocation corner, CustomVertex.PositionNormalTextured[] vertices,
            double meshBaseRadius, float[,] heightData)
        {
            // Figure out child lat/lon boundaries (radians)
            double north = MathEngine.DegreesToRadians(North);
            double west = MathEngine.DegreesToRadians(West);

            float mapMaxAlt = 600;
            float mapMinAlt = 1;
            float sd = 10;
            StatisticsInfo info = MyStatisticsMath.ComplexStatistics(heightData);
            //mapMaxAlt = (float)info.Max;
            //mapMinAlt = (float)info.Min;

            Raster raster = this.QuadTileSet.World.TerrainAccessor.GetRaster();
            if (raster != null)
            {
                mapMaxAlt =(float) raster.MaximumAlt;
                mapMinAlt = (float)raster.MinimumAlt;
            }

            //if (mapMinAlt > 0) mapMinAlt = 1;
            //if (mapMaxAlt < 1000) mapMaxAlt = 256;
            //FindAltitudeExtremes(heightData, out mapMaxAlt, out mapMinAlt);

            // Texture coordinate offsets
            float TuOffset = 0;
            float TvOffset = 0;

            switch (corner)
            {
                case ChildLocation.NorthWest:
                    // defaults are all good
                    break;
                case ChildLocation.NorthEast:
                    west = MathEngine.DegreesToRadians(0.5 * (West + East));
                    TuOffset = 0.5f;
                    break;
                case ChildLocation.SouthWest:
                    north = MathEngine.DegreesToRadians(0.5 * (North + South));
                    TvOffset = 0.5f;
                    break;
                case ChildLocation.SouthEast:
                    north = MathEngine.DegreesToRadians(0.5 * (North + South));
                    west = MathEngine.DegreesToRadians(0.5 * (West + East));
                    TuOffset = 0.5f;
                    TvOffset = 0.5f;
                    break;
            }

            double latitudeRadianSpan = MathEngine.DegreesToRadians(LatitudeSpan);
            double longitudeRadianSpan = MathEngine.DegreesToRadians(LongitudeSpan);

            double layerRadius = (double)QuadTileSet.LayerRadius;
            double scaleFactor = 1.0 / vertexCountElevated;
            int terrainLongitudeIndex = (int)(TuOffset * vertexCountElevated) + 1;
            int terrainLatitudeIndex = (int)(TvOffset * vertexCountElevated) + 1;

            int vertexCountElevatedPlus1 = vertexCountElevated / 2 + 1;

            double radius = 0;
            int vertexIndex = 0;
            for (int latitudeIndex = -1; latitudeIndex <= vertexCountElevatedPlus1; latitudeIndex++)
            {
                double latitudeFactor = latitudeIndex * scaleFactor;
                double latitude = north - latitudeFactor * latitudeRadianSpan;

                // Cache trigonometric values
                double cosLat = Math.Cos(latitude);
                double sinLat = Math.Sin(latitude);

                for (int longitudeIndex = -1; longitudeIndex <= vertexCountElevatedPlus1; longitudeIndex++)
                {
                    float alt = heightData[terrainLatitudeIndex + latitudeIndex, terrainLongitudeIndex + longitudeIndex]
                         * verticalExaggeration;
                    // Top of mesh for all (real terrain + struts)
                    radius = layerRadius + alt;
                    double longitudeFactor = longitudeIndex * scaleFactor;

                    if (World.Settings.UsePseudoColor)
                    {
                        // Texture coordinates
                        vertices[vertexIndex].Tu = 0;
                        vertices[vertexIndex].Tv = GetTvFromAltitude(alt, this.ImageFilePath, mapMaxAlt, mapMinAlt, 50,sd);
                        //vertices[vertexIndex].Tv = GetTvFromAltitude(alt, this.ImageFilePath, mapMaxAlt, mapMinAlt,
                        //    QuadTileSet.World.TerrainAccessor.TerrainTileService.RasterSet.Mean,
                        //    QuadTileSet.World.TerrainAccessor.TerrainTileService.RasterSet.StandardDeviation);

                    }
                    else
                    {
                        vertices[vertexIndex].Tu = TuOffset + (float)longitudeFactor;
                        vertices[vertexIndex].Tv = (float)TextureVForLatitude(latitude * 180.0 / Math.PI);
                    }

                    // Convert from spherical (radians) to cartesian
                    double longitude = west + longitudeFactor * longitudeRadianSpan;
                    double radCosLat = radius * cosLat;
                    double absX = radCosLat * Math.Cos(longitude);
                    double absY = radCosLat * Math.Sin(longitude);
                    vertices[vertexIndex].X = (float)(absX - localOrigin.X);
                    vertices[vertexIndex].Y = (float)(absY - localOrigin.Y);
                    vertices[vertexIndex].Z = (float)(radius * sinLat - localOrigin.Z);

                    vertexIndex++;
                }
            }
        }

        private void calculate_normals(ref CustomVertex.PositionNormalTextured[] vertices, short[] indices)
        {
            // Accumulate the face normals into a plain array. The version this replaces handed
            // every vertex its own List<Vector3>, which for a single tile meant two thousand
            // lists, each grown through several reallocations, on every mesh build - and while
            // the camera moves near the ground meshes are built continuously.
            Vector3[] accumulatedNormals = new Vector3[vertices.Length];

            for (int i = 0; i < indices.Length; i += 3)
            {
                int index0 = indices[i];
                int index1 = indices[i + 1];
                int index2 = indices[i + 2];

                Vector3 p1 = vertices[index0].Position;
                Vector3 p2 = vertices[index1].Position;
                Vector3 p3 = vertices[index2].Position;

                Vector3 v1 = p2 - p1;
                Vector3 v2 = p3 - p1;
                Vector3 normal = Vector3.Cross(v1, v2);

                normal.Normalize();

                // Store the face's normal for each of the vertices that make up the face.
                accumulatedNormals[index0] += normal;
                accumulatedNormals[index1] += normal;
                accumulatedNormals[index2] += normal;
            }

            // Normalizing the sum gives the same direction the old averaging gave, as a unit
            // vector instead of one shortened by how much the faces around the vertex disagree.
            for (int i = 0; i < vertices.Length; ++i)
            {
                if (accumulatedNormals[i] == Vector3.Empty)
                    continue;

                vertices[i].Normal = accumulatedNormals[i];
                vertices[i].Normal.Normalize();
            }

            // Adjust/Fold struts vertices using terrain border vertices positions
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
                        if (renderStruts && m_CurrentOpacity == 255)
                            p = ProjectOnMeshBase(p);
                        vertices[i * vertexDensity + j].Position = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
                    }
                }
                else
                {
                    Point3d p = new Point3d(vertices[i * vertexDensity + 1].Position.X, vertices[i * vertexDensity + 1].Position.Y, vertices[i * vertexDensity + 1].Position.Z);
                    if (renderStruts && m_CurrentOpacity == 255) p = ProjectOnMeshBase(p);
                    vertices[i * vertexDensity].Position = new Vector3((float)p.X, (float)p.Y, (float)p.Z);

                    p = new Point3d(vertices[i * vertexDensity + vertexDensity - 2].Position.X, vertices[i * vertexDensity + vertexDensity - 2].Position.Y, vertices[i * vertexDensity + vertexDensity - 2].Position.Z);
                    if (renderStruts && m_CurrentOpacity == 255) p = ProjectOnMeshBase(p);
                    vertices[i * vertexDensity + vertexDensity - 1].Position = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
                }
            }
        }

        // Project an elevated mesh point to the mesh base
        private Point3d ProjectOnMeshBase(Point3d p)
        {
            p = p + this.localOrigin;
            p = p.normalize();
            p = p * meshBaseRadius - this.localOrigin;
            return p;
        }

		public virtual bool Render(DrawArgs drawArgs)
		{
            try
            {
                // The image may be on disk while its texture is not built yet, see m_TexturePending.
                if (m_TexturePending)
                    CreatePendingTextures();

                // Same for the mesh: the update thread built the arrays, the buffers are made here.
                if (m_MeshPending)
                    CreatePendingMesh();

                if (!isInitialized ||
                    this.northWestVertices == null ||
                    this.northEastVertices == null ||
                    this.southEastVertices == null ||
                    this.southWestVertices == null)
                {
                    return false;
                }
                if (this.Level != 0)
                {
                    if (!DrawArgs.Camera.ViewFrustum.Intersects(BoundingBox))
                    {
                        return false;
                    }
                }

                bool northWestChildRendered = false;
                bool northEastChildRendered = false;
                bool southWestChildRendered = false;
                bool southEastChildRendered = false;

                if (northWestChild != null)
                    if (northWestChild.Render(drawArgs))
                        northWestChildRendered = true;

                if (southWestChild != null)
                    if (southWestChild.Render(drawArgs))
                        southWestChildRendered = true;

                if (northEastChild != null)
                    if (northEastChild.Render(drawArgs))
                        northEastChildRendered = true;

                if (southEastChild != null)
                    if (southEastChild.Render(drawArgs))
                        southEastChildRendered = true;

                if (QuadTileSet.RenderFileNames &&
                    (!northWestChildRendered || !northEastChildRendered || !southWestChildRendered || !southEastChildRendered))
                {
                    Vector3 referenceCenter = new Vector3(
                        (float)drawArgs.WorldCamera.ReferenceCenter.X,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                        (float)drawArgs.WorldCamera.ReferenceCenter.Z);

                    RenderDownloadRectangle(drawArgs, System.Drawing.Color.FromArgb(255, 0, 0).ToArgb(), referenceCenter);

                    Vector3 cartesianPoint = MathEngine.SphericalToCartesian(
                        CenterLatitude.Degrees,
                        CenterLongitude.Degrees,
                        drawArgs.WorldCamera.WorldRadius + drawArgs.WorldCamera.TerrainElevation);

                    if (ImageFilePath != null && drawArgs.WorldCamera.ViewFrustum.ContainsPoint(cartesianPoint))
                    {
                        Vector3 projectedPoint = drawArgs.WorldCamera.Project(cartesianPoint - referenceCenter);

                        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(  (int)projectedPoint.X - 100, (int)projectedPoint.Y,
                            200,  200);

                        drawArgs.defaultDrawingFont.DrawText(null,  ImageFilePath, rect, DrawTextFormat.WordBreak, System.Drawing.Color.Red);
                    }
                }

                if (northWestChildRendered && northEastChildRendered && southWestChildRendered && southEastChildRendered)
                {
                    return true;
                }

                Device device = DrawArgs.Device;

                // Read the array into a local before walking it: Dispose on the other thread swaps it
                // for null, and reading it again for the length or the slot can hit that null in the
                // middle of the loop, which threw here every time a tile was dropped while drawing.
                // A tile whose texture has not arrived yet must not claim its quadrant, or the
                // parent stops drawing its own coarser image there and the quadrant is filled
                // with an untextured mesh instead. That is what a layer still downloading its
                // tiles showed as soon as it was zoomed into: the coarse image disappeared and
                // nothing came back until every descendant had downloaded. Reporting false
                // here keeps the parent drawing, which is the usual way a quadtree refines.
                Texture[] tileTextures = textures;
                if (tileTextures == null)
                {
                    return false;
                }

                for (int i = 0; i < tileTextures.Length; i++)
                {
                    if (tileTextures[i] == null || tileTextures[i].Disposed)
                    {
                        return false;
                    }

                    device.SetTexture(i, tileTextures[i]);
                }

                drawArgs.numberTilesDrawn++;

                int numpasses = 1;
                int pass;

                    DrawArgs.Device.Transform.World = Matrix.Translation(
                        (float)(localOrigin.X - drawArgs.WorldCamera.ReferenceCenter.X),
                        (float)(localOrigin.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                        (float)(localOrigin.Z - drawArgs.WorldCamera.ReferenceCenter.Z)
                        );

                    // One draw covers the whole tile when every quadrant is visible and none of
                    // them carries a download indicator, which is a texture stage and so can only
                    // be set for the draw as a whole. Otherwise they go one by one.
                    bool anyQuadrantHidden = northWestChildRendered || southWestChildRendered ||
                        northEastChildRendered || southEastChildRendered;
                    bool anyIndicator = HasDownloadActivity(northWestChild) || HasDownloadActivity(southWestChild) ||
                        HasDownloadActivity(northEastChild) || HasDownloadActivity(southEastChild);

                    for (pass = 0; pass < numpasses; pass++)
                    {
                        if (anyQuadrantHidden || anyIndicator)
                        {
                            if (!northWestChildRendered)
                                Render(device, northWestVertices, northWestChild, 0, 1);
                            if (!southWestChildRendered)
                                Render(device, southWestVertices, southWestChild, 1, 1);
                            if (!northEastChildRendered)
                                Render(device, northEastVertices, northEastChild, 2, 1);
                            if (!southEastChildRendered)
                                Render(device, southEastVertices, southEastChild, 3, 1);
                        }
                        else
                        {
                            Render(device, northWestVertices, null, 0, 4);
                        }
                    }

                    DrawArgs.Device.Transform.World = DrawArgs.Camera.WorldMatrix;

                return true;
            }
            catch (DirectXException caught)
            {
                // This used to swallow the failure outright. A tile that gets this far has a mesh,
                // a texture and has passed the frustum test, so it is already counted as drawn,
                // and the parent then stops drawing its own quadrant for it - which is how a
                // layer can report thousands of tiles drawn and still put nothing on the screen.
                Log.Write(Log.Levels.Warning, "QTILE",
                    "Draw of tile " + Name + " L" + Level + " failed: " + caught.Message);
            }
			return false;
		}

        protected CustomVertex.PositionColored[] downloadRectangle = new CustomVertex.PositionColored[5];

		/// <summary>
		/// Render a rectangle around an image tile in the specified color
		/// </summary>
		public void RenderDownloadRectangle(DrawArgs drawArgs, int color, Vector3 referenceCenter)
		{
			// Render terrain download rectangle
			Vector3 northWestV = MathEngine.SphericalToCartesian((float)North, (float)West, QuadTileSet.LayerRadius) - referenceCenter;
			Vector3 southWestV = MathEngine.SphericalToCartesian((float)South, (float)West, QuadTileSet.LayerRadius) - referenceCenter;
			Vector3 northEastV = MathEngine.SphericalToCartesian((float)North, (float)East, QuadTileSet.LayerRadius) - referenceCenter;
			Vector3 southEastV = MathEngine.SphericalToCartesian((float)South, (float)East, QuadTileSet.LayerRadius) - referenceCenter;

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

			drawArgs.device.RenderState.ZBufferEnable = false;
			drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
			drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
			drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, downloadRectangle);
			drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
			drawArgs.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
			drawArgs.device.RenderState.ZBufferEnable = true;
		}

  		/// <summary>
		/// Render one of the 4 quadrants with optional download indicator
		/// </summary>
		void Render(Device device, CustomVertex.PositionNormalTextured[] verts, QuadTile child,
			int firstQuadrant, int quadrantCount)
		{
			bool isMultitexturing = false;
			
			if(!World.Settings.EnableSunShading)
			{
				if (World.Settings.ShowDownloadIndicator && child != null)
				{
					// Check/display download activity
					GeoImageDownloadRequest request = child.DownloadRequest; 
					if (child.isDownloadingTerrain)
					{
						device.SetTexture(1, QuadTileSet.DownloadTerrainTexture);
						isMultitexturing = true;
					}
						//else if (request != null)
					else if(child.WaitingForDownload)
					{
						if (child.IsDownloadingImage)
							device.SetTexture(1, QuadTileSet.DownloadInProgressTexture);
						else
							device.SetTexture(1, QuadTileSet.DownloadQueuedTexture);
						isMultitexturing = true;
					}
				}
			}
			
			if (isMultitexturing)
				device.SetTextureStageState(1, TextureStageStates.ColorOperation, (int)TextureOperation.BlendTextureAlpha);

			if(verts != null && vertexIndexes != null)
			{
				// Bind the GPU buffers when there are any. The draws below fall back to the
				// arrays when there are not, which is the case before the first upload and
				// whenever the upload failed, so no tile is dropped because of a buffer.
				bool usingBuffers = tileVertexBuffer != null && indexBuffer != null &&
					!tileVertexBuffer.Disposed && !indexBuffer.Disposed;
				if (usingBuffers)
				{
					device.SetStreamSource(0, tileVertexBuffer, 0, CustomVertex.PositionNormalTextured.StrideSize);
					device.Indices = indexBuffer;
				}

                if (QuadTileSet.Effect != null)
                {
                    Effect effect = QuadTileSet.Effect;

                    // FIXME: just use the first technique for now
                    effect.Technique = effect.GetTechnique(0);
                    effect.SetValue("WorldViewProj", Matrix.Multiply(device.Transform.World, Matrix.Multiply(device.Transform.View, device.Transform.Projection)));
                    try
                    {
                        effect.SetValue("Tex0", textures[0]);
                        effect.SetValue("Tex1", textures[1]);
                        effect.SetValue("Brightness", QuadTileSet.GrayscaleBrightness);
                        float opacity = (float)QuadTileSet.Opacity / 255.0f;
                        effect.SetValue("Opacity", opacity);
                    }
                    catch (Exception caught)
                    {
                        Log.Write(caught);
                    }

                    int numPasses = effect.Begin(0);
                    for (int i = 0; i < numPasses; i++)
                    {
                        effect.BeginPass(i);
                        #if LOD
                         device.DrawIndexedUserPrimitives(PrimitiveType.LineStrip, 0,
                                verts.Length, vertexIndexes.Length / 3, vertexIndexes, true, verts);
                        #else
                            DrawQuadrants(device, verts, firstQuadrant, quadrantCount);
                        #endif


                        effect.EndPass();
                    }

                    effect.End();
                }
                else if (!QuadTileSet.RenderGrayscale || (device.DeviceCaps.PixelShaderVersion.Major < 1))
                {
                    if (World.Settings.EnableSunShading)
                    {
                        Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
                        Vector3 sunVector = new Vector3(
                            (float)sunPosition.X,
                            (float)sunPosition.Y,
                            (float)sunPosition.Z);

                        device.RenderState.Lighting = true;
                        Material material = new Material();
                        material.Diffuse = System.Drawing.Color.White;
                        material.Ambient = System.Drawing.Color.White;

                        device.Material = material;
                        device.RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
                        device.RenderState.NormalizeNormals = true;
                        device.RenderState.AlphaBlendEnable = true;

                        device.Lights[0].Enabled = true;
                        device.Lights[0].Type = LightType.Directional;
                        device.Lights[0].Diffuse = System.Drawing.Color.White;
                        device.Lights[0].Direction = sunVector;

                        device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                        device.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
                        device.TextureState[0].ColorArgument2 = TextureArgument.TextureColor;
                    }
                    else
                    {
                        device.RenderState.Lighting = false;
                        device.RenderState.Ambient = World.Settings.StandardAmbientColor;
                    }

                    device.RenderState.TextureFactor = System.Drawing.Color.FromArgb(m_CurrentOpacity, 0, 0, 0).ToArgb();
                    device.TextureState[0].AlphaOperation = TextureOperation.BlendFactorAlpha;
                    device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                    device.TextureState[0].AlphaArgument2 = TextureArgument.TFactor;

#if LOD
                    device.DrawIndexedUserPrimitives(PrimitiveType.LineStrip, 0,
                           verts.Length, vertexIndexes.Length / 3, vertexIndexes, true, verts);
#else
                         DrawQuadrants(device, verts, firstQuadrant, quadrantCount);
#endif


                }
                else
                {
                    if (grayscaleEffect == null)
                    {
                        device.DeviceReset += new EventHandler(device_DeviceReset);
                        device_DeviceReset(device, null);
                    }

                    grayscaleEffect.Technique = "RenderGrayscaleBrightness";
                    grayscaleEffect.SetValue("WorldViewProj", Matrix.Multiply(device.Transform.World, Matrix.Multiply(device.Transform.View, device.Transform.Projection)));
                    // A tile disposed while this draws leaves nothing to bind. The effect has not been
                    // begun yet at this point, so dropping the tile here is safe and keeps the render
                    // from throwing on a null array every frame the camera moves.
                    Texture grayTexture = (textures != null && textures.Length > 0) ? textures[0] : null;
                    if (grayTexture == null || grayTexture.Disposed)
                        return;
                    grayscaleEffect.SetValue("Tex0", grayTexture);
                    grayscaleEffect.SetValue("Brightness", QuadTileSet.GrayscaleBrightness);
                    float opacity = (float)QuadTileSet.Opacity / 255.0f;
                    grayscaleEffect.SetValue("Opacity", opacity);

                    int numPasses = grayscaleEffect.Begin(0);
                    for (int i = 0; i < numPasses; i++)
                    {
                        grayscaleEffect.BeginPass(i);
#if LOD
                        device.DrawIndexedUserPrimitives(PrimitiveType.LineStrip, 0,
                               verts.Length, vertexIndexes.Length / 3, vertexIndexes, true, verts);
#else
                           DrawQuadrants(device, verts, firstQuadrant, quadrantCount);
#endif


                        grayscaleEffect.EndPass();
                    }

                    grayscaleEffect.End();
                }
			}
			if (isMultitexturing)
				device.SetTextureStageState(1, TextureStageStates.ColorOperation, (int)TextureOperation.Disable);
		}

		/// <summary>
		/// Draws the given run of quadrants, counting from north west, from the tile's vertex
		/// buffer, or from the array when the buffer is not there yet. Going through the buffer is
		/// the point of the whole exercise: with DrawIndexedUserPrimitives the driver was handed
		/// the same vertices again every frame, and one quadrant per call.
		/// </summary>
		void DrawQuadrants(Device device, CustomVertex.PositionNormalTextured[] verts,
			int firstQuadrant, int quadrantCount)
		{
			if (tileVertexBuffer != null && indexBuffer != null &&
				!tileVertexBuffer.Disposed && !indexBuffer.Disposed)
			{
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
					firstQuadrant * m_QuadrantVertexCount,
					quadrantCount * m_QuadrantVertexCount,
					firstQuadrant * m_QuadrantIndexCount,
					quadrantCount * m_QuadrantIndexCount / 3);
			}
			else
			{
				// No buffer to name a range in, so fall back to the arrays and draw the quadrants
				// one at a time. A run of them cannot be drawn from the arrays in one call, the
				// way it can from the buffer, because they are four separate arrays.
				for (int quadrant = firstQuadrant; quadrant < firstQuadrant + quadrantCount; quadrant++)
				{
					CustomVertex.PositionNormalTextured[] quadrantVertices = GetQuadrantVertices(quadrant);
					if (quadrantVertices == null)
						continue;

					device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
						quadrantVertices.Length, vertexIndexes.Length / 3, vertexIndexes, true, quadrantVertices);
				}
			}
		}

		CustomVertex.PositionNormalTextured[] GetQuadrantVertices(int quadrant)
		{
			switch (quadrant)
			{
				case 0: return northWestVertices;
				case 1: return southWestVertices;
				case 2: return northEastVertices;
				default: return southEastVertices;
			}
		}

		/// <summary>
		/// True when a child tile needs the download indicator drawn over its quadrant, which has
		/// to be set per quadrant and so rules out drawing them as one run.
		/// </summary>
		static bool HasDownloadActivity(QuadTile child)
		{
			if (child == null || !World.Settings.ShowDownloadIndicator || World.Settings.EnableSunShading)
				return false;

			return child.isDownloadingTerrain || child.WaitingForDownload;
		}

        void device_DeviceReset(object sender, EventArgs e)
        {
            Device device = (Device)sender;

            string outerrors = "";

            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream("Heiflow.Visualization.Shaders.grayscale.fx");
                
                grayscaleEffect =
                    Effect.FromStream(
                    device,
                    stream,
                    null,
                    null,
                    ShaderFlags.None,
                    null,
                    out outerrors);

                if (outerrors != null && outerrors.Length > 0)
                    Log.Write(Log.Levels.Error, outerrors);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
	}
}