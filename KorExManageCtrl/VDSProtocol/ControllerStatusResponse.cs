using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ControllerStatusResponse : ExResponse, IExOPData
    {
        public byte powerSupplyCount; // 전원장치 갯수
        public byte powerSupplyStatus;  // 전원장치 상태 0 :정상, 1: 비정상 
        public byte boardCount;    // 보드 갯수 
        public ushort boardStatus; // 비트별로 정상 :0 , 비정상 : 1


        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] data;
            byte[] status = new byte[2];

            idx = base.Deserialize(packet);
            powerSupplyCount = packet[idx++];
            powerSupplyStatus = packet[idx++];
            boardCount = packet[idx++];

            Array.Copy(packet, idx, status, 0, 2);
            boardStatus = Utility.toLittleEndianInt16(status);
            idx += 2;
            return idx;
        }


        public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            byte[] status;
            try
            {
                int idx = 0;
                byte[] data = base.Serialize();
                result = new byte[data.Length + 5];
                Array.Copy(data, 0, result, idx, data.Length);
                idx += data.Length;

                result[idx++] = powerSupplyCount;
                result[idx++] = powerSupplyStatus;
                result[idx++] = boardCount;
                status = Utility.toBigEndianInt16(boardStatus);
                Array.Copy(status, 0, result, idx, 2);
                idx += 2;


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
