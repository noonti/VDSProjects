using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public class KOREX_OFFICE
    {
        public int ID { get; set; }
        public String OFFICE_CODE { get; set; }
        public String OFFICE_NAME { get; set; }

        public String OFFICER_NAME { get; set; }

        public String TEL_NO { get; set; }

        public DateTime REG_DATE { get; set; }

    }
}
