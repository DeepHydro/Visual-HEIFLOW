using Heiflow.Core.Data;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Heiflow.Models.IO
{
    [Export(typeof(IDataCubeFileFactory))]
    public  class DataCubeFileFactory:IDataCubeFileFactory
    {
           public DataCubeFileFactory()
           {

           }
           private List<IDataCubeProvider> list = new List<IDataCubeProvider>();

           [ImportMany(AllowRecomposition = true)]
           public IEnumerable<IDataCubeProvider> Providers
           {
               get;
               private set;
           }

           public IDataCubeProvider Select(string filename)
           {
               IDataCubeProvider result = null;
               var extension = Path.GetExtension(filename);
               foreach (var provider in Providers)
               {
                   if (provider.Extension == extension)
                   {
                       result = provider;
                       break;
                   }
               }
               return result;
           }

           public void Add(IDataCubeProvider provider)
           {
               if (!list.Contains(provider))
               {
                   list.Add(provider);
               }
           }

           public void Composite()
           {
               Providers = list;
           }
    }
}
