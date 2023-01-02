using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Config
{
    /// <summary>
    /// 제어기 설정(일반)
    /// </summary>
    public class ControllerConfig
    {

        // 제어기 유형 (ITS or KorEx)
        public int ProtocolType { get; set; } // VDSCOnfig.CENTER_TYPE 1:ITS 2: korEx

        // 제어기 IP 주소 
        public String IpAddress { get; set; } // VDSCOnfig.IPADDRESS

        // 제어기 ID
        public String ControllerId { get; set; } // VDSConfig.VDS ID ??

        // 제어기 API 포트 
        public int ApiPort { get; set; }

        // 원격 접속 정보
        public int RemoteCtrlPort { get; set; }
        public String RemoteCtrlId { get; set; }
        public String RemoteCtrlPasswd { get; set; }

        // RTU Serial Port 
        public String RTUPort { get; set; }

        // RTU Baud Rate 
        public int BaudRate { get; set; }


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


        // 검지 방식 (1: 영상식 2: 레이더식)
        public int DeviceType { get; set; }

        // 검지 장치 주소
        public String DeviceAddress { get; set; }

        // 레이더 설치 위치
        public int DevicePos { get; set; }

        public int RemotePort { get; set; } // 원격 포트  
        public int LocalPort { get; set; }  // 로컬 포트 

        // 검지 구간(m)
        public double CheckDistance { get; set; }

        public String StreamingURL { get; set; }


        // 기타
        // 검지 애니매이션 사용 여부(1:사용 0:미사용)
        public int UseAnimation { get; set; }

        // 데이터삭제 주기(일)
        public int TrafficDataPeriod { get; set; }

        // 로그 파일 삭제 주기(일)
        public int LogFilePeriod { get; set; }

        // 유지보수 서버 정보
        public String MAServerAddress { get; set; }
        public int MAServerPort { get; set; }

        public int MAServerAPIPort { get; set; }


        public String MAServerUrl { get; set; }

    }
}
