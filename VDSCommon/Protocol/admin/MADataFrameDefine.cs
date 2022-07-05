using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Protocol.admin
{
    public static class MADataFrameDefine
    {
        public const int VDS_FRAME_HEADER = 27;
        public const byte VERSION = 0x01;

        /// <summary>
        /// REQUEST TYPE 정의 
        /// </summary>
        public const byte TYPE_REQUEST = 0x01;
        public const byte TYPE_RESPONSE = 0x02;
        public const byte TYPE_COMMAND = 0x03;


        public const byte OPCODE_AUTH_VDS = 0x10;
        public const byte OPCODE_EVENT_TRAFFIC = 0x20;
        public const byte OPCODE_HISTORIC_TRAFFIC = 0x21;
        public const byte OPCODE_HEARTBEAT = 0x30;

        

        public const byte OPCODE_CONTROL_SERVICE = 0x40;

        public const byte OPCODE_VDS_CONFIG = 0x50;


        public const String COMMAND_TRAFFIC_SEND = "TRAFFIC_DATA_SEND";
        public const String COMMAND_VDS_SERVICE = "VDS_SERVICE";


        public const String OPERATION_START = "START";
        public const String OPERATION_STOP = "STOP";




        public static IOpData GetDataFrame(byte reqeustType, byte opcode, byte[] data)
        {
            IOpData result = null;
            String jsonString = Utility.ByteToString(data);
            switch (opcode)
            {
                case OPCODE_AUTH_VDS:
                    switch (reqeustType)
                    {
                        case TYPE_REQUEST:
                            result = new MAAuthRequest();
                            break;
                        case TYPE_RESPONSE:
                            result = new MAAuthResponse();
                            break;
                    }
                    break;
                case OPCODE_EVENT_TRAFFIC:
                    result = new MATrafficDataEvent();
                    break;
                //case OPCODE_HISTORIC_TRAFFIC:
                //    switch (reqeustType)
                //    {
                //        case TYPE_REQUEST:
                //            result = new VDSHistoricTrafficDataRequest();
                //            break;
                //        case TYPE_RESPONSE:
                //            result = new VDSHistoricTrafficDataResponse();
                //            break;
                //    }


                    break;
                case OPCODE_HEARTBEAT:
                    switch (reqeustType)
                    {
                        case TYPE_REQUEST:
                            result = new MAHeartBeatRequest();
                            break;
                        case TYPE_RESPONSE:
                            result = new MAHeartBeatResponse();
                            break;
                    }
                    break;
                case OPCODE_CONTROL_SERVICE:
                    switch(reqeustType)
                    {
                        case TYPE_REQUEST:
                            result = new MAControlVDSRequest();
                            break;
                        case TYPE_RESPONSE:
                            result = new MAControlVDSResponse();
                            break;
                    }
                    break;

                case OPCODE_VDS_CONFIG:
                    switch (reqeustType)
                    {
                        case TYPE_REQUEST:
                            result = new MAVDSConfigRequest();
                            break;
                        case TYPE_RESPONSE:
                            result = new MAResponse();
                            break;
                    }
                    break;
            }

            if (result != null)
            {
                result.Deserialize(data);
            }
            return result;
        }
    }
}
