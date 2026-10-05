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
using System.Configuration;
using ILNumerics.Misc; 

namespace ILNumerics {

    /// <summary>
    /// The class provides static setting properties to control the behaviour of ILNumerics, see <a href="http://ilnumerics.net/$Configuration.html">Configuration</a> in the online documentation
    /// </summary>
    public class Settings {

        static Settings() {
            // Load the static cache values from the Default instance
            // This enables the user to change default values via app.config 
            // and still provides a fast setting provider for internal functions.
            LoadDefaults();
        }

        #region static local attributes
        // local settings caches 
        internal static int s_minimumQuicksortLength;
        internal static int s_maxNumberThreads;
        internal static bool s_maxNumberThreadsConfigured; 
        internal static int s_minParallelElement3Count;   // int.MaxValue; // 
        internal static int s_minParallelElement2Count;  // int.MaxValue; // 
        internal static int s_minParallelElement1Count; // int.MaxValue; // 
        internal static int s_minElementLength4SystemArrayCopy;
        internal static int s_maxSafeQuicksortRecursionDepth;
        internal static int s_memoryPoolProfileMinLength;
        internal static int s_memoryPoolProfileMaxLength;
        internal static bool s_measurePerformanceAtRuntime; 
        internal static string s_memoryPoolProfileFileName; //"ILNumerics.MemoryPool.Profiler.txt";
        internal static string s_nativeDependenciesAbsolutePath; 
        internal static bool s_useThreadAffinity;
        internal static bool s_isHosted;
        internal static bool s_allowInArrayAssignments; 
        internal static bool s_createRowVectorsByDefault;
        internal static int s_managedMultiplyBlockSize;
        internal static int s_managedMultiplyMaxElementSize;
        internal static bool s_OpenGL31_FIX_GL_CLIPVERTEX;
        internal static bool s_showMessageBoxOnGDIFallback;
        internal static LogicalConversionMode s_logicalArrayToBoolConversion = LogicalConversionMode.NonScalarThrowsException;


        #endregion

        #region static global caches
        /// <summary>
        /// Gets or sets a flag indicating if a message box is shown to the user, once the rendering driver falls back to GDI; default: true
        /// </summary>
        /// <remarks><para>If at runtime unrecoverable errors prevent the driver from rendering successfully, a message box will inform the user 
        /// about the issue before the driver falls backt to GDI rendering. Setting this flag to false prevents the message box from showing.</para>
        /// <para>Potential problems with accelerated drivers commonly include: 
        /// <list type="bullet">
        /// <item>Outdated drivers: for OpenGL GPU rendering, currently OpenGL version 3.1 or higher is required.</item>
        /// <item>Buggy drivers: especially 'first day support' drivers often show bugs with newer OpenGL functionality</item>
        /// <item>Deactivated drivers: mobile devices sometimes deactivate GPU rendering for energy savings</item>
        /// <item>Virtual machines and remote desktop connections do often provide limited access to graphics hardware only. This may leads to OpenGL contexts of 
        /// lower versions than neccessary and/or to the use of software implementations for OpenGL which may cause common problems with ILNumerics drawing controls. </item>
        /// </list>
        /// </para>
        /// </remarks>
        public static bool ShowMessageBoxOnGDIFallback {
            get {
                return s_showMessageBoxOnGDIFallback;
            }
            set {
                s_showMessageBoxOnGDIFallback = value;
            }
        }
        /// <summary>
        /// Disable automatic DesignMode determination (true). By default (false), ILNumerics determines DesignMode automatically.  
        /// </summary>
        /// <remarks>
        /// <para>By default IsHosted is false. This lets ILNumerics assume, that the library runs in the context of a regular application. At design time, 
        /// limited functionality is provided by the drawing controls only. OpenGL contexts are not created and replaced by GDI controls. At desing 
        /// time, no scene is displayed. At runtime, the drawing controls are functioning in the regular way.</para>
        /// <para>The determination of design mode is done by taking all references of the entry assembly into account. If the entry assembly references the ILNumerics dll, 
        /// no design mode is assumed. If ILNumerics is not among the referenced libraries of the entry assembly, ILNumerics is expected to be executed in the context of a desinger.</para>
        /// <para>For situations, where ILNumerics is intended to be loaded dynamically at runtim - without any reference to it existing in the entry assembly - this switch can be set to true. 
        /// This will cause ILNumerics to use another method to determine if it was loaded in a designer context: the entry assemblys name is compared to a blacklist of common designer executables. 
        /// Among them are devenv.exe and monodevelop.exe.</para>
        /// <para>The list of ... TODO: warum nciht einfach: "true - returns IsDesignMode = false" ? </para></remarks>
        public static bool IsHosted {
            get {
                return s_isHosted;
            }
            set {
                s_isHosted = value;
            }
        }
        /// <summary>
        /// Performance switch, dis-/allow direct assignments to input parameters - brings more efficient 
        /// memory management, default: true (safer, less efficient)
        /// </summary>
        /// <remarks>
        /// <para>If this switch is set to 'false', you promise not to assign any values to input parameters 
        /// of type <c>ILInArray</c>, <c>ILInCell</c> or <c>ILInLogical</c>. This allows ILNumerics for more efficent 
        /// memory management, decreases the overall memory footprint of the application and enables certain 
        /// array operations to be automatically computed in-place. Depending on the specific algorithm
        /// the performance profit may range from 1% up to even about 30%.</para>
        /// This switch should be set for the whole application globally. It is not recommended to change the state of 
        /// this switch once the application runs.
        /// <para>Since this switch targets the full application, all functions and modules involved must also follow 
        /// that contract! For all builtin functions of ILNumerics, compliance with this rule is garanteed. This means, if you are 
        /// not using any 3rd party algorithms and are able to make sure your own functions follow that scheme as well, it is safe 
        /// to set this switch to <c>false</c> and profit from faster execution times.</para>
        /// <para>See the <a href="http://ilnumerics.net/$PerfMemoryOpt.html">Optimizing Performance</a> article in the online documentation.</para></remarks>
        public static bool AllowInArrayAssignments {
            get {
                return s_allowInArrayAssignments; 
            }
            set {
                s_allowInArrayAssignments = value; 
            }
        }
        /// <summary>
        /// Work around OpenGL driver issues regarding GL_ClipDistance; set to true on problems on older GL 3.1 hardware (like GT 3XXM series); default: auto
        /// </summary>
        /// <remarks>On GL version 3.1 contexts, if the linking of affected shaders fails, the switch is automatically set to true.</remarks>
        public static bool OpenGL31_FIX_GL_CLIPVERTEX {
            get {
                return s_OpenGL31_FIX_GL_CLIPVERTEX; 
            }
            set {
                s_OpenGL31_FIX_GL_CLIPVERTEX = value; 
            }
        }

