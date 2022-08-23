using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.API.Model
{
    public class TRAFFIC_DATA
    {
        public String ID { get; set; }
        public String CONTROLLER_ID { get; set; }
        public String VDS_TYPE { get; set; }
        public int LANE { get; set; }
        public int DIRECTION { get; set; }
        public int LENGTH { get; set; }
        public double SPEED { get; set; }
        public int VEHICLE_CLASS { get; set; }

        public int OCCUPY_TIME { get; set; }
        public int LOOP1_OCCUPY_TIME { get; set; }
        public int LOOP2_OCCUPY_TIME { get; set; }

        public String REVERSE_RUN_YN { get; set; }
        public int VEHICLE_GAP { get; set; }

        public String DETECT_TIME { get; set; }
        public String REPORT_YN { get; set; }
        public DateTime REG_DATE { get; set; }
        public DateTime MODIFY_DATE { get; set; }


        public int I_LIMIT_COUNT { get; set; }
        public String I_START_DATE { get; set; }
        public String I_END_DATE { get; set; }
        public String I_REPORT_YN { get; set; }

        public int I_PAGE_NO { get; set; }
        public int I_PAGE_SIZE { get; set; }

        public int I_EXPIRE_DAY { get; set; }

    }
}
