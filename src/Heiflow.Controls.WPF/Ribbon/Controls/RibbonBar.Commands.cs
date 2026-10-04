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
    partial class RibbonBar
    {
        private static RoutedUICommand alignGroupsLeftCommand = new RoutedUICommand("Align Left", "AlignGroupsLeftCommand", typeof(RibbonBar));
        private static RoutedUICommand alignGroupsRightCommand = new RoutedUICommand("Align Right", "AlignGroupsRightCommand", typeof(RibbonBar));
        private static RoutedUICommand collapseRibbonBarCommand = new RoutedUICommand("", "CollapseRibbonBarCommand", typeof(RibbonBar));

        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(RibbonBar), new CommandBinding(alignGroupsLeftCommand, alignGroupsLeft));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonBar), new CommandBinding(alignGroupsRightCommand, alignGroupsRight));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonBar), new CommandBinding(collapseRibbonBarCommand, collapseRibbonBar));
        }

        private static void collapseRibbonBar(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            bar.IsExpanded = false;

        }

        private static void alignGroupsLeft(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            bar.AlignGroupsLeft();
       }


        private static void alignGroupsRight(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            bar.AlignGroupsRight();
        }

        public static RoutedUICommand AlignGroupsLeftCommand
        {
            get { return alignGroupsLeftCommand; }
        }

        public static RoutedUICommand AlignGroupsRightCommand
        {
            get { return alignGroupsRightCommand; }
        }

        public static RoutedCommand CollapseRibbonBarCommand { get { return collapseRibbonBarCommand; } }
    }
}
