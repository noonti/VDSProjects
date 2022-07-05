using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class StuckThreshold : IExOPData
    {
        public byte highTrafficDuration;
        public byte highOnDuration;
        public byte highOffDuration;
        public byte lowTrafficDuration;
        public byte lowOnDuration;
        public byte lowOffDuration;

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] data = new byte[2];
            try
            {
                if (packet.Length == 6)
                {

                    highTrafficDuration = packet[idx++];
                    highOnDuration = packet[idx++];
                    highOffDuration = packet[idx++];

                    lowTrafficDuration = packet[idx++];
                    lowOnDuration = packet[idx++];
                    lowOffDuration = packet[idx++];
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
            byte[] result = new byte[6];
            int idx = 0;
            try
            {
                result[idx++] = highTrafficDuration;
                result[idx++] = highOnDuration;
                result[idx++] = highOffDuration;

                result[idx++] = lowTrafficDuration;
                result[idx++] = lowOnDuration;
                result[idx++] = lowOffDuration;

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
