using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class KorexOffice
    {
        public int Id { get; set; }
        public String OfficeCode { get; set; }
        public String OfficeName { get; set; }

        public String OfficerName { get; set; }

        public String TelNo { get; set; }

        public DateTime RegDate { get; set; }
    }
}
