using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{

    
    public struct WorkData
    {
        public SessionContext session;
        public DataFrame frame;
        public int sendCount; // 전송 횟수
        public int sleepCount; // Sleep 대기 횟수(5분 간격으로 몇번 대기 )
        public DateTime lastSendTime; // 최종 전송 시간
        public DateTime nextSendTime; // 다음 전송 시간
        public String guid;

    };


    public static class DataFrameDefine
    {
        public const int VDS_FRAME_HEADER = 12;

        public const byte VERSION = 0x01;

        public const byte OP_FRAME_INITIAL_REQUEST = 0xA1;
        public const byte OP_FRAME_INITIAL_RESPONSE = 0xA2;
        public const byte OP_FRAME_RE_REQUEST = 0xA3;
        public const byte OP_FRAME_RE_RESPONSE = 0xA4;


        /// <summary>
        /// 교통 데이터 전송 요청
        /// </summary>
        public const byte OP_TRAFFIC_REQ = 0xB0;

        /// <summary>
        /// 교통 데이터 수신 응답
        /// </summary>
        public const byte OP_TRAFFIC_RES = 0xB1;



        public const byte OP_HISTORIC_REQ = 0xB2;
        public const byte OP_HISTORIC_RES = 0xB3;


        public const byte OP_START_STOP_REQ = 0xB8;
        public const byte OP_START_STOP_RES = 0xB9;

        public const byte OP_ECHO_BACK_REQ = 0xBA;
        public const byte OP_ECHO_BACK_RES = 0xBB;


        public const byte OP_VDS_STATUS_REQ = 0xBC;
        public const byte OP_VDS_STATUS_RES = 0xBD;

        public const byte OP_SET_TIME_REQ = 0xBE;
        public const byte OP_SET_TIME_RES = 0xBF;

        public static int InitWorkData(ref WorkData workData)
        {
            workData.sendCount = 0;
            workData.sleepCount = 0;
            workData.guid = Guid.NewGuid().ToString();

            return 1;
        }

        public static IOpData GetDataFrame(byte[] data)
        {
            IOpData result = null;
            switch(data[0])
            {
                case OP_TRAFFIC_REQ:
                    result = new TrafficRequest();
                    break;

                case OP_TRAFFIC_RES:
                    result = new TrafficResponse();
                    break;
                case OP_HISTORIC_REQ:
                    result = new HistoricalTrafficRequest();
                    break;
                case OP_HISTORIC_RES:
                    result = new HistoricalTrafficResponse();
                    break;
                case OP_START_STOP_REQ:
                    result = new VDSStartRequest();
                    break;

                case OP_START_STOP_RES:
                    result = new VDSStartResponse();
                    break;

                case OP_ECHO_BACK_REQ:
                    result = new EchoBackRequest();
                    break;

                case OP_ECHO_BACK_RES:
                    result = new EchoBackResponse();
                    break;

                case OP_VDS_STATUS_REQ:
                    result = new VDSStatusRequest();
                    break;

                case OP_VDS_STATUS_RES:
                    result = new VDSStatusResponse();
                    break;

                case OP_SET_TIME_REQ:
                    result = new VDSSetTimeRequest();
                    break;

                case OP_SET_TIME_RES:
                    result = new VDSSetTimeResponse();
                    break;
            }

            if(result!=null)
            {
                result.Deserialize(data);
            }
            return result;
        }
    }
}
