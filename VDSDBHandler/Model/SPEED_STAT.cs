using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public class SPEED_STAT
    {

        public String ID { get; set; }
        public int DETECT_DATE { get; set; }

        public int LANE_COUNT { get; set; }

        public String SPEED_INFO { get; set; }
        public String REPORT_YN { get; set; }

        public String REG_DATE { get; set; }
    }
}
