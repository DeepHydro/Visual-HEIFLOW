// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace  Heiflow.AI.GeneticProgramming
{
    [Serializable]
    public class FunctionTree : ICloneable
    {
        public short NodeValue;
        public List<FunctionTree> SubFunctionTree;
        //CTOR
        public FunctionTree()
        {
            NodeValue = -1;
        }
        public FunctionTree(FunctionTree node)
        {
            NodeValue = node.NodeValue;
            SubFunctionTree = node.SubFunctionTree;
        }
        #region ICloneable Members

        public object Clone()
        {
            FunctionTree clone = new FunctionTree();

            // clone node2 value
            clone.NodeValue = this.NodeValue;

            // clone its subtrees
            if (this.SubFunctionTree != null)
            {
                clone.SubFunctionTree = new List<FunctionTree>();
                // clone each child gene
                for (int i = 0; i < this.SubFunctionTree.Count; i++)
                {
                    FunctionTree cl = (FunctionTree)this.SubFunctionTree[i].Clone();
                    clone.SubFunctionTree.Add(cl);
                }
            }
            return clone;
        }

        #endregion

        /// <summary>
        /// String reprezentacija čvora
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (SubFunctionTree != null)
                // pretraži sve čvorove
                for (int i = 0; i < SubFunctionTree.Count; i++)
                    sb.Append(SubFunctionTree[i].ToString());

            // dodavanja vrijednosti genu
            sb.Append(NodeValue.ToString());
            sb.Append(";");
            return sb.ToString();
        }
        public static void ToListExpression(List<int> lstExpr, FunctionTree node)
        {
            //If subFunctTree is not null
            if (node.SubFunctionTree != null)
            {
                Debug.Assert(node.SubFunctionTree.Count != 0);
                // pretraži sve čvorove
                for (int i = 0; i < node.SubFunctionTree.Count; i++)
                    ToListExpression(lstExpr, node.SubFunctionTree[i]);
            }
            lstExpr.Add(node.NodeValue);
        }
    }
}
