using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.RenderableModel.Model.Renders
{
    /// <summary>
    /// How the dashes that run along the lines of a flow are drawn, see Shaders/Streamline.fx.
    /// </summary>
    /// <remarks>
    /// Every length here is in metres, the units the scene is drawn in, so that a setting means the
    /// same thing on a grid of any size. The render works them out from the style, which asks for
    /// them in cells and seconds, and from the size of a cell of its own grid.
    /// </remarks>
    public struct StreamlineLook
    {
        /// <summary>
        /// How wide a line is, in pixels.
        /// </summary>
        public float Width;

        /// <summary>
        /// How far along its own direction a vertex is looked for a second time, which is how the
        /// way a line runs across the screen is worked out. It only has to be long enough to stand
        /// out against the rounding of a float.
        /// </summary>
        public float TangentOffset;

        /// <summary>
        /// How far apart the heads of two dashes are.
        /// </summary>
        public float Spacing;

        /// <summary>
        /// How long the trail behind the head of a dash is, which is where the dash has faded away.
        /// </summary>
        public float Trail;
    }

    /// <summary>
    /// A render that can lay the flow out as lines that a shader runs along instead of as one arrow
    /// on a cell, see Shaders/Streamline.fx.
    /// </summary>
    public interface IStreamlineRender
    {
        /// <summary>
        /// True while the mesh holds lines rather than arrows.
        /// </summary>
        bool StreamlinesActive { get; }

        /// <summary>
        /// Three floats for every vertex of the mesh, in the order of VertexList: how far along its
        /// line the vertex lies, in the units the scene is drawn in, how fast the flow is there
        /// between 0 and 1, and which side of the line it lies on, 1 or -1. Null while the mesh
        /// holds arrows.
        /// </summary>
        float[] StreamlineFlow { get; }

        /// <summary>
        /// How fast a dash runs along a line, in metres per second.
        /// </summary>
        float FlowSpeed { get; }

        /// <summary>
        /// How the dashes are drawn.
        /// </summary>
        StreamlineLook Look { get; }
    }
}
