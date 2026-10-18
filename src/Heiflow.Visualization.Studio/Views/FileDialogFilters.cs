using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Studio
{
    /// <summary>
    /// The filters the file dialogs of the shell are given, kept together so that all of the dialogs ask
    /// for the same kinds of file in the same words.
    ///
    /// A filter is pairs joined by a vertical bar: first what the dialog shows to the picker, then what
    /// it matches, the pattern written with its wildcard spelled out as in "*.shp". A filter given the
    /// other way round, a bare pattern with no wildcard in it, or none at all, leaves the dialog showing
    /// every file there is, which is what a dialog that appears to have no working filter really is.
    /// </summary>
    internal static class FileDialogFilters
    {
        /// <summary>Project files of HydroEarth, the three dimensional ones among them.</summary>
        public const string Project = "HydroEarth project (*.ihmx)|*.ihmx";

        /// <summary>Vector layers, and the polygons drawn as water come out of these too.</summary>
        public const string Shapefile = "Shapefile (*.shp)|*.shp";

        /// <summary>
        /// Rasters: the grids of numbers that are read as such, GeoTIFF among them, together with the
        /// pictures that are read together with their world file, see RasterDrawing.
        /// </summary>
        public const string Raster = "GeoTIFF (*.tif; *.tiff)|*.tif;*.tiff|Picture (*.png; *.jpg; *.jpeg; *.bmp)|*.png;*.jpg;*.jpeg;*.bmp";

        /// <summary>Models in the DirectX .x format, what ModelFeatureProvider reads.</summary>
        public const string ThreeDModel = "3D model (*.x)|*.x";

        /// <summary>Observation databases of the ODM kind, kept in Access.</summary>
        public const string AccessDatabase = "Access database (*.mdb)|*.mdb";

        /// <summary>Attribute tables of a shapefile on their own.</summary>
        public const string DbfTable = "dBASE table (*.dbf)|*.dbf";

        /// <summary>Screenshots, see SceneWindow.SaveScreenshot.</summary>
        public const string Image = "Bitmap (*.bmp)|*.bmp|JPEG image (*.jpg)|*.jpg|PNG image (*.png)|*.png";

        /// <summary>Everything, put last so that it never becomes the entry a dialog opens with.</summary>
        public const string AllFiles = "All files (*.*)|*.*";
    }
}
