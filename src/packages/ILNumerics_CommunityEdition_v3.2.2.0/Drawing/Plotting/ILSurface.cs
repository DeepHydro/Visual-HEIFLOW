///
///    This file is part of ILNumerics Community Edition.
///
///    ILNumerics Community Edition - high performance computing for applications.
///    Copyright (C) 2006 - 2013 Haymo Kutschbach, http://ilnumerics.net
///
///    ILNumerics Community Edition is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License version 3 as published by
///    the Free Software Foundation.
///
///    ILNumerics Community Edition is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with ILNumerics Community Edition. See the file License.txt in the root
///    of your distribution package. If not, see <http://www.gnu.org/licenses/>.
///
///    In addition this software uses the following components and/or licenses: 
///
///    =================================================================================
///    The Open Toolkit Library License
///    
///    Copyright (c) 2006 - 2009 the Open Toolkit library.
///    
///    Permission is hereby granted, free of charge, to any person obtaining a copy
///    of this software and associated documentation files (the "Software"), to deal
///    in the Software without restriction, including without limitation the rights to 
///    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
///    the Software, and to permit persons to whom the Software is furnished to do
///    so, subject to the following conditions:
///
///    The above copyright notice and this permission notice shall be included in all
///    copies or substantial portions of the Software.
///
///    =================================================================================
///    Intel® Math Kernel Library 11.1 for Windows
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Intel® Math Kernel Library 10.3 for Linux
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Products / Software which is implicitly used by ILNumerics due to the inclusion 
///    of 3rd party components: 
///  
///        BLAS/ LAPACK; see: http://netlib.org
///        FFT Functions; see: http://www.spiral.net, http://fftw.org
///        OpenGL; see: http://opengl.org
///
///    =================================================================================


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ILNumerics.Exceptions;

namespace ILNumerics.Drawing.Plotting {
    [Serializable]
    public class ILSurface : ILGroup, IILColormapProvider {

        public enum ColorModes {
            /// <summary>
            /// All surface grid points have the same color
            /// </summary>
            Solid,
            /// <summary>
            /// Colors of the surface are mapped from a colormap
            /// </summary>
            Colormapped,
            /// <summary>
            /// Surface grid points are individually colored
            /// </summary>
            RBGA
        }

        #region attributes
        public static string SurfaceDefaultTag = "Surface";
        public static string WireframeTag = "SurfaceWireframe"; 
        public static string FillTag = "SurfaceFill"; 
        int m_m; 
        int m_n; 
        ILColormap m_colormap;
        ILArray<float> m_colorIndexValues = ILMath.localMember<float>(); 
        Tuple<float,float> m_dataRangeLimit = null; 
        bool? m_lighting;
        private ColorModes m_colorMode;
        #endregion

