// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Excel.Core.BinaryFormat
{
    /// <summary>
    /// If present the Calculate Message was in the status bar when Excel saved the file.
    /// This occurs if the sheet changed, the Manual calculation option was on, and the Recalculate Before Save option was off.    
    /// </summary>
    internal class XlsBiffUncalced : XlsBiffRecord
    {
        internal XlsBiffUncalced(byte[] bytes, uint offset, ExcelBinaryReader reader)
            : base(bytes, offset, reader)
        {
        }

    }
}