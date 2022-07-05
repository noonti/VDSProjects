using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ParamLoopConfig : IExOPData
    {
        public UInt32 loopInfo;
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            try
            {
                if (packet.Length == 4 )
                {
                    loopInfo = Utility.toLittleEndianInt32(packet);
                    idx = packet.Length;
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
            byte[] result;
            try
            {
                result = Utility.toBigEndianInt32(loopInfo);

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
