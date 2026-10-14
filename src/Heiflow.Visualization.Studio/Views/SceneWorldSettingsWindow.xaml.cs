using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Configuration;
using Microsoft.Win32;
using MahApps.Metro.Controls;

namespace Heiflow.Visualization.Studio.Views
{
    /// <summary>
    /// Edits the browsable properties of World.Settings, which is exactly what Config\SceneWorld.xml
    /// is made of, see class WorldSettings: the file is the settings object serialized, so showing
    /// the object and saving it back covers the whole file, and a property added to WorldSettings
    /// shows up here without touching this window.
    ///
    /// The grid writes into World.Settings while the user types, so the running scene follows along.
    /// That also means a change has to be undone when the window is closed without saving, which is
    /// what the copy taken in the Loaded handler is for.
    /// </summary>
    public partial class SceneWorldSettingsWindow : MetroWindow
    {
        /// <summary>The settings as they were when the window opened, put back on cancel.</summary>
        WorldSettings _backup;

        public SceneWorldSettingsWindow()
        {
            InitializeComponent();
            Loaded += SceneWorldSettingsWindow_Loaded;
        }

        void SceneWorldSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _backup = CloneSettings();
            txtFilePath.Text = SettingsFilePath();
            propertyGrid.Instance = World.Settings;

            // The cache panel edits the same World.Settings object as the grid, only through controls
            // of its own, so it is bound to it directly and is saved and undone with it.
            cachePanel.DataContext = World.Settings;
            cmbCacheType.ItemsSource = Enum.GetValues(typeof(TileCacheType));
            RefreshCaches();
        }

        #region Cache panel

        /// <summary>
        /// Shows where the one database is and how big it is, and how much is left of the tiles that
        /// were cached as files. This is what the window opens with and what the refresh button does:
        /// browsing fills the database while the window is open.
        /// </summary>
        void RefreshCaches()
        {
            txtDbLocation.Text = TileCacheInventory.DatabasePath;
            txtDbSize.Text = TileCacheInventory.DatabaseExists ?
                FormatBytes(TileCacheInventory.DatabaseSize) : (string)FindResource("CacheDbMissing");
            txtFileCacheSize.Text = FormatBytes(TileCacheInventory.FileCacheSize);
        }

