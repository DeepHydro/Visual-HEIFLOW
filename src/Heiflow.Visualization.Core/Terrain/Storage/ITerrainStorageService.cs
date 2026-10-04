using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HUST.WREIS.Dot3D.Renderable;


namespace HUST.WREIS.Dot3D.Terrain
{
    public interface ITerrainStorageService
    {
        float[,] LoadTerrain(TerrainTile qt);
        void SaveTerrain(TerrainTile qt, MemoryStream ms);
    }


 
}
