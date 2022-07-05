using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.API.Model
{
    public class RackStatus
    {
        /// <summary>
        /// 1: 발생, 0: 비발생
        /// </summary>
        public  byte IsLongPowerFail { get; set; }


        /// <summary>
        /// 1: 발생, 0: 비발생
        /// </summary>
        public  byte IsShortPowerFail { get; set; }


        /// <summary>
        /// 1: Open, 0: CLose
        /// </summary>
        public  byte IsFrontDoorOpen { get; set; }

        /// <summary>
        /// 1: Open, 0: CLose
        /// </summary>
        public  byte IsRearDoorOpen { get; set; }

        /// <summary>
        /// 1: 동작 ON(정상), 0: 동작 OFF(비정상)
        /// </summary>
        /// 
        public  byte IsFanOn { get; set; }

        /// <summary>
        /// 1: 동작 ON(동작), 0: 동작 OFF(비정상)
        /// </summary>
        /// 
        public  byte IsHeaterOn { get; set; }

        /// <summary>
        /// 1: 동작 ON, 0: 동작 OFF
        /// </summary>
        /// 
        public  byte IsAVROn { get; set; }


        /// <summary>
        /// 예약필드(6비트 째 값)
        /// </summary>
        public  byte Reserved6 { get; set; }

        /// <summary>
        /// 예약필드(7비트 째 값)
        /// </summary>
        public  byte Reserved7 { get; set; }


        /// <summary>
        /// 제어기 리셋( 1: 발생, 0: 비발생)
        /// </summary>
        public  byte IsReset { get; set; }

        /// <summary>
        /// 1: 발생, 0: 비발생
        /// </summary>
        public  byte IsDetectError { get; set; }


        /// <summary>
        /// 1: 발생, 0: 비발생
        /// </summary>
        public  byte IsSystemHalt { get; set; }


        /// <summary>
        /// RTU 상태값 
        /// </summary>
        public  byte RTSStatus { get; set; }
        public  byte PowerFailReset { get; set; }
        public  int Temperature { get; set; }
        public  int FanThreshold { get; set; }
        public  int AVRVoltThreshold { get; set; }
        public  int AVRAmpThreshold { get; set; }
        public  int HeaterThreshold { get; set; }
        public  int HumitidyThreshold { get; set; }
        public  int ReservedThreshold { get; set; }
    }
}
