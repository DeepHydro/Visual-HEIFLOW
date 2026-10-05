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
using System.Text;
using System.IO; 
using System.IO.Compression; 
using ILNumerics; 
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions; 


namespace ILNumerics {
    
    /// <summary>
    /// Matlab .mat file wrapper class
    /// </summary>
    /// <remarks>This class reads and writes Matlab .mat files version 6! ! 
    /// All numeric array types are supported. The reading and writing of 
    /// Matlab cell arrays is not supported yet.</remarks>
    public sealed class ILMatFile : IDisposable {

        #region attributes 
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string m_filename = "(unknown)"; 
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private ILCell m_data = ILMath.cell();
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string headerFromFile;
        #endregion
                      
        #region constants
        /// <summary>
        /// Inner types for MATLAB data elements
        /// </summary>
        public enum MatFileType : int {
            /// <summary>
            /// unknown 
            /// </summary>
            miUNKNOWN = 0,
            /// <summary>
            /// Int8
            /// </summary>
            miINT8 = 1,
            /// <summary>
            /// UInt8
            /// </summary>
            miUINT8 = 2,
            /// <summary>
            /// Int16
            /// </summary>
            miINT16 = 3,
            /// <summary>
            /// UInt16
            /// </summary>
            miUINT16 = 4,
            /// <summary>
            /// int32
            /// </summary>
            miINT32 = 5,
            /// <summary>
            /// UInt32
            /// </summary>
            miUINT32 = 6,
            /// <summary>
            /// float
            /// </summary>
            miSINGLE = 7,
            /// <summary>
            /// double
            /// </summary>
            miDOUBLE = 9,
            /// <summary>
            /// Int64
            /// </summary>
            miINT64 = 12,
            /// <summary>
            /// UInt64
            /// </summary>
            miUINT64 = 13,
            /// <summary>
            /// matrix type (general)
            /// </summary>
            miMATRIX = 14,
            /// <summary>
            /// compressed
            /// </summary>
            miCOMPRESSED = 15,
            /// <summary>
            /// utf8 encoded
            /// </summary>
            miUTF8 = 16,
            /// <summary>
            /// utf16 encoded
            /// </summary>
            miUTF16 = 17,
            /// <summary>
            /// utf32 encoded
            /// </summary>
            miUTF32 = 18
        }
        /// <summary>
        /// Types for matrix chunks
        /// </summary>
        public enum MatFileArrayClass {
            /// <summary>
            /// cell
            /// </summary>
            mxCELL_CLASS = 1,
            /// <summary>
            /// struct
            /// </summary>
            mxSTRUCT_CLASS = 2,
            /// <summary>
            /// object
            /// </summary>
            mxOBJECT_CLASS = 3,
            /// <summary>
            /// char
            /// </summary>
            mxCHAR_CLASS = 4,
            /// <summary>
            /// sparse
            /// </summary>
            mxSPARSE_CLASS = 5,
            /// <summary>
            /// double
            /// </summary>
            mxDOUBLE_CLASS = 6,
            /// <summary>
            /// float
            /// </summary>
            mxSINGLE_CLASS = 7,
            /// <summary>
            /// Int8
            /// </summary>
            mxINT8_CLASS = 8,
            /// <summary>
            /// UInt8
            /// </summary>
            mxUINT8_CLASS = 9,
            /// <summary>
            /// Int16
            /// </summary>
            mxINT16_CLASS = 10,
            /// <summary>
            /// UInt16
            /// </summary>
            mxUINT16_CLASS = 11,
            /// <summary>
            /// Int32
            /// </summary>
            mxINT32_CLASS = 12,
            /// <summary>
            /// UInt32
            /// </summary>
            mxUINT32_CLASS = 13,
            /// <summary>
            /// Int32
            /// </summary>
            mxINT64_CLASS = 14,
            /// <summary>
            /// UInt32
            /// </summary>
            mxUINT64_CLASS = 15
        }

        /// <summary>
        /// List of keywords which Matlab disallows for variable names
        /// </summary>
        public static readonly string[] ReservedKeywords = new string[] {
            "break",
            "case"  ,
            "catch"  ,
            "continue",
            "else"    ,
            "elseif"  ,
            "end"     ,
            "for"     ,
            "function",
            "global"  ,
            "if"      ,
            "otherwise",
            "persistent",
            "return"    ,
            "switch"    ,
            "try"       ,
            "while"                
        };

        private static int miSIZE_INT32    = 4;
        private static int miSIZE_INT16    = 2;
        private static int miSIZE_INT8     = 1;
        private static int miSIZE_UINT32   = 4;
        private static int miSIZE_UINT16   = 2;
        private static int miSIZE_UINT8    = 1;
        private static int miSIZE_DOUBLE   = 8;
        private static int miSIZE_SINGLE   = 4;
        private static int miSIZE_UTF32    = 4;
        private static int miSIZE_INT64    = 8;
        private static int miSIZE_UINT64   = 8;


        /* Matlab Array Types (Classes) */
        private static int mxUNKNOWN_CLASS = 0;
        private static int mxCELL_CLASS    = 1;
        private static int mxSTRUCT_CLASS  = 2;
        private static int mxOBJECT_CLASS  = 3;
        private static int mxCHAR_CLASS    = 4;
        private static int mxSPARSE_CLASS  = 5;
        private static int mxDOUBLE_CLASS  = 6;
        private static int mxSINGLE_CLASS  = 7;
        private static int mxINT8_CLASS    = 8;
        private static int mxUINT8_CLASS   = 9;
        private static int mxINT16_CLASS   = 10;
        private static int mxUINT16_CLASS  = 11;
        private static int mxINT32_CLASS   = 12;
        private static int mxUINT32_CLASS  = 13;
        private static int mxINT64_CLASS   = 14;
        private static int mxUINT64_CLASS  = 15;
        private static int mxFUNCTION_CLASS= 16;
        private static int mxOPAQUE_CLASS  = 17;
        
        private static int mtFLAG_COMPLEX  = 0x0800;
        private static int mtFLAG_GLOBAL   = 0x0400;
        private static int mtFLAG_LOGICAL  = 0x0200;
        private static int mtFLAG_TYPE     = 0xff;
        #endregion

