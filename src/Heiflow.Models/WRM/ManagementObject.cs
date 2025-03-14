using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.WRM
{
    public class ManagementObject
    {
        public ManagementObject()
        {
            IHRUList = new List<int>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public double SW_Ratio { get; set; }
        public int ObjType { get; set; }
        /// <summary>
        ///  0 is farm; 1 is industry
        /// </summary>
        public int FramType { get; set; }
        public int [] ObjType_HRU { get; set; }
        public double Drawdown { get; set; }
        public int SegID { get; set; }
        public int ReachID { get; set; }
        public int HRU_Num { get; set; }
        public int[] HRU_List { get; set; }
        public double[] HRU_Area { get; set; }
        public double[] Max_Pump_Rate { get; set; }
        public double Total_Area { get; set; }
        public double Max_Total_Pump { get; set; }
        public double Canal_Efficiency { get; set; }
        public double Canal_Ratio { get; set; }
        public int Inlet_Type { get; set; }
        public double Inlet_MinFlow { get; set; }
        public double Inlet_MaxFlow { get; set; }
        public double Inlet_Flow_Ratio { get; set; }
        public string SW_Cntl_Factor { get; set; }
        public string GW_Cntl_Factor { get; set; }
        public double [] SW_Cntl_Factor_List { get; set; }
        public double[] GW_Cntl_Factor_List { get; set; }
        public List<int> IHRUList { get; set; }
        public int Num_well_layer { get; internal set; }
        public double[] Canal_Efficiency_list { get; internal set; }
        public double[] Canal_Ratio_list { get; internal set; }
        public int [] Well_layers { get; set; }
        public double[] Well_ratios { get; set; }
        public double Returnflow_ratio { get; set; }

        public override string ToString()
        {
            var canal_eff = new double[HRU_Num];
            var canal_ratio = new double[HRU_Num];
            for (int i = 0; i < HRU_Num; i++)
            {
                canal_eff[i] = Canal_Efficiency;
                canal_ratio[i] = Canal_Ratio;
            }
            var str = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", ID, Name, SW_Ratio, ObjType, Drawdown, SegID, ReachID, HRU_Num, string.Join("\t", HRU_List),
                string.Join("\t", HRU_Area), string.Join("\t", canal_eff), string.Join("\t", canal_ratio));
            return str;
        }
    }
}
