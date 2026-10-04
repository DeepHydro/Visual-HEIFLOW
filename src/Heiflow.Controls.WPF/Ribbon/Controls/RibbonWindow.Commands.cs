using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public partial class RibbonWindow
    {
        private static RoutedUICommand closeCommand = new RoutedUICommand("Close", "CloseCommand", typeof(RibbonWindow));
        private static RoutedUICommand minimizeCommand = new RoutedUICommand("Minimize", "MinimizeCommand", typeof(RibbonWindow));
        private static RoutedUICommand maximizeCommand = new RoutedUICommand("Maximize", "MaximizeCommand", typeof(RibbonWindow));

        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(closeCommand, PerformClose));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(minimizeCommand, PerformMinimize));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(maximizeCommand, PerformMaximize));
        }

        private static void PerformClose(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonWindow window = (RibbonWindow)sender;
            window.Close();
        }

        private static void PerformMinimize(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonWindow window = (RibbonWindow)sender;
            window.WindowState = WindowState.Minimized;
        }

        private static void PerformMaximize(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonWindow window = (RibbonWindow)sender;
            window.WindowState = window.WindowState == WindowState.Maximized  ? WindowState.Normal : WindowState.Maximized;
        }

        public static RoutedUICommand CloseCommand {get{return closeCommand;}}
        public static RoutedUICommand MinimizeCommand { get { return minimizeCommand; } }
        public static RoutedUICommand MaximizeCommand { get { return maximizeCommand; } }
    }
}
