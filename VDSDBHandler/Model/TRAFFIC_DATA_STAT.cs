using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public class TRAFFIC_DATA_STAT
    {
        public int LANE { get; set; }
        public int TRAFFIC_COUNT { get; set; }
        public double AVG_SPEED { get; set; }
        public double AVG_LENGTH { get; set; }
        public double OCCUPY_RATIO { get; set; }

        public String I_START_DATE { get; set; }
        public String I_END_DATE { get; set; }

    }
}
