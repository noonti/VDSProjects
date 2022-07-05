using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VDSCommon.DataType
{
    public class TargetSummaryInfo : RadarData
    {
        public long ID { get; set; }
        public int ID_0 { get; set; }
        public int ID_1 { get; set; }
        public int START_CYCLE_0 { get; set; }
        public int START_CYCLE_1 { get; set; }
        public int AGE_0 { get; set; }
        public int AGE_1 { get; set; }
        public int MAG_MAX_0 { get; set; }
        public int MAG_MAX_1 { get; set; }

        public byte LANE { get; set; }
        public byte TRAVEL_DIRECTION { get; set; }

        public double LENGTH_X100 { get; set; }
        public double SPEED_X100 { get; set; }
        public double RANGE_X100 { get; set; }

        public double OCCUPY_TIME { get; set; }


        public DateTime CREATE_DATE { get; set; }

        public TargetSummaryInfo()
        {
            DataType = RadarDataType.TARGET_SUMMARY_INFO;
        }
    }
}
