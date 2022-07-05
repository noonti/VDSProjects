using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class SpeedCategory : IExOPData
    {
        public byte[] category = new byte[12];
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            try
            {
                if (packet.Length == 12)
                {
                    Array.Copy(packet, 0, category, 0, 12);
                    idx += 12;
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
            byte[] result = new byte[12];
            try
            {
                Array.Copy(category, 0, result,0,12);
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
