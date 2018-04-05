// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
namespace  Heiflow.AI.GeneticProgramming
{
    //terminali u GP sa sastoje od nezavisno promjenjivih i slucajno generiranih konstanti
    //ova klasa sadrzi naziv terminala koji je pohranjen pod tim nazivom u experimentalnim podacima
    // i slucalno generiranim konstantama
    // npr x1, x2, ... ili R1, R2 ...
    [Serializable]
    public class GPTerminal
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public bool IsConstant { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
