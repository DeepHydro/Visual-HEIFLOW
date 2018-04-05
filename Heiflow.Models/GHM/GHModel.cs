// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.IO;
using Heiflow.Models.Properties;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Heiflow.Models.GHM
{
    [Export(typeof(IBasicModel))]
    [ModelItem]
    public class GHModel : BaseModel
    {
        private GHMSerializer _GHMSerializer;
        private GHMPackage _GHMPackage;

        public GHModel()
        {
            Name = "GHM";
            Icon = Resources.ServiceWMSGroup16;
            LargeIcon = Resources.ServiceWMSGroup32;
            Packages = new Dictionary<string, IPackage>();
        }


        public GHMPackage MasterPackage
        {
            get
            {
                return _GHMPackage;
            }
        }

        public override bool New(IProgress progress)
        {
            _GHMSerializer = new GHMSerializer();
            _GHMSerializer.FileName = @"E:\Heihe\HRB\GeoData\GBHM\Model\section_v1.09_ylx\uhrb.ghm";
            _GHMSerializer.TimeReference = new TimeReference()
            {
                Start = new DateTime(2000, 1, 1),
                End = new DateTime(2010, 12, 31),
                TimeStep = 86400
            };
            _GHMSerializer.SpatialReference = new SpatialReference()
            {
                sr = @".\dem\geo_lambert.prj",
                X = 453035,
                Y = 4338102
            };

            Member mem = new Member()
            {
                Name = "Grid",
            };
            Item ele = new Item()
            {
                Name = "Elevation",
                Path = @".\dem\elevation.asc",
                Description = "Elevation"
            };
            mem.Spatial.Add(ele);

            Member para = new Member()
            {
                Name = "Parameter",
            };
            Item alpha = new Item()
            {
                Name = "Alpha",
                Path = @".\dem\elevation.asc",
                Description = "Elevation"
            };
            para.Spatial.Add(alpha);

            _GHMSerializer.Save();
            return true;
        }

        public override void Initialize()
        {

        }

        public override bool Validate()
        {
            return true;
        }


        public override bool Load(IProgress progress)
        {
            string masterfile = ControlFileName;
            if (File.Exists(masterfile))
            {
                ControlFileName = masterfile;
                _GHMSerializer = GHMSerializer.Open(masterfile);
                _GHMPackage = new GHMPackage()
                {
                    Serializer = _GHMSerializer,
                    GHModel = this
                };
                AddInSilence(_GHMPackage);

                foreach (var layer in _GHMSerializer.Layers)
                {
                    foreach (var mem in layer.Members)
                    {
                        foreach (var item in mem.Spatial)
                        {
                            item.FullPath = Path.Combine(ModelService.WorkDirectory, item.Path);
                            item.Grid = this.Grid;
                        }
                        foreach (var item in mem.Spatiotemporal)
                        {
                            item.FullPath = Path.Combine(ModelService.WorkDirectory, item.Path);
                            item.Grid = this.Grid;
                        }
                        foreach (var item in mem.TimeSeries)
                        {
                            item.FullPath = Path.Combine(ModelService.WorkDirectory, item.Path);
                            item.Grid = this.Grid;
                        }
                    }
                }
                return true;
            }   
            {            
                var msg= "\r\nThe model file dose not exist: " + ControlFileName;
                progress.Progress(msg);
                return false;
            }
        }

        public override bool LoadGrid(IProgress progress)
        {
            var gridnode = (from fl in
                                ((from layer in _GHMSerializer.Layers where layer.Name == "Base" select layer).First().Members)
                            where fl.Name == "Grid"
                            select fl).FirstOrDefault();
            var ele = (from gd in gridnode.Spatial where gd.Name == "Elevation" select gd).FirstOrDefault();
    
            //var provider = base.Project.Manager.GridFileFactory.Select(ele.FullPath);
            //var grid = provider.Provide(ele.FullPath) ;

            //grid.Extent(_GHMSerializer.SpatialReference);
            //grid.BuildTopology();

            //this.Grid = grid;
            return true;
        }
        public override void Clear()
        {
            
        }


        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            throw new NotImplementedException();
        }

        public override void Save(IProgress progress)
        {
            throw new NotImplementedException();
        }
    }
}
