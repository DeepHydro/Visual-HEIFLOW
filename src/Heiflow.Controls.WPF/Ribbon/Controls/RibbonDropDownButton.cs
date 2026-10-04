using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Diagnostics;
using Odyssey.Controls.Ribbon.Interfaces;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{

    [TemplatePart(Name = partPopup)]
    [ContentProperty("Items")]
    public class RibbonDropDownButton : MenuItem, IRibbonButton, ICommandSource
    {
        const string partPopup = "PART_Popup";

        static RibbonDropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonDropDownButton), new FrameworkPropertyMetadata(typeof(RibbonDropDownButton)));
        }

        public RibbonDropDownButton()
        {
            AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(menuItemClicked));
            AddHandler(RibbonButton.ClickEvent, new RoutedEventHandler(menuItemClicked));
        }

        private void menuItemClicked(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == this) return;
            IsDropDownPressed = false;
        }



        protected Popup popup;

        internal Popup Popup
        {
            get { return popup; }
        }


        public override void OnApplyTemplate()
        {
            if (popup != null)
            {
                popup.Closed -= OnPopupClosed;
                popup.Opened -= OnPopupOpened;
            }
            popup = GetTemplateChild(partPopup) as Popup;
            if (popup != null)
            {
                popup.Closed += new EventHandler(OnPopupClosed);
                popup.Opened += new EventHandler(OnPopupOpened);
            }

            base.OnApplyTemplate();
        }

        public static readonly RoutedEvent PopupOpenedEvent = EventManager.RegisterRoutedEvent("PopupOpened",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RibbonDropDownButton));

        public static readonly RoutedEvent PopupClosedEvent = EventManager.RegisterRoutedEvent("PopupClosed",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RibbonDropDownButton));

        public event RoutedEventHandler PopupOpened
        {
            add { AddHandler(PopupOpenedEvent, value); }
            remove { RemoveHandler(PopupOpenedEvent, value); }
        }

        public event RoutedEventHandler PopupClosed
        {
            add { AddHandler(PopupClosedEvent, value); }
            remove { RemoveHandler(PopupClosedEvent, value); }
        }

        protected virtual void OnPopupOpened(object sender, EventArgs e)
        {
            IsDropDownPressed = true;
            RoutedEventArgs args = new RoutedEventArgs(RibbonDropDownButton.PopupOpenedEvent);
            RaiseEvent(args);
        }

        protected virtual void OnPopupClosed(object sender, EventArgs e)
        {
            IsDropDownPressed = false;
            RoutedEventArgs args = new RoutedEventArgs(RibbonDropDownButton.PopupClosedEvent);
            RaiseEvent(args);
        }



        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(RibbonDropDownButton), new UIPropertyMetadata(null));




        public ImageSource SmallImage
        {
            get { return (ImageSource)GetValue(SmallImageProperty); }
            set { SetValue(SmallImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallImageProperty =
            DependencyProperty.Register("SmallImage", typeof(ImageSource), typeof(RibbonDropDownButton), new UIPropertyMetadata(null));



        public ImageSource LargeImage
        {
            get { return (ImageSource)GetValue(LargeImageProperty); }
            set { SetValue(LargeImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LargeImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LargeImageProperty =
            DependencyProperty.Register("LargeImage", typeof(ImageSource), typeof(RibbonDropDownButton), new UIPropertyMetadata(null));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonDropDownButton), new UIPropertyMetadata(new CornerRadius(3)));


        /// <summary>
        /// Gets or sets whether the drop down button is down.
        /// </summary>
        public bool IsDropDownPressed
        {
            get { return (bool)GetValue(IsDropDownPressedProperty); }
            set { SetValue(IsDropDownPressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDropDown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownPressedProperty =
            DependencyProperty.Register("IsDropDownPressed", typeof(bool), typeof(RibbonDropDownButton),
            new UIPropertyMetadata(false, IsDropDownPressedPropertyChangedCallback));

        static void IsDropDownPressedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonDropDownButton btn = d as RibbonDropDownButton;
            btn.OnDropDownPressedChanged((bool)e.OldValue, (bool)e.NewValue);

        }

        protected virtual void OnDropDownPressedChanged(bool oldValue, bool newValue)
        {
            if (popup != null)
            {
                if (newValue)
                {
                    popup.PlacementTarget = this;
                    CloseOpenedPopup(this);
                }
                else
                {
                    CloseOpenedPopup(null);
                }
                popup.IsOpen = newValue;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter || e.Key == Key.Space) { IsDropDownPressed = true; e.Handled = true; }
            if (e.Key == Key.Escape) { IsDropDownPressed = false; e.Handled = true; }
        }


        private static RibbonDropDownButton DroppedDownButton;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            EnsurePopupRemainsOnMouseUp();
            if (this.IsAncestorOf(e.OriginalSource as DependencyObject))
            {
                ToggleDropDownState();
                e.Handled = true;
            }
            base.OnMouseLeftButtonDown(e);
        }


        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = true;
            EnsurePopupDoesNotStayOpen();
            base.OnMouseLeftButtonUp(e);
        }

        protected virtual void ToggleDropDownState()
        {
            IsDropDownPressed ^= true;
        }


        public static void CloseOpenedPopup(RibbonDropDownButton caller)
        {
            RibbonDropDownButton btn = DroppedDownButton;
            if (btn != null && (btn != caller))
            {
                if (caller != null)
                {
                    // don't close the popup if the previous popup is a host for the current caller:

                    FrameworkElement parent = caller.Parent as FrameworkElement;
                    while (parent != null)
                    {
                        if (parent == btn) return;
                        FrameworkElement previous = parent;
                        parent = parent.Parent as FrameworkElement;
                        if (parent == null)
                        {
                            parent = previous.TemplatedParent as FrameworkElement;
                        }
                    }

                    if (btn.popup != null) btn.popup.IsOpen = false;
                    btn.IsDropDownPressed = false;
                }
            }
            DroppedDownButton = caller;
        }


        public bool IsFlat
        {
            get { return (bool)GetValue(IsFlatProperty); }
            set { SetValue(IsFlatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFlat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFlatProperty =
            DependencyProperty.Register("IsFlat", typeof(bool), typeof(RibbonDropDownButton), new UIPropertyMetadata(true));


        public bool ShowDropDownSymbol
        {
            get { return (bool)GetValue(ShowDropDownSymbolProperty); }
            set { SetValue(ShowDropDownSymbolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowDropDownSymbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDropDownSymbolProperty =
            DependencyProperty.Register("ShowDropDownSymbol", typeof(bool), typeof(RibbonDropDownButton), new UIPropertyMetadata(true));



        /// <summary>
        /// Gets or sets the maximum height for the dropdown panel.
        /// This is a dependency property.
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(RibbonDropDownButton), new UIPropertyMetadata(double.NaN));


        /// <summary>
        /// An item can be any possible element, not necassarily (but prefered) a RibbonMenuItem:
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return true;
            //return item is RibbonMenuItem || item is Separator;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonMenuItem();
        }


        public object DropDownFooter
        {
            get { return (object)GetValue(DropDownFooterProperty); }
            set { SetValue(DropDownFooterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownFooter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownFooterProperty =
            DependencyProperty.Register("DropDownFooter", typeof(object), typeof(RibbonDropDownButton), new UIPropertyMetadata(null));




        public object DropDownHeader
        {
            get { return (object)GetValue(DropDownHeaderProperty); }
            set { SetValue(DropDownHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownHeaderProperty =
            DependencyProperty.Register("DropDownHeader", typeof(object), typeof(RibbonDropDownButton), new UIPropertyMetadata(null));



        public DataTemplate DropDownHeaderTemplate
        {
            get { return (DataTemplate)GetValue(DropDownHeaderTemplateProperty); }
            set { SetValue(DropDownHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownHeaderTemplateProperty =
            DependencyProperty.Register("DropDownHeaderTemplate", typeof(DataTemplate), typeof(RibbonDropDownButton), new UIPropertyMetadata(null));



        public DataTemplate DropDownFooterTemplate
        {
            get { return (DataTemplate)GetValue(DropDownFooterTemplateProperty); }
            set { SetValue(DropDownFooterTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownFooterTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownFooterTemplateProperty =
            DependencyProperty.Register("DropDownFooterTemplate", typeof(DataTemplate), typeof(RibbonDropDownButton), new UIPropertyMetadata(null));

        protected void EnsurePopupRemainsOnMouseUp()
        {
            if (popup != null) popup.StaysOpen = true;
        }

        protected void EnsurePopupDoesNotStayOpen()
        {
            if (popup != null)
            {
                popup.StaysOpen = false;
            }
        }



        public PopupAnimation PopupAnimation
        {
            get { return (PopupAnimation)GetValue(PopupAnimationProperty); }
            set { SetValue(PopupAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupAnimationProperty =
            DependencyProperty.Register("PopupAnimation", typeof(PopupAnimation), typeof(RibbonDropDownButton), new UIPropertyMetadata(PopupAnimation.Slide));




        public PlacementMode PopupPlacement
        {
            get { return (PlacementMode)GetValue(PopupPlacementProperty); }
            set { SetValue(PopupPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupPlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register("PopupPlacement", typeof(PlacementMode), typeof(RibbonDropDownButton), new UIPropertyMetadata(PlacementMode.Bottom));




        /// <summary>
        /// Gets or sets the with for the drop down menu.
        /// This is a dependency property.
        /// </summary>
        public double DropDownWidth
        {
            get { return (double)GetValue(DropDownWidthProperty); }
            set { SetValue(DropDownWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownWidthProperty =
            DependencyProperty.Register("DropDownWidth", typeof(double), typeof(RibbonDropDownButton), new UIPropertyMetadata(double.NaN));



        //protected override System.Collections.IEnumerator LogicalChildren
        //{
        //    get
        //    {
        //        List<object> list = new List<object>();
        //        if (DropDownHeader != null) list.Add(DropDownHeader);
        //        if (DropDownFooter != null) list.Add(DropDownFooter);
        //        return list.GetEnumerator();
        //    }
        //}
    }
}

