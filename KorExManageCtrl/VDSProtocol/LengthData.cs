using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class LengthData : IExOPData
    {
        public ushort[] lengthCategory = new ushort[3];

        public LengthData()
        {
        }

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] count = new byte[2];
            for (int i = 0; i < 3 && idx < packet.Length; i++)
            {
                Array.Copy(packet, idx, count, 0, 2);
                idx += 2;
                lengthCategory[i] = Utility.toLittleEndianInt16(count);
            }
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[6]; ;
            byte[] category = new byte[2];
            try
            {
                int idx = 0;
                for (int i = 0; i < 3; i++)
                {
                    category = Utility.toBigEndianInt16(lengthCategory[i]);
                    Array.Copy(category, 0, result, idx, 2);
                    idx += 2;
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
