using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public class SPEED_STAT_DETAIL
    {
        public String ID { get; set; }
        public String PARENT_ID { get; set; }

        public int LANE { get; set; }

        public int CATEGORY_1_COUNT { get; set; }

        public int CATEGORY_2_COUNT { get; set; }

        public int CATEGORY_3_COUNT { get; set; }
        public int CATEGORY_4_COUNT { get; set; }
        public int CATEGORY_5_COUNT { get; set; }
        public int CATEGORY_6_COUNT { get; set; }
        public int CATEGORY_7_COUNT { get; set; }
        public int CATEGORY_8_COUNT { get; set; }
        public int CATEGORY_9_COUNT { get; set; }
        public int CATEGORY_10_COUNT { get; set; }
        public int CATEGORY_11_COUNT { get; set; }
        public int CATEGORY_12_COUNT { get; set; }

        public String REG_DATE { get; set; }

    }
}
