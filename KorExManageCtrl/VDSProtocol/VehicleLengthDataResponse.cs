using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class VehicleLengthDataResponse : ExResponse, IExOPData
    {
        public byte laneNo;

        public LengthData lengthData = new LengthData();


        new public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] data;
            idx = base.Deserialize(packet);
            laneNo = packet[idx++];
            int cnt = packet.Length - idx;
            data = new byte[cnt];
            Array.Copy(packet, idx, data, 0, cnt);
            idx += lengthData.Deserialize(data);
            return idx;

        }


        new public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            try
            {
                int idx = 0;
                byte[] data = base.Serialize();
                byte[] length = lengthData.Serialize();

                if (data.Length > 0 && length.Length > 0)
                {
                    result = new byte[data.Length + length.Length + 1];
                    Array.Copy(data, 0, result, idx, data.Length);
                    idx += data.Length;
                    result[idx++] = laneNo;
                    Array.Copy(length, 0, result, idx, length.Length);
                    idx += length.Length;

                }
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
