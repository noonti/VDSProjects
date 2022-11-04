using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class SpeedDataResponse : ExResponse, IExOPData
    {
        public byte laneNo;

        public SpeedData speedData = new SpeedData();


        new public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] data;
            idx = base.Deserialize(packet);
            laneNo = packet[idx++];
            int cnt = packet.Length - idx;
            data = new byte[cnt];
            Array.Copy(packet, idx, data, 0, cnt);
            idx += speedData.Deserialize(data);
            return idx;

        }


        new public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            try
            {
                int idx = 0;
                byte[] data = base.Serialize();
                byte[] speed = speedData.Serialize();

                if (data.Length > 0 && speed.Length > 0)
                {
                    result = new byte[data.Length + speed.Length + 1];
                    Array.Copy(data, 0, result, idx, data.Length);
                    idx += data.Length;
                    result[idx++] = laneNo;
                    Array.Copy(speed, 0, result, idx, speed.Length);
                    idx += speed.Length;

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