        void btnCacheRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshCaches();
        }

        /// <summary>Lets the user pick the database, instead of typing the path of it.</summary>
        void btnBrowseDb_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = (string)FindResource("CacheBrowse"),
                Filter = "SQLite cache|*.gmdb|All files|*.*",
                DefaultExt = ".gmdb",
                OverwritePrompt = false,
                FileName = Path.GetFileName(TileCacheInventory.DatabasePath),
                InitialDirectory = Path.GetDirectoryName(TileCacheInventory.DatabasePath)
            };

            if (dialog.ShowDialog() != true)
                return;

            // A path under the directory the application runs in is written the way Layers.xml writes
            // it, relative, so the database stays where it is when the application is moved.
            string path = dialog.FileName;
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            if (path.StartsWith(baseDirectory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                path = path.Substring(baseDirectory.Length + 1);

            World.Settings.TileCacheDbPath = path;

            // World.Settings does not tell the binding when a property changes, so the box is filled
            // in here as well.
            txtDbPath.Text = path;
            RefreshCaches();
        }

        void btnClearDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, (string)FindResource("CacheClearDatabaseConfirm"),
                (string)FindResource("SceneWorldSettingsTitle"), MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            ClearResult result = TileCacheInventory.ClearDatabase();
            RefreshCaches();

            string message = result.DatabaseDeleted ? (string)FindResource("CacheDatabaseDeleted") :
                string.Format((string)FindResource("CacheDatabaseEmptied"), result.RowsDeleted);

            if (!string.IsNullOrEmpty(result.Problem))
                message += Environment.NewLine + Environment.NewLine + result.Problem;

            MessageBox.Show(this, message, (string)FindResource("SceneWorldSettingsTitle"),
                MessageBoxButton.OK, result.DatabaseDeleted ? MessageBoxImage.Information : MessageBoxImage.Warning);
        }

        void btnClearFileCache_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, (string)FindResource("CacheClearFileCacheConfirm"),
                (string)FindResource("SceneWorldSettingsTitle"), MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            List<string> failures = new List<string>();
            int deleted = TileCacheInventory.ClearFileCache(failures);
            RefreshCaches();

            string message = string.Format((string)FindResource("CacheFileCacheCleared"), deleted);
            if (failures.Count > 0)
                message += Environment.NewLine + Environment.NewLine + (string)FindResource("CacheClearFailed") +
                    Environment.NewLine + string.Join(Environment.NewLine, failures.Take(10).ToArray());

            MessageBox.Show(this, message, (string)FindResource("SceneWorldSettingsTitle"),
                MessageBoxButton.OK, failures.Count > 0 ? MessageBoxImage.Warning : MessageBoxImage.Information);
        }

        /// <summary>
        /// Writes the cache settings to Layers.xml as well, so the file the layers are described in
        /// names the same database as SceneWorld.xml, and says where that went.
        /// </summary>
        string SaveLayersCache()
        {
            string path = ConfigurationManager.LayersFilePath;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "Layers.xml");

            if (!File.Exists(path))
                return (string)FindResource("CacheLayersNotFound") + " " + path;

            try
            {
                ConfigurationManager.SaveTileCache(path);
                return (string)FindResource("CacheSavedToLayers") + " " + path;
            }
            catch (Exception caught)
            {
                return (string)FindResource("CacheSaveFailed") + " " + path + Environment.NewLine + caught.Message;
            }
        }

        static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }
            return size.ToString(unit == 0 ? "0" : "0.0") + " " + units[unit];
        }

        #endregion

        /// <summary>The file the settings were loaded from, and the one Save writes them back to.</summary>
        string SettingsFilePath()
        {
            string path = World.Settings.SettingsFilePath;
            if (!string.IsNullOrEmpty(path))
                return path;

            // Nothing was loaded, fall back to where MainWindow reads them from.
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "SceneWorld.xml");
        }

        void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            // The grid matches the filter against the property name in lower case.
            propertyGrid.Filter = txtFilter.Text.ToLower();
        }

        /// <summary>Throws away what the grid changed and reads the file again.</summary>
        void btnReload_Click(object sender, RoutedEventArgs e)
        {
            string path = SettingsFilePath();
            if (!File.Exists(path))
            {
                MessageBox.Show(this, path, (string)FindResource("SceneWorldSettingsTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                WorldSettings reloaded = (WorldSettings)SettingsBase.Load(new WorldSettings(), path);
                CopySettings(reloaded, World.Settings);
                _backup = CloneSettings();

                // The grid only reads the values when the instance is set, so set it again.
                propertyGrid.Instance = null;
                propertyGrid.Instance = World.Settings;
            }
            catch (Exception caught)
            {
                MessageBox.Show(this, caught.Message, (string)FindResource("SceneWorldSettingsTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Writes the settings back to Config\SceneWorld.xml.</summary>
        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string path = SettingsFilePath();
            try
            {
                // Save() writes to the file the settings were loaded from, and says nothing when
                // that fails, so the path is given here and a failure is reported.
                World.Settings.FileName = path;
                World.Settings.Save(path);
                _backup = CloneSettings();

                // The database the layers share is named in Layers.xml too, so it is written there
                // with it: the settings say where the tiles go, the file describes the layers that
                // put them there, and the two must not name two different databases.
                string report = (string)FindResource("SceneWorldSettingsSaved") + " " + path +
                    Environment.NewLine + SaveLayersCache();

                MessageBox.Show(this, report,
                    (string)FindResource("SceneWorldSettingsTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception caught)
            {
                MessageBox.Show(this, caught.Message, (string)FindResource("SceneWorldSettingsTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Closing the window any other way than saving counts as a cancel, so the scene goes back
        /// to the settings it had when the window opened.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            if (_backup != null)
                CopySettings(_backup, World.Settings);

            base.OnClosed(e);
        }

        static WorldSettings CloneSettings()
        {
            XmlSerializer ser = new XmlSerializer(typeof(WorldSettings));
            using (MemoryStream ms = new MemoryStream())
            {
                ser.Serialize(ms, World.Settings);
                ms.Position = 0;
                return (WorldSettings)ser.Deserialize(ms);
            }
        }

        static void CopySettings(WorldSettings from, WorldSettings to)
        {
            foreach (PropertyInfo p in typeof(WorldSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanRead || !p.CanWrite || p.GetIndexParameters().Length > 0)
                    continue;

                // FileName is [XmlIgnore], a clone does not carry it, and copying it over would
                // make World.Settings forget where it was loaded from.
                if (p.Name == "FileName")
                    continue;

                try
                {
                    p.SetValue(to, p.GetValue(from, null), null);
                }
                catch
                {
                    // A value that cannot be copied keeps the one it has.
                }
            }
        }
    }

    /// <summary>Shows a System.Drawing.Color as a WPF brush, see the colour template in the XAML.</summary>
    public class ColorToBrushConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is System.Drawing.Color))
                return System.Windows.Media.Brushes.Transparent;

            System.Drawing.Color color = (System.Drawing.Color)value;
            return new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
