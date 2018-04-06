//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using DotSpatial.Data;
using DotSpatial.Modeling.Forms;
using DotSpatial.Modeling.Forms.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Processing.Models
{
    public class MFUZF : Tool
    {
        #region Constants and Fields

        // Declare input and output parameter arrays
        private Parameter[] _inputParam;
        //private Parameter[] _outputParam;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of the buffer tool
        /// </summary>
        public MFUZF()
        {
            this.Name = "UZF";
            this.Category = "Modflow";
            this.Description = "UZF Package";
            this.ToolTip = "UZF Package";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or Sets the input paramater array. 
        /// Number of parameter and parameter types are defined during initialize. 
        /// </summary>
        public override Parameter[] InputParameters
        {
            get
            {
                return _inputParam;
            }
        }

        /// <summary>
        /// Gets or Sets the input paramater array. 
        /// Number of parameter and parameter types are defined during initialize. 
        /// </summary>
        public override Parameter[] OutputParameters
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Once the parameters have been configured, the Execute command can be called, it returns true if succesful
        /// </summary>
        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            //Get the needed input and output parameters
            IFeatureSet inputFeatures = _inputParam[0].Value as IFeatureSet;
            DoubleParam dp = _inputParam[1] as DoubleParam;
            double bufferDistance = 1;
            if (dp != null)
            {
                bufferDistance = dp.Value;
            }
           // IFeatureSet outputFeatures = _outputParam[0].Value as IFeatureSet;

            return true;
        }


        /// <summary>
        /// Inititalize input and output arrays with parameter types and default values.
        /// </summary>
        public override void Initialize()
        {
            _inputParam = new Parameter[3];
            _inputParam[0] = new FeatureSetParam("Model Grid Feature");
            _inputParam[1] = new FeatureSetParam("Hydrogeology Zone Feature");
            _inputParam[2] = new StringParam("Fields");     
        }

        #endregion
    }
}
