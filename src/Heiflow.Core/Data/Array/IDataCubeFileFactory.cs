using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public interface IDataCubeFileFactory
    {
        IEnumerable<IDataCubeProvider> Providers { get; }
        IDataCubeProvider Select(string filename);
    }
}