        #region private helper
        /// <summary>
        /// size of single elements stored in Matlab's *.mat files
        /// </summary>
        /// <param name="type">one of Matlab's inner element types</param>
        /// <returns>size in bytes </returns>
        private static int sizeOf(MatFileType type)
        {
            switch ( type )
            {
                case MatFileType.miINT8:
                    return miSIZE_INT8;
                case MatFileType.miUINT8:
                    return miSIZE_UINT8;
                case MatFileType.miINT16:
                    return miSIZE_INT16;
                case MatFileType.miUINT16:
                    return miSIZE_UINT16;
                case MatFileType.miINT32:
                    return miSIZE_INT32;
                case MatFileType.miUINT32:
                    return miSIZE_UINT32;
                case MatFileType.miINT64:
                    return miSIZE_INT64; 
                case MatFileType.miUINT64:
                    return miSIZE_UINT64; 
                case MatFileType.miDOUBLE:
                    return miSIZE_DOUBLE;
                case MatFileType.miSINGLE:
                    return miSIZE_SINGLE;
                case MatFileType.miUTF32:
                    return miSIZE_UTF32;
                default:
                    throw new ILArgumentException("Invalid MatFileType specified: " + type.ToString()); 
            }
        }
        private ILBaseArray read_compressed(BinaryReader br, int len) {
            throw new NotImplementedException("Compressed matfile format is not supported yet! Use '-v6' option in Matlab to create the matfile!"); 
            //long startpos = br.BaseStream.Position; 
            ////ZOutputStream zstream = new ZOutputStream(br.BaseStream); 
            //GZipStream str = new GZipStream(br.BaseStream,CompressionMode.Decompress); 
            //BinaryReader bread = new BinaryReader(str);
            //MatFileType dataType = (MatFileType)Enum.Parse(typeof(MatFileType), bread.ReadInt32().ToString()); 
            //int elementLength = bread.ReadInt32();
            //ILBaseArray ret = null; 
            //if (dataType == MatFileType.miMATRIX) {
            //    ret = read_miMATRIX(bread);     
            //}
            //return ret; 
        }
        private void read_header(BinaryReader br) {
            headerFromFile = br.ReadBytes(116).ToString();
            // skip subsystem data 
            br.ReadBytes(8);
            // version 
            int version = br.ReadInt16();
            if (br.ReadChar() != 'I' || br.ReadByte() != 'M')
                throw new Exception("This file eventually was written on a machine, which is not compatible " +
                    " to this one due to an endian type issue!");
        }
        /// <summary>
        /// read ONE array (arbitrary dimensions/type) from MAT file 
        /// </summary>
        /// <param name="br">binary reader initialized and pointing to the beginning of the subarray element.</param>
        /// <returns>ILBaseArray of size and type originally stored into the mat file.</returns>
        private ILBaseArray read_miMATRIX(BinaryReader br) {
            long entryPositionInStream = br.BaseStream.Position;
            bool complex = false;
            bool logical = false;
            int mxClass = 0;
            int[] dimensions = new int[0];
            MatFileType storageType = MatFileType.miUNKNOWN;
            int nrElements = 1;
            string name;
            ILBaseArray ret;
            // read array flags 
            Int32 readInt = br.ReadInt32();
            if (readInt != 6)
                throw new Exception("found invalid datatype in array flag! currently only 'mxArray' types are supported!");
            readInt = br.ReadInt32();
            if (readInt != 8)
                throw new Exception("unexpected array flag length. expected: 8 /found: " + readInt);
            readInt = br.ReadInt32();
            complex = (readInt & mtFLAG_COMPLEX) != 0;
            logical = (readInt & mtFLAG_LOGICAL) != 0;
            mxClass = readInt & 0x00ff;
            // unknown
            br.ReadInt32();
            // Read dimensions array 
            readInt = br.ReadInt32();
            if (readInt != 5)
                throw new Exception("found invalid datatype in dimension flag!");
            readInt = br.ReadInt32();
            if (readInt < 2)
                throw new Exception("Invalid number of dimensions found: " + readInt);
            dimensions = new int[(int)readInt / 4];
            for (int i = 0; i < dimensions.Length; i++) {
                dimensions[i] = br.ReadInt32();
                nrElements *= dimensions[i];
            }
            // padidng if needed
            if ((dimensions.Length % 2) != 0)
                br.ReadInt32();
            // read Name - check for small data element format 
            readInt = br.ReadInt32();
            int nrSmallBytes = (int)((readInt & 0xffff0000) >> 16);
            if (nrSmallBytes != 0) {
                // process small element format 
                if ((readInt & 0xffff) != 1)
                    throw new Exception("Invalid datype for (compressed) name element found: " + (readInt & 0x00ff));
                StringBuilder nameBuild = new StringBuilder();
                nameBuild.Append(br.ReadChars(nrSmallBytes));
                // padding if needed
                while (nrSmallBytes < 4) {
                    br.ReadByte();
                    nrSmallBytes++;
                }
                name = nameBuild.ToString();
            } else {
                // process 'long' format 
                if (readInt != 1)
                    throw new Exception("Invalid datype for name element found: " + readInt);
                readInt = br.ReadInt32();
                StringBuilder nameBuild = new StringBuilder();
                nameBuild.Append(br.ReadChars(readInt));
                while (readInt % 8 != 0) {
                    readInt++;
                    br.ReadByte();
                }
                name = nameBuild.ToString();
            }
            // read data flags + check if small format
            readInt = br.ReadInt32();
            nrSmallBytes = (Int16)((readInt & 0xffff0000) >> 16);
            System.Array realData = null;
            System.Array imagData = null;
            int len;
            if (nrSmallBytes != 0 && nrElements <= 4) {
                // small data element format for scalars only! 
                // process small format -> real part 
                storageType = (MatFileType)(readInt & 0xffff);
                len = nrSmallBytes;
                readElementGeneric(br, storageType, out realData, ref len, 4);
                // padding
                //while (nrSmallBytes < 4 && br.BaseStream.Position < br.BaseStream.Length) {
                //    br.ReadByte(); 
                //    nrSmallBytes++; 
                //}
            } else {
                // read regular data : real part
                storageType = (MatFileType)Enum.Parse(typeof(MatFileType), readInt.ToString());
                len = br.ReadInt32();
                nrSmallBytes = len;
                readElementGeneric(br, storageType, out realData, ref len);
                // (padding is done in readElementGeneric)
            }

            // read imag part + check if small format
            if (complex) {
                readInt = br.ReadInt32();
                nrSmallBytes = (Int16)((readInt & 0xffff0000) >> 16);
                if (nrSmallBytes != 0 && nrElements <= 4) {
                    // process small format -> imag part 
                    storageType = (MatFileType)(readInt & 0xffff);
                    len = nrSmallBytes;
                    readElementGeneric(br, storageType, out imagData, ref len, 4);
                    // padding
                    //while (nrSmallBytes < 4 && br.BaseStream.Position < br.BaseStream.Length) {
                    //    br.ReadByte(); 
                    //    nrSmallBytes++; 
                    //}
                } else {
                    // read regular data : image part
                    storageType = (MatFileType)Enum.Parse(typeof(MatFileType), readInt.ToString()); ;
                    len = br.ReadInt32();
                    nrSmallBytes = len;
                    readElementGeneric(br, storageType, out imagData, ref len);
                    // (padding's done in readElementGeneric)
                }
            }
            // convert to original data type 
            if (complex) {
                if (mxClass == mxSINGLE_CLASS) {
                    fcomplex[] retArr = new fcomplex[nrElements];
                    float[] realPart = Convert2SingleArray(realData);
                    float[] imagPart = Convert2SingleArray(imagData);
                    for (int i = 0; i < nrElements; i++) {
                        retArr[i] = new fcomplex(realPart[i], imagPart[i]);
                    }
                    ret = new ILRetArray<fcomplex>(retArr, dimensions);
                } else {
                    complex[] retArr = new complex[nrElements];
                    double[] realPart = Convert2DoubleArray(realData);
                    double[] imagPart = Convert2DoubleArray(imagData);
                    for (int i = 0; i < nrElements; i++) {
                        retArr[i] = new complex(realPart[i], imagPart[i]);
                    }
                    ret = new ILRetArray<complex>(retArr, dimensions);
                }
            } else if (logical) {
                int numNonzero = 0;
                byte[] retArr = Convert2Logical(realData, out numNonzero);
                ret = new ILRetLogical(retArr, new ILSize(dimensions), numNonzero);
            } else {
                if (false) {

                    
                } else if (mxClass ==  mxDOUBLE_CLASS) {
                    
                    double[] dataArr =   Convert2DoubleArray(realData);
                    ret = new ILRetArray<double>(dataArr, dimensions);

#region HYCALPER AUTO GENERATED CODE

                   
                } else if (mxClass ==  mxUINT8_CLASS) {
                   
                    byte[] dataArr =  Convert2ByteArray(realData);
                    ret = new ILRetArray<byte>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxUINT64_CLASS) {
                   
                    UInt64[] dataArr =  Convert2UInt64Array(realData);
                    ret = new ILRetArray<UInt64>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxINT64_CLASS) {
                   
                    Int64[] dataArr =  Convert2Int64Array(realData);
                    ret = new ILRetArray<Int64>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxUINT32_CLASS) {
                   
                    UInt32[] dataArr =  Convert2UInt32Array(realData);
                    ret = new ILRetArray<UInt32>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxINT32_CLASS) {
                   
                    Int32[] dataArr =  Convert2Int32Array(realData);
                    ret = new ILRetArray<Int32>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxUINT16_CLASS) {
                   
                    UInt16[] dataArr =  Convert2UInt16Array(realData);
                    ret = new ILRetArray<UInt16>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxINT16_CLASS) {
                   
                    Int16[] dataArr =  Convert2Int16Array(realData);
                    ret = new ILRetArray<Int16>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxUINT8_CLASS) {
                   
                    char[] dataArr =  Convert2CharArray(realData);
                    ret = new ILRetArray<char>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxINT8_CLASS) {
                   
                    byte[] dataArr =  Convert2ByteArray(realData);
                    ret = new ILRetArray<byte>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxSINGLE_CLASS) {
                   
                    float[] dataArr =  Convert2SingleArray(realData);
                    ret = new ILRetArray<float>(dataArr, dimensions);
                   
                } else if (mxClass ==  mxCHAR_CLASS) {
                   
                    char[] dataArr =  Convert2CharArray(realData);
                    ret = new ILRetArray<char>(dataArr, dimensions);

#endregion HYCALPER AUTO GENERATED CODE
               } else {
                    throw new Exception("element data type is not supported");
                }
            }
            // set name       
            ret.Name = name;
            return ret;
        }


        

