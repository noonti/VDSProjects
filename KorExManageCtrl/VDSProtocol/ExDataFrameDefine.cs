using KorExManageCtrl.VDSProtocol_v2._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public enum KOR_EX_SESSION_STATE
    {
        SESSION_OFFLINE = 0, 
        SESSION_INIT = 1, //  
        SESSION_ONLINE = 2,// 
       
    }

    public struct WorkData
    {
        public SessionContext session;
        public ExDataFrame frame;
        public int sendCount; // 전송 횟수
        public int sleepCount; // Sleep 대기 횟수(5분 간격으로 몇번 대기 )
        public DateTime lastSendTime; // 최종 전송 시간
        public DateTime nextSendTime; // 다음 전송 시간
        public String guid;

    };


    public static class ExDataFrameDefine
    {

        public const int HEADER_SIZE = 45;



        public const byte OP_CSN_CHECK_COMMAND = 0xFF;  // CSN 장비 인증

        public const byte OP_CHECK_SESSION_COMMAND = 0xFE; // 통신 세션 유효성 확인

        public const byte OP_SYNC_VDS_COMMAND = 0x01;  // 제어기 동기화

        public const byte OP_TRAFFIC_DATA_COMMAND = 0x04; // 교통 데이터

        public const byte OP_SPEED_DATA_COMMAND = 0x05; // 속도 데이터

        public const byte OP_VEHICLE_LENGTH_COMMAND = 0x06; // 

        public const byte OP_ACCU_TRAFFIC_COMMAND = 0x07;   // 누적 교통량 데이터

        public const byte OP_TRAFFIC_THRESHOLD_COMMAND = 0x08;

        public const byte OP_HW_STATUS_COMMAND = 0x0B; // 

        public const byte OP_VDS_RESET_COMMAND = 0x0C; // 제어기 리셋

        public const byte OP_VDS_INIT_COMMAND = 0x0D; // 제어기 초기화

        public const byte OP_PARAM_DOWNLOAD_COMMAND = 0x0E; // 파라메터 다운로드

        public const byte OP_PARAM_UPLOAD_COMMAND = 0x0F; // 파라메터 업로드

        public const byte OP_ONLINE_STATUS_COMMAND = 0x11; // 온라인 상태

        public const byte OP_MEMORY_STATUS_COMMAND = 0x12; // 메모리 검사

        public const byte OP_MESSAGE_ECHO_COMMAND = 0x13; // 메시지 에코


        public const byte OP_SEQ_TRANSFER_COMMAND = 0x14; // 일련번호 검사

        public const byte OP_VDS_VERSION_COMMAND = 0x15;  // 버젼 정보

        public const byte OP_INDIV_TRAFFIC_COMMAND = 0x16; // 개별 차량 데이터

        public const byte OP_STILL_IMAGE_COMMAND = 0x17; // 정지 영상

        

        public const byte OP_CHANGE_RTC_COMMAND = 0x18; // RTC 변경

        public const byte OP_REVERSE_RUN_COMMAND = 0x19; // 역주행 정보

        public const byte OP_SET_FANHEATER_COMMAND = 0x20; // FAN/HEATER 동작온도 설정


        public const byte OP_SYSTEM_STATUS_COMMAND = 0x1E; // 상태 요청

        public const byte OP_SET_TEMPERATURE__COMMAND = 0x20; // Fan/Heater 동작 온도 설정 요청




        // Result Code 
        public const byte ACK_NORMAL = 0x06;
        public const byte NAK_ERROR = 0x15;

        public const byte NAK_NO_ERROR = 0x00; // 
        public const byte NAK_INTERNAL_ERROR = 0x01; // 내부 시스템 장애로 인한 수행실패
        public const byte NAK_DATALEN_ERROR = 0x02; // 데이터 길이 정보가 잘못된 경우
        public const byte NAK_CSN_ERROR = 0x03; //CSN (Controller Station Number)값이 잘못된 경우
        public const byte NAK_OPCODE_ERROR = 0x04; //OP code가 잘못된 경우.
        public const byte NAK_NAN_ERROR = 0x05;   // 필드 값 범위 초과되었거나 정의된 값이 아닌 경우.
        public const byte NAK_NOTREADY_ERROR = 0x06; //요청된 데이터가 준비되지 않음
        public const byte NAK_ETC_ERROR = 0xFF; //기타 오류
        //public const byte NAK_TIMEOUT_ERROR = 0x07;

        // Loop 장애 정보
        public const byte LOOP_NORMAL = 0x00;
        public const byte LOOP_STUCK_ON = 0x01;
        public const byte LOOP_STUCK_OFF = 0x02;
        public const byte LOOP_OSCILLATION = 0x03;

        public const byte INCIDENT_NORMAL = 0x00;
        public const byte INCIDENT_DETECTED = 0x01;


        // VDS STatus 


        public static IExOPData GetExOpData(bool bRequestFrame, byte opCode , byte[] data)
        {
            IExOPData result = null;
            
            switch (opCode)
            {
                case OP_CSN_CHECK_COMMAND:
                    if(bRequestFrame)
                        result = new CSNCheckDataRequest();
                    else
                        result = new ExResponse();
                    break;

                case OP_SYNC_VDS_COMMAND:
                    if (bRequestFrame)
                        result = new ControllerSyncRequest();
                    //else
                    //    result = new ControllerSyncResponse();
                    break;
                case OP_TRAFFIC_DATA_COMMAND:
                    if (bRequestFrame)
                        result = new ExRequest();
                    else
                        result = new TrafficDataExResponse();
                    break;
                case OP_SPEED_DATA_COMMAND:
                    if (bRequestFrame)
                        result = new ExRequest();
                    else
                        result = new SpeedDataExResponse();
                    break;
                case OP_VEHICLE_LENGTH_COMMAND:
                    if (bRequestFrame)
                        result = new VehicleLengthDataRequest();
                    else
                        result = new VehicleLengthDataResponse();
                    break;
                case OP_ACCU_TRAFFIC_COMMAND:
                    if (bRequestFrame)
                        result = new AccuTrafficDataRequest();
                    else
                        result = new AccuTrafficDataResponse();
                    break;
                case OP_TRAFFIC_THRESHOLD_COMMAND:
                    if (bRequestFrame)
                        result = new SetErrorThresholdRequest();
                    else
                        result = new SetErrorThresholdResponse();
                    break;
                case OP_HW_STATUS_COMMAND:
                    if (bRequestFrame)
                        result = new ControllerStatusRequest();
                    else
                        result = new ControllerStatusResponse();
                    break;
                case OP_VDS_RESET_COMMAND:
                    if (bRequestFrame)
                        result = new ResetControllerRequest();
                    else
                        result = new ResetControllerResponse();
                    break;
                case OP_VDS_INIT_COMMAND:
                    if (bRequestFrame)
                        result = new InitControllerRequest();
                    else
                        result = new InitControllerResponse();
                    break;
                case OP_PARAM_DOWNLOAD_COMMAND:
                    if (bRequestFrame)
                        result = new ParamDownloadRequest();
                    else
                        result = new ParamDownloadResponse();
                    break;

                case OP_PARAM_UPLOAD_COMMAND:
                    if (bRequestFrame)
                        result = new ParamUploadRequest();
                    else
                        result = new ParamUploadResponse();
                    break;

                case OP_CHECK_SESSION_COMMAND:
                    result = new ExResponse();
                    //if (bRequestFrame) // 통신세션 유효성 요청
                    //    result = new CheckSessionRequest();
                    //else
                    //    result = new CheckSessionResponse();
                    break;
                case OP_ONLINE_STATUS_COMMAND:
                    if (bRequestFrame)
                        result = new CheckOnlineStatusRequest();
                    else
                        result = new CheckOnlineStatusResponse();
                    break;

                case OP_MEMORY_STATUS_COMMAND:
                    if (bRequestFrame)
                        result = new CheckMemoryStatusRequest();
                    else
                        result = new CheckMemoryStatusResponse();
                    break;

                
                case OP_MESSAGE_ECHO_COMMAND:
                    if (bRequestFrame)
                        result = new EchoMessageRequest();
                    else
                        result = new EchoMessageResponse();
                    break;

                case OP_SEQ_TRANSFER_COMMAND:
                    if (bRequestFrame)
                        result = new CheckSeqNoRequest();
                    else
                        result = new CheckSeqNoResponse();
                    break;

                case OP_VDS_VERSION_COMMAND:
                    if (bRequestFrame)
                        result = new VDSVersionRequest();
                    else
                        result = new VDSVersionResponse();
                    break;

                case OP_INDIV_TRAFFIC_COMMAND:
                    if (bRequestFrame)
                        result = new IndivTrafficDataRequest();
                    else
                        result = new IndivTrafficDataResponse();
                    break;

                case OP_REVERSE_RUN_COMMAND:
                    if (data.Length > 0) // 역주행 정보 통지(Data 있음)
                        result = new ReverseRunRequest();
                    else
                        result = new ReverseRunResponse(); // 역주행 정보 통지에 대한 응답이므로 데이터 없음

                    break;

                case OP_CHANGE_RTC_COMMAND:
                    if (bRequestFrame)
                        result = new RealTimeClock();
                    else
                        result = new ExResponse();
                    break;

                case OP_SYSTEM_STATUS_COMMAND:
                    if (bRequestFrame)
                        result = new ExRequest();
                    else
                        result = new SystemStatus();
                    break;
                case OP_SET_FANHEATER_COMMAND:
                    if (bRequestFrame)
                        result = new SetTemperatureRequest();
                    else
                        result = new ExResponse();
                    break;
           

            }

            if (result != null)
            {
                result.Deserialize(data);
            }
            return result;
        }

        public static IExOPData CreateParam(int index)
        {
            IExOPData result = null;
            // 파라메터 정보 Deserialize
            switch (index)
            {
                case 1: // Loop Detector Configuration : 루프 검지기 지정 4bytes
                    //result = new ParamLoopConfig();
                    result = new ParamLaneConfig();
                    break;
                case 2: // Speed Loop   Configuration : 속도 검지 루프 지정
                    result = new SpeedLoopConfig();
                    break;

                case 3: // Polling Cycle : 수집 주기
                    result = new PollingCycle();
                    break;

                case 4: // Vehicle   Definition : 차량 감응시간 정의
                    result = new VehiclePulseNumber();
                    break;

                case 5: // Speed   Categories : 차량 속도 구분
                    result = new SpeedCategory();
                    break;

                case 6: // Vehicle Length   Categories : 차량 길이 구분
                    result = new LengthCategory();
                    break;
                case 7: // Speed Category   Accumulation Disable/Enable : 속도 별 누적 치 계산
                    result = new VDSValue((byte)index);
                    break;
                case 8: // Vehicle Length   Accumulation Disable/Enable : 차량길이 별 누적 치 계산
                    result = new VDSValue((byte)index);
                    break;
                case 9: // Speed   Calculation Disable/Enable : 속도 계산 가능 여부
                    result = new VDSValue((byte)index);
                    break;
                case 10: // Vehicle Length   Calculation Disable/Enable : 차량길이 계산 가능 여부
                    result = new VDSValue((byte)index);
                    break;
                case 11: // Speed Loop   Dimensions : 속도 Loop 간격 및 길이
                    result = new SpeedLoopDimension();
                    break;

                case 12: // Upper Volume   Limit : 교통량 상한치
                case 13: // Upper Speed   Limit : 차량 속도 상한치
                case 14: // Upper Vehicle   Length Limit : 차량길이 상한치
                    result = new PollingThreshold((byte)index);

                    break;
                case 15: // Incident   Detection Threshold : 유고정의를 위한 임계치
                    result = new IncidentDetectThreshold();
                    break;
                case 16: // Loop Detector   Stuck ON / OFF : Stuck   ON/OFF 임계치
                    result = new StuckThreshold();
                    break;
                case 17: // Loop Detector   Oscillation Threshold : Oscillation   임계치
                    result = new VDSValue((byte)index);
                    break;

                case 20: // Auto   Re-Synchronization Waiting Period
                    result = new VDSValue((byte)index);
                    break;

                case 21: // 역주행 사용 여부
                    result = new VDSValue((byte)index);
                    break;

                default:  // 18,19,22,23,24 : reserved 
                    break;



            }
            return result;
        }


        public static int InitWorkData(ref WorkData workData)
        {
            workData.sendCount = 0;
            workData.sleepCount = 0;
            workData.guid = Guid.NewGuid().ToString();

            return 1;
        }
    }
}
