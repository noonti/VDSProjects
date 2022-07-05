using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class SpeedData : IExOPData
    {
        public ushort[] speedCategory = new ushort[12];
        //byte[][] speedCategory = new byte[12][];


        public SpeedData()
        {
        }

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] count = new byte[2];
            for(int i = 0;i<12 && idx<packet.Length;i++)
            {
                Array.Copy(packet, idx, count, 0, 2);
                idx += 2;
                speedCategory[i] = Utility.toLittleEndianInt16(count);
            }
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[24]; ;
            byte[] category = new byte[2];
            try
            {
                int idx = 0;
                for(int i =0;i<12;i++)
                {
                    category = Utility.toBigEndianInt16(speedCategory[i]);
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
