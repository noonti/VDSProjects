using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ControllerSyncRequest :  IExOPData
    {
        public byte frameNo;
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            //idx = base.Deserialize(packet);
            frameNo = packet[idx++];
            return idx;
        }


        public byte[] Serialize()
        {
            byte[] result = new byte[1];;
            try
            {
                result[0] = frameNo;

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
