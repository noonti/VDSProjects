using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class AccuTrafficDataResponse : IExOPData
    {
        public ushort[] volumData = new ushort[16];



        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            
            byte[] volume = new byte[2];
            for (int i = 0; i < 16 && idx < packet.Length; i++)
            {
                Array.Copy(packet, idx, volume, 0, 2);
                idx += 2;
                volumData[i] = Utility.toLittleEndianInt16(volume);
            }
            return idx;

        }


        public byte[] Serialize()
        {
            byte[] result = null;
            byte[] volume = null;
            try
            {
                int idx = 0;
                result = new byte[32];
                for (int i = 0; i < 16 && idx < result.Length ; i++)
                {
                    volume = Utility.toBigEndianInt16(volumData[i]);
                    Array.Copy(volume, 0, result, idx, 2);
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
