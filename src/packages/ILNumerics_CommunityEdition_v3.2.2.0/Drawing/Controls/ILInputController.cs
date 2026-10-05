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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics.Drawing.Plotting;

namespace ILNumerics.Drawing {
    public class ILInputController {

        private enum Methods {
            MouseMove,
            MouseClick,
            MouseDoubleClick,
            MouseDown,
            MouseUp,
            MouseEnter,
            MouseLeave,
            MouseWheel
        }

        #region attributes
        private ILNode m_nodeUnderCursor;
        private ILNode m_currentDragTarget; 
        #endregion

        #region properties
        private ILNode CurrentDragTarget { 
            get { return m_currentDragTarget; }
            set { m_currentDragTarget = value; }
        }
        internal IILDriver Driver { get; set; }
        public ILNode NodeUnderCursor {
            get { return m_nodeUnderCursor; }
        }
        private ILGroup Root {
            get {
                return Driver.SceneSyncRoot;
            }
        }
        #endregion

        #region public interface 
        protected void SetNodeUnderCursor(ILMouseEventArgs e) {
            if (e.Target != m_nodeUnderCursor) {
                if (m_nodeUnderCursor != null) {
                    // mouse leave 
                    ILMouseEventArgs oldArgs = e.Clone();
                    oldArgs.Target = m_nodeUnderCursor;
                    OnMouseLeave(oldArgs);
                }
                m_nodeUnderCursor = e.Target;
                OnMouseEnter(e);
            }
        }
        #endregion

        #region ctors
        public ILInputController(IILDriver driver) {
            Driver = driver; 
        }
        #endregion

        #region event handlers 
        public virtual void OnMouseWheel(ILMouseEventArgs e) {
            e.Target = NodeUnderCursor;
            ProcessEvent(e, Methods.MouseWheel);
        }
        public virtual void OnMouseClick(ILMouseEventArgs e) {
            e.Target = NodeUnderCursor;
            ProcessEvent(e, Methods.MouseClick);
        }
        public virtual void OnMouseDoubleClick(ILMouseEventArgs e) {
            e.Target = NodeUnderCursor;
            ProcessEvent(e, Methods.MouseDoubleClick);
        }
        public virtual void OnMouseDown(ILMouseEventArgs e) {
            e.Target = NodeUnderCursor; 
            ProcessEvent(e, Methods.MouseDown);
            CurrentDragTarget = NodeUnderCursor; 
        }
        public virtual void OnMouseUp(ILMouseEventArgs e) {
            if (CurrentDragTarget != null) {
                e.Target = CurrentDragTarget;
                CurrentDragTarget = null;
            } else {
                e.Target = NodeUnderCursor;
            }
            ProcessEvent(e, Methods.MouseUp); 
        }
        public virtual void OnMouseMove(ILMouseEventArgs e) {
            if (CurrentDragTarget == null) {
                int? id = Driver.PickAt(e.Location, e.TimeMS);
                ILNode node = null; 
                if (id.HasValue) {
                    node = Driver.SceneSyncRoot.FindById<ILNode>(id.Value);
                    if (node == null) {
                        node = Driver.LocalSceneSyncRoot.FindById<ILNode>(id.Value);
                    }
                }
                ILMouseEventArgs ecopy = e.Clone();
                ecopy.Target = node;
                SetNodeUnderCursor(ecopy);
                e.Target = node; 
            } else { 
                e.Target = CurrentDragTarget;
            }
            ProcessEvent(e, Methods.MouseMove); 
        }
        public virtual void OnMouseEnter(ILMouseEventArgs e) {
            // Target has been set in SetNodeUnderCursor()
            ProcessEvent(e, Methods.MouseEnter, false); 
        }
        public virtual void OnMouseLeave(ILMouseEventArgs e) {
            // Target has been set in SetNodeUnderCursor()
            ProcessEvent(e, Methods.MouseLeave, false);
        }
        #endregion 

        #region private helpers
        private void ProcessEvent(ILMouseEventArgs e, Methods method, bool bubbleUp = true) {
            if (e.Target == null) return; 
            // acquire the path from target to root
            Stack<ILNode> path = new Stack<ILNode>();
            ILNode cur = e.Target.Parent;
            while (cur != null) {
                path.Push(cur);
                cur = cur.Parent;
            }
            // capture
            e.DirectionUp = false; 
            while (path.Count > 0) {
                cur = path.Pop(); 
                cur.TranslateEventLocation(true, e); 
                switch (method) {
                    case Methods.MouseMove:
                        cur.OnMouseMove(e);
                        break;
                    case Methods.MouseClick:
                        cur.OnMouseClick(e);
                        break;
                    case Methods.MouseDoubleClick:
                        cur.OnMouseDoubleClick(e);
                        break;
                    case Methods.MouseDown:
                        cur.OnMouseDown(e);
                        break;
                    case Methods.MouseUp:
                        cur.OnMouseUp(e);
                        break;
                    case Methods.MouseEnter:
                        cur.OnMouseEnter(e);
                        break;
                    case Methods.MouseLeave:
                        cur.OnMouseLeave(e);
                        break;
                    case Methods.MouseWheel:
                        cur.OnMouseWheel(e);
                        break;
                    default:
                        path.Pop();
                        break;
                }
                if (e.Cancel) break;
            }
            if (!e.Cancel) {
                e.DirectionUp = true;
                // bubbling
                cur = e.Target;
                // translate viewport for target event node 
                cur.TranslateEventLocation(true, e);
                while (cur != null) {
                    switch (method) {
                        case Methods.MouseMove:
                            cur.OnMouseMove(e);
                            break;
                        case Methods.MouseClick:
                            cur.OnMouseClick(e);
                            break;
                        case Methods.MouseDoubleClick:
                            cur.OnMouseDoubleClick(e);
                            break;
                        case Methods.MouseDown:
                            cur.OnMouseDown(e);
                            break;
                        case Methods.MouseUp:
                            cur.OnMouseUp(e);
                            break;
                        case Methods.MouseEnter:
                            cur.OnMouseEnter(e);
                            break;
                        case Methods.MouseLeave:
                            cur.OnMouseLeave(e);
                            break;
                        case Methods.MouseWheel:
                            cur.OnMouseWheel(e);
                            break;
                        default:
                            break;
                    }
                    cur.TranslateEventLocation(false, e);
                    if (bubbleUp) {
                        cur = cur.Parent;
                    }  else {
                        cur = null;
                    }
                }
            }
            if (e.Refresh) {
                Driver.Render(); 
            }
        }
        internal static void checkFilterDirection(ILMouseEventArgs e, out bool useX, out bool useY, PointF mouseDown) {
            if (!e.AltPressed) {
                useX = true;
                useY = true;
            } else {
                float distX = Math.Abs(mouseDown.X - e.LocationF.X);
                float distY = Math.Abs(mouseDown.Y - e.LocationF.Y);
                if (distX > distY) {
                    useX = true;
                    useY = false;
                } else {
                    useX = false;
                    useY = true;
                }
            }
        }
        #endregion

    }
}
