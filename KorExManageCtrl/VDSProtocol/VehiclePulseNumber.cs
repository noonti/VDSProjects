using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class VehiclePulseNumber : IExOPData
    {
        public byte presenceNumber;
        public byte noPresenceNumber;
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            try
            {
                if (packet.Length == 2)
                {
                    presenceNumber = packet[idx++];
                    noPresenceNumber = packet[idx++];
                }
            }
            catch (Exception ex)
            {
                idx = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return idx;


        }

        public byte[] Serialize()
        {
            byte[] result = new byte[2];
            int idx = 0;
            try
            {
                result[idx++] = presenceNumber;
                result[idx++] = noPresenceNumber;
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
