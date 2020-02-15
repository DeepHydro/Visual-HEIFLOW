using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic
{
   public class LoadingObjectState
    {
       public LoadingObjectState()
       {
           State = LoadingState.Normal;
           Message = "None";
           Object = null;
       }

       public LoadingState State
       {
           get;
           set;
       }

       public object Object
       {
           get;
           set;
       }

       public string Message
       {
           get;
           set;
       }

       public DataCube<float> DataCube
       {
           get;
           set;
       }
    }
}