        /// <summary>
        /// Control layout of vectors when not specified explicitly.
        /// <list type="bullet">
        /// <item>
        /// <term>true</term>
        /// <description>When a vector is created without exlicitely specifying its shape, create a row vector</description>
        /// </item>
        /// <item>
        /// <term>false (default)</term>
        /// <description>When a vector is created without exlicitely specifying its shape, create a column vector</description>
        /// </item></list>
        /// </summary>
        /// <remarks>This setting affects the way ILNumerics handles the default shape for vectors created. One example is <see cref="ILNumerics.ILMath.array{T}( T[])"/>, 
        /// where only the number of elements is given - but no size is specified. By default, ILNumerics will interpret the elements as targeting the <b>first</b> 
        /// dimension, ie. dimension #0. This will create a column vector. Setting this switch to 'true' will make ILNumerics to create a row vector in 
        /// such situations instead.</remarks>
        public static bool CreateRowVectorsByDefault {
            get {
                return s_createRowVectorsByDefault; 
            }
            set {
                s_createRowVectorsByDefault = value; 
            }
        }
        /// <summary>
        /// Upper limit of the range of array length to gather profiler information for
        /// </summary>
        public static int MemoryPoolProfileMaxLength {
            get {
                return s_memoryPoolProfileMaxLength;
            }
            set {
                s_memoryPoolProfileMaxLength = value;
            }
        }
        /// <summary>
        /// Lower limit of the range of array length to gather profiler information for
        /// </summary>
        public static int MemoryPoolProfileMinLength {
            get {
                return s_memoryPoolProfileMinLength;
            }
            set {
                s_memoryPoolProfileMinLength = value;
            }
        }
        /// <summary>
        /// If set to any non empty value, this setting triggers the memory pool profiler
        /// </summary>
        /// <remarks>Profiling the memory pool gives insights into those functions, which potentially 
        /// cause memory leaks in your application. It writes extensive stack trace information into the 
        /// logfile determined by this setting. It will contain the stack trace of any function, requesting 
        /// memory from the pool, which is never given back to the pool. Use this information in order to find and 
        /// correct those places - and increase stability and performance of your application. However, 
        /// make sure, this switch is cleared (or renamed) for production systems, since running the profiler 
        /// will diminish performance significantly.</remarks>
        public static string MemoryPoolProfileFileName {
            get {
                return s_memoryPoolProfileFileName;
            }
            set {
                s_memoryPoolProfileFileName = value;
            }
        }
        /// <summary>
        /// The absolute directory where ILNumerics should look for native dependencies (LAPACK, HDF5 etc.); default: empty string 
        /// </summary>
        /// <remarks><para>By default (i.e.: empty string) ILNumerics will automatically determine the include path for native dependencies on startup. 
        /// In order to do so, the bitrate (Environment.Is64BitProcess) is examined and depending on its value one of 'bin32' or 'bin64' is 
        /// added to the beginning of the current PATH environment variable.</para>
        /// <para>In order to overwrite this behavior, one may set the absolut path to be included here. Note, this will prevent the 
        /// automatic (bitrate dependend) behaviour! When configuring the NativeDependenciesAbsolutePath the user must keep the 
        /// current bitrate into account and is responsible for placing the right binary distribution files into that folder. </para></remarks>
        public static string NativeDependenciesAbsolutePath {
            get {
                return s_nativeDependenciesAbsolutePath;
            }
            set {
                s_nativeDependenciesAbsolutePath = value;
            }
        }
        /// <summary>
        /// Gives the current setting for the reporting of runtime performance measures to the windows performance monitor (perfmon) (readonly)
        /// </summary>
        /// <remarks>Activating this switch requires administrative rights on each system the application is run - at least for the first time! This is 
        /// necessary in order to register the performance counters to the system. Afterwards, the application does not require administrative rights anymore.
        /// <para>Measuring the performance at runtime does not produce a substantial impact on the performance of your algorithm. However, due to the need for elevated rights 
        /// the feature is disabled by default. In order to enable it, a configuration variable named "ILNMeasurePerformanceAtRuntime" needs to be set in your 
        /// application configuration file.</para></remarks>
        public static bool MeasurePerformanceAtRuntime {
            get { return s_measurePerformanceAtRuntime; }
        }
        /// <summary>
        /// Block size used for blocked managed matrix multiply, default: 150
        /// </summary>
        public static int ManagedMultiplyBlockSize {
            get { return s_managedMultiplyBlockSize; }
            set { s_managedMultiplyBlockSize = value; }
        }
        /// <summary>
        /// Threshold on number of elements in either input matrix below which matrix multiplication is done managed only, default: 200
        /// </summary>
        public static int ManagedMultiplyMaxElementSize {
            get { return s_managedMultiplyMaxElementSize; }
            set { s_managedMultiplyMaxElementSize = value; }
        }