        private  double[]  Convert2DoubleArray(System.Array input) {
            // keep track of type matches ! No checks will be made! 
            
            double[] ret = new  double[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( double)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }

#region HYCALPER AUTO GENERATED CODE

       

        private  sbyte[]  Convert2SByteArray(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            sbyte[] ret = new  sbyte[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( sbyte)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
       

        private  byte[]  Convert2ByteArray(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            byte[] ret = new  byte[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( byte)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
       

        private  UInt64[]  Convert2UInt64Array(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            UInt64[] ret = new  UInt64[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt64)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
       

        private  UInt32[]  Convert2UInt32Array(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            UInt32[] ret = new  UInt32[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt32)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
       

        private  UInt16[]  Convert2UInt16Array(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            UInt16[] ret = new  UInt16[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( UInt16)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
       

        private  Int64[]  Convert2Int64Array(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            Int64[] ret = new  Int64[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int64)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
       

        private  Int32[]  Convert2Int32Array(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            Int32[] ret = new  Int32[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int32)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
       

        private  Int16[]  Convert2Int16Array(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            Int16[] ret = new  Int16[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( Int16)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
       

        private  char[]  Convert2CharArray(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            char[] ret = new  char[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( char)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
       

        private  float[]  Convert2SingleArray(System.Array input) {
            // keep track of type matches ! No checks will be made! 
           
            float[] ret = new  float[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(char)input.GetValue(i);
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(UInt64)input.GetValue(i);
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(UInt32)input.GetValue(i);
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(UInt16)input.GetValue(i);
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(Int64)input.GetValue(i);
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(Int32)input.GetValue(i);
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(Int16)input.GetValue(i);
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(float)input.GetValue(i);
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(double)input.GetValue(i);
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(byte)input.GetValue(i);
                    }
                    break;
                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        ret[i] = ( float)(sbyte)input.GetValue(i);
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }

#endregion HYCALPER AUTO GENERATED CODE

        private byte[] Convert2Logical(System.Array input, out int numNonzero) {
            // keep track of type matches ! No checks will be made! 
            numNonzero = 0;
            byte[] ret = new byte[input.Length];
            switch (input.GetType().Name.ToLower()) {
                case "char[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((char)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "uint64[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((UInt64)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "uint32[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((UInt32)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "uint16[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((UInt16)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "int64[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((Int64)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "int32[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((Int32)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "int16[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((Int16)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "single[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((float)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "double[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((double)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "byte[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((byte)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;

                case "sbyte[]":
                    for (int i = 0; i < input.Length; i++) {
                        if ((sbyte)input.GetValue(i) == 0) {
                            ret[i] = 0;
                        } else {
                            ret[i] = 1;
                            numNonzero++;
                        }
                    }
                    break;
                default:
                    throw new InvalidCastException("cannot convert from '" + input.GetType().Name + "'!");
            }
            return ret;
        }
        private static void readElementGeneric(BinaryReader br, MatFileType storageType, out System.Array realData, ref Int32 len) {
            readElementGeneric(br, storageType, out realData, ref len, 8);
        }
        /// <summary>
        /// read array of supported matlab data types 
        /// </summary>
        /// <param name="br">binary reader, opened and correctly positioned</param>
        /// <param name="storageType">actual storage type</param>
        /// <param name="realData">output: on return, the array read</param>
        /// <param name="len">input: number of bytes to read, on return: number of elements in array</param>
        /// <param name="paddBytes">padding border, the stream will be read to the next border of length 'paddBytes'.</param>
        private static void readElementGeneric(BinaryReader br, MatFileType storageType, out System.Array realData, ref Int32 len, int paddBytes) {
            Int32 readInt = len;
            switch (storageType) {
                case MatFileType.miINT8:
                    realData = System.Array.CreateInstance(typeof(sbyte), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadSByte(), i);
                    }
                    while (len % paddBytes != 0) {
                        br.ReadByte();
                        len++;
                    }
                    break;
                case MatFileType.miUINT8:
                    realData = System.Array.CreateInstance(typeof(byte), readInt / sizeOf(storageType));
                    len = realData.Length;
                    realData = br.ReadBytes(len);
                    while (len % paddBytes != 0) {
                        br.ReadByte();
                        len++;
                    }
                    break;
                case MatFileType.miINT16:
                    realData = System.Array.CreateInstance(typeof(short), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadInt16(), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadInt16();
                    }
                    break;
                case MatFileType.miUINT16:
                    realData = System.Array.CreateInstance(typeof(UInt16), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadUInt16(), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadUInt16();
                    }
                    break;
                case MatFileType.miINT32:
                    realData = System.Array.CreateInstance(typeof(Int32), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadInt32(), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadInt32();
                    }
                    break;
                case MatFileType.miUINT32:
                    realData = System.Array.CreateInstance(typeof(UInt32), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadUInt32(), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadUInt32();
                    }
                    break;
                case MatFileType.miSINGLE:
                    realData = System.Array.CreateInstance(typeof(float), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadSingle(), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadSingle();
                    }
                    break;
                case MatFileType.miDOUBLE:
                    realData = System.Array.CreateInstance(typeof(double), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadDouble(), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadDouble();
                    }
                    break;
                case MatFileType.miINT64:
                    realData = System.Array.CreateInstance(typeof(Int64), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadInt64(), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadInt64();
                    }
                    break;
                case MatFileType.miUINT64:
                    realData = System.Array.CreateInstance(typeof(UInt64), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadUInt64(), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadInt64();
                    }
                    break;
                case MatFileType.miMATRIX:
                    throw new NotSupportedException("matrix data type not expected as inner datatype!");
                case MatFileType.miCOMPRESSED:
                    throw new NotSupportedException("Compressed matrix are not supported (yet)! ");
                case MatFileType.miUTF8:
                    realData = System.Array.CreateInstance(typeof(UTF8Encoding), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadUInt16(), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadInt16();
                    }
                    break;
                case MatFileType.miUTF16:
                    throw new NotSupportedException("UTF16 data type not supported (yet)!");
                case MatFileType.miUTF32:
                    realData = System.Array.CreateInstance(typeof(UTF32Encoding), readInt / sizeOf(storageType));
                    len = realData.Length;
                    for (int i = 0; i < len; i++) {
                        realData.SetValue(br.ReadChars(2), i);
                    }
                    while ((len * sizeOf(storageType) % paddBytes) != 0) {
                        len++;
                        br.ReadChars(2);
                    }
                    break;
                default:
                    throw new Exception("Unknown element data type found! Cancelling...");
            }
        }
        private void write(BinaryWriter fileout) {
            try {
                // write MAT-header
                StringBuilder headerLine = new StringBuilder("vers. 5.0 MAT-FILE Creator: ILNumerics, 2011");
                while (headerLine.Length < 123)
                    headerLine.Append(' ');
                fileout.Write(headerLine.ToString());
                fileout.Write((short)0x0100);
                fileout.Write((short)0x4d49);
                int tmpInt = 0;
                foreach (string name in Keys) {
                    ILBaseArray arr = this[name];
                    using (ILScope.Enter(arr)) {
                        // determine overall length
                        int[] dimensionSubelement = createDimensionSubelement(arr);
                        byte[] nameElememData; int nameElemType;
                        createNameSubelement(name, out nameElemType, out nameElememData);
                        // write subarray header               
                        // mxarray subelement type
                        fileout.Write((int)MatFileType.miMATRIX);
                        // determine length of single array element
                        int elemLen = getElementLength(arr);
                        //System.Diagnostics.Debug.Assert(arr.IsMatrix,"TODO: n-dim. arrays are not implemented yet!"); 
                        // overall length of subarray container 
                        int allLength = 16 // array flags 
                                      + dimensionSubelement.Length * 4   // dimension element
                                      + nameElememData.Length + 8; // name element 
                        int dataSubelemLen = elemLen * arr.Size.NumberOfElements + 8;
                        // recognize padding! 
                        if (dataSubelemLen % 8 != 0)
                            dataSubelemLen += (8 - ((elemLen * arr.Size.NumberOfElements) % 8));
                        allLength += dataSubelemLen;
                        if (arr.IsComplex)
                            allLength += dataSubelemLen;
                        fileout.Write(allLength);
                        // subelement: array flags 
                        // miUInt32 , length: 8
                        fileout.Write((int)MatFileType.miUINT32);
                        fileout.Write(8);
                        // write array flags  
                        int flag = getElementClass(arr);
                        if (arr.IsComplex)
                            flag |= mtFLAG_COMPLEX;
                        if (arr is ILBaseLogical)
                            flag |= mtFLAG_LOGICAL;
                        fileout.Write(flag);
                        fileout.Write(0);      // this is later used for sparse arrays 
                        // write dimensions tag     
                        for (int i = 0; i < dimensionSubelement.Length; i++)
                            fileout.Write(dimensionSubelement[i]);
                        // write array name      
                        fileout.Write((int)MatFileType.miINT8);
                        fileout.Write(name.Length);
                        fileout.Write(nameElememData);
                        // write matrix elements     
                        allLength = arr.Size.NumberOfElements;
                        if (false) {

                        } else if (arr is ILDenseArray<double>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miDOUBLE);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<double> tmp = (arr as ILDenseArray<double>);
                            
                            double[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( //
                                    arrArray[i]  // 
                                );
                            }

#region HYCALPER AUTO GENERATED CODE

                        } else if (arr is ILDenseArray<fcomplex>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miSINGLE);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<fcomplex> tmp = (arr as ILDenseArray<fcomplex>);
                           
                            fcomplex[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]  .real 
                                );
                            }
                        } else if (arr is ILDenseArray<complex>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miDOUBLE);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<complex> tmp = (arr as ILDenseArray<complex>);
                           
                            complex[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]  .real 
                                );
                            }
                        } else if (arr is ILDenseArray<UInt32>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miUINT32);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<UInt32> tmp = (arr as ILDenseArray<UInt32>);
                           
                            UInt32[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]   
                                );
                            }
                        } else if (arr is ILDenseArray<UInt16>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miUINT16);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<UInt16> tmp = (arr as ILDenseArray<UInt16>);
                           
                            UInt16[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]   
                                );
                            }
                        } else if (arr is ILDenseArray<Int32>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miINT32);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<Int32> tmp = (arr as ILDenseArray<Int32>);
                           
                            Int32[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]   
                                );
                            }
                        } else if (arr is ILDenseArray<Int16>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miINT16);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<Int16> tmp = (arr as ILDenseArray<Int16>);
                           
                            Int16[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]   
                                );
                            }
                        } else if (arr is ILDenseArray<float>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miSINGLE);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<float> tmp = (arr as ILDenseArray<float>);
                           
                            float[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]   
                                );
                            }
                        } else if (arr is ILDenseArray<char>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miUINT16);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<char> tmp = (arr as ILDenseArray<char>);
                           
                            char[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( (UInt16)
                                    arrArray[i]   
                                );
                            }
                        } else if (arr is ILDenseArray<byte>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miUINT8);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<byte> tmp = (arr as ILDenseArray<byte>);
                           
                            byte[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]   
                                );
                            }
                        } else if (arr is ILDenseArray<UInt64>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miUINT64);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<UInt64> tmp = (arr as ILDenseArray<UInt64>);
                           
                            UInt64[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]   
                                );
                            }
                        } else if (arr is ILDenseArray<Int64>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miINT64);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<Int64> tmp = (arr as ILDenseArray<Int64>);
                           
