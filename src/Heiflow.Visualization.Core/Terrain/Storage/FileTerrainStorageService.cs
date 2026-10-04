using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HUST.WREIS.Dot3D.Renderable;
using HUST.WREIS.Dot3D.Terrain;

namespace HUST.WREIS.Dot3D.Terrain
{
    public class FileTerrainStorageService : ITerrainStorageService
    {
        #region ITerrainStorageService 成员

        public float[,] LoadTerrain(TerrainTile qt)
        {
            float[,] ElevationData = new float[qt.SamplesPerTile, qt.SamplesPerTile];
            if (File.Exists(qt.TerrainTileFilePath))
            {
                // Load elevation file
                try
                {
                    // TerrainDownloadRequest's FlagBadTile() creates empty files
                    // as a way to flag "bad" terrain tiles.
                    // Remove the empty 'flag' files after preset time.
                    try
                    {
                        FileInfo tileInfo = new FileInfo(qt.TerrainTileFilePath);
                        if (tileInfo.Length == 0)
                        {
                            TimeSpan age = DateTime.Now.Subtract(tileInfo.LastWriteTime);
                            if (age < qt.Owner.TerrainTileRetryInterval)
                            {
                                // This tile is still flagged bad
                                qt.IsInitialized = true;
                            }
                            else
                            {
                                // remove the empty 'flag' file
                                File.Delete(qt.TerrainTileFilePath);
                            }
                        }
                    }
                    catch
                    {
                        // Ignore any errors in the above block, and continue.
                        // For example, if someone had the empty 'flag' file
                        // open, the delete would fail.
                    }

                    using (Stream s = File.OpenRead(qt.TerrainTileFilePath))
                    {
                        BinaryReader reader = new BinaryReader(s);
                        if (qt.Owner.DataType.ToLower() == "int16")
                        {
                            /*
                            byte[] tfBuffer = new byte[SamplesPerTile*SamplesPerTile*2];
                            if (s.Read(tfBuffer,0,tfBuffer.Length) < tfBuffer.Length)
                                throw new IOException(string.Format("End of file error while reading terrain file '{0}'.", TerrainTileFilePath) );

                            int offset = 0;
                            for(int y = 0; y < SamplesPerTile; y++)
                                for(int x = 0; x < SamplesPerTile; x++)
                                    ElevationData[x,y] = tfBuffer[offset++] + (short)(tfBuffer[offset++]<<8);
                            */
                            for (int y = 0; y < qt.SamplesPerTile; y++)
                                for (int x = 0; x < qt.SamplesPerTile; x++)
                                    ElevationData[x, y] = reader.ReadInt16();
                        }
                        if (qt.Owner.DataType.ToLower() == "float32")
                        {
                            /*
                            byte[] tfBuffer = new byte[SamplesPerTile*SamplesPerTile*4];
                            if (s.Read(tfBuffer,0,tfBuffer.Length) < tfBuffer.Length)
                                    throw new IOException(string.Format("End of file error while reading terrain file '{0}'.", TerrainTileFilePath) );
                            */
                            for (int y = 0; y < qt.SamplesPerTile; y++)
                                for (int x = 0; x < qt.SamplesPerTile; x++)
                                {
                                    ElevationData[x, y] = reader.ReadSingle();
                                }
                        }
                        qt.IsInitialized = true;
                        qt.IsValid = true;
                    }
                }
                catch (IOException)
                {
                    // If there is an IO exception when reading the terrain tile,
                    // then either something is wrong with the file, or with
                    // access to the file, so try and remove it.
                    try
                    {
                        File.Delete(qt.TerrainTileFilePath);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(String.Format("Error while trying to delete corrupt terrain tile {0}", qt.TerrainTileFilePath), ex);
                    }
                }
                catch (Exception ex)
                {
                    // Some other type of error when reading the terrain tile.
                    throw new ApplicationException(String.Format("Error while trying to read terrain tile {0}", qt.TerrainTileFilePath), ex);
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

        }

        #endregion
    }
}
