using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class LaneInfo : IExOPData
    {
        public byte averageSpeed;
        public byte averageLength;

        public LaneInfo()
        {
            averageLength = 0;
            averageSpeed = 0;

        }

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            averageSpeed = packet[idx++];
            averageLength = packet[idx++];

            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            try
            {
                int idx = 0;
                result = new byte[2];
                result[idx++] = averageSpeed;
                result[idx++] = averageLength;
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
