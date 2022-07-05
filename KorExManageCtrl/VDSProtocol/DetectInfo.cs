using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class DetectInfo : IExOPData
    {
        public byte trafficCount;
        public byte[] occupyTime = new byte[2];

        public DetectInfo()
        {
            trafficCount = 0;
            occupyTime[0] = 0x00;
            occupyTime[1] = 0x00;

        }

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            trafficCount = packet[idx++];
            Array.Copy(packet, idx, occupyTime, 0, 2);
            idx += 2;
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            try
            {
                int idx = 0;
                result = new byte[3];

                result[idx++] = trafficCount;
                Array.Copy(occupyTime, 0, result, idx, 2);
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