        #region properties
        public ColorModes ColorMode {
            get {
                return m_colorMode;
            }
            set {
                switch (value) {
                    case ColorModes.Solid:
                        if (!m_lighting.HasValue || m_lighting.GetValueOrDefault()) {
                            Fill.AutoNormals = true;
                            Fill.AutoComputeNormals();
                        } 
                        if (!Fill.Color.HasValue || Fill.Color.GetValueOrDefault().IsEmpty) {
                            Fill.Color = Color.DarkGreen; 
                        }
                        m_colorMode = ColorModes.Solid;
                        OnPropertyChanged("ColorMode");
                        break;
                    case ColorModes.Colormapped:
                        
                        if (m_colorIndexValues.S[0] != m_m && m_colorIndexValues.S[1] != m_n) 
                            m_colorIndexValues.a = Positions[":;:;0"]; 
                        if (m_dataRangeLimit == null) {
                            float mn, mx; 
                            m_colorIndexValues.GetLimits(out mn, out mx); 
                            m_dataRangeLimit = Tuple.Create(mn, mx); 
                        }
                        if (!m_lighting.HasValue || !m_lighting.GetValueOrDefault()) {
                            Fill.AutoNormals = false;
                            if (!Fill.Normals.IsEmpty)
                                Fill.Normals.Update(null); 
                        }
                        if (Fill.Color.HasValue && !Fill.Color.GetValueOrDefault().IsEmpty) {
                            Fill.Color = null;
                        }
                        m_colorMode = ColorModes.Colormapped;
                        Computation.ColorsFromColormap(this); 
                        OnPropertyChanged("ColorMode");
                        break;
                    case ColorModes.RBGA:
                        if (Fill.Colors.DataCount != m_n * m_m)
                            Computation.SetFillColorFromArray(ILMath.tosingle(ILMath.rand(Rows, Columns,4)), this); 
                        if (!m_lighting.HasValue || m_lighting.GetValueOrDefault()) {
                            Fill.AutoNormals = true; 
                            Fill.AutoComputeNormals(); 
                        }
                        if (Fill.Color.HasValue && !Fill.Color.GetValueOrDefault().IsEmpty) {
                            Fill.Color = null;
                        }
                        m_colorMode = ColorModes.RBGA; 
                        OnPropertyChanged("ColorMode");
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Get the current data range which corresponds to the current colormap or sets it
        /// </summary>
        public Tuple<float, float> DataRange {
            get {
                return m_dataRangeLimit; 
            }
            set {
                if (m_dataRangeLimit != value) {
                    m_dataRangeLimit = value;
                    if (m_colorMode == ColorModes.Colormapped) {
                        Computation.ColorsFromColormap(this); 
                    }
                    OnPropertyChanged("DataRange");
                }
            }
        }
        /// <summary>
        /// Gets the colormap used for coloring in colormapped modes or sets it 
        /// </summary>
        public ILColormap Colormap {
            get {
                if (m_colormap == null) {
                    m_colormap = new ILColormap();
                    m_colormap.Changed += Colormap_Changed;
                }
                return m_colormap;
            }
            set {
                if (m_colormap != value) {
                    if (m_colormap != null) {
                        m_colormap.Changed -= Colormap_Changed;  
                    }
                    m_colormap = value;
                    if (m_colormap != null) {
                        m_colormap.Changed += Colormap_Changed;
                    }
                    if (value != null && m_colorMode == ColorModes.Colormapped) {
                        Computation.ColorsFromColormap(this); 
                    }
                    OnPropertyChanged("Colormap");
                }
            }
        }
        /// <summary>
        /// Determines, if the surface plot utilizes lighting. Default: Auto (true for individual color mode, false for colormap mode)
        /// </summary>
        /// <remarks>Lighting makes it potentially harder to match the colors of the surface with the colorscale of the colormap /colorbar. 
        /// Therefore, for those surfaces which are using a colormap to determine the surface colors, lighting is disabled per default. For all 
        /// other surfaces, ligthing is enabled by default. By assigning a value to this property, this behavior can be overwritten.</remarks>
        public bool? UseLighting {
            get { return Fill.AutoNormals && !Fill.Normals.IsEmpty; }
            set {
                if (value.HasValue) {
                    if ((m_lighting.HasValue && m_lighting.GetValueOrDefault() != value.GetValueOrDefault()) || !m_lighting.HasValue) {
                        if (value.GetValueOrDefault()) {
                            Fill.AutoNormals = true;
                            Fill.AutoComputeNormals(); 
                        } else {
                            Fill.AutoNormals = false;
                            if (!Fill.Normals.IsEmpty)
                                Fill.Normals.Update(null);
                        }
                        OnPropertyChanged("UseLighting");
                    }
                } else if (m_lighting.HasValue) {
                    // value is null:
                    switch (m_colorMode) {
                        case ColorModes.Solid:
                            Fill.AutoNormals = true;
                            Fill.AutoComputeNormals(); 
                            break;
                        case ColorModes.Colormapped:
                            Fill.AutoNormals = false;
                            if (!Fill.Normals.IsEmpty)
                                Fill.Normals.Update(null);
                            break;
                        case ColorModes.RBGA:
                            Fill.AutoNormals = true;
                            Fill.AutoComputeNormals(); 
                            break;
                        default:
                            break;
                    }
                    m_lighting = null;
                    OnPropertyChanged("UseLighting");
                }
            }
        }
        /// <summary>
        /// Gets a reference to the lines shape representing the wireframes of the surface
        /// </summary>
        [XmlIgnore]
        public ILLines Wireframe {
            get {
                return First<ILLines>(WireframeTag);
            }
        }
        /// <summary>
        /// Gets a reference to the triangles shape representing the fill area of the surface 
        /// </summary>
        [XmlIgnore]
        public ILTriangles Fill {
            get {
                return First<ILTriangles>(FillTag);
            }
        }
        /// <summary>
        /// Gets the number of rows currently held in the surface, readonly
        /// </summary>
        public int Rows {
            get {
                return m_m;
            }
        }
        /// <summary>
        /// Gets the number of columns currently held in the surface, readonly
        /// </summary>
        public int Columns {
            get {
                return m_n;
            }
        }
        /// <summary>
        /// Gets the positioning values for this surface or sets them
        /// </summary>
        /// <remarks><para>The coords property converts the underlying buffer data to/from matrix for X,Y and Z values. No data are 
        /// stored redundantly. Therefore, the conversion may introduces a workload if frequently used.</para>
        /// <para>The array returned on get access reflects the data stored in the vertex buffers of the shapes. It is a lazy copy 
        /// on write clone and therefore cannot be used to alter the data in the buffers. In order to alter the surface data, 
        /// one must query the complete data set, do all modifications on the array returned and store the full array back by
        /// using the set accessor. </para>
        /// <para>Assigning data to the <c>Positions</c> property is a buffer changing operation. Therefore, one must call 
        /// <see cref="ILNumerics.Drawing.ILGroup.Configure(bool,bool)"/> on the group node or any group node above in order to populate the changes 
        /// for rendering.</para></remarks>
        [XmlIgnore]
        public ILRetArray<float> Positions {
            get {
                return Computation.PositionsBuffer2Array(this);
            }
            set {
                using (ILScope.Enter()) {
                    ILArray<float> A = value; 
                    Computation.Array2PositionIndicesBuffers(A, this);
                    if (m_colorMode == ColorModes.Colormapped) {
                        Colors = A;
                    }
                }
                OnPropertyChanged("Coords"); 
                //Configure();
            }
        }
        /// <summary>
        /// Gets the coloring values for this surface or sets them
        /// </summary>
        /// <remarks>
        /// <para>The color array returned on get access and the one expected on set access contain coloring information for 
        /// every grid point of the surface. The array has the size [m x n x 4] or [m x n x 1], where m and n are the number of rows and columns of 
        /// the surface grid. The size of the 3rd dimension determines the coloring mode of the surface: 
        /// <list type="bullet">
        /// <item>Providing an array of the size [m x n x <b>4</b>] will set the surface in RGBA mode. Individual RBGA tuples are expected for each grid point. 
        /// The surface will have lighting enabled and the colorbar / colormap will get disabled.</item>
        /// <item>Providing a matrix of size [m x n] will set the surface in colormapped mode. Values are taken as <i>references</i> into the current colormap. If 
        /// later the current colormap is changed, the resulting colors will change as well. Values of the matrix are mapped onto  
        /// the colorrange of the current colormap. <see cref="DataRange"/> can be used to adjust the data range used for mapping. Lighting will be disabled.</item>
        /// <item>Providing an empty array will cause the surface to switch to solid coloring. Neither the colormap nor individual color values are used for 
        /// coloring the surface. The colors of the <see cref="Fill"/> area and the <see cref="Wireframe"/> 
        /// still can individually get configured by accessing its individual <see cref="ILNumerics.Drawing.ILNode.Color"/> or <see cref="ILNumerics.Drawing.ILShape.Colors"/> properties. 
        /// Lighting will be enabled, the colormap will be disabled.</item>
        /// </list>
        /// </para>
        /// <para>The array returned on get access reflects the state of the surface. In RGBA mode, the array reflects the data stored in the 
        /// vertex colors buffer of the shapes. In colormapped mode the value reflects the indices data used to map the grid points to the current colormap.</para>
        /// <para>The array returned cannot be used to alter the data in the buffers directly. In order to alter the surface data, 
        /// one must query the complete data set, do all modifications on the array returned and store the full array back by
        /// using the set accessor.</para>
        /// <para>Assigning data to the <c>Colors</c> property is a buffer changing operation. Therefore, one must call the derived  
        /// <see cref="ILNumerics.Drawing.ILGroup.Configure(bool,bool)"/> on the group node or any group node above in order to populate changes made 
        /// to the rendering output.</para></remarks>
        [XmlIgnore]
        public ILRetArray<float> Colors {
            get {
                switch (m_colorMode) {
                    case ColorModes.Solid:
                        return ILMath.empty<float>(); 
                    case ColorModes.Colormapped:
                        return m_colorIndexValues; 
                    case ColorModes.RBGA:
                    default:
                        return Computation.GetArrayFromFillColorsBuffer(this);
                }
            }
            set {
                SetPropertyColors(value);
            }
        }
        public bool IsColormapped { get { return m_colorMode == ColorModes.Colormapped; } }
        #endregion

        #region ctors
        private ILSurface() { }

        /// <summary>
        /// Creates a new surface object
        /// </summary>
        /// <param name="ZXYPositions">Positions data for the grid points, matrix of size [m x n x [1|2|3]]</param>
        /// <param name="C">[optional] Colors for the grid points, size [m x n x [1|4]], default: colormapped heights (Z-values)</param>
        /// <param name="colormap">[optional] Colormap to be used for colormappings, default: 'ILNumerics'</param>
        /// <param name="tag">[optional]tag used to identify the surface within the scene graph</param>
        /// <param name="colorsDataRange">[optional] if not null, the lower (Item1) and the upper (Item2) limit of the inherent data range of the
        /// <paramref name="C"/> parameter in colormap mode. If null, the maximum and the minimum values are taken from <paramref name="C"/>.</param>
        /// <remarks>
        /// <para>The <paramref name="ZXYPositions"/> parameter defines the grid positions of the new surface. The array 
        /// is of size [m x n x 3]. m and m are the number of rows / columns respectively. The position coordinates for each 
        /// grid point is stored along the 3rd dimension. Note the order of the coordinates: Z,X,Y ! Z is obligatory, X and Y are optional. 
        /// If X and/or Y are ommitted (i.e. a matrix with Z values is provided only), a regular grid will be created for the missing axis, 
        /// having evenly spaced grid distances of 1. </para>
        /// <para>The optional <paramref name="C"/> parameter is used to define the coloring of each grid point. One of the 
        /// following modes are available: 
        /// <list type="bullet">
        /// <item><paramref name="C"/> is an 3d array of size [m x n x [3|4]] with individual RGB[A] color tupels for every grid point.</item>
        /// <item>If <paramref name="C"/> is a matrix of size [m x n], the values are mapped into the current colormap. If 
        /// the current colormap is changed, the resulting colors will change as well. Values of <paramref name="C"/> are mapped onto  
        /// the colorrange of the current colormap. <paramref name="colorsDataRange"/> can be used to adjust the data range used for mapping.</item>
        /// </list>
        /// </para>
        /// <para>The optional <paramref name="colormap"/> parameter can be used to set the surfaces colormap to an individual map. If this 
        /// parameter is ommitted, the predefined 'ILNumerics' colormap is taken by default.</para>
        /// </remarks>
        public ILSurface(ILInArray<float> ZXYPositions, ILInArray<float> C = null,
                        Tuple<float, float> colorsDataRange = null,
                        ILColormap colormap = null, object tag = null) : base(tag ?? SurfaceDefaultTag) {

            Add(new ILLines(WireframeTag));
            Wireframe.Color = Color.FromArgb(50, Color.Black); 
            Add(new ILTriangles(FillTag)); 
            Wireframe.Colors = Fill.Colors; 
            Wireframe.Positions = Fill.Positions; 

            using (ILScope.Enter(ZXYPositions, C)) {
                if (ILMath.isnullorempty(ZXYPositions) || ZXYPositions.S[0] < 2 || ZXYPositions.S[1] < 2) {
                    //throw new ILArgumentException("Coords parameter must be a matrix with at least 2 rows and 2 columns. See the Coords property!");
                }
                Computation.Array2PositionIndicesBuffers(ZXYPositions, this);

                if (colorsDataRange != null) {
                    m_dataRangeLimit = colorsDataRange;
                }
                m_colormap = colormap ?? new ILColormap(); 

                if (!object.Equals(C, null)) {
                    Colors = C;
                } else {
                    // use default colormap with height mapping 
                    Colors = ZXYPositions[":;:;0"];
                }
                if (ILMath.isnullorempty(ZXYPositions) || ZXYPositions.S[0] < 2 || ZXYPositions.S[1] < 2) {
                    ColorMode = ColorModes.Colormapped; 
                }
            }
        }

        internal ILSurface(ILSurface source)
            : base(source) {
            m_m = source.m_m;
            m_n = source.m_n;
            m_colormap = source.m_colormap != null ? source.m_colormap.Copy() : null;
            m_colorIndexValues.a = source.m_colorIndexValues.C; 
            m_dataRangeLimit = source.m_dataRangeLimit; 
            m_lighting = source.m_lighting; 
            m_colorMode = source.m_colorMode; 
        }
        #endregion

        #region public interface 
        /// <summary>
        /// Resize the grid size (number of rows and columns) 
        /// </summary>
        /// <param name="rows">number of rows</param>
        /// <param name="columns">number of columns</param>
        public void Resize(int rows, int columns) {
            if (rows == m_n && columns == m_n) return;
            if (rows < 2 || columns < 2) {
                throw new ILArgumentException("Surface resize: requires number of rows and columns greater 2");
            }
            using (ILScope.Enter()) {
                if (Rows > rows && Columns > columns) {
                    ILArray<float> A = Positions;
                    A.a = A[ILMath.r(0, rows - 1), ILMath.r(0, columns - 1)];
                    UpdateColormapped(A);
                } else {
                    ILArray<float> ret = ILMath.zeros<float>(rows, columns);
                    int takeRows = Math.Min(rows, Rows);
                    int takeCols = Math.Min(columns, Columns);
                    if (Rows > 1 && Columns > 1) {
                        ret[ILMath.r(0, takeRows), ILMath.r(0, takeCols)] =
                            Positions[ILMath.r(0, takeRows), ILMath.r(0, takeCols)];
                    }
                    UpdateColormapped(ret);
                }
            }
        }
        /// <summary>
        /// Update positions data [optional] and switches to solid coloring mode 
        /// </summary>
        /// <param name="ZXYPositions">[optional] positions data matrix, size: [M x N x [1|2|3]] with Z, [X and Y] values, resizing allowed</param>
        /// <param name="solidColor">Color for the surface fill</param>
        public void UpdateSolidColor(Color solidColor, ILInArray<float> ZXYPositions = null) {
            using (ILScope.Enter(ZXYPositions)) {
                if (!ILMath.isnull(ZXYPositions)) {
                    if (ILMath.isempty(ZXYPositions) || ZXYPositions.S[0] < 2 || ZXYPositions.S[1] < 2) {
                        Computation.Array2PositionIndicesBuffers(ILMath.zeros<float>(0, 0), this);
                        m_colorIndexValues.a = ILMath.empty<float>();
                    } else {
                        Computation.Array2PositionIndicesBuffers(ZXYPositions, this);
                    }
                    Configure();
                }
                if (!m_lighting.HasValue) {
                    Fill.AutoNormals = true;
                } else {
                    Fill.AutoNormals = false;
                    Fill.Normals.Update(null);
                }
                Fill.Color = solidColor;
                m_colorMode = ColorModes.Solid;
                OnPropertyChanged("ColorMode");
                OnPropertyChanged("Colors");
            }
        }
        /// <summary>
        /// Update positions data [optional] and switches to solid coloring mode 
        /// </summary>
        /// <param name="ZXYPositions">[optional] positions data matrix, size: [M x N x [1|2|3]] with Z, [X and Y] values, resizing allowed</param>
        /// <param name="RGBAcolors">RGBA color component tuples for every grid point, size [M x N x [3|4]]</param>
        public void UpdateRGBA(ILInArray<float> ZXYPositions = null, ILInArray<float> RGBAcolors = null) {
            using (ILScope.Enter(ZXYPositions, RGBAcolors)) {
                if (!ILMath.isnull(ZXYPositions)) {
                    if (ILMath.isempty(ZXYPositions) || ZXYPositions.S[0] < 2 || ZXYPositions.S[1] < 2) {
                        Computation.Array2PositionIndicesBuffers(ILMath.zeros<float>(0, 0), this);
                    } else {
                        Computation.Array2PositionIndicesBuffers(ZXYPositions, this);
                    }
                }
                if (!m_lighting.HasValue) {
                    Fill.AutoNormals = true;
                } else {
                    Fill.AutoNormals = false;
                    Fill.Normals.Update(null);
                }
                if (!ILMath.isnull(RGBAcolors)) {
                    if (RGBAcolors.S[0] != Rows || RGBAcolors.S[1] != Columns) {
                        throw new ILArgumentException(String.Format("Invalid sizeof RGBAcolors parameter. Expected: same size as ZXYPositions. Found: [{0} x {1}]",
                                    RGBAcolors.S[0], RGBAcolors.S[1]));
                    } else {
                        Computation.SetFillColorFromArray(RGBAcolors, this); 
                    }
                }
                m_colorMode = ColorModes.RBGA;
                Fill.Color = null; 
                Configure();
                OnPropertyChanged("ColorMode");
                OnPropertyChanged("Colors");
            }
        }
        /// <summary>
        /// Update positions data [optional] and color data [optional], switch to colormapped mode 
        /// </summary>
        /// <param name="ZXYPositions">[optional] positions data matrix, size: [M x N x [1|2|3]] with Z, [X and Y] values, resizing allowed</param>
        /// <param name="colormap">[optional] if given, the current colormap will be changed</param>
        /// <param name="dataRange">[optional] if defined, this range is used to map data values to the colormap instead of min/max(dataValues)</param>
        /// <param name="dataValues">[optional] matrix [M x N], with data values. If ommited: use Z values as data values</param>
        public void UpdateColormapped(ILInArray<float> ZXYPositions = null, 
                        ILColormap colormap = null,
                        Tuple<float, float> dataRange = null,
                        ILInArray<float> dataValues = null) {

            using (ILScope.Enter(ZXYPositions, dataValues)) {
                if (!ILMath.isnull(ZXYPositions)) {
                    #region positions provided
                    if (ILMath.isempty(ZXYPositions) || ZXYPositions.S[0] < 2 || ZXYPositions.S[1] < 2) {
                        Computation.Array2PositionIndicesBuffers(ILMath.zeros<float>(0, 0), this);
                        Computation.SetFillColorFromArray(ILMath.zeros<float>(0, 0), this);
                        m_colorIndexValues.a = ILMath.empty<float>();
                        return;
                    }
                    Computation.Array2PositionIndicesBuffers(ZXYPositions, this);
                    if (colormap != null) {
                        m_colormap = colormap;
                        OnPropertyChanged("Colormap");
                    } else {
                        m_colormap = new ILColormap();
                    }
                    if (!ILMath.isnull(dataValues)) {
                        if (dataValues.S[2] == 1 && dataValues.S[0] == ZXYPositions.S[0] && dataValues.S[1] == ZXYPositions.S[1]) {
                            m_colorIndexValues.a = dataValues;
                        } else {
                            throw new ILArgumentException(String.Format("Invalid data values array specified. Expected: same size as ZXYPosition matrix. Found: [{0} x {1}]. Was UpdateRGB() intended?",
                                                                        dataValues.S[0], dataValues.S[1]));
                        }
                    } else {
                        m_colorIndexValues.a = ZXYPositions[":;:;0"];
                    }
                    if (dataRange != null && dataRange != m_dataRangeLimit) {
                        m_dataRangeLimit = dataRange;
                        OnPropertyChanged("DataRange");
                    } else {
                        float min, max;
                        m_colorIndexValues.GetLimits(out min, out max);
                        m_dataRangeLimit = new Tuple<float, float>(min, max);
                        OnPropertyChanged("DataRange");
                    }
                    #endregion
                } else if (!ILMath.isnull(dataValues)) {
                    #region no positions provided
                    if (dataValues.S[0] < 2 || dataValues.S[1] < 2 || !dataValues.IsMatrix) {
                        throw new ILArgumentException("Invalid size of dataValues argument: Expected matrix with 2 rows and columns minimum"); 
                    }
                    m_colorIndexValues.a = dataValues;
                    if (Rows != m_colorIndexValues.S[0] || Columns != m_colorIndexValues.S[1]) {
                        Resize(m_colorIndexValues.S[0], m_colorIndexValues.S[1]); 
                    }
                    if (colormap != null) {
                        m_colormap = colormap;
                        OnPropertyChanged("Colormap");
                    } else if (m_colormap == null) {
                        m_colormap = new ILColormap();
                        OnPropertyChanged("Colormap");
                    }
                    if (dataRange != null && dataRange != m_dataRangeLimit) {
                        m_dataRangeLimit = dataRange;
                    } else {
                        float min, max;
                        m_colorIndexValues.GetLimits(out min, out max);
                        m_dataRangeLimit = new Tuple<float, float>(min, max);
                    }
                    OnPropertyChanged("DataRange");
                    #endregion
                } else {
                    #region no positions, no data values provided
                    if (colormap != null) {
                        m_colormap = colormap;
                        OnPropertyChanged("Colormap");
                    } else if (m_colormap == null) {
                        m_colormap = new ILColormap();
                        OnPropertyChanged("Colormap");
                    }
                    if (dataRange != null) {
                        m_dataRangeLimit = dataRange;
                        OnPropertyChanged("DataRange");
                    }
                    if (m_colorIndexValues == null || m_colorIndexValues.S[0] != Rows || m_colorIndexValues.S[1] != Columns) {
                        m_colorIndexValues = Positions[":;:;0"]; 
                    }
                    #endregion
                }
                m_colorMode = ColorModes.Colormapped;
                OnPropertyChanged("ColorMode");
                Computation.ColorsFromColormap(this);
                Configure(); 
            }
        }
        /// <summary>
        /// Create a copy of the surface. This function is used internally. 
        /// </summary>
        /// <returns></returns>
        internal override ILNode Copy() {
            return new ILSurface(this); 
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILSurface();
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILSurface ret = (ILSurface)base.Synchronize(copy, syncParams);
            if (copy == null || copy.SynchedVersion != Version) {
                ret.m_m = m_m;
                ret.m_n = m_n;
                ret.m_colorIndexValues.a = m_colorIndexValues.C; 
                if (m_colormap != null) 
                    ret.m_colormap = m_colormap.Synchronize(ret.m_colormap); 
                ret.m_dataRangeLimit = m_dataRangeLimit;
                ret.m_colorMode = m_colorMode; 
                ret.m_lighting = m_lighting; 
            }
            return ret; 
        }
        #endregion

        #region private helper
        private void SetPropertyColors(ILInArray<float> A) {
            using (ILScope.Enter(A)) {
                if (ILMath.isnullorempty(A) || A.S[0] < 2 || A.S[1] < 2) {
                    // solid coloring mode
                    if (m_colorMode != ColorModes.Solid) {
                        ColorMode = ColorModes.Solid;
                        OnPropertyChanged("Colors");
                    }
                } else if (A.S[0] == m_m && A.S[1] == m_n) {
                    if (A.S[2] == 1) {
                        // colormap mode
                        m_colorIndexValues.a = A; 
                        m_dataRangeLimit = null; 
                        ColorMode = ColorModes.Colormapped;
                        OnPropertyChanged("Colors");

                    } else if (A.S[2] == 4 || A.S[2] == 3) {
                        // RGBA mode
                        ColorMode = ColorModes.RBGA;
                        Computation.SetFillColorFromArray(A, this);
                        OnPropertyChanged("Colors");
                    }
                } else {
                    throw new ILArgumentException("Invalid array provided for surfaces Colors property. Data must be empty or [m x n x [1|4]]");
                }
            }
        }
        protected void Colormap_Changed(object sender, EventArgs args) {
            if (ColorMode == ColorModes.Colormapped) {
                Computation.ColorsFromColormap(this); 
            }
        }
        [System.Security.SecuritySafeCritical]
        private class Computation : ILMath {

            internal static void 
                Array2PositionIndicesBuffers(ILInArray<float> A, ILSurface surface) {
                using (ILScope.Enter(A)) {
                    if (isnullorempty(A) || A.S[0] < 2 || A.S[1] < 2) {
                        surface.Wireframe.Positions.Update(null); 
                        surface.Fill.Positions.Update(null);
                        surface.Wireframe.Indices.Update(null);
                        surface.Fill.Indices.Update(null);
                        return; 
                    }

                    int m = A.S[0]; 
                    int n = A.S[1];
                    ILArray<float> vert = zeros<float>(3, m * n);

                    // take Z values from 1st slice (obligatory)
                    vert["2;:"] = reshape(A[full, full, 0], 1, m * n);

                    if (A.S[2] > 1) {
                        // take X values from 2nd slice
                        vert["0;:"] = reshape(A[full, full, 1], 1, m * n);
                    } else {
                        vert["0;:"] = reshape(repmat(linspace<float>(0, n-1, n), m, 1), 1, m * n);
                    }
                    if (A.S[2] > 2) {
                        // take Y values from 3rd slice
                        vert["1;:"] = reshape(A[full, full, 2], 1, m * n);
                    } else {
                        vert["1;:"] = reshape(repmat(linspace<float>(0, m - 1, m), 1, n), 1, m * n);
                    }
                    surface.Wireframe.Positions.Update(vert);

                    if (surface.Rows != m || surface.Columns != n) {
                        // prepare indices: vertical
                        ILArray<int> buffer = zeros<int>(1, 2 * ((m-1) * n + (n-1) * m)); 
                        ILArray<int> ind = counter<int>(0, 1, size(m,n))[r(0, end - 1), full];
                        ind.a = reshape(ind, 1, ind.S.NumberOfElements);
                        ind["1;:"] = ind + 1;
                        ind.a = reshape(ind, 1, ind.S.NumberOfElements);
                        int lastVertIDX = ind.S.NumberOfElements;
                        System.Diagnostics.Debug.Assert(lastVertIDX == 2 * (m-1) * n); 
                        buffer[0,r(0,ind.S.NumberOfElements-1)] = ind; 
                        // horizontal indices
                        ind.a = counter<int>(0, 1, size(m, n))[full, r(0, end - 1)];
                        ind.a = reshape(ind, 1, ind.S.NumberOfElements);
                        ind["1;:"] = ind + m;
                        buffer[0, r(lastVertIDX, end)] = ind;
                        surface.Wireframe.Indices.Update(buffer);

                        // Fill - shared vertices
                        surface.Fill.Positions = surface.Wireframe.Positions;
                        //surface.Fill.Colors = new ILColorsBuffer();
                        //surface.Fill.Color = System.Drawing.Color.Blue; 
                        ILArray<int> lind = counter<int>(0, 1, size(m, n))[r(0, end - 1), r(0, end - 1)];
                        ind.a = reshape(lind, 1, lind.S.NumberOfElements);
                        // A[0,0] is the _lower_ left corner! So we have to inverse the winding order for lighting!
                        ind["5;:"] = lind + m + 1;
                        ind["1;:"] = ind["5;:"];
                        ind["2;:"] = lind + m;
                        ind["3;:"] = lind;
                        ind["4;:"] = lind + 1;

                        //ind["5;:"] = lind + 1;
                        //ind["4;:"] = lind + m + 1;
                        //ind["1;:"] = lind + m;
                        //ind["3;:"] = lind;
                        //ind["2;:"] = ind["4;:"];
                        surface.Fill.Indices.Update(ind);
                        surface.m_m = m; 
                        surface.m_n = n; 
                    }
                 }
            }

            internal static ILRetArray<float> PositionsBuffer2Array(ILSurface surface) {
                using (ILScope.Enter()) {
                    if (isempty(surface.Fill.Positions.Storage)) {
                        return empty<float>(0,0,3); 
                    }
                    ILArray<float> ret = zeros<float>(surface.m_m,surface.m_n);
                    ILArray<float> tmp = surface.Fill.Positions.Storage;
                    ret[":;:;1"] = reshape(tmp["0;:"], surface.m_m, surface.m_n);
                    ret[":;:;2"] = reshape(tmp["1;:"], surface.m_m, surface.m_n);
                    ret[":;:;0"] = reshape(tmp["2;:"], surface.m_m, surface.m_n); 
                    return ret; 
                }
            }

            internal static ILRetArray<float> GetArrayFromFillColorsBuffer(ILSurface surface) { 
                using (ILScope.Enter()) {
                    if (isempty(surface.Fill.Colors.Storage)) {
                        return empty<float>(0, 0, 4);
                    }
                    ILArray<float> ret = zeros<float>(surface.m_m, surface.m_n, 4);
                    ILArray<float> buff = surface.Fill.Colors.Storage;
                    ret[":;:;0"] = reshape(buff["0;:"], surface.m_m, surface.m_n);
                    ret[":;:;1"] = reshape(buff["1;:"], surface.m_m, surface.m_n);
                    ret[":;:;2"] = reshape(buff["2;:"], surface.m_m, surface.m_n);
                    ret[":;:;3"] = reshape(buff["3;:"], surface.m_m, surface.m_n);
                    return ret; 
                }
            }

            internal static void SetFillColorFromArray(ILInArray<float> A, ILSurface surface) {
                using (ILScope.Enter(A)) {
                    int m = surface.m_m, n = surface.m_n;
                    if (isnullorempty(A) || A.S[0] < 2 || A.S[1] < 2) {
                        surface.Fill.Colors.Update(null);
                        surface.Fill.Color = Color.Gray; 
                        surface.m_colorIndexValues.a = empty<float>();
                        surface.m_colormap = null;
                        surface.m_dataRangeLimit = Tuple.Create(0f, 0f);
                    }
                    if (A.S[2] < 3 || A.S[2] > 4) {
                        throw new ILArgumentException("Parameter A must be a 3 dimensional array of size (m x n x 3) or (m x n x 4)"); 
                    }
                    ILArray<float> buff = zeros<float>(4, m * n);

                    buff[0, full] = reshape(A[full, full, 0], 1, m * n);
                    buff[1, full] = reshape(A[full, full, 1], 1, m * n);
                    buff[2, full] = reshape(A[full, full, 2], 1, m * n);
                    if (A.S[2] == 4) {
                        buff[3, full] = reshape(A[full, full, 3], 1, m * n);
                    } else {
                        buff[3, full] = 1;
                    }
                    surface.Fill.Colors.Update(buff);
                    surface.Fill.Color = null;
                }
            }

            internal static void ColorsFromColormap(ILSurface surface) {
                using (ILScope.Enter()) {
                    ILArray<float> mapped = surface.Colormap.Map(surface.m_colorIndexValues, surface.DataRange).T;
                    surface.Fill.Colors.Update(mapped);
                    surface.Fill.Color = null;
                }
            }
        }
        #endregion

        #region IILAxisDataProvider Members

        public float GetRangeMinValue(AxisNames AxisName) {
            return m_dataRangeLimit.Item1; 
        }

        public float GetRangeMaxValue(AxisNames AxisName) {
            return m_dataRangeLimit.Item2;
        }

        public AxisScale ScaleMode(AxisNames AxisName) {
            return AxisScale.Linear; 
        }

        #endregion

        public static ILSurface CreateWireframeSolid(ILInArray<float> A, Color color) {
            using (ILScope.Enter(A)) {
                ILSurface ret = new ILSurface(0); 
                ret.UpdateSolidColor(color, A);
                ret.Fill.Visible = false; 
                return ret; 
            }
        }
    }
}
