using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private void RegisterHandlers()
        {
            AddHandler(RibbonGroup.ExecuteLauncherEvent, new RoutedEventHandler(OnExecuteLauncher));
        }

        /// <summary>
        /// Occurs when the DialogLauncher Button is clicked.
        /// </summary>
        public static readonly RoutedEvent ExecuteLauncherEvent = EventManager.RegisterRoutedEvent("ExecuteLauncherEvent",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RibbonGroup));

        /// <summary>
        /// Occurs when the DialogLauncher Button is clicked.
        /// </summary>
        public event RoutedEventHandler ExecuteLauncher
        {
            add { AddHandler(ExecuteLauncherEvent, value); }
            remove { RemoveHandler(ExecuteLauncherEvent, value); }
        }

        /// <summary>
        /// Occurs when the DialogLauncher Button is clicked.
        /// </summary>
        protected virtual void OnExecuteLauncher(object sender, RoutedEventArgs e)
        {
        }

    }
}
