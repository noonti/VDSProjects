using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public class TARGET_SUMMARY_INFO
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

        public String REPORT_YN { get; set; }

        public String CREATE_TIME { get; set; }

        public DateTime REG_DATE { get; set; }
        public DateTime MODIFY_DATE { get; set; }

        public String I_SEARCH_TYPE { get; set; }
        public long I_ID { get; set; }
        public int I_LIMIT_COUNT { get; set; }
        public int I_LANE { get; set; }
        public String I_START_DATE { get; set; }
        public String I_END_DATE { get; set; }
        public String I_REPORT_YN { get; set; }



        public TARGET_SUMMARY_INFO()
        {
            I_SEARCH_TYPE = "ID";
            I_ID = 0;
            I_LIMIT_COUNT =  255;
            I_LANE = 0;
            I_START_DATE = String.Empty;
            I_END_DATE = String.Empty;
            I_REPORT_YN = String.Empty;

        }

    }
}
