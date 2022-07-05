using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class PollingThreshold : IExOPData
    {
        public byte paramIndex;
        public byte threshold;

        public PollingThreshold(byte index)
        {
            paramIndex = index;
        }

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            try
            {
                threshold = packet[idx++];

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
            byte[] result = new byte[1];
            try
            {
                result[0] = threshold;
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
