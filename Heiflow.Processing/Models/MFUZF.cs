// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
