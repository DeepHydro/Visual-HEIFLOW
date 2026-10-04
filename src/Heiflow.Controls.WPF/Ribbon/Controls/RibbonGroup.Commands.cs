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
    partial class RibbonGroup
    {
        private static RoutedUICommand launchDialogCommand = new RoutedUICommand("Launch", "LaunchDialogCommand", typeof(RibbonGroup));

        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(RibbonGroup),
                new CommandBinding(launchDialogCommand, launchDialog));
        }

        private static void launchDialog(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonGroup group = (RibbonGroup)sender;
            RoutedEventArgs args = new RoutedEventArgs(ExecuteLauncherEvent);
            group.RaiseEvent(args);

        }

        public static RoutedUICommand LaunchDialogCommand
        {
            get { return launchDialogCommand; }
        }
    }
}
