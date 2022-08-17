using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public class TRAFFIC_STAT_DETAIL
    {

        public String ID { get; set; }
        public String PARENT_ID{ get; set; }

        public int LANE { get; set; }

        public int LARGE_COUNT { get; set; }

        public int MIDDLE_COUNT { get; set; }

        public int SMALL_COUNT { get; set; }

        public int SPEED { get; set; }

        public int OCCUPY { get; set; }

        public int CAR_LENGTH { get; set; }

        public String REG_DATE { get; set; }


    }
}
