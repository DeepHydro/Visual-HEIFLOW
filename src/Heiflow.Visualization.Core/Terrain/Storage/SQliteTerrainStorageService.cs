using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HUST.WREIS.Dot3D.Renderable;
using HUST.WREIS.Dot3D.Terrain;
using Heiflow.Spatial;

namespace HUST.WREIS.Dot3D.Terrain
{
     public class SQliteTerrainStorageService:ITerrainStorageService
    {
        public SQliteTerrainStorageService(string dbPath, int terrainType)
        {
            mDbPath = dbPath;
            mTerrainType = terrainType;
        }

        private string mDbPath;
     //   private TerrainTileService mOwner;
        private int mTerrainType = 3;

        #region ITerrainStorageService 成员

        public float[,] LoadTerrain(TerrainTile qt)
        {
            float[,] ElevationData = new float[qt.SamplesPerTile, qt.SamplesPerTile];
            GPoint pt = new GPoint(qt.Row, qt.Col);
            MemoryStream ms = Heiflow.Spatial.Internals.Cache.Instance.ImageCache.GetImageMemoryStream(mTerrainType, pt, qt.TargetLevel);
            if (ms != null)
            {
                BinaryReader br = new BinaryReader(ms);

                if (qt.Owner.DataType.ToLower() == "int16")
                {
                    for (int y = 0; y < qt.SamplesPerTile; y++)
                        for (int x = 0; x < qt.SamplesPerTile; x++)
                            ElevationData[x, y] = br.ReadInt16();
                }
                else if (qt.Owner.DataType.ToLower() == "float32")
                {
                    for (int y = 0; y < qt.SamplesPerTile; y++)
                        for (int x = 0; x < qt.SamplesPerTile; x++)
                        {
                            if (ms.Position < ms.Length)
                            {
                               // ElevationData[x, y] = br.ReadSingle();
                                ElevationData[x, y] = br.ReadInt16();
                            }
                            else
                            {
                                ElevationData[x, y] = 0;
                            }
                        }
                }
            }
            else
            {
                qt.StartDownload();
            }
            return ElevationData;
        }

        public void SaveTerrain(TerrainTile qt, MemoryStream ms)
        {
            GPoint pt = new GPoint(qt.Row, qt.Col);
            if (!Heiflow.Spatial.Internals.Cache.Instance.ImageCache.Exists(mTerrainType, pt, qt.TargetLevel) && ms != null)
            {
                var array = ms.ToArray();
                Heiflow.Spatial.Internals.Cache.Instance.ImageCache.PutImageToCache(array, mTerrainType, pt, qt.TargetLevel);
                ms.Close();
            }
        }
        #endregion
    }
}
