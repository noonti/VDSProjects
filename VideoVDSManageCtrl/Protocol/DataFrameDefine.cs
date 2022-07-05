using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace VideoVDSManageCtrl.Protocol
{
    public static class DataFrameDefine
    {
        public const int VDS_FRAME_HEADER = 27;
        public const byte VERSION = 0x01;

        /// <summary>
        /// REQUEST TYPE 정의 
        /// </summary>
        public const byte TYPE_REQUEST =    0x01;
        public const byte TYPE_RESPONSE =   0x02;
        public const byte TYPE_COMMAND =    0x03;


        public const byte OPCODE_AUTH_VDS =         0x10;
        public const byte OPCODE_EVENT_TRAFFIC =    0x20;
        public const byte OPCODE_HISTORIC_TRAFFIC = 0x21;
        public const byte OPCODE_HEARTBEAT =        0x30;



        public static IOpData GetDataFrame(byte reqeustType, byte opcode , byte[] data)
        {
            IOpData result = null;
            String jsonString = Utility.ByteToString(data);
            switch (opcode)
            {
                case OPCODE_AUTH_VDS:
                    switch(reqeustType)
                    {
                        case TYPE_REQUEST:
                            result = new VDSAuthRequest();
                            break;
                        case TYPE_RESPONSE:
                            result = new VDSAuthResponse();
                            break;
                    }
                    break;
                case OPCODE_EVENT_TRAFFIC:
                    result = new VDSTrafficDataEvent();
                    break;
                case OPCODE_HISTORIC_TRAFFIC:
                    switch (reqeustType)
                    {
                        case TYPE_REQUEST:
                            result = new VDSHistoricTrafficDataRequest();
                            break;
                        case TYPE_RESPONSE:
                            result = new VDSHistoricTrafficDataResponse();
                            break;
                    }

                    
                    break;
                case OPCODE_HEARTBEAT:
                    switch (reqeustType)
                    {
                        case TYPE_REQUEST:
                            result = new VDSHeartBeatRequest();
                            break;
                        case TYPE_RESPONSE:
                            result = new VDSHeartBeatResponse();
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
