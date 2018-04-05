// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    public class MFNameManager
    {
        public MFNameManager()
        {
            MasterList = new List<PackageInfo>();
        }

        public List<PackageInfo> MasterList { get; private set; }
        public bool IsDirty { get; set; }

        public void New()
        {
            MasterList.Clear();
            IsDirty = true;
        }
        /// <summary>
        /// Add package info and set Dirty state to True
        /// </summary>
        /// <param name="info"></param>
        public void Add(PackageInfo info)
        {
            if (!Exists(info))
            {
                MasterList.Add(info);
                IsDirty = true;
            }
        }
        /// <summary>
        /// Add package info without change Dirty state of itself
        /// </summary>
        /// <param name="info"></param>
        public void AddInSilence(PackageInfo info)
        {
            if (!Exists(info))
            {
                MasterList.Add(info);
            }
        }

        public void Remove(PackageInfo info)
        {
            if (Exists(info))
            {
                MasterList.Remove(info);
                IsDirty = true;
            }
        }

        public void RemoveBy(int fid)
        {
            var pi = SelectBy(fid);
            if (pi != null)
            {
                MasterList.Remove(pi);
                IsDirty = true;
            }
        }

        public PackageInfo SelectBy(int fid)
        {
            var buf = from p in MasterList where p.FID == fid select p;
            if (buf.Any())
                return buf.First();
            else
                return null;
        }

        private bool Exists(PackageInfo info)
        {
            return MasterList.Exists(p => p.FID == info.FID);
        }

        public int NextFID()
        {
            int fid = 1;
            var buf = from info in MasterList select info.FID;
            if (buf.Any())
            {
                fid = buf.Last() + 1;
            }
            return fid;
        }

        public int GetFID(string file_ext)
        {
            var buf = from pck in MasterList where pck.FileExtension == file_ext select pck;
            if (buf.Any())
                return buf.First().FID;
            else
                return -1;
        }

        public void Clear()
        {
            MasterList.Clear();
        }

        public void Save(string namefile)
        {
            StreamWriter sw = new StreamWriter(namefile);
            string line = string.Format("# Name file for MODFLOW. Created on {0} by Visual HEIFLOW", DateTime.Now.ToString());
            sw.WriteLine(line);
            //Write List at first
            foreach (var pckinfo in MasterList)
            {
                if (pckinfo.ModuleName == "LIST")
                {
                    line = string.Format("{0}\t\t{1}\t\t{2}\t\t{3}", pckinfo.ModuleName, pckinfo.FID, pckinfo.RelativeFileName, pckinfo.IOState.ToString());
                    sw.WriteLine(line);
                    break;
                }
            }
            foreach (var pckinfo in MasterList)
            {
                if (pckinfo.IOState == IOState.OLD)
                {
                    line = string.Format("{0}\t\t{1}\t\t{2}\t\t{3}", pckinfo.ModuleName, pckinfo.FID, pckinfo.RelativeFileName, pckinfo.IOState.ToString());
                    sw.WriteLine(line);
                }
            }
            foreach (var pckinfo in MasterList)
            {
                if (pckinfo.ModuleName == "DATA")
                {
                    if (pckinfo.Format == FileFormat.Binary)
                    {
                        line = string.Format("{0}(BINARY)\t{1}\t{2}\t{3}", pckinfo.ModuleName, pckinfo.FID, pckinfo.RelativeFileName, pckinfo.IOState.ToString());
                    }
                    else
                    {
                        line = string.Format("{0}\t{1}\t{2}\t{3}", pckinfo.ModuleName, pckinfo.FID, pckinfo.RelativeFileName, pckinfo.IOState.ToString());
                    }
                    sw.WriteLine(line);
                }
            }
            sw.Close();
            IsDirty = false;

        }

        public PackageInfo GetList(int fid, string prj_name)
        {
            PackageInfo info = new PackageInfo()
            {
                FID = fid,
                FileName = Modflow.OutputDic + prj_name + ".lst",
                FileExtension = ".lst",
                ModuleName = "LIST",
                IOState = IOState.REPLACE
            };
            return info;
        }
    }
}