        /// <summary>
        /// Determine the minimum length for arrays to be sorted via Quicksort algorithm, smaller arrays are sorted via insertion sort
        /// </summary>
        public static int MinimumQuicksortLength {
            get { return s_minimumQuicksortLength; }
            set { s_minimumQuicksortLength = value; }
        }
        /// <summary>
        /// Maximum number of threads for parallel execution of internal functions in ILNumerics
        /// </summary>
        /// <remarks>
        /// <para>In order to maximize execution speed of numerical algorithms, the value of <c>MaxNumberThreads</c> should be equal to the number 
        /// of <b>real</b> processor cores on the system. For processors utilizing <a href="http://en.wikipedia.org/wiki/Hyper-threading">Hyper-threading</a> the number on virtual cores 
        /// may be higher. However, since those virtual cores share certain ressources for execution, parallel utiliziation can not efficently be done with them. In this cases, the number of 'cores' 
        /// appearing e.g. in the windows task manager is misleading and the true number of independent cores should be used for <c>MaxNumberThreads</c> instead. Consult your proccessor vendor in order to find out, how many 
        /// independant cores your system utilizes.</para>
        /// <para>Since the number of independent cores is not reliably determined by .NET, ILNumerics defaults to 2 cores on all multicore machines. Therefore, this setting should be <a href="http://ilnumerics.net/$Configuration.html">set manually</a> for 
        /// better processor utilization on multicore machines.</para>
        /// <para>If your algorithm uses custom parallel execution models, it may 
        /// be necessary to set this value to '1'. ILNumerics will run single threaded than - leaving you the option to configure 
        /// the execution on parallel threads on your own.</para>
        /// <para>The setting of this value also effects the corresponding value of any unmanaged optimized support library (e.g. MKL) internally used by ILNumerics.</para>
        /// </remarks>
        public static int MaxNumberThreads {
            get { return s_maxNumberThreads; }
            set {
                if (value < 1)
                    throw new ILNumerics.Exceptions.ILArgumentException("number of worker threads must be greater than 0");
                Misc.ILThreadPool.Pool.MaxNumberThreads = value - 1; 
                s_maxNumberThreads = value;
            }
        }
        /// <summary>
        /// Determine, if the current setting of <see cref="MaxNumberThreads"/> is the result of a custom configuration
        /// </summary>
        public static bool MaxNumberThreadsConfigured {
            get { return s_maxNumberThreadsConfigured; }
            private set { s_maxNumberThreadsConfigured = value; }
        }

