///
///    This file is part of ILNumerics Community Edition.
///
///    ILNumerics Community Edition - high performance computing for applications.
///    Copyright (C) 2006 - 2013 Haymo Kutschbach, http://ilnumerics.net
///
///    ILNumerics Community Edition is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License version 3 as published by
///    the Free Software Foundation.
///
///    ILNumerics Community Edition is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with ILNumerics Community Edition. See the file License.txt in the root
///    of your distribution package. If not, see <http://www.gnu.org/licenses/>.
///
///    In addition this software uses the following components and/or licenses: 
///
///    =================================================================================
///    The Open Toolkit Library License
///    
///    Copyright (c) 2006 - 2009 the Open Toolkit library.
///    
///    Permission is hereby granted, free of charge, to any person obtaining a copy
///    of this software and associated documentation files (the "Software"), to deal
///    in the Software without restriction, including without limitation the rights to 
///    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
///    the Software, and to permit persons to whom the Software is furnished to do
///    so, subject to the following conditions:
///
///    The above copyright notice and this permission notice shall be included in all
///    copies or substantial portions of the Software.
///
///    =================================================================================
///    Intel® Math Kernel Library 11.1 for Windows
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Intel® Math Kernel Library 10.3 for Linux
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Products / Software which is implicitly used by ILNumerics due to the inclusion 
///    of 3rd party components: 
///  
///        BLAS/ LAPACK; see: http://netlib.org
///        FFT Functions; see: http://www.spiral.net, http://fftw.org
///        OpenGL; see: http://opengl.org
///
///    =================================================================================


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// The class provides texture storage for a single class of texures (e.g. one font)
    /// </summary>
    /// <remarks>The texture items are stored in a single texture sheet 
    /// and organized via a simple binary tree.</remarks>
    public abstract class ILTextureStorage {

        /// <summary>
        /// Fires, when the managed texture sheets are cleared. Registrars must rebuild needed items afterwards
        /// </summary>
        public event EventHandler Cleared;
        public void OnCleared() {
            if (Cleared != null) {
                Cleared(this, EventArgs.Empty); 
            }
        }
        public delegate ILTextureStorage TextureStorageFactory(); 

        #region attributes / properties
        protected Dictionary<string,ILTextureData> m_items;
        protected int m_height;
        protected int m_width;
        protected int? m_textureId; 
        /// <summary>
        ///  cache, which texture has been bound at last
        /// </summary>
        protected bool m_disposed = true; 
        protected static int[] m_tmpData; 
        protected Node m_root; 

        /// <summary>
        /// overall height of the internal texture sheet
        /// </summary>
        public int Height {
            get { return m_height; }
        }

        /// <summary>
        /// current width of the internal texture sheet
        /// </summary>
        public int Width {
            get { return m_width; }
        } 
        /// <summary>
        /// Key used to identify the texure in the graphic system 
        /// </summary>

        public static Size DefaultSize { get; set; }
        private static Dictionary<object, ILTextureStorage> Storages = new Dictionary<object,ILTextureStorage>(); 
        #endregion

        #region constructor
        static ILTextureStorage() {
            DefaultSize = new Size(1024,1024); 
        }
        /// <summary>
        /// construct new storage
        /// </summary>
        /// <param name="height">absolute height (permanent)</param>
        /// <param name="width">absolute width (permanent)</param>
        /// <remarks>Suggested size parameter will be increased to the next power of two.</remarks>
        public ILTextureStorage(Size size) {
            Create(size);
        }

        protected void Create(Size size) {
            m_items = new Dictionary<string, ILTextureData>();
            if (size.Width <= 0 || size.Height <= 0 || size.IsEmpty)
                throw new ArgumentOutOfRangeException("size", size, "Size greater than zero expected.");
            // make size a power of 2
            m_height = (int)Math.Pow(2, Math.Ceiling(Math.Log(size.Height, 2)));
            m_width = (int)Math.Pow(2, Math.Ceiling(Math.Log(size.Width, 2)));
            if (m_height > 1024) m_height = 1024; // limit to 1024 for compatibility reasons 
            if (m_width > 1024) m_width = 1024;
            m_root = new Node();
            m_root.Rect = new Rectangle(0, 0, m_width, m_height);
            InitTexture();
            m_disposed = false;
        }
        #endregion

        #region public / abstract interface
        public static ILTextureStorage GetStorage(object key, TextureStorageFactory factory) {
            ILTextureStorage ret;
            if (!Storages.TryGetValue(key, out ret)) {
                ret = factory(); 
                Storages[key] = ret; 
            }
            return ret; 
        }

        /// <summary>
        /// fetch texture item from storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual ILTextureData Get (string key) {
            ILTextureData ret = null; 
            if (!m_items.TryGetValue(key,out ret)) {
                return null; //throw new ArgumentException("no texture item has been found for key: " + key); 
            }
            return ret;
        }
        /// <summary>
        /// try to fetch item by key
        /// </summary>
        /// <param name="key">unique key</param>
        /// <param name="item">[output] item found</param>
        /// <returns>true: item was found, false otherwise</returns>
        public virtual bool TryGetTextureItem(string key, out ILTextureData item) {
            if (m_items.ContainsKey(key)) {
                item = m_items[key]; 
                return true; 
            }
            item = null; 
            return false; 
        }
        /// <summary>
        /// test, if a key exists in the texture storage
        /// </summary>
        /// <param name="key">unique key to be tested for</param>
        /// <returns>true if a texture item associated with that key exists, false otherwise</returns>
        public bool Exists (string key) {
            return m_items.ContainsKey(key); 
        }
        /// <summary>
        /// store bitmap into texture sheet
        /// </summary>
        /// <param name="key">unique key for item</param>
        /// <param name="data">item bitmap data</param>
        /// <param name="textureStorageSize">used rectangle in data bitmap</param>
        public virtual bool Store(string key, Bitmap data, RectangleF textureStorageSize, RectangleF textureGlyphRect, SizeF itemScreenSize) {
            if (textureStorageSize.Height > m_height || textureStorageSize.Width > m_width) 
                throw new ArgumentException("texture size is too large for this packer!");
            // add to packer  
            ILTextureData item; Node node;
            Rectangle itemRect = Rectangle.Ceiling(textureStorageSize); 
            System.Diagnostics.Debug.Assert(m_items.ContainsKey(key) == false); 
            //if (m_items.TryGetValue(key,out item)) {
            //    item.Height = itemRect.Height;
            //    item.Width = itemRect.Width; 
            //    // todo: remove item from packer & from texture sheet (somehow...[?])
            //    node = m_root.Insert(item);
            //    if (node == null) 
            //        return false; 
            //} else {
                item = new ILTextureData(itemRect.Height, itemRect.Width); 
                node = m_root.Insert(item); 
                if (node == null) 
                    return false; 
                m_items.Add(key, item);
            //} 
                //glyphRect.Offset(bmpRect.X,bmpRect.Y); 
                //item.TextureRectangle = RectangleF.FromLTRB((0.5f + node.Rect.Left + (glyphRect.X - bmpRect.X)) / m_width,
                //                                        (0.5f + node.Rect.Top + (glyphRect.Y - bmpRect.Y)) / m_height,
                //                                        (node.Rect.Right - 0.5f - (bmpRect.Right - glyphRect.Right)) / m_width,
                //                                        (node.Rect.Bottom - 0.5f - (bmpRect.Bottom - glyphRect.Bottom)) / m_height);
                //item.TextureRectangle = RectangleF.FromLTRB((node.Rect.Left + (textureGlyphRect.X - textureStorageSize.X)) / m_width,
                //                                        (node.Rect.Top + (textureGlyphRect.Y - textureStorageSize.Y)) / m_height,
                //                                        (node.Rect.Right - (textureStorageSize.Right - textureGlyphRect.Right)) / m_width,
                //                                        (node.Rect.Bottom - (textureStorageSize.Bottom - textureGlyphRect.Bottom)) / m_height);
                item.TextureRectangle = RectangleF.FromLTRB((node.Rect.Left - 0f + (textureGlyphRect.X - itemRect.X)) / m_width,
                                                        (node.Rect.Top - 0f + (textureGlyphRect.Y - itemRect.Y)) / m_height,
                                                        (node.Rect.Right + 0f - (itemRect.Right - textureGlyphRect.Right)) / m_width,
                                                        (node.Rect.Bottom + 0f - (itemRect.Bottom - textureGlyphRect.Bottom)) / m_height);
                item.GlyphScreenSize = itemScreenSize;
                //item.TextureRectangle = RectangleF.FromLTRB( (0f + node.Rect.Left) / m_width,
            //                                        (0f + node.Rect.Top) / m_height,
            //                                        (node.Rect.Right - 0f) / m_width,
            //                                        (node.Rect.Bottom - 0f) /m_height);            
            Store(data, textureStorageSize, node.Rect); 
            return true; 
        }
        /// <summary>
        /// initialize texture sheet 
        /// </summary>
        protected abstract void InitTexture();
        /// <summary>
        /// store item in texture sheet in GL
        /// </summary>
        /// <param name="data">new item bitmap data</param>
        /// <param name="location">area in bitmap data to be stored</param>
        /// <param name="rect">rectangle specifying area to store the data into,
        /// texture coords: range from 0...1.0</param>
        protected abstract void Store(Bitmap data, RectangleF location, RectangleF rect);
        /// <summary>
        /// select the texture storage as current in the GL
        /// </summary>
        /// <remarks>Calling this function before an storage / render operation is 
        /// obligatory in specific rendering machines (e.g. OpenGL). For GL's, where 
        /// it is not neccessary, the implementation must ignore any calls to this function.</remarks>
        public abstract void MakeCurrent(); 

        public abstract void Resize(Size size); 
        /// <summary>
        /// Dispose off any texture storage's ressources
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true); 
        }
        /// <summary>
        /// Dispose off manually
        /// </summary>
        /// <param name="manual"></param>
        /// <remarks>The true disposing is done in the concrete implementation.</remarks>
        public virtual void Dispose(bool manual) {
            if (!m_disposed) {
                if (manual) {
                    // free texture from GL 
                }
                m_disposed = true; 
            }
        }
        /// <summary>
        /// Finalizer, disposing ressources
        /// </summary>
        ~ILTextureStorage() {
            Dispose(false); 
        }
        private static ILSimpleTexInterpreter s_interpreter = new ILSimpleTexInterpreter();

        public delegate Bitmap TextRenderer(string item, Font font, out RectangleF textureStorageSize, out RectangleF textureGlyphSize, out SizeF glyphScreenSize, bool oversample = true);
        public virtual ILRenderQueueItem Store(string item, Font font, PointF offset, Color color, TextRenderer renderer = null, Func<string, Font, string> hashFunc = null) {
            string key;
            if (hashFunc == null) {
                key = ILHashCreator.Hash(item, font);
            } else {
                key = hashFunc(item, font);
            }
            ILTextureData texItem;
            RectangleF textureStorageSize, textureGlyphSize;
            SizeF glyphScreenSize; 
            if (item == "\r") {
                return new ILRenderQueueItem(key, new RectangleF(), new RectangleF(), Color.Empty) { Text = item };
            } else if (item == "\n") {
                return new ILRenderQueueItem(key, new RectangleF(0, 0, 0, font.Height), new RectangleF(), Color.Empty) { Text = item };
            } else if (!TryGetTextureItem(key, out texItem)) {
                if (renderer == null) {
                    renderer = s_interpreter.TransformItem;
                }
                Bitmap itemBMP = renderer(item, font, out textureStorageSize, out textureGlyphSize, out glyphScreenSize);
                if (!Store(key, itemBMP, textureStorageSize, textureGlyphSize, glyphScreenSize)) {
                    return null; 
                }
                texItem = Get(key);
                //return new ILRenderQueueItem(key, new RectangleF(offset, bmpSize.Size), texItem.TextureRectangle, color) { Text = item };
            }
            return new ILRenderQueueItem(key, new RectangleF(offset, texItem.GlyphScreenSize), texItem.TextureRectangle, color) { Text = item };
        }
        #endregion

        #region helper functions

        #endregion 

        #region Node - binary tree
        /// <summary>
        /// class representing a binary tree, used to manage the items on the texture sheet
        /// </summary>
        /// <remarks>This code is a slightly modified version of the OpenTK.Utilities framework
        /// TextPrinter/TextureStorage classes. See http://opentk.com for details.</remarks>
        protected class Node
        {
            public Node()
            {
            }

            Node left, right;
            RectangleF rect;
            int use_count;

            public RectangleF Rect { get { return rect; } set { rect = value; } }
            public Node Left { get { return left; } set { left = value; } }
            public Node Right { get { return right; } set { right = value; } }

            #region --- Constructor ---

            public bool Leaf
            {
                get { return left == null && right == null; }
            }

            #endregion

            #region public Node Insert(ILTextureData item)

            public Node Insert(ILTextureData item)
            {
                if (!this.Leaf)
                {
                    // Recurse towards left child, and if that fails, towards the right.
                    Node new_node = left.Insert(item);
                    return new_node ?? right.Insert(item);
                }
                else
                {
                    // We have recursed to a leaf.

                    // If it is not empty go back.
                    if (use_count != 0)
                        return null;

                    // If this leaf is too small go back.
                    if (rect.Width < item.Width || rect.Height < item.Height)
                        return null;

                    // If this leaf is the right size, insert here.
                    if (rect.Width == item.Width && rect.Height == item.Height)
                    {
                        use_count = 1;
                        return this;
                    }

                    // This leaf is too large, split it up. We'll decide which way to split
                    // by checking the width and height difference between this rectangle and
                    // out item's bounding box. If the width difference is larger, we'll split
                    // horizontaly, else verticaly.
                    left = new Node();
                    right = new Node();

                    float dw = this.rect.Width - item.Width + 1;
                    float dh = this.rect.Height - item.Height + 1;

                    if (dw > dh)
                    {
                        left.rect = new RectangleF(rect.Left, rect.Top, item.Width, rect.Height);
                        right.rect = new RectangleF(rect.Left + item.Width, rect.Top, rect.Width - item.Width, rect.Height);
                    }
                    else
                    {
                        left.rect = new RectangleF(rect.Left, rect.Top, rect.Width, item.Height);
                        right.rect = new RectangleF(rect.Left, rect.Top + item.Height, rect.Width, rect.Height - item.Height);
                    }

                    return left.Insert(item);
                }
            }

            #endregion

            #region public void Clear()
            
            public void Clear()
            {
                if (left != null)
                    left.Clear();
                if (right != null)
                    right.Clear();

                left = right = null;
            }

            #endregion
        }

        #endregion

    }
}
