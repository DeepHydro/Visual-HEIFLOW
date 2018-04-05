// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.GeneticProgramming
{
    public class AlphaCharEnum
    {
        char[] alphabet = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
                          'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 
                          'U', 'V', 'W', 'X', 'Y', 'Z' };

        public string AlphabetFromIndex(int index)
        {
            if (index == 0)
                return "";
            int firstLetter = index / 26;
            int secondLetter = index % 26;
            if (firstLetter == 0)
            {
                return alphabet[secondLetter - 1].ToString();
            }
            else
            {
                if (firstLetter > 26)
                    return "";//Not support number
                else if (secondLetter == 0 && firstLetter == 1)
                    return alphabet[25].ToString();
                else if (secondLetter == 0 && firstLetter < 1)
                    return alphabet[firstLetter - 1].ToString();
                else if (secondLetter == 0 && firstLetter > 1)
                    return alphabet[firstLetter - 2].ToString() + "Z";
                else
                    return alphabet[firstLetter - 1].ToString() + alphabet[secondLetter - 1].ToString();
            }

        }
    }
}