        /// <summary>
        /// Threshold used to determine, if computations of O(n^3) built-in-functions are done in parallel on multicore machines
        /// </summary>
        public static int MinParallelElement3Count {
            get { return s_minParallelElement3Count; }
            set { s_minParallelElement3Count = value; }
        }

        /// <summary>
        /// Threshold used to determine, if computations of O(n^2) built-in-functions are done in parallel on multicore machines
        /// </summary>
        public static int MinParallelElement2Count {
            get { return s_minParallelElement2Count; }
            set { s_minParallelElement2Count = value; }
        }

        /// <summary>
        /// Threshold used to determine, if computations of O(n) built-in-functions are done in parallel on multicore machines
        /// </summary>
        public static int MinParallelElement1Count {
            get { return s_minParallelElement1Count; }
            set { s_minParallelElement1Count = value; }
        }
        
        /// <summary>
        /// Maximal recursion depth the quicksort can go. default: 100 (for array length up to 2^100)
        /// </summary>
        public static int MaxSafeQuicksortRecursionDepth {
            get { return s_maxSafeQuicksortRecursionDepth; }
            set { s_maxSafeQuicksortRecursionDepth = value; }
        }

        /// <summary>
        /// Controls implicit conversions from logical arrays to System.Boolean
        /// </summary>
        /// <remarks>This setting specifies, how logical arrays are converted to System.Boolean. Those 
        /// conversions are important, in order to simplify expressions like 
        /// <code>if (A &gt; B) { ... }</code> on arrays A and B. 
        /// <para>Here, the comparison <code>A &gt; B</code> creates a logical array of the same size than A and B. The logical array contains
        /// the result of the elementwise 'greater than' comparison. This setting here controls, how that logical is converted
        /// to a System.Boolean, in order to evaluate the 'if' condition. </para>
        /// <para>The default is <c>LogicalConversionMode.NonScalarThrowsException</c>, which would cause an exception to be thrown in 
        /// the example above. Only scalar logical arrays can be used in such implicit conversions than.</para>
        /// <para>In order to further simplify the syntax, the <c>LogicalConversionMode.ImplicitAllAll</c> setting can be used. The above expression
        /// would evaluate to true, if <b>all</b> elements of B are greater than corresponding elements of A. This settings therefore 
        /// eases the syntax for most situations. However, since most comparison operators comply to the underlying <b>all</b> rule, 
        /// the '!=' (not equal to) operator does not - at least to the extend of common intuition. In an expression: 
        /// <code>if (A != B) { ... }</code> one would intuitively expect to execute the code block, if <b>at least one element</b> of A does not equal the corresponding element
        /// of B. However, due to the <b>all</b> rule, this is not the case! In fact, the code would be executed only, if <b>all</b> elements would 
        /// evaluate to true. I.e. if no single pair of corresponding elements in A and B are equal. In order to 
        /// get the intuitively expected behavior, one would override this by:
        /// <code>if (ILMath.any(A != B)) { ... } </code> 
        /// Since it may cause hard to find bugs, this setting should be used with care.</para></remarks>
        public static LogicalConversionMode LogicalArrayToBoolConversion {
            get { return s_logicalArrayToBoolConversion; }
            set { s_logicalArrayToBoolConversion = value; }
        }
        /// <summary>
        /// Determine, if main and worker threads should bind to constant cpus or not. The default is not to bind.
        /// </summary>
        /// <remarks>It usually is more efficient, to leave control of cpu binding to the runtime. However, if in certain situations, more control is required, this flag can 
        /// be used to make ILNumerics worker threads be affine to corresponding (native) threads.</remarks>
        public static bool UseThreadAffinity {
            get { return s_isHosted; }
            set { s_isHosted = value; }
        }
        #endregion

