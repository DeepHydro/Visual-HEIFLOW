using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Configuration;
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
        }

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

                MessageBox.Show(this, (string)FindResource("SceneWorldSettingsSaved") + " " + path,
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
