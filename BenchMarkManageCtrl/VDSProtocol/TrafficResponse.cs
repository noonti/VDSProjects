using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class TrafficResponse : OPData, IOpData
    {
        public byte lane;
        public byte direction;
        public byte[] checkTime = new byte[8];
        public byte status;


        public TrafficResponse()
        {
            _OPCode = DataFrameDefine.OP_TRAFFIC_RES;
        }


        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                if (packet.Length == 12 && _OPCode == packet[0])
                {
                    //data1[idx++] = 0xB0; // Op 0 
                    //data1[idx++] = 0x01; //1차선 1
                    //data1[idx++] = 0x01; //상행  2
                    //data1[idx++] = 0x01; //년    3 
                    //data1[idx++] = 0x01; //년
                    //data1[idx++] = 0x04; //월
                    //data1[idx++] = 0x15; //일
                    //data1[idx++] = 0x0A; //시
                    //data1[idx++] = 0x0B; //분
                    //data1[idx++] = 0x0C; //초
                    //data1[idx++] = 0x01; //ms       

                    //data1[idx++] = 0x00; //처리상태 11

                    lane = packet[1];
                    direction = packet[2];
                    Array.Copy(packet, 3, checkTime, 0, 8);
                    status = packet[11];
                    nResult = 1;

                }
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[12];
            try
            {
                result[0] = _OPCode;
                result[1] = lane;
                result[2] = direction;
                Array.Copy(checkTime, 0, result, 3, 8);
                result[11] = status;
            }
            catch (Exception ex)
            {
                result = null;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return result;
        }
    }
}
