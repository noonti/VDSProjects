using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon.API.Model;

namespace VDSCommon
{
    public static class VDSRackStatus
    {

        /// <summary>
        /// 1: 발생, 0: 비발생
        /// </summary>
        public static byte IsLongPowerFail { get; set; }


        /// <summary>
        /// 1: 발생, 0: 비발생
        /// </summary>
        public static byte IsShortPowerFail { get; set; }


        /// <summary>
        /// 1: Open, 0: CLose
        /// </summary>
        public static byte IsFrontDoorOpen { get; set; }

        /// <summary>
        /// 1: Open, 0: CLose
        /// </summary>
        public static byte IsRearDoorOpen { get; set; }

        /// <summary>
        /// 1: 동작 ON(정상), 0: 동작 OFF(비정상)
        /// </summary>
        /// 
        public static byte IsFanOn { get; set; }

        /// <summary>
        /// 1: 동작 ON(동작), 0: 동작 OFF(비정상)
        /// </summary>
        /// 
        public static byte IsHeaterOn { get; set; }

        /// <summary>
        /// 1: 동작 ON, 0: 동작 OFF
        /// </summary>
        /// 
        public static byte IsAVROn { get; set; }

        /// <summary>
        /// 0: 임계치에 의한 동작모드, 1: 강제제어모드
        /// </summary>
        public static byte heaterMode { get; set; }

        /// <summary>
        /// 0: 임계치에 의한 동작모드, 1: 강제제어모드
        /// </summary>
        public static byte fanMode { get; set; }


        /*/// <summary>
        /// 예약필드(6비트 째 값)
        /// </summary>
        public static byte Reserved6 { get; set; }

        /// <summary>
        /// 예약필드(7비트 째 값)
        /// </summary>
        public static byte Reserved7 { get; set; }*/


        /// <summary>
        /// 제어기 리셋( 1: 발생, 0: 비발생)
        /// </summary>
        public static byte IsReset { get; set; }

        /// <summary>
        /// 1: 발생, 0: 비발생
        /// </summary>
        public static byte IsDetectError { get; set; }


        /// <summary>
        /// 1: 발생, 0: 비발생
        /// </summary>
        public static byte IsSystemHalt { get; set; }


        /// <summary>
        /// RTU 상태값 
        /// </summary>
        public static byte RTSStatus { get; set; }
        public static byte PowerFailReset { get; set; }
        public static int Temperature { get; set; }
        public static int FanThreshold { get; set; }
        public static int AVRVoltThreshold { get; set; }
        public static int AVRAmpThreshold { get; set; }
        public static int HeaterThreshold { get; set; }
        public static int HumitidyThreshold { get; set; }
        public static int ReservedThreshold { get; set; }


        

        public static UInt16 ToKICTStatus()
        {
            UInt16 result = 0;

            result = (UInt16)(
                        IsLongPowerFail +
                        (IsShortPowerFail << 1) +
                        (IsFrontDoorOpen << 2) +
                        (IsRearDoorOpen << 3) +
                        (IsFanOn << 4) +
                        (IsHeaterOn << 5) +
                        (IsReset << 6) +
                        (IsDetectError << 7) + 
                        (IsSystemHalt << 8)
                       );
            return result;
        }
        public static UInt16 GetKorExControllerStatus(UInt32 csn, bool isDowonloaded, bool selfSync)
        {
            UInt16 status = ToKorExStatus();
            // 2: 파라미터 RAM이 기본값을 가지고 있어  HOST로부터  파라메터 다운로드 필요한 경우 1 설정 
            if (!isDowonloaded) 
                status = (UInt16)(status | 0x0002);
            // 3: HOST 로부터 동기화 명령 제외한 전역 주소 0xFFFFFFFF 를 수신한 경우 1 설정 
            if (csn == 0xFFFFFFFF) 
                status = (UInt16)(status | 0x0004);
            // 4: Sync 명령 받지 못한 경우 일정시간(1초) 이후 제어기 스스로 동기화 한 경우 1 설정 
            if (selfSync) 
                status = (UInt16)(status | 0x0008);
            return status;
        }



        public static UInt16 ToKorExStatus()
        {
            UInt16 result = 0;
            /*
            0 Long Power Fail. 2초를 초과한 시간 동안 POWER FAIL이 발생한 경우 1로 설정.
            1 Short Power Fail.2초 이하의 시간 동안 POWER FAIL이 발생한 경우 1로 설정.
            2 Default Parameter. 파라미터 RAM이 기본값을 가지고 있어, HOST로부터 파라미터 다운로드가 필요한 경우 1로 설정.
            3 Received   Broadcast Message(동기화 명령은 제외). HOST로부터 전역주소 0xFFFFFFFF를 수신한 경우 1로 설정.
            4 Auto Resynchronization. HOST로부터 SYNC 명령을 받지 못한 경우, 일정시간 이후(기본1초) 제어기 스스로 동기화한 경우 1로 설정
            5 FRONT DOOR OPENED. 앞문이 개방된 경우 1로 설정.
            6 REAR DOOR OPENED. 뒷문이 개방된 경우 1로 설정.
            7 FAN OPERATED. 팬이 작동된 경우 1로 설정.
            8 HEATER OPERATED. 히터가 작동된 경우 1로 설정.
            9 CONTROLLER RESET. 제어기 스스로 리셋 한 경우 1로 설정.
            */
            result = (UInt16)(
                        IsLongPowerFail +
                        (IsShortPowerFail << 1) +
                        // 2: 파라미터 RAM이 기본값을 가지고 있어  HOST로부터  파라메터 다운로드 필요한 경우 1 설정 
                        // 3: HOST 로부터 동기화 명령 제외한 전역 주소 0xFFFFFFFF 를 수신한 경우 1 설정 
                        // 4: Sync 명령 받지 못한 경우 일정시간(1초) 이후 제어기 스스로 동기화 한 경우 1 설정 
                        (IsFrontDoorOpen << 5) +
                        (IsRearDoorOpen << 6) +
                        (IsFanOn << 7) +
                        (IsHeaterOn << 8) +

                        (IsReset << 9) 
                        //(IsDetectError << 7) +
                        //(IsSystemHalt << 8)
                       );
            return result;
        }

