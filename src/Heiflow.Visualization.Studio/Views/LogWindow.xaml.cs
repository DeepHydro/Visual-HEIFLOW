using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Heiflow.Core;
using MahApps.Metro.Controls;

namespace Heiflow.Visualization.Studio.Views
{
    /// <summary>
    /// Shows the end of the log the application writes into Log\HydroEarth.log, see class Log, and
    ///throws it away again. That file is kept open and appended to from the moment the application
    /// starts and is never rolled over, so it is not deleted here: what it holds can be emptied, see
    /// Log.Clear, while the other files in the same directory, the dumps of the exceptions for one,
    /// can be deleted outright.
    ///
    /// Only the end of the file is read, both because that is what is asked for and because a log that
    /// has been filling up for months is far too long to read every time the window is refreshed.
    /// </summary>
    public partial class LogWindow : MetroWindow
    {
        /// <summary>One file in the log directory, with the checkbox the user picks it for deletion with.</summary>
        public class FileRow
        {
            public string Path { get; set; }
            public bool Selected { get; set; }
            public string Name { get; set; }
            public string KindText { get; set; }
            public string SizeText { get; set; }
            public string ModifiedText { get; set; }
        }

        /// <summary>One line of the log, see Parse.</summary>
        public class EntryRow
        {
            public string Time { get; set; }
            public int Level { get; set; }
            public string LevelText { get; set; }
            public string Category { get; set; }
            public string Message { get; set; }
        }

        /// <summary>What the level box offers, and the level it stands for: -1 is everything.</summary>
        class LevelChoice
        {
            public string Text { get; set; }
            public int Level { get; set; }
        }

        public LogWindow()
        {
            InitializeComponent();
            Loaded += LogWindow_Loaded;
        }

        void LogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cmbLines.ItemsSource = new int[] { 100, 500, 1000, 5000 };
            cmbLines.SelectedItem = 500;

            cmbLevel.ItemsSource = new LevelChoice[]
            {
                new LevelChoice { Text = (string)FindResource("LogLevelAll"), Level = -1 },
                new LevelChoice { Text = (string)FindResource("LogLevelError"), Level = Log.Levels.Error },
                new LevelChoice { Text = (string)FindResource("LogLevelWarning"), Level = Log.Levels.Warning },
                new LevelChoice { Text = (string)FindResource("LogLevelDebug"), Level = Log.Levels.Debug }
            };
            cmbLevel.DisplayMemberPath = "Text";
            cmbLevel.SelectedIndex = 0;

            RefreshFiles();
            RefreshEntries();
        }

        /// <summary>The files in the log directory, newest first, the one being written to marked.</summary>
        void RefreshFiles()
        {
            List<FileRow> rows = new List<FileRow>();
            string current = Log.LogFilePath;

            if (Directory.Exists(Log.LogPath))
            {
                foreach (FileInfo file in new DirectoryInfo(Log.LogPath).EnumerateFiles())
                {
                    rows.Add(new FileRow
                    {
                        Path = file.FullName,
                        Name = file.Name,
                        KindText = string.Equals(file.FullName, current, StringComparison.OrdinalIgnoreCase) ?
                            (string)FindResource("LogCurrentFile") : "",
                        SizeText = FormatBytes(file.Length),
                        ModifiedText = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
            }

            lvFiles.ItemsSource = rows.OrderByDescending(row => row.ModifiedText).ToList();
        }

        /// <summary>Reads the end of the log again, and shows what the filter leaves of it.</summary>
        void RefreshEntries()
        {
            string path = Log.LogFilePath;
            if (!File.Exists(path))
            {
                lvEntries.ItemsSource = null;
                txtStatus.Text = (string)FindResource("LogNoFile") + " " + path;
                return;
            }

            LevelChoice level = cmbLevel.SelectedItem as LevelChoice;
            string category = txtCategory.Text.Trim();

            List<EntryRow> entries = new List<EntryRow>();
            foreach (string line in ReadLastLines(path, RequestedLines()))
            {
                EntryRow entry = Parse(line);
                if (entry == null)
                {
                    // Not a line of its own, so it runs on from the entry before it: an exception and
                    // its stack trace are written one line at a time.
                    if (entries.Count > 0)
                        entries[entries.Count - 1].Message += Environment.NewLine + line;

                    continue;
                }

                entries.Add(entry);
            }

            // Newest first, which is what the window is opened for, and saves scrolling to the end.
            List<EntryRow> rows = entries.Where(entry =>
                    (level == null || level.Level < 0 || entry.Level <= level.Level) &&
                    (category.Length == 0 || entry.Category.IndexOf(category, StringComparison.OrdinalIgnoreCase) >= 0))
                .Reverse().ToList();

            lvEntries.ItemsSource = rows;
            txtStatus.Text = FormatBytes(new FileInfo(path).Length) + "   " +
                string.Format((string)FindResource("LogShowing"), rows.Count);
        }

        /// <summary>How many lines were asked for, the box being editable so any number can be typed.</summary>
        int RequestedLines()
        {
            int count;
            if (int.TryParse(cmbLines.Text, out count) && count > 0)
                return Math.Min(count, 20000);

            return 500;
        }

        /// <summary>
        /// The last lines of the file. Only the end of it is read: the log is appended to for as long
        /// as the application runs, so it can be far longer than anything worth reading here, and the
        /// newest lines are the ones asked for. The window is given room for lines longer than the
        /// average, and lines that fall outside it are simply not shown.
        /// </summary>
        static List<string> ReadLastLines(string path, int count)
        {
            // Lines are far longer than the average when an exception and its stack trace are written,
            // so the window is widened until it holds the number of lines asked for, or until the
            // whole file has been read.
            long length = new FileInfo(path).Length;
            for (long window = (long)count * 1024 + 65536; ; window *= 4)
            {
                List<string> lines = ReadWindow(path, count, window);
                if (lines.Count >= count || window >= length || window > 64 * 1024 * 1024)
                    return lines;
            }
        }

        static List<string> ReadWindow(string path, int count, long window)
        {
            Queue<string> last = new Queue<string>(count);

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                window = Math.Min(window, stream.Length);
                stream.Position = stream.Length - window;

                // The window starts in the middle of a line, which is thrown away. The encoding is
                // left to the reader: bytes cut in two would break a character in half.
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    if (window < stream.Length)
                        reader.ReadLine();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length == 0)
                            continue;

                        last.Enqueue(line);
                        if (last.Count > count)
                            last.Dequeue();
                    }
                }
            }

