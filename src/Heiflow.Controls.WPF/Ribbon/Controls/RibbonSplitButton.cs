using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Markup;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    [TemplatePart(Name = partDropDown)]
    [ContentProperty("Items")]
    public class RibbonSplitButton : RibbonDropDownButton
    {
        const string partDropDown = "PART_DropDown";

        static RibbonSplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonSplitButton), new FrameworkPropertyMetadata(typeof(RibbonSplitButton)));
        }


        private Control dropDownBtn;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (dropDownBtn != null)
            {
                dropDownBtn.MouseDown -= dropDownBtn_MouseDown;
                dropDownBtn.MouseUp -= dropDownBtn_MouseUp;
            }
            dropDownBtn = GetTemplateChild(partDropDown) as Control;
            if (dropDownBtn != null)
            {
                dropDownBtn.MouseDown += new MouseButtonEventHandler(dropDownBtn_MouseDown);
                dropDownBtn.MouseUp += new MouseButtonEventHandler(dropDownBtn_MouseUp);
            }
        }

        void dropDownBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            IsDropDownPressed ^= true;
            EnsurePopupRemainsOnMouseUp();
        }

        void dropDownBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            EnsurePopupDoesNotStayOpen();
            e.Handled = true;
        }




        public ClickMode ClickMode
        {
            get { return (ClickMode)GetValue(ClickModeProperty); }
            set { SetValue(ClickModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClickMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickModeProperty =
            DependencyProperty.Register("ClickMode", typeof(ClickMode), typeof(RibbonSplitButton), new UIPropertyMetadata(ClickMode.Release));




        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsPressed = true;
            base.OnMouseLeftButtonDown(e);
            EnsurePopupRemainsOnMouseUp();
            if (ClickMode == ClickMode.Press) PerformClick();
        }

        private void PerformClick()
        {
            OnClick();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsPressed = false;
            base.OnMouseLeftButtonUp(e);
            EnsurePopupDoesNotStayOpen();
            if (ClickMode == ClickMode.Release) PerformClick();

        }

        protected override void ToggleDropDownState()
        {
            // do not show the popup menu at this place.
        }

    }
}
