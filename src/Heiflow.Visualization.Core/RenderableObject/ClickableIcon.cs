using HUST.WREIS.Dot3D.Renderable;
using Microsoft.DirectX;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace HUST.WREIS.Dot3D
{
    public delegate void ClickableIconSelected(object sender, ClickableIconEventArgs e);
    
    public class ClickableIconEventArgs:EventArgs
    {
        public ClickableIconEventArgs(DrawArgs drawArgs)
        {
            mDrawArgs = drawArgs;
        }
        private DrawArgs mDrawArgs;
        private Icon mIcon;

        public DrawArgs DrawArgs
        {
            get
            {
                return mDrawArgs;
            }
        }

        public Icon Icon
        {
            get
            {
                return mIcon;
            }
            set
            {
                mIcon = value;
            }
        }
    }
    
    public class ClickableIcons:Icons
    {
        private bool _Clickable = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataSource"></param>
        /// <param name="refreshInterval"></param>
        /// <param name="parentWorld"></param>
        /// <param name="cache"></param>
		public ClickableIcons(string name, string dataSource, TimeSpan refreshInterval, World parentWorld, Cache cache)
            : base(name,dataSource,refreshInterval,parentWorld,cache)
		{
			this.RenderPriority = RenderPriority.Icons;
		}


        public ClickableIcons(string name):base(name)
        {
             
        }
        [Category("Behavior")]
        public bool Clickable
        {
            get
            {
                return _Clickable;
            }
            set
            {
                _Clickable = value;
            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            foreach (RenderableObject ro in m_children)
            {
                if (!ro.IsOn)
                    continue;
                if (!ro.isSelectable)
                    continue;

                Icon icon = ro as Icon;
                if (icon == null)
                {
                    // Child is not an icon
                    if (ro.PerformSelectionAction(drawArgs))
                        return true;
                    continue;
                }

                if (!drawArgs.WorldCamera.ViewFrustum.ContainsPoint(icon.Position))
                {
                    continue;
                }

                Vector3 referenceCenter = new Vector3(
                    (float)drawArgs.WorldCamera.ReferenceCenter.X,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Z);

                Vector3 projectedPoint = drawArgs.WorldCamera.Project(icon.Position - referenceCenter);
                if (!icon.SelectionRectangle.Contains(
                    DrawArgs.LastMousePosition.X - (int)projectedPoint.X,
                    DrawArgs.LastMousePosition.Y - (int)projectedPoint.Y))
                {
                    continue;
                }

                try
                {
                    if (DrawArgs.IsLeftMouseButtonDown && !DrawArgs.IsRightMouseButtonDown)
                    {
                       ClickableIconEventArgs e=new ClickableIconEventArgs(drawArgs);
                       e.Icon = icon;
                        OnIconLeftButtonClicked(this,e);
                        //if (icon.OnClickZoomAltitude != double.NaN || icon.OnClickZoomHeading != double.NaN || icon.OnClickZoomTilt != double.NaN)
          
                        //{
                        //    drawArgs.WorldCamera.SetPosition(
                        //        icon.Latitude,
                        //        icon.Longitude,
                        //        icon.OnClickZoomHeading,
                        //        icon.OnClickZoomAltitude,
                        //        icon.OnClickZoomTilt);
                        //}
                  
                    }
                    else if (!DrawArgs.IsLeftMouseButtonDown && DrawArgs.IsRightMouseButtonDown)
                    {
                        ScreenOverlay[] overlays = icon.Overlays;
                        if (overlays != null && overlays.Length > 0)
                        {
                            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
                            foreach (ScreenOverlay curOverlay in overlays)
                            {
                                contextMenu.MenuItems.Add(curOverlay.Name, new System.EventHandler(icon.OverlayOnOpen));
                            }
                            contextMenu.Show(DrawArgs.ParentControl, DrawArgs.LastMousePosition);
                        }

                        if (icon.ContextMenu != null)
                        {
                            System.Drawing.Point p = new System.Drawing.Point(DrawArgs.LastMousePosition.X, DrawArgs.LastMousePosition.Y);
                            icon.ContextMenu.Show(DrawArgs.ParentControl, p);                       
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            return false;
        }
       
    
    }
}
