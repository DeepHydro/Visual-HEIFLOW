//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

namespace  Heiflow.AI.Genetic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents tree node of genetic programming tree.
    /// </summary>
    /// 
    /// <remarks><para>In genetic programming a chromosome is represented by a tree, which
    /// is represented by <see cref="GPTreeChromosome"/> class. The <see cref="GPTreeNode"/>
    /// class represents single node of such genetic programming tree.</para>
    /// 
    /// <para>Each node may or may not have children. This means that particular node of a genetic
    /// programming tree may represent its sub tree or even entire tree.</para>
    /// </remarks>
    /// 
    public class GPTreeNode : ICloneable
    {
        /// <summary>
        /// Gene represented by the chromosome.
        /// </summary>
        public IGPGene Gene;

        /// <summary>
        /// List of node's children.
        /// </summary>
        public List<GPTreeNode> Children;

        /// <summary>
        /// Initializes a new instance of the <see cref="GPTreeNode"/> class.
        /// </summary>
        /// 
        internal GPTreeNode( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GPTreeNode"/> class.
        /// </summary>
        /// 
        public GPTreeNode( IGPGene gene )
        {
            Gene = gene;
        }

        /// <summary>
        /// Get string representation of the node.
        /// </summary>
        /// 
        /// <returns>Returns string representation of the node.</returns>
        /// 
        /// <remarks><para>String representation of the node lists all node's children and
        /// then the node itself. Such node's string representations equals to
        /// its reverse polish notation.</para>
        /// 
        /// <para>For example, if nodes value is '+' and its children are '3' and '5', then
        /// nodes string representation is "3 5 +".</para>
        /// </remarks>
        /// 
        public override string ToString( )
        {
            StringBuilder sb = new StringBuilder( );

            if ( Children != null )
            {
                // walk through all nodes
                foreach ( GPTreeNode node in Children )
                {
                    sb.Append( node.ToString( ) );
                }
            }

            // add gene value
            sb.Append( Gene.ToString( ) );
            sb.Append( " " );

            return sb.ToString( );
        }

        /// <summary>
        /// Clone the tree node.
        /// </summary>
        /// 
        /// <returns>Returns exact clone of the node.</returns>
        /// 
        public object Clone( )
        {
            GPTreeNode clone = new GPTreeNode( );

            // clone gene
            clone.Gene = this.Gene.Clone( );
            // clone its children
            if ( this.Children != null )
            {
                clone.Children = new List<GPTreeNode>( );
                // clone each child gene
                foreach ( GPTreeNode node in Children )
                {
                    clone.Children.Add( (GPTreeNode) node.Clone( ) );
                }
            }
            return clone;
        }
    }
}
