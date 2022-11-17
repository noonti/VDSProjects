using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Config
{
    /// <summary>
    /// 도로공사 설정
    /// </summary>
    public class KorExConfig
    {
        /// <summary>
        /// 수집서버 주소
        /// </summary> 
        public String centerAddress = "127.0.0.1"; // 수집서버(센터) 주소 

        /// <summary>
        /// 수집서버 포트
        /// </summary>
        public int centerPort = 10000;

        public int RETRY_COUNT = 3; // 재시도 횟수 

        public int centerPollingPeriod { get; set; }
        public int localPollingPeriod { get; set; }
        public int checkSessionTime { get; set; }
        public byte[] csn = new byte[8];
        public String vdsType { get; set; }
        public String vdsGroup { get; set; }
        public String vdsNo { get; set; }

        public String siteName { get; set; }

        public int sensorCount { get; set; }
        public int powerSupplyCount { get; set; }
        public int boardCount { get; set; }

        public int versionNo { get; set; }
        public int releaseNo { get; set; }
        public int releaseYear { get; set; }
        public int releaseMonth { get; set; }
        public int releaseDay { get; set; }

        public int inverseGapTime { get; set; }
        public int inverseCheckTime { get; set; }
        public int inverseCheckDistance { get; set; }

        public int stopGapTime { get; set; }
        public int stopCheckTime { get; set; }
        public int stopGapDistance { get; set; }
        
    }
}
