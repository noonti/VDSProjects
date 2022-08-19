using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public class TRAFFIC_STAT
    {
        public String ID { get; set; }
        public int DETECT_DATE { get; set; }

        public int FRAME_NO { get; set; }

        public String ERROR_INFO { get; set; }

        public int LANE_COUNT { get; set; }

        public String LANE_INFO { get; set; }

        public String REPORT_YN { get; set;  }

        public String REG_DATE { get; set; }
        public String I_START_DATE { get; set; }
        public String I_END_DATE { get; set; }

        public int I_LANE { get; set; }

        public List<TRAFFIC_STAT_DETAIL> trafficStatDetailList = new List<TRAFFIC_STAT_DETAIL>();

    }
}
