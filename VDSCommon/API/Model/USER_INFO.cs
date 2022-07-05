using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.API.Model
{
    public class USER_INFO
    {
        public String USER_ID {get;set;}
        public String PASSWD { get; set; }
        public String USER_NAME { get; set; }
        public int USER_TYPE { get; set; }
        public String DEPT_NAME { get; set; }
        public String APPROVE_YN { get; set; }
        public DateTime REG_DATE { get; set; }

        public int CURRENT_PAGE { get; set; }
        public int PAGE_SIZE { get; set; }


    }
}