        #region public interface 
        /// <summary>
        /// (Re)load settings from the application configuration file
        /// </summary>
        public static void LoadDefaults() {
            
            tryLoadSetting<int>("ILNMinimumQuicksortLength", ref s_minimumQuicksortLength, int.Parse, 10);
            if (!tryLoadSetting<int>("ILNMaxNumberThreads", ref s_maxNumberThreads, int.Parse, 2)) {
                // determine number of threads in another way: here: hardcoded
                if (Environment.ProcessorCount > 1 && !System.Diagnostics.Debugger.IsAttached)
                    s_maxNumberThreads = 2;
                else
                    s_maxNumberThreads = 1;
                MaxNumberThreadsConfigured = false;
                // TODO: try to determine number of PHYSICAL cores automatically
                // try to load number of physical cores rather than logical cores - utilize CPUID via native function pointer!  
            } else {
                MaxNumberThreadsConfigured = true; 
            }
            tryLoadSetting<int>("ILNMinParallelElement3Count", ref s_minParallelElement3Count, int.Parse, 500);
            tryLoadSetting<int>("ILNMinParallelElement2Count", ref s_minParallelElement2Count, int.Parse, 1000);
            tryLoadSetting<int>("ILNMinParallelElement1Count", ref s_minParallelElement1Count, int.Parse, (System.Environment.Is64BitProcess) ? 2000 : 2000);
            tryLoadSetting<int>("ILNMaxSafeQuicksortRecursionDepth", ref s_maxSafeQuicksortRecursionDepth, int.Parse, 100);
            tryLoadSetting<LogicalConversionMode>("ILNLogicalArrayToBoolConversion", ref s_logicalArrayToBoolConversion, logicalEnumParse, LogicalConversionMode.NonScalarThrowsException);
            tryLoadSetting<string>("ILNMemoryPoolProfileFileName", ref s_memoryPoolProfileFileName, dummyStringCopy, "");
            tryLoadSetting<int>("ILNMemoryPoolProfileMaxLength", ref s_memoryPoolProfileMaxLength, int.Parse, 500);
            tryLoadSetting<int>("ILNMemoryPoolProfileMinLength", ref s_memoryPoolProfileMinLength, int.Parse, 500);
            tryLoadSetting<int>("ILNMinElementLength4SystemArrayCopy", ref s_minElementLength4SystemArrayCopy, int.Parse, 50);
            tryLoadSetting<bool>("ILNUseThreadAffinity", ref s_isHosted, bool.Parse, false);
            tryLoadSetting<bool>("ILNCreateRowVectorsByDefault", ref s_createRowVectorsByDefault, bool.Parse, false);
            tryLoadSetting<bool>("ILNAllowInArrayAssignments", ref s_allowInArrayAssignments, bool.Parse, true);
            tryLoadSetting<bool>("ILNMeasurePerformanceAtRuntime", ref s_measurePerformanceAtRuntime, bool.Parse, false);
            tryLoadSetting<int>("ILNManagedMultiplyBlockSize", ref s_managedMultiplyBlockSize, int.Parse, 150);
            tryLoadSetting<int>("ILNManagedMultiplyMaxElementSize", ref s_managedMultiplyMaxElementSize, int.Parse, 200);
            tryLoadSetting<bool>("ILNOpenGL31_FIX_GL_CLIPVERTEX", ref s_OpenGL31_FIX_GL_CLIPVERTEX, bool.Parse, false);
            tryLoadSetting<bool>("ILNShowMessageBoxOnGDIFallback", ref s_showMessageBoxOnGDIFallback, bool.Parse, true);
            tryLoadSetting<string>("ILNNativeDependenciesAbsolutePath", ref s_nativeDependenciesAbsolutePath, dummyStringCopy, "");
            tryLoadSetting<bool>("ILNIsHosted", ref s_isHosted, bool.Parse, false);
        }
        #endregion

        #region private helper 
        private static bool tryLoadSetting<T>(string settingsName, ref T obj, System.Converter<string, T> convert, T defaultValue) {
            string value = System.Configuration.ConfigurationManager.AppSettings[settingsName];
            T tmp;
            if (!string.IsNullOrEmpty(value)) {
                try {
                    tmp = convert(value); // may throws exception! 
                    obj = tmp;
                    return true;
                } catch (Exception exc) {
                    tmp = defaultValue;
                    string msg = "Error reading default setting from application configuration, appsettings section: Invalid value for: '" + settingsName + "'";
                    System.Diagnostics.Trace.WriteLine(msg);
                    throw new ILNumerics.Exceptions.ILArgumentException(msg, exc);
                }
            } else {
                obj = defaultValue;
                return false;
            }
        }
        internal static string dummyStringCopy(string val) { return val.Trim(); }
        internal static LogicalConversionMode logicalEnumParse(string val) {
            LogicalConversionMode ret;
            if (Enum.TryParse<LogicalConversionMode>(val, out ret)) {
                return ret;
            }
            throw new ILNumerics.Exceptions.ILArgumentException("The configuration value for 'ILNLogicalArrayToBoolConversion' is not valid.");
        }
        #endregion




    }
}
