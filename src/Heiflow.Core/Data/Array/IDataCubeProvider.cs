using Heiflow.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public interface IDataCubeProvider : IFileProvider
    {
        DataCube<float> Provide(string filename);
        DataCube<float> ProvideSingleStep(string filename);
    }
}
