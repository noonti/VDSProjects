using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class SpeedLoopConfig : IExOPData
    {
        public byte[] loopNo = new byte[32];

       
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            Array.Copy(packet, idx, loopNo, 0, 32);
            idx += 32;
            return idx;
        }


        public byte[] Serialize()
        {
            byte[] result;
            int idx = 0;
            try
            {
                result = loopNo;

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
