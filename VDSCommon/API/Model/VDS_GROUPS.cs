using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.API.Model
{
    public class VDS_GROUPS
    {
        public int ID { get; set; }
        public String GROUP_CODE { get; set; }
        public int PARENT_ID { get; set; }
        public int DEPTH { get; set; }
        public String TITLE { get; set; }
        public String OFFICER_NAME { get; set; }
        public String TEL_NO { get; set; }
        public String USE_YN { get; set; }
        public DateTime REG_DATE { get; set; }
    }
}
