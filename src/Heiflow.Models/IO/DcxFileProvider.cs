using Heiflow.Core.Data;
using Heiflow.Core.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    [Export(typeof(IDataCubeProvider))]
    public class DcxFileProvider : IDataCubeProvider
    {
        private string _FileName;
        public DcxFileProvider()
        {

        }

        public DataCube<float> Provide(string filename)
        {
            DataCubeStreamReader sr = new DataCubeStreamReader(filename);
            sr.LoadDataCube();
            return sr.DataCube;
        }
        public DataCube<float> ProvideSingleStep(string filename)
        {
            DataCubeStreamReader sr = new DataCubeStreamReader(filename);
            sr.LoadDataCubeSingleStep();
            return sr.DataCube;
        }
        public string FileTypeDescription
        {
            get
            {
                return "data cube file";
            }
        }

        public string Extension
        {
            get
            {
                return ".dcx";
            }
        }

        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }
    }
}