        public static byte SetRTSStatus(byte rtuStatus, byte[] deviceInfo )
        {
            RTSStatus = rtuStatus;

            /* bit
             * 0 : FRONT DOOR   (Close : 0, Open : 1) 
             * 1 : REAR  DOOR   (Close : 0, Open : 1) 
             * 2 : FAN          (OFF : 0, ON : 1) 
             * 3 : HEATER       (OFF : 0, ON : 1) 
             * 4 : AVR          (OFF : 0, ON : 1) 
             * 5 : HEATER 동작 모드(임계치에 의한 동작모드0 , 강제제어모드: 1)
             * 6 : FAN 동작 모드(임계치에 의한 동작모드0 , 강제제어모드: 1)
             */

            IsLongPowerFail = 0;
            IsShortPowerFail = 0;
            IsFrontDoorOpen = (byte)(RTSStatus & 0x01);
            IsRearDoorOpen = (byte)((RTSStatus & 0x02) >> 1);
            IsFanOn = (byte)((RTSStatus & 0x04) >> 2);
            IsHeaterOn = (byte)((RTSStatus & 0x08) >> 3);
            IsAVROn = (byte)((RTSStatus & 0x10) >> 4);
            

            heaterMode = (byte)((RTSStatus & 0x20) >> 5);
            fanMode = (byte)((RTSStatus & 0x40) >> 6);

            IsReset = 0;
            IsDetectError = 0;
            IsSystemHalt = 0;

            if (deviceInfo.Length == 8 ) // 8 bytes
            {
                PowerFailReset = deviceInfo[0]; // DF[0] 검지기 상태 정보 PFR
                Temperature = Utility.GetThresholdToInt(deviceInfo[1]); // DF[1] 온도 계측값
                FanThreshold = Utility.GetThresholdToInt(deviceInfo[2]); // DF[2] Fan 동작 임계값
                AVRVoltThreshold = deviceInfo[3]; // DF[3] AVR 전압 임계값
                AVRAmpThreshold = deviceInfo[4]; // DF[4] AVR 전류 임계값
                HeaterThreshold = Utility.GetThresholdToInt(deviceInfo[5]); // DF[5] Heater 동작 임계값
                HumitidyThreshold = deviceInfo[6]; // DF[6] 습도계측값
                ReservedThreshold = deviceInfo[7]; // DF[7] 예비용
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO,$"IsLongPowerFail={IsLongPowerFail}, IsShortPowerFail={IsShortPowerFail}, IsFrontDoorOpen={IsFrontDoorOpen}, IsRearDoorOpen={IsRearDoorOpen}, IsFanOn={IsFanOn}, IsHeaterOn={IsHeaterOn}, IsAVROn={IsAVROn}");
            Utility.AddLog(LOG_TYPE.LOG_INFO, $"ToKICTStatus = {ToKICTStatus()}");
            return RTSStatus;

        }

        public static byte GetPowerSupplyStatus()
        {
            return 0;
        }

        public static UInt16 GetBoardStatus()
        {
            return 0;
        }

        public static RackStatus GetRackStatus()
        {
            RackStatus result = new RackStatus();
            result.IsLongPowerFail = IsLongPowerFail;

            result.IsShortPowerFail = IsShortPowerFail;
            result.IsFrontDoorOpen = IsFrontDoorOpen;
            result.IsRearDoorOpen = IsRearDoorOpen;
            result.IsFanOn =  IsFanOn;
            result.IsHeaterOn =  IsHeaterOn;
            result.IsAVROn =  IsAVROn;
            result.Reserved6 = heaterMode;
            result.Reserved7 = fanMode;

            result.IsReset =  IsReset;
            result.IsDetectError =  IsDetectError;
            result.IsSystemHalt = IsSystemHalt;
            result.RTSStatus = RTSStatus;
            result.PowerFailReset = PowerFailReset;

            result.Temperature =  Temperature;
            result.FanThreshold =  FanThreshold;

            result.AVRVoltThreshold = AVRVoltThreshold;
            result.AVRAmpThreshold =  AVRAmpThreshold;
            result.HeaterThreshold =  HeaterThreshold;
            result.HumitidyThreshold = HumitidyThreshold;
            result.ReservedThreshold = ReservedThreshold;
            return result;

        }
    }
}
