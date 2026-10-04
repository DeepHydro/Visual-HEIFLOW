using Heiflow.Applications;
using Heiflow.Core;
using Heiflow.Models.Generic;
using Heiflow.Models.IO;
using Heiflow.Presentation.Controls;
using Heiflow.Tools.Conversion;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    public class VGSManager : MyAppManager, IApplication
    {
        private AggregateCatalog _catalog;
        private IContainer _components;
        private IEnumerable<IModuleController> moduleControllers;

        public VGSManager()
        {
            InitializeComponent();
            Directories = new List<string> { "Application Extensions", "Plugins" };
            VGSManager.Instance = this;
        }

        public AggregateCatalog AggregateCatalog
        {
            get
            {
                return _catalog;
            }
        }

        /// <summary>
        /// Gets or sets the list of string paths (relative to this one) to search for plugins.
        /// </summary>
        public List<string> Directories { get; set; }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _components = new Container();
            _catalog = GetCatalog();
            CompositionContainer = new CompositionContainer(_catalog, CompositionOptions.DisableSilentRejection);
        }

        public void Run()
        {
            ConfigurationManager.ApplicationPath = this.ApplicationPath;
            ConfigurationManager.SettingsPath = ApplicationPath + "\\Config";
            BaseModel.ConfigPath = ApplicationPath + "\\Application Extensions\\VHF\\Config";
            LoadSettings();
            moduleControllers = CompositionContainer.GetExportedValues<IModuleController>();
            foreach (IModuleController moduleController in moduleControllers) 
            {
                moduleController.Initialize(); 
            }
            foreach (IModuleController moduleController in moduleControllers) 
            { 
                moduleController.Run(); 
            }
        }

        public void Shutdown()
        {
            foreach (IModuleController moduleController in moduleControllers)
            {
                moduleController.Shutdown();
            }
            _catalog.Dispose();
            CompositionContainer.Dispose();
        }

        private void LoadSettings()
        {
            World.LoadSettings(ConfigurationManager.SettingsPath);
            Engine3DSettings settings = new Engine3DSettings();
            settings = (Engine3DSettings)SettingsBase.Load(settings, SettingsBase.LocationType.Application);
            ConfigurationManager.Engine3DSettings = settings;
        }

        public void Compose()
        {
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(CompositionContainer);
            CompositionContainer.Compose(batch);
        }

        public bool CheckLicense()
        {
            //bool ischecked = false;
            //string path= Path.Combine(this.ApplicationPath, "vgs.dll");
            //if (File.Exists(path))
            //{
            //    SecurityFile _SecurityFile = new SecurityFile(path);
            //    _SecurityFile.Validate();
            //    ischecked = _SecurityFile.Authenticated;
            //    if(ischecked)
            //    {
            //        //update datetime
            //        _SecurityFile.Date = System.DateTime.Now;
            //        _SecurityFile.Update("IHM3D");
            //    }
            //}
            //return ischecked;
            return true;
        }

        private AggregateCatalog GetCatalog()
        {
            var catalog = new AggregateCatalog();

            // Add main exe
            Assembly mainExe = Assembly.GetEntryAssembly();
            if (mainExe != null)
            {
                // if there is a managed entry assembly running, add it.
                catalog.Catalogs.Add(new AssemblyCatalog(mainExe));
                Trace.WriteLine("Cataloging: " + mainExe.FullName);
            }

            Assembly modelDll = typeof(Heiflow.Models.Integration.HeiflowModel).Assembly;
            catalog.Catalogs.Add(new AssemblyCatalog(modelDll));
            Assembly cntlDll = typeof(Heiflow.Controls.MessageService).Assembly;
            catalog.Catalogs.Add(new AssemblyCatalog(cntlDll));
            Assembly presentDll = typeof(IProjectExplorer).Assembly;
            catalog.Catalogs.Add(new AssemblyCatalog(presentDll));
            Assembly vgsappDll = typeof(VGSModuleController).Assembly;
            catalog.Catalogs.Add(new AssemblyCatalog(vgsappDll));
            Assembly toolDll = typeof(ToTIFSets).Assembly;
            catalog.Catalogs.Add(new AssemblyCatalog(toolDll));

            return catalog;
        }
    }
}