            return last.ToList();
        }

        /// <summary>
        /// Splits a line the way Log.Write builds it: the time, the level, the category padded to four
        /// characters, and the message. A line that is not built that way, the second line of an
        /// exception for one, gives null and is put with the entry before it.
        /// </summary>
        static EntryRow Parse(string line)
        {
            // "2026-10-14 16:24:36Z 5 CONF Loading ...", the time being DateTime.ToString("u").
            if (line.Length < 27 || line[20] != ' ')
                return null;

            DateTime time;
            if (!DateTime.TryParse(line.Substring(0, 20), CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out time))
                return null;

            string rest = line.Substring(21);
            int space = rest.IndexOf(' ');
            if (space < 0)
                return null;

            int level;
            if (!int.TryParse(rest.Substring(0, space), NumberStyles.Integer, CultureInfo.InvariantCulture, out level))
                return null;

            // The category is four characters wide, padded when shorter, and clipped when longer.
            string afterLevel = rest.Substring(space + 1);
            if (afterLevel.Length < 5)
                return null;

            return new EntryRow
            {
                Time = line.Substring(0, 20),
                Level = level,
                LevelText = LevelName(level),
                Category = afterLevel.Substring(0, 4).Trim(),
                Message = afterLevel.Substring(5)
            };
        }

        /// <summary>The name of a level, see Log.Levels. Written as it is in the file, a number.</summary>
        static string LevelName(int level)
        {
            if (level == Log.Levels.Error)
                return "Error";
            if (level == Log.Levels.Warning)
                return "Warning";
            if (level == Log.Levels.Debug)
                return "Debug";
            if (level == Log.Levels.Verbose)
                return "Verbose";

            return level.ToString(CultureInfo.InvariantCulture);
        }

        void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                RefreshEntries();
        }

        void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshFiles();
            RefreshEntries();
        }

        /// <summary>Throws away what the log holds, see Log.Clear.</summary>
        void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, (string)FindResource("LogClearConfirm"), Title,
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                Log.Clear();
            }
            catch (Exception caught)
            {
                MessageBox.Show(this, caught.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            RefreshFiles();
            RefreshEntries();
        }

        /// <summary>
        /// Deletes the files that are ticked. The one being written to is not among them: it is open,
        /// so deleting it fails, and emptying it is what the other button is for.
        /// </summary>
        void btnDeleteFiles_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<FileRow> rows = lvFiles.ItemsSource as IEnumerable<FileRow>;
            List<FileRow> ticked = rows == null ? new List<FileRow>() : rows.Where(row => row.Selected).ToList();

            if (ticked.Count == 0)
            {
                MessageBox.Show(this, (string)FindResource("LogNothingSelected"), Title,
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show(this, string.Format((string)FindResource("LogDeleteConfirm"), ticked.Count), Title,
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            List<string> failures = new List<string>();
            int deleted = 0;
            foreach (FileRow row in ticked)
            {
                try
                {
                    File.Delete(row.Path);
                    deleted++;
                }
                catch (Exception caught)
                {
                    failures.Add(row.Name + ": " + caught.Message);
                }
            }

            RefreshFiles();

            string message = string.Format((string)FindResource("LogDeleted"), deleted);
            if (failures.Count > 0)
                message += Environment.NewLine + Environment.NewLine + (string)FindResource("LogDeleteFailed") +
                    Environment.NewLine + string.Join(Environment.NewLine, failures.Take(10).ToArray());

            MessageBox.Show(this, message, Title, MessageBoxButton.OK,
                failures.Count > 0 ? MessageBoxImage.Warning : MessageBoxImage.Information);
        }

        void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(Log.LogPath))
                return;

            Process.Start(new ProcessStartInfo("explorer.exe", Log.LogPath));
        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
    }
}
