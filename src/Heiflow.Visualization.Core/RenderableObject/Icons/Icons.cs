using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HUST.WREIS.Dot3D.Renderable
{
    /// <summary>
    /// Holds a collection of icons
    /// </summary>
    public class Icons : RenderableObjectList
    {
        /// <summary>
        /// Texture cache
        /// </summary>
        protected Hashtable m_textures = new Hashtable();

        protected Sprite m_sprite;

        static int hotColor = Color.White.ToArgb();
        static int normalColor = Color.FromArgb(150, 255, 255, 255).ToArgb();
        static int nameColor = Color.White.ToArgb();
        static int descriptionColor = Color.White.ToArgb();

        System.Timers.Timer refreshTimer;


        /// <summary>
        /// The closest icon the mouse is currently over
        /// </summary>
        protected Icon mouseOverIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.Icons"/> class 
        /// </summary>
        /// <param name="name"></param>
        public Icons(string name)
            : base(name)
        {
        }

        public Icons(string name,
            string dataSource,
            TimeSpan refreshInterval,
            World parentWorld,
            Cache cache)
            : base(name, dataSource, refreshInterval, parentWorld, cache)
        {
        }

        /// <summary>
        /// Adds an icon to this layer. Deprecated.
        /// </summary>
        public void AddIcon(Icon icon)
        {
            Add(icon);
        }

        #region RenderableObject methods

        /// <summary>
        /// Add a child object to this layer.
        /// </summary>
        public override void Add(RenderableObject ro)
        {
            m_children.Add(ro);
            isInitialized = false;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            if (!isOn)
                return;

            if (m_sprite != null)
            {
                m_sprite.Dispose();
                m_sprite = null;
            }

            m_sprite = new Sprite(drawArgs.device);

            System.TimeSpan smallestRefreshInterval = System.TimeSpan.MaxValue;

            // Load all textures
            foreach (RenderableObject ro in m_children)
            {
                Icon icon = ro as Icon;
                if (icon == null)
                {
                    // Child is not an icon
                    if (ro.IsOn)
                        ro.Initialize(drawArgs);
                    continue;
                }

                if (icon.RefreshInterval.TotalMilliseconds != 0 && icon.RefreshInterval != TimeSpan.MaxValue && icon.RefreshInterval < smallestRefreshInterval)
                    smallestRefreshInterval = icon.RefreshInterval;

                // Child is an icon
                icon.Initialize(drawArgs);

                object key = null;
                IconTexture iconTexture = null;

                if (icon.TextureFileName != null && icon.TextureFileName.Length > 0)
                {
                    if (icon.TextureFileName.ToLower().StartsWith("http://") && icon.SaveFilePath != null)
                    {
                        //download it
                        try
                        {
                            HUST.WREIS.Dot3D.Net.WebDownload webDownload = new HUST.WREIS.Dot3D.Net.WebDownload(icon.TextureFileName);
                            webDownload.DownloadType = HUST.WREIS.Dot3D.Net.DownloadType.Unspecified;

                            System.IO.FileInfo saveFile = new System.IO.FileInfo(icon.SaveFilePath);
                            if (!saveFile.Directory.Exists)
                                saveFile.Directory.Create();

                            webDownload.DownloadFile(saveFile.FullName);
                        }
                        catch { }

                        iconTexture = (IconTexture)m_textures[icon.SaveFilePath];
                        if (iconTexture == null)
                        {
                            key = icon.SaveFilePath;
                            iconTexture = new IconTexture(drawArgs.device, icon.SaveFilePath);
                        }
                    }
                    else
                    {
                        // Icon image from file
                        iconTexture = (IconTexture)m_textures[icon.TextureFileName];
                        if (iconTexture == null)
                        {
                            key = icon.TextureFileName;
                            iconTexture = new IconTexture(drawArgs.device, icon.TextureFileName);
                        }
                    }
                }
                else
                {
                    // Icon image from bitmap
                    if (icon.Image != null)
                    {
                        iconTexture = (IconTexture)m_textures[icon.Image];
                        if (iconTexture == null)
                        {
                            // Create new texture from image
                            key = icon.Image;
                            iconTexture = new IconTexture(drawArgs.device, icon.Image);
                        }
                    }
                }

                if (iconTexture == null)
                    // No texture set
                    continue;

                if (key != null)
                {
                    // New texture, cache it
                    m_textures.Add(key, iconTexture);

                    // Use default dimensions if not set
                    if (icon.Width == 0)
                        icon.Width = iconTexture.Width;
                    if (icon.Height == 0)
                        icon.Height = iconTexture.Height;
                }
            }

            // Compute mouse over bounding boxes
            foreach (RenderableObject ro in m_children)
            {
                Icon icon = ro as Icon;
                if (icon == null)
                    // Child is not an icon
                    continue;

                if (GetTexture(icon) == null)
                {
                    // Label only 
                    icon.SelectionRectangle = drawArgs.defaultDrawingFont.MeasureString(null, icon.Name, DrawTextFormat.None, 0);
                }
                else
                {
                    // Icon only
                    icon.SelectionRectangle = new Rectangle(0, 0, icon.Width, icon.Height);
                }

                // Center the box at (0,0)
                icon.SelectionRectangle.Offset(-icon.SelectionRectangle.Width / 2, -icon.SelectionRectangle.Height / 2);
            }

            if (refreshTimer == null && smallestRefreshInterval != TimeSpan.MaxValue)
            {
                refreshTimer = new System.Timers.Timer(smallestRefreshInterval.TotalMilliseconds);
                refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(refreshTimer_Elapsed);
                refreshTimer.Start();
            }

            isInitialized = true;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (m_textures != null)
            {
                foreach (IconTexture iconTexture in m_textures.Values)
                    iconTexture.Texture.Dispose();
                m_textures.Clear();
            }

            if (m_sprite != null)
            {
                m_sprite.Dispose();
                m_sprite = null;
            }

            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                refreshTimer = null;
            }
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!isInitialized)
                Initialize(drawArgs);
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

                //if (!drawArgs.WorldCamera.ViewFrustum.ContainsPoint(icon.Position))
                //    continue;

                Vector3 referenceCenter = new Vector3(
                    (float)drawArgs.WorldCamera.ReferenceCenter.X,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Y,
                    (float)drawArgs.WorldCamera.ReferenceCenter.Z);

                Vector3 projectedPoint = drawArgs.WorldCamera.Project(icon.Position - referenceCenter);
                if (!icon.SelectionRectangle.Contains(
                    DrawArgs.LastMousePosition.X - (int)projectedPoint.X,
                    DrawArgs.LastMousePosition.Y - (int)projectedPoint.Y))
                    continue;

                try
                {
                    if (DrawArgs.IsLeftMouseButtonDown && !DrawArgs.IsRightMouseButtonDown)
                    {
                        if (icon.OnClickZoomAltitude != double.NaN || icon.OnClickZoomHeading != double.NaN || icon.OnClickZoomTilt != double.NaN)
                        {
                            drawArgs.WorldCamera.SetPosition(
                                icon.Latitude,
                                icon.Longitude,
                                icon.OnClickZoomHeading,
                                icon.OnClickZoomAltitude,
                                icon.OnClickZoomTilt);
                        }
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

                        if (icon.ContextMenu != null && icon.ParentControl != null)
                        {
                            Point p = new Point(DrawArgs.LastMousePosition.X, DrawArgs.LastMousePosition.Y);
                            icon.ContextMenu.Show(icon.ParentControl, p);
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

        public override void Render(DrawArgs drawArgs)
        {
            if (!isOn)
                return;

            if (!isInitialized)
                return;

            // First render everything except icons
            foreach (RenderableObject ro in m_children)
            {
                //	if(ro is Icon)
                //		continue;

                if (!ro.IsOn)
                    continue;

                // Child is not an icon
                ro.Render(drawArgs);
            }

            int closestIconDistanceSquared = int.MaxValue;
            Icon closestIcon = null;

            // Now render just the icons
            m_sprite.Begin(SpriteFlags.AlphaBlend);
            foreach (RenderableObject ro in m_children)
            {
                if (!ro.IsOn)
                    continue;

                Icon icon = ro as Icon;
                if (icon == null)
                    continue;

                Vector3 translationVector = new Vector3(
                (float)(icon.PositionD.X - drawArgs.WorldCamera.ReferenceCenter.X),
                (float)(icon.PositionD.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                (float)(icon.PositionD.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

                // Find closest mouse-over icon
                Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

                int dx = DrawArgs.LastMousePosition.X - (int)projectedPoint.X;
                int dy = DrawArgs.LastMousePosition.Y - (int)projectedPoint.Y;
                if (icon.SelectionRectangle.Contains(dx, dy))
                {
                    // Mouse is over, check whether this icon is closest
                    int distanceSquared = dx * dx + dy * dy;
                    if (distanceSquared < closestIconDistanceSquared)
                    {
                        closestIconDistanceSquared = distanceSquared;
                        closestIcon = icon;
                    }
                }

                if (icon != mouseOverIcon)
                    Render(drawArgs, icon, projectedPoint);
            }

            // Render the mouse over icon last (on top)
            if (mouseOverIcon != null)
            {
                Vector3 translationVector = new Vector3(
                    (float)(mouseOverIcon.PositionD.X - drawArgs.WorldCamera.ReferenceCenter.X),
                    (float)(mouseOverIcon.PositionD.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                    (float)(mouseOverIcon.PositionD.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

                Render(drawArgs, mouseOverIcon, drawArgs.WorldCamera.Project(translationVector));
            }

            mouseOverIcon = closestIcon;

            m_sprite.End();
        }

        #endregion

        public event ClickableIconSelected IconLeftButtonClicked;
        public void OnIconLeftButtonClicked(object sender, ClickableIconEventArgs e)
        {
            if (IconLeftButtonClicked != null)
                IconLeftButtonClicked(sender, e);
        }
        /// <summary>
        /// Draw the icon
        /// </summary>
        protected virtual void Render(DrawArgs drawArgs, Icon icon, Vector3 projectedPoint)
        {
            if (!icon.isInitialized)
                icon.Initialize(drawArgs);

            if (!drawArgs.WorldCamera.ViewFrustum.ContainsPoint(icon.Position))
                return;

            // Check icons for within "visual" range
            double distanceToIcon = Vector3.Length(icon.Position - drawArgs.WorldCamera.Position);
            if (distanceToIcon > icon.MaximumDisplayDistance)
                return;
            if (distanceToIcon < icon.MinimumDisplayDistance)
                return;

            IconTexture iconTexture = GetTexture(icon);
            bool isMouseOver = icon == mouseOverIcon;
            if (isMouseOver)
            {
                // Mouse is over
                isMouseOver = true;

                if (icon.isSelectable)
                {
                    DrawArgs.MouseCursor = CursorType.Hand;
                    OnIconLeftButtonClicked(this, new ClickableIconEventArgs(drawArgs) { Icon = icon });
                }

                string description = icon.Description;
                if (description == null)
                    description = icon.ClickableActionURL;
                if (description != null)
                {
                    // Render description field
                    DrawTextFormat format = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Top;
                    int left = 10;
                    //if (World.Settings.showLayerManager)
                    //    left += World.Settings.layerManagerWidth;
                    Rectangle rect = Rectangle.FromLTRB(left, 10, drawArgs.screenWidth - 10, drawArgs.screenHeight - 10);

                    // Draw outline
                    drawArgs.defaultDrawingFont.DrawText(
                        m_sprite, description,
                        rect,
                        format, 0xb0 << 24);

                    rect.Offset(2, 0);
                    drawArgs.defaultDrawingFont.DrawText(
                        m_sprite, description,
                        rect,
                        format, 0xb0 << 24);

                    rect.Offset(0, 2);
                    drawArgs.defaultDrawingFont.DrawText(
                        m_sprite, description,
                        rect,
                        format, 0xb0 << 24);

                    rect.Offset(-2, 0);
                    drawArgs.defaultDrawingFont.DrawText(
                        m_sprite, description,
                        rect,
                        format, 0xb0 << 24);

                    // Draw description
                    rect.Offset(1, -1);
                    drawArgs.defaultDrawingFont.DrawText(
                        m_sprite, description,
                        rect,
                        format, descriptionColor);
                }
            }

            int color = isMouseOver ? hotColor : normalColor;
            if (iconTexture == null || isMouseOver || icon.NameAlwaysVisible)
            {
                // Render label
                if (icon.Name != null)
                {
                    // Render name field
                    const int labelWidth = 1000; // Dummy value needed for centering the text
                    if (iconTexture == null)
                    {
                        // Center over target as we have no bitmap
                        Rectangle rect = new Rectangle(
                            (int)projectedPoint.X - (labelWidth >> 1),
                            (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1)),
                            labelWidth,
                            drawArgs.screenHeight);

                        drawArgs.defaultDrawingFont.DrawText(m_sprite, icon.Name, rect, DrawTextFormat.Center, color);
                    }
                    else
                    {
                        // Adjust text to make room for icon
                        int spacing = (int)(icon.Width * 0.3f);
                        if (spacing > 10)
                            spacing = 10;
                        int offsetForIcon = (icon.Width >> 1) + spacing;

                        Rectangle rect = new Rectangle(
                            (int)projectedPoint.X + offsetForIcon,
                            (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1)),
                            labelWidth,
                            drawArgs.screenHeight);

                        drawArgs.defaultDrawingFont.DrawText(m_sprite, icon.Name, rect, DrawTextFormat.WordBreak, color);
                    }
                }
            }

            if (iconTexture != null)
            {
                // Render icon
                float xscale = (float)icon.Width / iconTexture.Width;
                float yscale = (float)icon.Height / iconTexture.Height;
                m_sprite.Transform = Matrix.Scaling(xscale, yscale, 0);

                if (icon.IsRotated)
                    m_sprite.Transform *= Matrix.RotationZ((float)icon.Rotation.Radians - (float)drawArgs.WorldCamera.Heading.Radians);

                m_sprite.Transform *= Matrix.Translation(projectedPoint.X, projectedPoint.Y, 0);
                m_sprite.Draw(iconTexture.Texture,
                    new Vector3(iconTexture.Width >> 1, iconTexture.Height >> 1, 0),
                    Vector3.Empty,
                    color);

                // Reset transform to prepare for text rendering later
                m_sprite.Transform = Matrix.Identity;
            }
        }

        /// <summary>
        /// Retrieve an icon's texture
        /// </summary>
        protected IconTexture GetTexture(Icon icon)
        {
            object key = null;

            if (icon.Image == null)
            {
                key = (icon.TextureFileName.ToLower().StartsWith("http://") ? icon.SaveFilePath : icon.TextureFileName);
            }
            else
            {
                key = icon.Image;
            }
            if (key == null)
                return null;

            IconTexture res = (IconTexture)m_textures[key];
            return res;
        }

        bool isUpdating = false;
        private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (isUpdating)
                return;
            isUpdating = true;
            try
            {
                for (int i = 0; i < this.ChildObjects.Count; i++)
                {
                    RenderableObject ro = (RenderableObject)this.ChildObjects[i];
                    if (ro != null && ro.IsOn && ro is Icon)
                    {
                        Icon icon = (Icon)ro;

                        if (icon.RefreshInterval == TimeSpan.MaxValue || icon.LastRefresh > System.DateTime.Now - icon.RefreshInterval)
                            continue;

                        object key = null;
                        IconTexture iconTexture = null;

                        if (icon.TextureFileName != null && icon.TextureFileName.Length > 0)
                        {
                            if (icon.TextureFileName.ToLower().StartsWith("http://") && icon.SaveFilePath != null)
                            {
                                //download it
                                try
                                {
                                    HUST.WREIS.Dot3D.Net.WebDownload webDownload = new HUST.WREIS.Dot3D.Net.WebDownload(icon.TextureFileName);
                                    webDownload.DownloadType = HUST.WREIS.Dot3D.Net.DownloadType.Unspecified;

                                    System.IO.FileInfo saveFile = new System.IO.FileInfo(icon.SaveFilePath);
                                    if (!saveFile.Directory.Exists)
                                        saveFile.Directory.Create();

                                    webDownload.DownloadFile(saveFile.FullName);
                                }
                                catch { }

                                iconTexture = (IconTexture)m_textures[icon.SaveFilePath];
                                if (iconTexture != null)
                                {
                                    IconTexture tempTexture = iconTexture;
                                    m_textures[icon.SaveFilePath] = new IconTexture(DrawArgs.Device, icon.SaveFilePath);
                                    tempTexture.Dispose();
                                }
                                else
                                {
                                    key = icon.SaveFilePath;
                                    iconTexture = new IconTexture(DrawArgs.Device, icon.SaveFilePath);

                                    // New texture, cache it
                                    m_textures.Add(key, iconTexture);

                                    // Use default dimensions if not set
                                    if (icon.Width == 0)
                                        icon.Width = iconTexture.Width;
                                    if (icon.Height == 0)
                                        icon.Height = iconTexture.Height;
                                }

                            }
                            else
                            {
                                // Icon image from file
                                iconTexture = (IconTexture)m_textures[icon.TextureFileName];
                                if (iconTexture != null)
                                {
                                    IconTexture tempTexture = iconTexture;
                                    m_textures[icon.SaveFilePath] = new IconTexture(DrawArgs.Device, icon.TextureFileName);
                                    tempTexture.Dispose();
                                }
                                else
                                {
                                    key = icon.SaveFilePath;
                                    iconTexture = new IconTexture(DrawArgs.Device, icon.TextureFileName);

                                    // New texture, cache it
                                    m_textures.Add(key, iconTexture);

                                    // Use default dimensions if not set
                                    if (icon.Width == 0)
                                        icon.Width = iconTexture.Width;
                                    if (icon.Height == 0)
                                        icon.Height = iconTexture.Height;
                                }
                            }
                        }
                        else
                        {
                            // Icon image from bitmap
                            if (icon.Image != null)
                            {
                                iconTexture = (IconTexture)m_textures[icon.Image];
                                if (iconTexture != null)
                                {
                                    IconTexture tempTexture = iconTexture;
                                    m_textures[icon.SaveFilePath] = new IconTexture(DrawArgs.Device, icon.Image);
                                    tempTexture.Dispose();
                                }
                                else
                                {
                                    key = icon.SaveFilePath;
                                    iconTexture = new IconTexture(DrawArgs.Device, icon.Image);

                                    // New texture, cache it
                                    m_textures.Add(key, iconTexture);

                                    // Use default dimensions if not set
                                    if (icon.Width == 0)
                                        icon.Width = iconTexture.Width;
                                    if (icon.Height == 0)
                                        icon.Height = iconTexture.Height;
                                }
                            }
                        }

                        icon.LastRefresh = System.DateTime.Now;
                    }
                }
            }
            catch { }
            finally
            {
                isUpdating = false;
            }
        }
    }
}
