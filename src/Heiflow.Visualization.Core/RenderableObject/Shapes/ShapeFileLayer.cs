using System;
using System.Collections;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using ICSharpCode.SharpZipLib.Zip;
using Utility;
using System.Linq;
using System.Collections.Generic;
using Heiflow.Core;

namespace HUST.WREIS.Dot3D
{
	/// <summary>
	/// Summary description for ShapeFileLayer.
	/// </summary>
	/// 
    //TODO: Upper and lower random color limits, line "caps" styles
	public class ShapeFileLayer : Renderable.RenderableObject
	{
		int m_NumberRootTilesHigh = 150;
		ShapeTileArgs m_ShapeTileArgs;
		ShapeTile[] m_RootTiles;
		string m_ShapeFilePath;
		
		double m_BoundingBoxXMin;
		double m_BoundingBoxYMin;
		double m_BoundingBoxXMax;
		double m_BoundingBoxYMax;
		double m_BoundingBoxZMin;
		double m_BoundingBoxZMax;
		double m_BoundingBoxMMin;
		double m_BoundingBoxMMax;

		double m_ScalarFilterMin = double.NaN;
		double m_ScalarFilterMax = double.NaN;
		double m_MinimumViewingAltitude = 0;
		double m_MaximumViewingAltitude = double.MaxValue;
		double m_lztsd = 36.0;


		int m_IconWidth = 0;
		int m_IconHeight = 0;
		string m_IconFilePath = null;
		Texture m_IconTexture = null;
		SurfaceDescription m_IconTextureDescription;

		byte m_IconOpacity = 255;

        public double North
        {
            get
            {
                return m_BoundingBoxYMax;
            }
        }

        public double South
        {
            get
            {
                return m_BoundingBoxYMin;
            }
        }

        public double East
        {
            get
            {
                return m_BoundingBoxXMax;
            }
        }

        public double West
        {
            get
            {
                return m_BoundingBoxXMin;
            }
        }

        public double MinAltitude
        {
            get
            {
                return m_MinimumViewingAltitude;
            }
        }

        public double MaxAltitude
        {
            get
            {
                return m_MaximumViewingAltitude;
            }
        }

		public ShapeFileLayer(
			string id,
			World parentWorld,
			string shapeFilePath,
			double minimumViewingAltitude,
			double maximumViewingAltitude,
			float lztsd,
			GeographicBoundingBox bounds,
			string dataKey,
			bool scaleColorsToData,
			double scalarFilterMin,
			double scalarFilterMax,
			double scaleMin,
			double scaleMax,
			string[] noDataValues,
			string[] activeDataValues,
			bool polygonFill,
			bool outlinePolygons,
			System.Drawing.Color polygonFillColor,
			ShapeFillStyle shapeFillHatchStyle,
			System.Drawing.Color lineColor,
			float lineWidth,
			bool showLabels,
			System.Drawing.Color labelColor,
			string iconFilePath,
			int iconWidth,
			int iconHeight,
			byte iconOpacity) : base(id, parentWorld.Position, parentWorld.Orientation)
		{

			this.RenderPriority = HUST.WREIS.Dot3D.Renderable.RenderPriority.LinePaths;

			m_MinimumViewingAltitude = minimumViewingAltitude;
			m_MaximumViewingAltitude = maximumViewingAltitude;
			m_lztsd = lztsd;

			m_ShapeTileArgs = new ShapeTileArgs(
				parentWorld,
				//new System.Drawing.Size(256*16, 256*16),
                 new System.Drawing.Size(256 * 8, 256 * 8),
				parentWorld.EquatorialRadius,
				this,
				dataKey,
				scaleColorsToData,
				scaleMin,
				scaleMax,
				noDataValues,
				activeDataValues,
				polygonFill,
				outlinePolygons,
				polygonFillColor,
				shapeFillHatchStyle,
				lineColor,
				labelColor,
				lineWidth,
				showLabels
				);

			m_ScalarFilterMin = scalarFilterMin;
			m_ScalarFilterMax = scalarFilterMax;

			m_ShapeFilePath = shapeFilePath;

			m_IconFilePath = iconFilePath;
			m_IconWidth = iconWidth;
			m_IconHeight = iconHeight;
			m_IconOpacity = iconOpacity;
			/*Produces tile tree for whole earth*/
			/*Need to implement clipping*/
            m_NumberRootTilesHigh =  (int)(180.0f / m_lztsd);

			double tileSize = 180.0f/m_NumberRootTilesHigh;
		
            m_RootTiles = new ShapeTile[m_NumberRootTilesHigh * (m_NumberRootTilesHigh * 2)];

			System.Console.WriteLine("North:{0} South:{1} East:{2} West:{3}",
				bounds.North,bounds.South,bounds.East,bounds.West);
			int istart = 0;
			int iend = m_NumberRootTilesHigh;
			int jstart = 0;
			int jend = m_NumberRootTilesHigh * 2;

			int createdtiles = 0;
            for (int i = istart; i < iend; i++)
            {
                for (int j = jstart; j < jend; j++)
                {
                    double north = (i + 1) * tileSize - 90.0f;
                    double south = i * tileSize - 90.0f;
                    double west = j * tileSize - 180.0f;
                    double east = (j + 1) * tileSize - 180.0f;
                    m_RootTiles[i * m_NumberRootTilesHigh * 2 + j] = new ShapeTile(
                            new GeographicBoundingBox(
                            north,
                            south,
                            west,
                            east),
                            m_ShapeTileArgs);
                    createdtiles++;
                }
            }

            List<ShapeTile> tiles = new List<ShapeTile>();
            for (int i = 0; i < createdtiles; i++)
            {
                if (m_RootTiles[i].m_GeoBB.Intersects(bounds))
                {
                    tiles.Add(m_RootTiles[i]);
                }
            }
            m_RootTiles = tiles.ToArray();

            //m_RootTiles=new ShapeTile[1];

            //m_RootTiles[0] = new ShapeTile(bounds, m_ShapeTileArgs);
            //createdtiles++;
            //createdtiles = m_RootTiles.Length;


            //for (int i = istart; i < iend; i++)
            //{
            //    for (int j = jstart; j < jend; j++)
            //    {
            //        double north = (i + 1) * tileSize + bounds.South;
            //        double south = i * tileSize + bounds.South;
            //        double west = j * tileSize + bounds.West;
            //        double east = (j + 1) * tileSize + bounds.West;
            //        m_RootTiles[i * m_NumberRootTilesHigh * 2 + j] = new ShapeTile(
            //                new GeographicBoundingBox(
            //                north,
            //                south,
            //                west,
            //                east),
            //                m_ShapeTileArgs);
            //        createdtiles++;
            //    }
            //}
			Console.WriteLine("Created Tiles "+createdtiles);

            ActualMax = double.MinValue;
            ActualMin = double.MaxValue;
		}

		public override void Dispose()
		{
            //isInitialized = false;
            //if(m_IconTexture != null && !m_IconTexture.Disposed)
            //{
            //    m_IconTexture.Dispose();
            //}
            //if(m_Sprite != null && !m_Sprite.Disposed)
            //{
            //    m_Sprite.Dispose();
            //}
            //foreach(ShapeTile shapeTile in m_RootTiles)
            //{
            //    shapeTile.Dispose();
            //}
		}

