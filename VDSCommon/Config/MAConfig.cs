using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Config
{
    public class MAConfig
    {
        // 제어기 IP 주소 
        public String IpAddress { get; set; } 

        // 제어기 API 포트 
        public int ApiPort { get; set; }


        // DB Address
        public String DBAddress { get; set; }

        // DB Port 
        public int DBPort { get; set; }

        // DB Name 
        public String DBName { get; set; }

        // DB User 
        public String DBUser { get; set; }
        // DB passwd 
        public String DBPasswd { get; set; }


    }
}
