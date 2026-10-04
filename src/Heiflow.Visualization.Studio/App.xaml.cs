using Heiflow.Applications;
using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Studio.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Heiflow.Visualization.Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        VGSManager _VGSManager;
        public App()
        {
            this.Exit += App_Exit;
        }

        private  void App_Exit(object sender, ExitEventArgs e)
        {
            _VGSManager.Shutdown();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
         
            Uri uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            _VGSManager = new VGSManager()
            {
                AppMode = Presentation.Controls.AppMode.HE
            };

            VGSManager.Instance = _VGSManager;
            _VGSManager.ApplicationPath = System.IO.Path.GetDirectoryName(uri.LocalPath);
            foreach (string moduleAssembly in Settings.Default.ModuleAssemblies)
            {
              //  _VGSManager.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(moduleAssembly));
            }

            _VGSManager.Compose();
            _VGSManager.Run();
        }

    }
}