                            Int64[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]   
                                );
                            }
                        } else if (arr is ILDenseArray<sbyte>) {
                            // header of array subdata element      
                            fileout.Write((int) MatFileType.miINT8);
                            fileout.Write(allLength * elemLen);
                            ILDenseArray<sbyte> tmp = (arr as ILDenseArray<sbyte>);
                           
                            sbyte[] arrArray = tmp.Storage.GetArrayForRead();
                            for (int i = 0; i < arr.Size.NumberOfElements; i++) {
                                fileout.Write( 
                                    arrArray[i]   
                                );
                            }

#endregion HYCALPER AUTO GENERATED CODE
                       } else {
                            throw new FormatException("The format of the array was not known!");
                        }
                        // pad to 8 byte border 
                        tmpInt = allLength * elemLen;
                        byte dummy = 0;
                        while (tmpInt % 8 != 0) {
                            fileout.Write(dummy);
                            tmpInt++;
                        }
                        #region imaginary part
                        if (arr.IsComplex) {
                            if (arr is ILDenseArray<complex>) {
                                ILDenseArray<complex> tmp = (arr as ILDenseArray<complex>);
                                // header of array subdata element      
                                fileout.Write((int)MatFileType.miDOUBLE);
                                fileout.Write(allLength * elemLen);
                                complex[] arrArray = tmp.Storage.GetArrayForRead();
                                for (int i = 0; i < arrArray.Length; i++) {
                                    fileout.Write(arrArray[i].imag
                                    );
                                }
                            } else if (arr is ILDenseArray<fcomplex>) {
                                fileout.Write((int)MatFileType.miSINGLE);
                                fileout.Write(allLength * elemLen);
                                ILDenseArray<fcomplex> tmp = (arr as ILDenseArray<fcomplex>);
                                fcomplex[] arrArray = tmp.Storage.GetArrayForRead();
                                for (int i = 0; i < arrArray.Length; i++) {
                                    fileout.Write(arrArray[i].imag
                                    );
                                }
                            }
                            // pad to 8 byte border 
                            tmpInt = allLength * elemLen;
                            dummy = 0;
                            while (tmpInt % 8 != 0) {
                                fileout.Write(dummy);
                                tmpInt++;
                            }
                        }
                        #endregion
                    }
                }
                fileout.Close();
            } catch (Exception e) {
                throw e;
            }
        }
        /// <summary>
        /// create name subelement for Matfile storage - padded to 8 byte border
        /// </summary>
        /// <param name="arrName">name property</param>
        /// <param name="type">will be 'miINT8' on return</param>
        /// <param name="data">return data array </param>
        private void createNameSubelement(string arrName, out int type, out byte[] data) {
            int len = arrName.Length;
            if (len % 8 != 0)
                len += (8 - len % 8);
            data = new byte[len];
            for (int i = 0; i < arrName.Length; i++) {
                data[i] = (byte)arrName[i];
            }
            type = (int)MatFileType.miINT8;
            return;
        }
        private int[] createDimensionSubelement(ILBaseArray arr) {
            int[] ret;
            int LengthInt;
            // determine length of dimensions array byte (padding to full 8 byte border)
            if (arr.Size.NumberOfDimensions % 2 == 0) {
                LengthInt = (arr.Size.NumberOfDimensions + 2);
            } else {
                // must pad to 8 byte border
                LengthInt = (arr.Size.NumberOfDimensions + 3);
            }
            ret = new int[LengthInt];
            ret[0] = (int)MatFileType.miINT32;
            ret[1] = arr.Size.NumberOfDimensions * 4;
            for (int i = 0; i < arr.Size.NumberOfDimensions; i++) {
                ret[i + 2] = arr.Size[i];
            }
            return ret;
        }
        private int IndexOf(string name) {
            for (int i = 0; i < Count; i++) {
                if (m_data.GetValue<string>(i, 1, 0) == name)
                    return i;
            }
            return -1;
        }
        private string findName(ILBaseArray value) {
            string elementType = string.Empty;
            Type type = value.GetType();
            if (type.GetGenericArguments() != null && type.GetGenericArguments().Length > 0) {
                elementType = type.GetGenericArguments()[0].Name;
            } else {
                elementType = type.Name;
            }
            string dimString = String.Join("x", value.Size.ToIntArray());
            string baseName = String.Format("{0}_{1}", elementType, dimString);

            // make the new name unique
            string tempName = baseName;
            int postFix = 0;
            while (IndexOf(tempName) >= 0) {
                tempName = baseName + "_" + postFix.ToString();
                postFix++;
            }
            return tempName;
        }
        /// <summary>
        /// get mat file array class type corresponding to this arra element type
        /// </summary>
        /// <param name="arr">arra with generic system type or complex/fcomplex</param>
        /// <returns>mat file array class type code (int value)</returns>
        private static int getElementClass(ILBaseArray arr) {
            Type arrType = arr.GetType();
            if (arr is ILRetLogical || arr is ILLogical)
                return (int)MatFileArrayClass.mxINT8_CLASS;
            if (!arrType.IsGenericType)
                throw new ILInvalidOperationException("The array type to be saved is not applicable!");
            string innerType = arrType.GetGenericArguments()[0].Name;
            switch (innerType) {
                case "Double":
                    return (int)MatFileArrayClass.mxDOUBLE_CLASS;
                case "Single":
                    return (int)MatFileArrayClass.mxSINGLE_CLASS;
                case "Int16":
                    return (int)MatFileArrayClass.mxINT16_CLASS;
                case "Int32":
                    return (int)MatFileArrayClass.mxINT32_CLASS;
                case "Int64":
                    return (int)MatFileArrayClass.mxINT64_CLASS;
                case "UInt16":
                    return (int)MatFileArrayClass.mxUINT16_CLASS;
                case "UInt32":
                    return (int)MatFileArrayClass.mxUINT32_CLASS;
                case "UInt64":
                    return (int)MatFileArrayClass.mxUINT64_CLASS;
                case "complex":
                    return (int)MatFileArrayClass.mxDOUBLE_CLASS;
                case "fcomplex":
                    return (int)MatFileArrayClass.mxSINGLE_CLASS;
                case "Byte":
                    return (int)MatFileArrayClass.mxUINT8_CLASS;
                case "Char":
                    return (int)MatFileArrayClass.mxCHAR_CLASS;
                default:
                    throw new ILInvalidOperationException("Arrays of inner element type: '" + innerType + "' can not be written as Matfile!");
            }
        }
        /// <summary>
        /// get storage length of inner array elements 
        /// </summary>
        /// <param name="arr">base array in question</param>
        /// <returns>storage length in bytes</returns>
        private static int getElementLength(ILBaseArray arr) {
            Type arrType = arr.GetType();
            string innerType;
            if (arr is ILRetLogical || arr is ILLogical)
                return 1;
            if (!arrType.IsGenericType)
                throw new ILInvalidOperationException("The array type to be saved is not applicable!");
            innerType = arrType.GetGenericArguments()[0].Name;
            switch (innerType) {
                case "Double":
                    return miSIZE_DOUBLE;
                case "Single":
                    return miSIZE_SINGLE;
                case "Int16":
                    return miSIZE_INT16;
                case "Int32":
                    return miSIZE_INT32;
                case "Int64":
                    return miSIZE_INT64;
                case "UInt16":
                    return miSIZE_UINT16;
                case "UInt32":
                    return miSIZE_UINT32;
                case "UInt64":
                    return miSIZE_UINT64;
                case "complex":
                    return miSIZE_DOUBLE;
                case "fcomplex":
                    return miSIZE_SINGLE;
                case "Byte":
                    return miSIZE_INT8;
                case "Char":
                    return miSIZE_UINT16;
                default:
                    throw new ILInvalidOperationException("Arrays of inner element type: '" + innerType + "' can not be written as Matfile!");
            }
        }
        #endregion

        #region public interface
        /// <summary>
        /// Convert MatFileType enumeration member to string representation
        /// </summary>
        /// <param name="type">MatFileType enumeration member</param>
        /// <returns>String representing the Matlab's inner element type</returns>
        /// <remarks>This function is obsolete. You may directly use the enumeration's functionality instead.</remarks>
        public static String typeToString(MatFileType type)
        {
            String s;
            switch (type)
            {
                case MatFileType.miUNKNOWN:
                    s = "unknown";
                    break;
                case MatFileType.miINT8:
                    s = "int8";
                    break;
                case MatFileType.miUINT8:
                    s = "uint8";
                    break;
                case MatFileType.miINT16:
                    s = "int16";
                    break;
                case MatFileType.miUINT16:
                    s = "uint16";
                    break;
                case MatFileType.miINT32:
                    s = "int32";
                    break;
                case MatFileType.miUINT32:
                    s = "uint32";
                    break;
                case MatFileType.miSINGLE:
                    s = "single";
                    break;
                case MatFileType.miDOUBLE:
                    s = "double";
                    break;
                case MatFileType.miINT64:
                    s = "int64";
                    break;
                case MatFileType.miUINT64:
                    s = "uint64";
                    break;
                case MatFileType.miMATRIX:
                    s = "matrix";
                    break;
                case MatFileType.miCOMPRESSED:
                    s = "compressed";
                    break;
                case MatFileType.miUTF8:
                    s = "uft8";
                    break;
                case MatFileType.miUTF16:
                    s = "utf16";
                    break;
                case MatFileType.miUTF32:
                    s = "utf32";
                    break;
                default:
                    s = "unknown";
                    break; 
            }
            return s;
        }

        /// <summary>
        /// Add array to collection of arrays in this ILMatFile container
        /// </summary>
        /// <param name="A">Array to be added to ILMatFile</param>
        /// <returns>String used to identify the array in the collection of arrays</returns>
        /// <remarks><para>The internal <c>Name</c> property of array given will be used as identification key.</para>
        /// <para>Note, the test if elements of A are supported by MatFile specification is done if the MatFile is to be written to stream ('write').</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILInvalidOperationException">If the internal name of A does not fullfill the restrictions given by Matlab.</exception>
        public string AddArray(ILBaseArray A, string key = "") {
            using (ILScope.Enter(A)) {
                string name = key;
                if (String.IsNullOrEmpty(name)) {
                    name = A.Name;
                }
                if (String.IsNullOrEmpty(name)) {
                    name = findName(A);
                }
                this[name] = A;
                return name;
            }
        }

        /// <summary>
        /// Path to mat file, if this object was created from an existing mat file.
        /// </summary>
        public string Filelocation {
            get {
                return m_filename;
            }
        }
        /// <summary>
        /// List all key names currently stored with arrays 
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
        public IList<string> Keys {
            get {
                List<string> ret = new List<string>();
                for (int i = 0; i < m_data.S[0]; i++) {
                    ret.Add(m_data.GetValue<string>(i, 1, 0));
                }
                return ret;
            }
        }
        /// <summary>
        /// Retrieve a cell with all arrays stored in the mat file
        /// </summary>
        /// <remarks>The cell returned will be clone of the arrays stored in the mat file. Altering any cell 
        /// elements will leave the arrays in the matfile (class/memory object) untouched.
        /// <para>The cell returned will be of size [n,2], where n is the number of arrays contained. The 
        /// first row saved the arrays, the second row containes scalar string arrays with the name of 
        /// the array in the corresponding row.</para>
        /// </remarks>
        public ILRetCell Arrays {
            get { return m_data.C; }
            protected set { m_data.a = value; }
        }
        /// <summary>
        /// Add or replace named array to this MatFile
        /// </summary>
        /// <param name="name">The assigned name of the array</param>
        /// <remarks><para>The get acces is provided for compatibility reasons only. It may be removed in a future version! Use one of the typed retrieval functions <see cref="ILNumerics.ILMatFile.GetArray{T}(string)"/> or <see cref="ILNumerics.ILMatFile.GetArray{T}(ILBaseArray)"/> instead.</para>
        /// <para>For get access the name must exist as key in the container. Use the MatFile.Keys property to get a list of all names if needed</para>
        /// <para>For set access, the name given must not be null or empty. It cannot be one of the <see cref="ReservedKeywords">ReservedKeywords</see>.
        /// If the name allready exist in the collection as name, the array currently assigned to it will be replaced. If the value is null, the current array will be removed from the list. If the name does 
        /// not already exist, the new array will be added and assigned to this name.</para>
        /// <para>Restrictions on array names: Matlab allowes variables to have names of maximum length 63. Therefore, if the 
        /// name given was larger than 63, it will be abbreviated. Names must start with a letter and contain only digits, (ASCII) letters or underscores '_'.</para></remarks>
        /// <exception cref="ILArgumentException">If the name does not fullfill the restrictions given from Matlab</exception>
        public ILBaseArray this[string name] {
            get {
                return m_data.GetBaseArray(IndexOf(name), 0); 
            }
            set {
                using (ILScope.Enter(value)) {
                    #region test if name is valid
                    if (String.IsNullOrEmpty(name)) {
                        name = findName(value);
                    }
                    foreach (string nono in ReservedKeywords) {
                        if (name == nono)
                            throw new ILArgumentException("MatFile: the name " + nono + " is a reserved keyword in Matlab and may not be used as array name!");
                    }
                    if (name.Length > 63)
                        name = name.Substring(0, 63);
                    if (!Char.IsLetter(name[0]))
                        throw new ILArgumentException("MatFile: the name must start with a letter!");
                    int i;
                    for (i = 1; i < name.Length; i++) {
                        char c = name[i];
                        if (!Char.IsLetter(c) && !Char.IsDigit(c) && c != '_')
                            throw new ILArgumentException("MatFile: variable names must contain letters, digits or underscores only!");
                    }
                    #endregion
                    i = IndexOf(name);
                    if (i >= 0) {
                        // alter array
                        if (object.Equals(value, null))
                            // remove from 
                            m_data[i, ILMath.full] = null;
                        else
                            m_data.SetValue(value, i, 0);
                    } else {
                        if (object.Equals(value, null))
                            return;
                        // add array 
                        m_data.SetValue(name, m_data.S[0], 1);
                        m_data.SetValue(value, m_data.S[0] - 1, 0);
                    }
                }
            }
        }
        /// <summary>
        /// Number of arrays in the mat file container
        /// </summary>
        public int Count { get { return m_data.S[0]; } }
        /// <summary>
        /// Retrieve array by name
        /// </summary>
        /// <typeparam name="T">Expected type of the array</typeparam>
        /// <param name="name">Name of the array to retrieve</param>
        /// <returns>A clone of the array found or null, if no array with the given name exists</returns>
        public ILRetArray<T> GetArray<T>(string name) {
            int ind = IndexOf(name);
            if (ind >= 0)
                return m_data.GetArray<T>(ind, 0);
            throw new ILArgumentException("no array with name '" + name + "' found"); 
        }
        /// <summary>
        /// Retrieve array by index
        /// </summary>
        /// <typeparam name="T">Expected type of the array</typeparam>
        /// <param name="index">Index of the array</param>
        /// <returns>A clone of the array found or null, if no array at the given index exists</returns>
        public ILRetArray<T> GetArray<T>(ILBaseArray index) {
            using (ILScope.Enter(index)) {
                if (!index.IsScalar || !index.IsNumeric)
                    throw new ILArgumentException("index argument must be a numeric scalar");
                int ind = ILMath.toint32(index).GetValue(0);
                if (ind >= m_data.S[0])
                    throw new ILArgumentException("index argument must be in range 0..Count-1");
                return m_data.GetArray<T>(ind, 0);
            }
        }
        /// <summary>
        /// Write this mat file into a binary stream
        /// </summary>
        /// <param name="stream">Stream to receive data. This will commonly be a FileStream object.</param>
        /// <remarks>
        /// <para>This method writes the full content of the current mat file into a binary stream. The file 
        /// afterwards is suitable to be read again by ILNumerics.MatFile classes or by compatible *.mat file 
        /// readers - including Matlab, e.g.</para>
        /// <example><code>
        /// MatFile m = new MatFile(myarrays); 
        /// using (Stream s = new FileStream("test.mat",FileMode.Create)) {
        ///     m.Write(s);
        /// }
        /// </code></example></remarks>
        public void Write(Stream stream) {
            using (BinaryWriter fileout = new BinaryWriter(stream)) {
                write(fileout);
            }
        }
        /// <summary>
        /// Write all arrays to *.mat file
        /// </summary>
        /// <param name="filename">Filename of the file to write the mat file to</param>
        /// <remarks>
        /// <para>The method writes the full content of the matfile to the file specified. If the filename 
        /// points to a file which already exists, that file will be overwritten. Otherwise a new file will
        /// be created. </para>
        /// <para>The file will be suitable for reading by ILNumerics.MatFile classes or by compatible *.mat file 
        /// readers - including e.g. matlab</para>
        /// </remarks>
        public void Write(string filename) {
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate)) {
                Write(fs);
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Create MatFile object from existing mat file 
        /// </summary>
        /// <param name="file2open">Path to Matlab mat file to open</param>
        /// <remarks>Curently mat files up to Matlab version 6.5 are supported. Compressed mat file content is not supported yet.</remarks>
        public ILMatFile(string file2open) {

            using (ILScope.Enter()) {
                m_filename = file2open;
                using (FileStream fs = File.OpenRead(file2open)) {
                    BinaryReader br = new BinaryReader(fs);
                    read_header(br);
                    // read elements 
                    ILCell targetCell = ILMath.cell(new ILSize(0, 2), null);
                    while (br.BaseStream.Position < br.BaseStream.Length - 7) {
                        MatFileType dataType = (MatFileType)Enum.Parse(typeof(MatFileType), br.ReadInt32().ToString());
                        // the length of this chunk may be used for error checking, but....
                        int len = br.ReadInt32();
                        switch (dataType) {
                            case MatFileType.miCOMPRESSED:
                                ILBaseArray mat = read_compressed(br, len);
                                targetCell.SetValue(mat.Name, targetCell.S[0], 1); 
                                targetCell.SetValue(mat, targetCell.S[0] - 1, 0);
                                break;
                            case MatFileType.miMATRIX:
                                mat = read_miMATRIX(br);
                                targetCell.SetValue(mat.Name, targetCell.S[0], 1); 
                                targetCell.SetValue(mat, targetCell.S[0] - 1, 0);
                                break;
                            default:
                                // ignore all other elements, not supported yet
                                break;
                        }
                    }
                    m_data.a = targetCell;
                    br.Close();
                }
            }
        }
        /// <summary>
        /// Create MatFile object from ILBaseArray
        /// </summary>
        /// <param name="input">ILBaseArray of arbitrary size/type</param>
        /// <exception cref="ArgumentNullException">If input array was null or one of the names in the input arrays does not fullfill the restrictions made from Matlab</exception>
        public ILMatFile(params ILBaseArray[] input) {
            if (object.Equals(input,null))
                throw new ArgumentNullException ("input array may not be null!"); 
            //m_data = new ILCell(input.Length,2);
            for (int i=0; i < input.Length; i++) {
                if (String.IsNullOrWhiteSpace(input[i].Name)) {
                    this["ILArray" + i.ToString()] = input[i]; 
                } else {
                    this[input[i].Name] = input[i]; 
                }
            }
        }
        /// <summary>
        /// Create an empty MatFile object
        /// </summary>
        public ILMatFile() {
            m_data = new ILCell(0,2);
        }
        #endregion


        #region IDisposable Members
        /// <summary>
        /// Dispose all arrays of the matfile object
        /// </summary>
        /// <remarks>Calling dispose should be the last action for a matfile object. It is recommended to 
        /// utilize the matfile class in using blocks (C#) only.</remarks>
        public void Dispose() {
            if (!object.Equals(m_data,null)) 
                m_data.Dispose(); 
        }

        #endregion
    }
}