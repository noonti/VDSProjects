using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class LengthCategory : IExOPData
    {
        public byte[] category = new byte[3];
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            try
            {
                if (packet.Length == 3)
                {
                    Array.Copy(packet, 0, category, 0, 3);
                    idx += 3;
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
            byte[] result = new byte[3];
            try
            {
                Array.Copy(category, 0, result, 0, 3);
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
