using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.IO
{
   public class DataCubeFileInfo
    {
       public DataCubeFileInfo()
       {

       }

       public string[] VariableNames
       {
           get;
           set;
       }
       public int VariableNum
       {
           get;
           set;
       }
       public int CellNum
       {
           get;
           set;
       }

       public int TotalTimeSteps
       {
           get;
           set;
       }
    }
}
