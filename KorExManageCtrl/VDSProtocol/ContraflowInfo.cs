using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ContraflowInfo : IExOPData
    {
        public byte laneCount;
        public byte[] reverseRun = new byte[16];


        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            laneCount = packet[idx++];
            for(int i=0;i< laneCount; i++)
            {
                reverseRun[i] = packet[idx++];
            }
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[laneCount + 1] ;
            try
            {
                int idx = 0;
                result[idx++] = laneCount;
                for(int i=0;i< laneCount; i++)
                {
                    result[idx++] = reverseRun[i];
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