		public override byte Opacity 
		{
			get
			{
				return base.Opacity;
			}
			set
			{
				if(m_RootTiles != null)
				{
					foreach(ShapeTile tile in m_RootTiles)
					{
						if(tile != null)
						{
							tile.Opacity = value;
						}
					}
				}
				base.Opacity = value;	
			}
		}

		Sprite m_Sprite = null;
		public override void Initialize(DrawArgs drawArgs)
		{
			try
			{
				m_Sprite = new Sprite(drawArgs.device);
				if(m_IconFilePath != null && File.Exists(m_IconFilePath))
				{
					m_IconTexture = ImageHelper.LoadIconTexture(m_IconFilePath);
					m_IconTextureDescription = m_IconTexture.GetLevelDescription(0);
				}
				if(m_ShapeFilePath.ToLower().EndsWith(".zip"))
				{					
					loadZippedShapeFile(m_ShapeFilePath);
				}
				else
				{					
					loadShapeFile(m_ShapeFilePath);
				}

				if((m_ShapeTileArgs.ShowLabels && m_ShapeTileArgs.DataKey != null) && m_IconTexture != null)
				{
					foreach(ShapeRecord record in m_ShapeTileArgs.ShapeRecords)
					{
						if(record.Value != null)
						{
							if(record.Point != null)
							{
								Shapefile_Point p = new Shapefile_Point();
								p.X = record.Point.X;
								p.Y = record.Point.Y;
								p.Tag = record.Value;
								m_LabelList.Add(p);
							}
							else if(record.MultiPoint != null)
							{
								Shapefile_Point p = new Shapefile_Point();
								p.X = 0.5 * (record.MultiPoint.BoundingBox.West + record.MultiPoint.BoundingBox.East);
								p.Y = 0.5 * (record.MultiPoint.BoundingBox.North + record.MultiPoint.BoundingBox.South);
								p.Tag = record.Value;
								m_LabelList.Add(p);
							}
							else if(record.PolyLine != null)
							{
								Shapefile_Point p = new Shapefile_Point();
								p.X = 0.5 * (record.PolyLine.BoundingBox.West + record.PolyLine.BoundingBox.East);
								p.Y = 0.5 * (record.PolyLine.BoundingBox.North + record.PolyLine.BoundingBox.South);
								p.Tag = record.Value;
								m_LabelList.Add(p);
							}
							else if(record.Polygon != null)
							{
								Shapefile_Point p = new Shapefile_Point();
								p.X = 0.5 * (record.Polygon.BoundingBox.West + record.Polygon.BoundingBox.East);
								p.Y = 0.5 * (record.Polygon.BoundingBox.North + record.Polygon.BoundingBox.South);
								p.Tag = record.Value;
								m_LabelList.Add(p);
							}
						}
					}
				}

                foreach (ShapeTile shapeTile in m_RootTiles)
                {
                    //	if(shapeTile!=null&&(shapeTile.m_GeoBB.North-shapeTile.m_GeoBB.South)<=m_lztsd)
                    shapeTile.Initialize(drawArgs);
                }
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
			finally
			{
				isInitialized = true;
			}
		}

        List<Shapefile_Point> m_LabelList = new List<Shapefile_Point>();
        List<Vector3> m_PointList = new List<Vector3>();

		public override void Update(DrawArgs drawArgs)
		{
			if(drawArgs.WorldCamera.AltitudeAboveTerrain >= m_MinimumViewingAltitude &&
				drawArgs.WorldCamera.AltitudeAboveTerrain <= m_MaximumViewingAltitude)
			{
				if(!isInitialized)
				{
					Initialize(drawArgs);
				}

                //foreach(ShapeTile shapeTile in m_RootTiles)
                //{
                ////	if(shapeTile!=null&&(shapeTile.m_GeoBB.North-shapeTile.m_GeoBB.South)<=m_lztsd)
                //        shapeTile.Update(drawArgs);
                //}
			}
			else
			{
				if(isInitialized)
				{
					Dispose();
				}
			}

		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		public override void Render(DrawArgs drawArgs)
		{
            //if(!isInitialized || 
            //    drawArgs.WorldCamera.AltitudeAboveTerrain < m_MinimumViewingAltitude ||
            //    drawArgs.WorldCamera.AltitudeAboveTerrain > m_MaximumViewingAltitude
            //    )
            //{
            //    return;
            //}

            if (!isInitialized)
            {
            }

			try
			{
				foreach(ShapeTile shapeTile in m_RootTiles)
				{
				//	if(shapeTile!=null&&(shapeTile.m_GeoBB.North-shapeTile.m_GeoBB.South)<=m_lztsd)
						shapeTile.Render(drawArgs);
				}

				Vector3 referenceCenter = new Vector3(
					(float)drawArgs.WorldCamera.ReferenceCenter.X,
					(float)drawArgs.WorldCamera.ReferenceCenter.Y,
					(float)drawArgs.WorldCamera.ReferenceCenter.Z);

				if(m_PointList.Count > 0)
				{
					drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
						
					float curPointSize = drawArgs.device.RenderState.PointSize;
						
					drawArgs.device.RenderState.PointSize = 5.0f;
					drawArgs.device.RenderState.ZBufferEnable = false;
					CustomVertex.PositionColored[] verts = new Microsoft.DirectX.Direct3D.CustomVertex.PositionColored[1];
					Vector3 camPoint = MathEngine.SphericalToCartesian(drawArgs.WorldCamera.Latitude.Degrees, drawArgs.WorldCamera.Longitude.Degrees, m_ShapeTileArgs.LayerRadius);
					
					drawArgs.device.Transform.World = Matrix.Translation(-referenceCenter);
					foreach(Vector3 v in m_PointList)
					{
						if(Vector3.Subtract(v, camPoint).Length() < m_ShapeTileArgs.LayerRadius)
						{
							verts[0].Color = m_ShapeTileArgs.LabelColor.ToArgb();
							verts[0].X = v.X;
							verts[0].Y = v.Y;
							verts[0].Z = v.Z;

							drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
							drawArgs.device.DrawUserPrimitives(PrimitiveType.PointList, 1, verts);
						}
					}

					drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
					drawArgs.device.RenderState.PointSize = curPointSize;
					drawArgs.device.RenderState.ZBufferEnable = true;
				}

				if(m_LabelList.Count > 0)
				{
					System.Drawing.Color iconColor = System.Drawing.Color.FromArgb(m_IconOpacity, 255, 255, 255);
					foreach(Shapefile_Point p in m_LabelList)
					{
						Vector3 cartesianPoint = MathEngine.SphericalToCartesian(p.Y, p.X, drawArgs.WorldCamera.WorldRadius + drawArgs.WorldCamera.TerrainElevation);
					
						if(!drawArgs.WorldCamera.ViewFrustum.ContainsPoint(cartesianPoint) ||
							MathEngine.SphericalDistanceDegrees(p.Y, p.X, drawArgs.WorldCamera.Latitude.Degrees, drawArgs.WorldCamera.Longitude.Degrees) > 90.0)
							continue;

						Vector3 projectedPoint = drawArgs.WorldCamera.Project(cartesianPoint - referenceCenter);

						/*if(isMouseOver)
						{
							// Mouse is over
							isMouseOver = true;

							if(icon.isSelectable)
								DrawArgs.MouseCursor = CursorType.Hand;

							string description = icon.Description;
							if(description==null)
								description = icon.ClickableActionURL;
							if(description!=null)
							{
								// Render description field
								DrawTextFormat format = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Bottom;
								int left = 10;
								if(World.Settings.showLayerManager)
									left += World.Settings.layerManagerWidth;
								Rectangle rect = Rectangle.FromLTRB(left, 10, drawArgs.screenWidth - 10, drawArgs.screenHeight - 10 );

								// Draw outline
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect,
									format, 0xb0 << 24 );
					
								rect.Offset(2,0);
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect,
									format, 0xb0 << 24 );

								rect.Offset(0,2);
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect,
									format, 0xb0 << 24 );

								rect.Offset(-2,0);
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect,
									format, 0xb0 << 24 );

								// Draw description
								rect.Offset(1,-1);
								drawArgs.defaultDrawingFont.DrawText(
									m_sprite, description,
									rect, 
									format, descriptionColor );
							}
						}*/
					m_Sprite.Begin(SpriteFlags.AlphaBlend);

						if(m_IconTexture != null)
						{
							float xscale = (float)m_IconWidth / m_IconTextureDescription.Width;
							float yscale = (float)m_IconHeight / m_IconTextureDescription.Height;
							m_Sprite.Transform = Matrix.Scaling(xscale,yscale,0);
							m_Sprite.Transform *= Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
							m_Sprite.Draw( m_IconTexture,
								new Vector3( m_IconWidth>>1, m_IconHeight>>1,0),
								Vector3.Empty,
								iconColor.ToArgb() );
				
							// Reset transform to prepare for text rendering later
							m_Sprite.Transform = Matrix.Identity;
						}

						if(m_ShapeTileArgs.ShowLabels && m_ShapeTileArgs.DataKey != null)
						{
						
							// Render label
							if(p.Tag != null)
							{
								// Render name field
								const int labelWidth = 1000; // Dummy value needed for centering the text
								if(m_IconTexture==null)
								{
									// Center over target as we have no bitmap
									Rectangle rect = new Rectangle(
										(int)projectedPoint.X - (labelWidth>>1), 
										(int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1)),
										labelWidth, 
										drawArgs.screenHeight );

									drawArgs.defaultDrawingFont.DrawText(m_Sprite, p.Tag.ToString(), rect, DrawTextFormat.Center, m_ShapeTileArgs.LabelColor);
								}
								else
								{
									// Adjust text to make room for icon
									int spacing = (int)(m_IconWidth * 0.3f);
									if(spacing>10)
										spacing = 10;
									int offsetForIcon = (m_IconWidth>>1) + spacing;

									Rectangle rect = new Rectangle(
										(int)projectedPoint.X + offsetForIcon, 
										(int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1)),
										labelWidth, 
										drawArgs.screenHeight );

									drawArgs.defaultDrawingFont.DrawText(m_Sprite, p.Tag.ToString(), rect, DrawTextFormat.WordBreak, m_ShapeTileArgs.LabelColor);
								}
							}
						}
						
						m_Sprite.End();
					}
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}

				
		//Loads a Zipped Shapefile without extracting
		//return==true means it was successfull
		private void loadZippedShapeFile(string shapeFilePath)
		{							
			//ZipFileIndexes
			int shpIndex = -1;
			int	dbfIndex = -1;

			try
			{		
				//Navigate the Zip to find the files and update their index
				ZipFile zFile = new ZipFile(shapeFilePath);
				foreach (ZipEntry ze in zFile) 
				{
					if(ze.Name.ToLower().EndsWith(".shp"))
						shpIndex=(int)ze.ZipFileIndex;					
					else if(ze.Name.ToLower().EndsWith(".dbf"))
                        dbfIndex = (int)ze.ZipFileIndex;					
				}							
			}
			catch { /* Ignore */ }			


			if((dbfIndex == -1)||(shpIndex == -1))
				return ;
			
			System.Random random = new Random(Path.GetFileName(shapeFilePath).GetHashCode());

			ArrayList metaValues = new ArrayList();
			
			if(m_ShapeTileArgs.DataKey != null)
			{
				ExtendedZipInputStream dbfReader =
					new ExtendedZipInputStream(File.OpenRead(shapeFilePath));

				ZipEntry dbfEntry= null;			
				dbfEntry = dbfReader.GetNextEntry();
				for(int p=0;p<dbfIndex;p++)
				{
					dbfEntry = dbfReader.GetNextEntry();
				}

				if(!dbfEntry.IsFile)
					return;
				
				byte dbfVersion = dbfReader.ReadByte();

				byte updateYear = dbfReader.ReadByte();
				byte updateMonth = dbfReader.ReadByte();
				byte updateDay = dbfReader.ReadByte();

				int numberRecords = dbfReader.ReadInt32();
				short headerLength = dbfReader.ReadInt16();

				short recordLength = dbfReader.ReadInt16();
				byte[] reserved = dbfReader.ReadBytes(20);

				int numberFields = (headerLength - 33) / 32;

				// Read Field Descriptor Array
				DBF_Field_Header[] fieldHeaders = new DBF_Field_Header[numberFields];

				for (int i = 0; i < numberFields; i++)
				{
					char[] fieldNameChars = dbfReader.ReadChars(10);
					char fieldNameTerminator = dbfReader.ReadChar();
					string fn = new string(fieldNameChars);
					fieldHeaders[i].FieldName = fn.Trim().Replace(" ","");

					fieldHeaders[i].FieldType = dbfReader.ReadChar();
					byte[] reserved1 = dbfReader.ReadBytes(4);

					if(String.Compare(fieldHeaders[i].FieldName.Trim(), m_ShapeTileArgs.DataKey, true) == 0)
					{
						m_ShapeTileArgs.DataKey = fieldHeaders[i].FieldName;
							
						if(fieldHeaders[i].FieldType == 'N')
						{
							m_ShapeTileArgs.UseScalar = true;
						}
						else
						{
							m_ShapeTileArgs.UseScalar = false;
						}

					}

					fieldHeaders[i].FieldLength = dbfReader.ReadByte();

					byte[] reserved2 = dbfReader.ReadBytes(15);
					
				}
				byte headerTerminator = dbfReader.ReadByte();

				double scalarMin = double.MaxValue;
				double scalarMax = double.MinValue;
				for (int i = 0; i < numberRecords; i++)
				{
					//	Shapefile_Polygon curPoly = (Shapefile_Polygon)this.m_ShapeTileArgs.ShapeRecords[i];
								
					byte isValid = dbfReader.ReadByte();
					for (int j = 0; j < fieldHeaders.Length; j++)
					{
						char[] fieldDataChars = dbfReader.ReadChars(fieldHeaders[j].FieldLength);
						string fieldData = new string(fieldDataChars);

						if(fieldHeaders[j].FieldName == m_ShapeTileArgs.DataKey)
						{
							metaValues.Add(fieldData);

							if(fieldHeaders[j].FieldType == 'N')
							{
								try
								{
									if(m_ShapeTileArgs.ScaleMin == double.NaN || m_ShapeTileArgs.ScaleMax == double.NaN)
									{
										double data = double.Parse(fieldData);
										if(m_ShapeTileArgs.ScaleMin == double.NaN && data < scalarMin)
										{
											scalarMin = data;
										}

										if(m_ShapeTileArgs.ScaleMax == double.NaN && data > scalarMax)
										{
											scalarMax = data;
										}
									}
								}
								catch(Exception ex)
								{
									Log.Write(ex);
								}
							}
							else
							{
								if(!m_ShapeTileArgs.ColorAssignments.Contains(fieldData))
								{
									System.Drawing.Color newColor = 
										System.Drawing.Color.FromArgb(
										1 + random.Next(254),
										1 + random.Next(254),
										1 + random.Next(254));

									m_ShapeTileArgs.ColorAssignments.Add(fieldData, newColor);		
								}
							}
						}
					}

					if(m_ShapeTileArgs.UseScalar && m_ShapeTileArgs.ScaleMin == double.NaN)
					{
						m_ShapeTileArgs.ScaleMin = scalarMin;
					}
					if(m_ShapeTileArgs.UseScalar && m_ShapeTileArgs.ScaleMax == double.NaN)
					{
						m_ShapeTileArgs.ScaleMax = scalarMax;
					}
				}
				dbfReader.Close();
										
			}


			ExtendedZipInputStream shpReader= new ExtendedZipInputStream(File.OpenRead(shapeFilePath));
			
			ZipEntry shpEntry= null;			
			shpEntry = shpReader.GetNextEntry();
			for(int p=0;p<shpIndex;p++)
			{
				shpEntry = shpReader.GetNextEntry();
			}

			if(!shpEntry.IsFile)
				return ;

	
			//get file header info
			// Big-Endian Integer File Type
			byte[] fileTypeBytes = shpReader.ReadBytes(4);
			int fileType = 16 * 16 * 16 * 16 * 16 * 16 * fileTypeBytes[0] + 16 * 16 * 16 * 16 * fileTypeBytes[1] + 16 * 16 * fileTypeBytes[2] + fileTypeBytes[3];

			byte[] unused1 = shpReader.ReadBytes(5 * 4);

			byte[] fileLengthBytes = shpReader.ReadBytes(4);
			int fileLength = 16 * 16 * 16 * 16 * 16 * 16 * fileLengthBytes[0] + 16 * 16 * 16 * 16 * fileLengthBytes[1] + 16 * 16 * fileLengthBytes[2] + fileLengthBytes[3];

			int version = shpReader.ReadInt32();
			int shapeType = shpReader.ReadInt32();

			m_BoundingBoxXMin = shpReader.ReadDouble();
			m_BoundingBoxYMin = shpReader.ReadDouble();
			m_BoundingBoxXMax = shpReader.ReadDouble();
			m_BoundingBoxYMax = shpReader.ReadDouble();
			m_BoundingBoxZMin = shpReader.ReadDouble();
			m_BoundingBoxZMax = shpReader.ReadDouble();
			m_BoundingBoxMMin = shpReader.ReadDouble();
			m_BoundingBoxMMax = shpReader.ReadDouble();
			
			//start reading records...
			int bytesRead = 100;
			int counter = 0;

			while (bytesRead < shpEntry.Size)
			{
				ArrayList pendingPoints = new ArrayList();
				
				//read record header
				byte[] recordNumberBytes = shpReader.ReadBytes(4);
				byte[] contentLengthBytes = shpReader.ReadBytes(4);

				int recordNumber = 16 * 16 * 16 * 16 * 16 * 16 * recordNumberBytes[0] + 16 * 16 * 16 * 16 * recordNumberBytes[1] + 16 * 16 * recordNumberBytes[2] + recordNumberBytes[3];
				int contentLength = 16 * 16 * 16 * 16 * 16 * 16 * contentLengthBytes[0] + 16 * 16 * 16 * 16 * contentLengthBytes[1] + 16 * 16 * contentLengthBytes[2] + contentLengthBytes[3];

				//read shape type to determine record structure and content
				int recordShapeType = shpReader.ReadInt32();
				
				ShapeRecord newRecord = new ShapeRecord();

				if(recordShapeType == 0) //Null shape type -- generally used as a placeholder
				{
					newRecord.Null = new Shapefile_Null();
				}
				else if(recordShapeType == 1) //Point shape type
				{
					newRecord.Point = new Shapefile_Point();
					newRecord.Point.X = shpReader.ReadDouble();
					newRecord.Point.Y = shpReader.ReadDouble();					

					pendingPoints.Add(
						MathEngine.SphericalToCartesian(newRecord.Point.Y, newRecord.Point.X, m_ShapeTileArgs.LayerRadius));
				}
				else if(recordShapeType == 8) //Multi-point shape type
				{
					newRecord.MultiPoint = new Shapefile_MultiPoint();
					newRecord.MultiPoint.BoundingBox.West = shpReader.ReadDouble();
					newRecord.MultiPoint.BoundingBox.South = shpReader.ReadDouble();
					newRecord.MultiPoint.BoundingBox.East = shpReader.ReadDouble();
					newRecord.MultiPoint.BoundingBox.North = shpReader.ReadDouble();

					newRecord.MultiPoint.NumPoints = shpReader.ReadInt32();
					
					newRecord.MultiPoint.Points = new Shapefile_Point[newRecord.MultiPoint.NumPoints];
					for(int i = 0; i < newRecord.MultiPoint.NumPoints; i++)
					{
						newRecord.MultiPoint.Points[i] = new Shapefile_Point();
						newRecord.MultiPoint.Points[i].X = shpReader.ReadDouble();
						newRecord.MultiPoint.Points[i].Y = shpReader.ReadDouble();
						
						pendingPoints.Add(
							MathEngine.SphericalToCartesian(newRecord.MultiPoint.Points[i].Y, newRecord.MultiPoint.Points[i].X, m_ShapeTileArgs.LayerRadius));
					}
				}
				else if(recordShapeType == 3)
				{
					newRecord.PolyLine = new Shapefile_PolyLine();
				
					newRecord.PolyLine.BoundingBox.West = shpReader.ReadDouble();
					newRecord.PolyLine.BoundingBox.South = shpReader.ReadDouble();
					newRecord.PolyLine.BoundingBox.East = shpReader.ReadDouble();
					newRecord.PolyLine.BoundingBox.North = shpReader.ReadDouble();
				
					newRecord.PolyLine.NumParts = shpReader.ReadInt32();
					newRecord.PolyLine.NumPoints = shpReader.ReadInt32();
					
					newRecord.PolyLine.Parts = new int[newRecord.PolyLine.NumParts];

					for (int i = 0; i < newRecord.PolyLine.Parts.Length; i++)
					{
						newRecord.PolyLine.Parts[i] = shpReader.ReadInt32();					
					}

					newRecord.PolyLine.Points = new Shapefile_Point[newRecord.PolyLine.NumPoints];
					for (int i = 0; i < newRecord.PolyLine.Points.Length; i++)
					{
						newRecord.PolyLine.Points[i] = new Shapefile_Point();
						newRecord.PolyLine.Points[i].X = shpReader.ReadDouble();
						newRecord.PolyLine.Points[i].Y = shpReader.ReadDouble();						
					}
				}
				else if(recordShapeType == 5)
				{
					newRecord.Polygon = new Shapefile_Polygon();
				
					newRecord.Polygon.BoundingBox.West = shpReader.ReadDouble();
					newRecord.Polygon.BoundingBox.South = shpReader.ReadDouble();
					newRecord.Polygon.BoundingBox.East = shpReader.ReadDouble();
					newRecord.Polygon.BoundingBox.North = shpReader.ReadDouble();
				
					newRecord.Polygon.NumParts = shpReader.ReadInt32();
					newRecord.Polygon.NumPoints = shpReader.ReadInt32();
					
					newRecord.Polygon.Parts = new int[newRecord.Polygon.NumParts];

					for (int i = 0; i < newRecord.Polygon.Parts.Length; i++)
					{
						newRecord.Polygon.Parts[i] = shpReader.ReadInt32();
						
					}
				

					newRecord.Polygon.Points = new Shapefile_Point[newRecord.Polygon.NumPoints];

					for (int i = 0; i < newRecord.Polygon.Points.Length; i++)
					{
						newRecord.Polygon.Points[i] = new Shapefile_Point();
						

						byte[] temp=new byte[16];
						for(int t=0;t<16;t++)
						{
							temp[t]=shpReader.ReadByte();
						}		
						newRecord.Polygon.Points[i].X=BitConverter.ToDouble(temp,0);
						newRecord.Polygon.Points[i].Y=BitConverter.ToDouble(temp,8);
					}
					
				}

				bool ignoreRecord = false;
					
				if(metaValues != null && metaValues.Count > 0)
				{
					newRecord.Value = metaValues[counter];
				
					if(m_ShapeTileArgs.ActiveDataValues != null)
					{
						ignoreRecord = true;
						if(m_ShapeTileArgs.UseScalar)
						{
							double currentValue = double.Parse(newRecord.Value.ToString());
							foreach(string activeValueString in m_ShapeTileArgs.ActiveDataValues)
							{
								double activeValue = double.Parse(activeValueString);
								if(activeValue == currentValue)
								{
									ignoreRecord = false;
									break;
								}
							}
						}
						else
						{
							string currentValue = (string)newRecord.Value;
							foreach(string activeValue in m_ShapeTileArgs.ActiveDataValues)
							{
								if(String.Compare(activeValue.Trim(), currentValue.Trim(), true) == 0)
								{
									ignoreRecord = false;
									break;
								}
							}
						}
					}
					else
					{
						if(m_ShapeTileArgs.UseScalar)
						{
							double currentValue = double.Parse(newRecord.Value.ToString());
							if(m_ScalarFilterMin != double.NaN)
							{
								if(currentValue < m_ScalarFilterMin)
								{
									ignoreRecord = true;
								}
							}
				
							if(m_ScalarFilterMax != double.NaN)
							{
								if(currentValue > m_ScalarFilterMax)
								{
									ignoreRecord = true;
								}
							}

							if(m_ShapeTileArgs.NoDataValues != null)
							{
								foreach(string noDataValueString in m_ShapeTileArgs.NoDataValues)
								{
									double noDataValue = double.Parse(noDataValueString);
									//TODO: might consider using epsilon if floating point errors occur
									if(noDataValue == currentValue)
									{
										ignoreRecord = true;
										break;
									}
								}
							}
						}
						else
						{
							string currentValue = (string)newRecord.Value;
							if(m_ShapeTileArgs.NoDataValues != null)
							{
								foreach(string noDataValue in m_ShapeTileArgs.NoDataValues)
								{
									if(String.Compare(currentValue.Trim(), noDataValue.Trim(), true) == 0)
									{
										ignoreRecord = true;
										break;
									}
								}
							}
						}
					}
				}
				
				if(!ignoreRecord)
				{
					m_ShapeTileArgs.ShapeRecords.Add(newRecord);

					if(pendingPoints.Count > 0)
					{
						foreach(Vector3 v in pendingPoints)
							m_PointList.Add(v);
					}
				}

				bytesRead += 8 + contentLength * 2;
				counter++;
			}
		}

        public double ActualMax { get; set; }
        public double ActualMin { get; set; }

        public void UpdateData(double[] data, DrawArgs drawArgs)
        {
            m_ShapeTileArgs.DataValues = data;
            m_ShapeTileArgs.ScaleMin = data.Min();
            m_ShapeTileArgs.ScaleMax = data.Max();
            foreach (ShapeTile shapeTile in m_RootTiles)
            {
                shapeTile.Initialize(drawArgs);
            }
        }

		private void loadShapeFile(string shapeFilePath)
		{
            List<double> DataValues = new List<double>();
          
			FileInfo shapeFile = new FileInfo(shapeFilePath);
			FileInfo dbaseFile = new FileInfo(shapeFile.FullName.Replace(".shp", ".dbf"));
			
			System.Random random = new Random(shapeFile.Name.GetHashCode());

            List<string> metaValues = new List<string>();
			
			if(m_ShapeTileArgs.DataKey != null && dbaseFile.Exists)
			{
				using (BinaryReader dbfReader = new BinaryReader(new BufferedStream(dbaseFile.OpenRead()), System.Text.Encoding.ASCII))
				{
					// First Read 32-byte file header
					int bytesRead = 0;
					byte dbfVersion = dbfReader.ReadByte();

					byte updateYear = dbfReader.ReadByte();
					byte updateMonth = dbfReader.ReadByte();
					byte updateDay = dbfReader.ReadByte();

					int numberRecords = dbfReader.ReadInt32();
					short headerLength = dbfReader.ReadInt16();

					short recordLength = dbfReader.ReadInt16();
					byte[] reserved = dbfReader.ReadBytes(20);

					bytesRead += 32;
					int numberFields = (headerLength - 33) / 32;

					// Read Field Descriptor Array
					DBF_Field_Header[] fieldHeaders = new DBF_Field_Header[numberFields];

					for (int i = 0; i < numberFields; i++)
					{
						char[] fieldNameChars = dbfReader.ReadChars(10);
						char fieldNameTerminator = dbfReader.ReadChar();
						string fn = new string(fieldNameChars);
						fieldHeaders[i].FieldName = fn.Trim().Replace(" ","");

						fieldHeaders[i].FieldType = dbfReader.ReadChar();
						byte[] reserved1 = dbfReader.ReadBytes(4);

						if(String.Compare(fieldHeaders[i].FieldName.Trim(), m_ShapeTileArgs.DataKey, true) == 0)
						{
							m_ShapeTileArgs.DataKey = fieldHeaders[i].FieldName;
							
							if(fieldHeaders[i].FieldType == 'N')
							{
								m_ShapeTileArgs.UseScalar = true;
							}
							else
							{
								m_ShapeTileArgs.UseScalar = false;
							}

						}

						fieldHeaders[i].FieldLength = dbfReader.ReadByte();

						byte[] reserved2 = dbfReader.ReadBytes(15);
						bytesRead += 32;

					}
					byte headerTerminator = dbfReader.ReadByte();

					double scalarMin = double.MaxValue;
					double scalarMax = double.MinValue;
					for (int i = 0; i < numberRecords; i++)
					{
					//	Shapefile_Polygon curPoly = (Shapefile_Polygon)this.m_ShapeTileArgs.ShapeRecords[i];
								
						byte isValid = dbfReader.ReadByte();
						for (int j = 0; j < fieldHeaders.Length; j++)
						{
							char[] fieldDataChars = dbfReader.ReadChars(fieldHeaders[j].FieldLength);
							string fieldData = new string(fieldDataChars);

							if(fieldHeaders[j].FieldName == m_ShapeTileArgs.DataKey)
							{
								metaValues.Add(fieldData);

								if(fieldHeaders[j].FieldType == 'N')
								{
									try
									{
										if(m_ShapeTileArgs.ScaleMin == double.NaN || m_ShapeTileArgs.ScaleMax == double.NaN)
										{
											double data = double.Parse(fieldData);
											if(m_ShapeTileArgs.ScaleMin == double.NaN && data < scalarMin)
											{
												scalarMin = data;
											}

											if(m_ShapeTileArgs.ScaleMax == double.NaN && data > scalarMax)
											{
												scalarMax = data;
											}
										}
									}
									catch(Exception ex)
									{
										Log.Write(ex);
									}
								}
								else
								{
									if(!m_ShapeTileArgs.ColorAssignments.Contains(fieldData))
									{
										System.Drawing.Color newColor = 
											System.Drawing.Color.FromArgb(
											1 + random.Next(254),
											1 + random.Next(254),
											1 + random.Next(254));

										m_ShapeTileArgs.ColorAssignments.Add(fieldData, newColor);		
									}
								}
							}
						}

						if(m_ShapeTileArgs.UseScalar && m_ShapeTileArgs.ScaleMin == double.NaN)
						{
							m_ShapeTileArgs.ScaleMin = scalarMin;
						}
						if(m_ShapeTileArgs.UseScalar && m_ShapeTileArgs.ScaleMax == double.NaN)
						{
							m_ShapeTileArgs.ScaleMax = scalarMax;
						}
					}
				}
			}

			FileInfo shapeFileInfo = new FileInfo(shapeFilePath);

			using( FileStream fs = File.OpenRead(shapeFileInfo.FullName) )
			{
				using (BinaryReader reader = new BinaryReader(new BufferedStream(fs)))
				{
					//get file header info
					// Big-Endian Integer File Type
					byte[] fileTypeBytes = reader.ReadBytes(4);
					int fileType = 16 * 16 * 16 * 16 * 16 * 16 * fileTypeBytes[0] + 16 * 16 * 16 * 16 * fileTypeBytes[1] + 16 * 16 * fileTypeBytes[2] + fileTypeBytes[3];

					byte[] unused1 = reader.ReadBytes(5 * 4);

					byte[] fileLengthBytes = reader.ReadBytes(4);
					int fileLength = 16 * 16 * 16 * 16 * 16 * 16 * fileLengthBytes[0] + 16 * 16 * 16 * 16 * fileLengthBytes[1] + 16 * 16 * fileLengthBytes[2] + fileLengthBytes[3];

					int version = reader.ReadInt32();
					int shapeType = reader.ReadInt32();

					m_BoundingBoxXMin = reader.ReadDouble();
					m_BoundingBoxYMin = reader.ReadDouble();
					m_BoundingBoxXMax = reader.ReadDouble();
					m_BoundingBoxYMax = reader.ReadDouble();
					m_BoundingBoxZMin = reader.ReadDouble();
					m_BoundingBoxZMax = reader.ReadDouble();
					m_BoundingBoxMMin = reader.ReadDouble();
					m_BoundingBoxMMax = reader.ReadDouble();

					//start reading records...
					int bytesRead = 100;
					int counter = 0;

					while (bytesRead < shapeFileInfo.Length)
					{
                        List<Vector3> pendingPoints = new List<Vector3>();
					
						//read record header
						byte[] recordNumberBytes = reader.ReadBytes(4);
						byte[] contentLengthBytes = reader.ReadBytes(4);

						int recordNumber = 16 * 16 * 16 * 16 * 16 * 16 * recordNumberBytes[0] + 16 * 16 * 16 * 16 * recordNumberBytes[1] + 16 * 16 * recordNumberBytes[2] + recordNumberBytes[3];
						int contentLength = 16 * 16 * 16 * 16 * 16 * 16 * contentLengthBytes[0] + 16 * 16 * 16 * 16 * contentLengthBytes[1] + 16 * 16 * contentLengthBytes[2] + contentLengthBytes[3];

						//read shape type to determine record structure and content
						int recordShapeType = reader.ReadInt32();
						
						ShapeRecord newRecord = new ShapeRecord();

						if(recordShapeType == 0) //Null shape type -- generally used as a placeholder
						{
							newRecord.Null = new Shapefile_Null();
						}
						else if(recordShapeType == 1) //Point shape type
						{
							newRecord.Point = new Shapefile_Point();
							newRecord.Point.X = reader.ReadDouble();
							newRecord.Point.Y = reader.ReadDouble();

							pendingPoints.Add(
								MathEngine.SphericalToCartesian(newRecord.Point.Y, newRecord.Point.X, m_ShapeTileArgs.LayerRadius));
						}
						else if(recordShapeType == 8) //Multi-point shape type
						{
							newRecord.MultiPoint = new Shapefile_MultiPoint();
							newRecord.MultiPoint.BoundingBox.West = reader.ReadDouble();
							newRecord.MultiPoint.BoundingBox.South = reader.ReadDouble();
							newRecord.MultiPoint.BoundingBox.East = reader.ReadDouble();
							newRecord.MultiPoint.BoundingBox.North = reader.ReadDouble();

							newRecord.MultiPoint.NumPoints = reader.ReadInt32();
							newRecord.MultiPoint.Points = new Shapefile_Point[newRecord.MultiPoint.NumPoints];
							for(int i = 0; i < newRecord.MultiPoint.NumPoints; i++)
							{
								newRecord.MultiPoint.Points[i] = new Shapefile_Point();
								newRecord.MultiPoint.Points[i].X = reader.ReadDouble();
								newRecord.MultiPoint.Points[i].Y = reader.ReadDouble();

								pendingPoints.Add(
									MathEngine.SphericalToCartesian(newRecord.MultiPoint.Points[i].Y, newRecord.MultiPoint.Points[i].X, m_ShapeTileArgs.LayerRadius));
							}
						}
                        else if (recordShapeType == 3 || recordShapeType == 13)
						{
							newRecord.PolyLine = new Shapefile_PolyLine();
						
							newRecord.PolyLine.BoundingBox.West = reader.ReadDouble();
							newRecord.PolyLine.BoundingBox.South = reader.ReadDouble();
							newRecord.PolyLine.BoundingBox.East = reader.ReadDouble();
							newRecord.PolyLine.BoundingBox.North = reader.ReadDouble();
						
							newRecord.PolyLine.NumParts = reader.ReadInt32();
							newRecord.PolyLine.NumPoints = reader.ReadInt32();
							newRecord.PolyLine.Parts = new int[newRecord.PolyLine.NumParts];

							for (int i = 0; i < newRecord.PolyLine.Parts.Length; i++)
							{
								newRecord.PolyLine.Parts[i] = reader.ReadInt32();
							}

							newRecord.PolyLine.Points = new Shapefile_Point[newRecord.PolyLine.NumPoints];
							for (int i = 0; i < newRecord.PolyLine.Points.Length; i++)
							{
								newRecord.PolyLine.Points[i] = new Shapefile_Point();
								newRecord.PolyLine.Points[i].X = reader.ReadDouble();
								newRecord.PolyLine.Points[i].Y = reader.ReadDouble();
							}
						}
						else if(recordShapeType == 5)
						{
							newRecord.Polygon = new Shapefile_Polygon();
						
							newRecord.Polygon.BoundingBox.West = reader.ReadDouble();
							newRecord.Polygon.BoundingBox.South = reader.ReadDouble();
							newRecord.Polygon.BoundingBox.East = reader.ReadDouble();
							newRecord.Polygon.BoundingBox.North = reader.ReadDouble();
						
							newRecord.Polygon.NumParts = reader.ReadInt32();
							newRecord.Polygon.NumPoints = reader.ReadInt32();
							newRecord.Polygon.Parts = new int[newRecord.Polygon.NumParts];

							for (int i = 0; i < newRecord.Polygon.Parts.Length; i++)
							{
								newRecord.Polygon.Parts[i] = reader.ReadInt32();
							}

							newRecord.Polygon.Points = new Shapefile_Point[newRecord.Polygon.NumPoints];
							for (int i = 0; i < newRecord.Polygon.Points.Length; i++)
							{
								newRecord.Polygon.Points[i] = new Shapefile_Point();
								newRecord.Polygon.Points[i].X = reader.ReadDouble();
								newRecord.Polygon.Points[i].Y = reader.ReadDouble();
							}
						}

						bool ignoreRecord = false;
							
						if(metaValues != null && metaValues.Count > 0)
						{
							newRecord.Value = metaValues[counter];
						
							if(m_ShapeTileArgs.ActiveDataValues != null)
							{
								ignoreRecord = true;
								if(m_ShapeTileArgs.UseScalar)
								{
									double currentValue = double.Parse(newRecord.Value.ToString());
									foreach(string activeValueString in m_ShapeTileArgs.ActiveDataValues)
									{
										double activeValue = double.Parse(activeValueString);
										if(activeValue == currentValue)
										{
											ignoreRecord = false;
											break;
										}
									}
								}
								else
								{
									string currentValue = (string)newRecord.Value;
									foreach(string activeValue in m_ShapeTileArgs.ActiveDataValues)
									{
										if(String.Compare(activeValue.Trim(), currentValue.Trim(), true) == 0)
										{
											ignoreRecord = false;
											break;
										}
									}
								}
							}
							else
							{
								if(m_ShapeTileArgs.UseScalar)
								{
									double currentValue = double.Parse(newRecord.Value.ToString());
									if(m_ScalarFilterMin != double.NaN)
									{
										if(currentValue < m_ScalarFilterMin)
										{
											ignoreRecord = true;
										}
									}
						
									if(m_ScalarFilterMax != double.NaN)
									{
										if(currentValue > m_ScalarFilterMax)
										{
											ignoreRecord = true;
										}
									}

                                    //if (currentValue > 750)
                                    //    currentValue = 200;

                                    if (currentValue > ActualMax)
                                        ActualMax = currentValue;

                                    if (currentValue < ActualMin)
                                        ActualMin = currentValue;

									if(m_ShapeTileArgs.NoDataValues != null)
									{
										foreach(string noDataValueString in m_ShapeTileArgs.NoDataValues)
										{
											double noDataValue = double.Parse(noDataValueString);
											//TODO: might consider using epsilon if floating point errors occur
											if(noDataValue == currentValue)
											{
												ignoreRecord = true;
												break;
											}
										}
									}
                                   DataValues.Add(currentValue);
								}
								else
								{
									string currentValue = (string)newRecord.Value;
									if(m_ShapeTileArgs.NoDataValues != null)
									{
										foreach(string noDataValue in m_ShapeTileArgs.NoDataValues)
										{
											if(String.Compare(currentValue.Trim(), noDataValue.Trim(), true) == 0)
											{
												ignoreRecord = true;
												break;
											}
										}
									}
								}
							}
						}
						
						if(!ignoreRecord)
						{
							m_ShapeTileArgs.ShapeRecords.Add(newRecord);

							if(pendingPoints.Count > 0)
							{
								foreach(Vector3 v in pendingPoints)
									m_PointList.Add(v);
							}
						}

						bytesRead += 8 + contentLength * 2;
						counter++;
					}
				}
			}
            m_ShapeTileArgs.ScaleMax = ActualMax;
            m_ShapeTileArgs.ScaleMin = ActualMin;
            m_ShapeTileArgs.DataValues = DataValues.ToArray();
		}
	}


	public class ExtendedZipInputStream 
	{
		private ZipInputStream zis;

		public ExtendedZipInputStream ( System.IO.Stream baseInputStream )
		{
			zis=new ZipInputStream(baseInputStream);
		}
		
		public ICSharpCode.SharpZipLib.Zip.ZipEntry GetNextEntry()
		{
			return zis.GetNextEntry();
		}

		public byte ReadByte()
		{
			return (byte)zis.ReadByte();
		}

		public int ReadByteAsInt()
		{
			return zis.ReadByte();
		}

		public Int32 ReadInt32()
		{
			byte[]temp=new byte[4];
			for(int i=0;i<4;i++)
				temp[i]=(byte)zis.ReadByte();			
			return BitConverter.ToInt32(temp,0);
		}
		
		public Int16 ReadInt16()
		{
			byte[]temp=new byte[2];
			for(int i=0;i<2;i++)
				temp[i]=(byte)zis.ReadByte();			
			return BitConverter.ToInt16(temp,0);
		}

		public double ReadDouble()
		{
			byte[]temp=new byte[8];
			for(int i=0;i<8;i++)
				temp[i]=(byte)zis.ReadByte();
			return BitConverter.ToDouble(temp,0);
		}

		public byte[] ReadBytes(int count)
		{
			byte[]temp=new byte[count];
			for(int i=0;i<count;i++)
				temp[i]=(byte)zis.ReadByte();
			return temp;
		}
		public char ReadChar()
		{
			return (char)zis.ReadByte();
		}
		public char[] ReadChars(int count)
		{
			char[]temp=new char[count];			
			for(int i=0;i<count;i++)
				temp[i]=(char)zis.ReadByte();

			return temp;
		}

		public void Close()
		{
			zis.Close();
		}
	}


	public class ShapeTileArgs
	{
		public static float TileDrawDistance = 2.5f;
		public static float TileSpreadFactor = 2.0f;
		public System.Collections.Hashtable ColorAssignments = new Hashtable();
		public ShapeFileLayer ParentShapeFileLayer;
		public double LayerRadius;

		bool m_ScaleColors = false;
		bool m_ShowLabels = false;
		string m_DataKey = null;
		bool m_OutlinePolygons = false;
		bool m_PolygonFill = false;
		ShapeFillStyle m_ShapeFillStyle = ShapeFillStyle.Solid;
		System.Drawing.Color m_PolygonColor;
		System.Drawing.Color m_LineColor;
		System.Drawing.Color m_LabelColor;
		bool m_UseScalar = false;
		float m_LineWidth = 1.0f;
		double m_ScaleMin = double.MaxValue;
		double m_ScaleMax = double.MinValue;
		bool m_IsPolyLine = false;
		string[] m_NoDataValues = null;
		string[] m_ActiveDataValues = null;
        List<ShapeRecord> m_ShapeRecords = new List<ShapeRecord>();
		World m_ParentWorld;
		Size m_TilePixelSize;

		public string[] ActiveDataValues
		{
			get
			{
				return m_ActiveDataValues;
			}
		}

		public bool ScaleColors
		{
			get
			{
				return m_ScaleColors;
			}
		}	

		public ShapeFillStyle ShapeFillStyle
		{
			get
			{
				return m_ShapeFillStyle;
			}
		}
		public bool IsPolyLine
		{
			get
			{
				return m_IsPolyLine;
			}
			set
			{
				m_IsPolyLine = value;
			}
		}
		public float LineWidth
		{
			get
			{
				return m_LineWidth;
			}
		}
		public bool OutlinePolygons
		{
			get
			{
				return m_OutlinePolygons;
			}
		}
		public System.Drawing.Color PolygonColor
		{
			get
			{
				return m_PolygonColor;
			}
		}
		public System.Drawing.Color LineColor
		{
			get
			{
				return m_LineColor;
			}
		}
		public System.Drawing.Color LabelColor
		{
			get
			{
				return m_LabelColor;
			}
		}
		public string[] NoDataValues
		{
			get
			{
				return m_NoDataValues;
			}
		}
		public bool PolygonFill
		{
			get
			{
				return m_PolygonFill;
			}
		}
		public bool UseScalar
		{
			get
			{
				return m_UseScalar;
			}
			set
			{
				m_UseScalar = value;
			}
		}
		public double ScaleMin
		{
			get
			{
				return m_ScaleMin;
			}
			set
			{
				m_ScaleMin = value;
			}
		}
		public double ScaleMax
		{
			get
			{
				return m_ScaleMax;
			}
			set
			{
				m_ScaleMax = value;
			}
		}
		public string DataKey
		{
			get
			{
				return m_DataKey;
			}
			set
			{
				m_DataKey = value;
			}
		}
		public List<ShapeRecord> ShapeRecords
		{
			get
			{
				return m_ShapeRecords;
			}
			set
			{
				m_ShapeRecords = value;
			}
		}
		public World ParentWorld
		{
			get
			{
				return m_ParentWorld;
			}
		}
		public Size TilePixelSize
		{
			get
			{
				return m_TilePixelSize;
			}
		}
		public bool ShowLabels
		{
			get
			{
				return m_ShowLabels;
			}
		}
        public double[] DataValues { get; set; }

		public ShapeTileArgs(
			World parentWorld,
			Size tilePixelSize,
			double layerRadius,
			ShapeFileLayer parentShapeLayer,
			string dataKey,
			bool scaleColors,
			double scaleMin,
			double scaleMax,
			string[] noDataValues,
			string[] activeDataValues,
			bool polygonFill,
			bool outlinePolygons,
			System.Drawing.Color polygonColor,
			ShapeFillStyle shapeFillStyle,
			System.Drawing.Color lineColor,
			System.Drawing.Color labelColor,
			float lineWidth,
			bool showLabels
			)
		{
			ParentShapeFileLayer = parentShapeLayer;
			LayerRadius = layerRadius;

			m_ParentWorld = parentWorld;
			m_TilePixelSize = tilePixelSize;
			m_DataKey = dataKey;
			m_ScaleColors = scaleColors;
			m_PolygonFill = polygonFill;
			m_ScaleMin = scaleMin;
			m_ScaleMax = scaleMax;
			m_NoDataValues = noDataValues;
			m_ActiveDataValues = activeDataValues;
			m_PolygonFill = polygonFill;
			m_OutlinePolygons = outlinePolygons;
			m_PolygonColor = polygonColor;
			m_ShapeFillStyle = shapeFillStyle;
			m_LineColor = lineColor;
			m_LabelColor = labelColor;
			m_LineWidth = lineWidth;
			m_ShowLabels = showLabels;

		}
	}

	/// <summary>
	/// Polygon shape types can be filled with any of these styles, which are the same as the HatchStyles in the GDI .NET framework.
	/// The exception is the "Solid" style, signifies a "solid" fill style (no hatching).
	/// </summary>
	public enum ShapeFillStyle
	{
		Solid,
		BackwardDiagonal,
		Cross,
		DarkDownwardDiagonal,
		DarkHorizontal,
		DarkUpwardDiagonal,
		DarkVertical,
		DashedDownwardDiagonal,
		DashedHorizontal,
		DashedUpwardDiagonal,
		DashedVertical,
		DiagonalBrick,
		DiagonalCross,
		Divot,
		DottedDiamond,
		DottedGrid,
		ForwardDiagonal,
		Horizontal,
		LargeCheckerBoard,
		LargeConfetti,
		LargeGrid,
		LightDownwardDiagonal,
		LightHorizontal,
		LightUpwardDiagonal,
		LightVertical,
		Max,
		Min,
		NarrowHorizontal,
		NarrowVertical,
		OutlinedDiamond,
		Percent05,
		Percent10,
		Percent20,
		Percent25,
		Percent30,
		Percent40,
		Percent50,
		Percent60,
		Percent70,
		Percent75,
		Percent80,
		Percent90,
		Plaid,
		Shingle,
		SmallCheckerBoard,
		SmallConfetti,
		SmallGrid,
		SolidDiamond,
		Sphere,
		Trellis,
		Wave,
		Weave,
		WideDownwardDiagonal,
		WideUpwardDiagonal,
		ZigZag
	}

	public class Shapefile_Polygon
	{
		public GeographicBoundingBox BoundingBox = new GeographicBoundingBox();
		public int NumParts;
		public int NumPoints;
		public int[] Parts;
		public Shapefile_Point[] Points;
	}

	public class Shapefile_PolyLine
	{
		public GeographicBoundingBox BoundingBox = new GeographicBoundingBox();
		public int NumParts;
		public int NumPoints;
		public int[] Parts;
		public Shapefile_Point[] Points;
	}

	public class Shapefile_Null
	{
		
	}

	public class Shapefile_Point
	{
		public double X;
		public double Y;
		public object Tag = null;
	}

	public class Shapefile_MultiPoint
	{
		public GeographicBoundingBox BoundingBox = new GeographicBoundingBox();
		public int NumPoints;
		public Shapefile_Point[] Points;
	}

	struct DBF_Field_Header
	{
		public string FieldName;
		public char FieldType;
		public byte FieldLength;
	}

    public	class ShapeRecord
	{
		#region Private Members
		string m_Id;
		Shapefile_Null m_Null = null;
		Shapefile_Point m_Point = null;
		Shapefile_MultiPoint m_MultiPoint = null;
		Shapefile_PolyLine m_PolyLine = null;
		Shapefile_Polygon m_Polygon = null;
		object m_Value = null;
		#endregion

		#region Properties
		public string ID
		{
			get
			{
				return m_Id;
			}
			set
			{
				m_Id = value;
			}
		}
		public Shapefile_Null Null
		{
			get
			{
				return m_Null;
			}
			set
			{
				m_Null = value;
			}
		}
		public Shapefile_Point Point
		{
			get
			{
				return m_Point;
			}
			set
			{
				m_Point = value;
			}
		}
		public Shapefile_MultiPoint MultiPoint
		{
			get
			{
				return m_MultiPoint;
			}
			set
			{
				m_MultiPoint = value;
			}
		}
		public Shapefile_PolyLine PolyLine
		{
			get
			{
				return m_PolyLine;
			}
			set
			{
				m_PolyLine = value;
			}
		}

		public Shapefile_Polygon Polygon
		{
			get
			{
				return m_Polygon;
			}
			set
			{
				m_Polygon = value;
			}
		}

		public object Value
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = value;
			}
		}
		
		#endregion

	}
}
