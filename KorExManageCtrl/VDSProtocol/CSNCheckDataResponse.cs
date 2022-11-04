using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class CSNCheckDataResponse : ExResponse, IExOPData
    {
        public byte[] csn = new byte[8];

        public CSNCheckDataResponse()
        {
        }

        new public int Deserialize(byte[] packet)
        {
            int idx = base.Deserialize(packet);
            Array.Copy(packet, idx, csn, 0, 8);
            idx += 8;
            return idx;
        }


        new public byte[] Serialize()
        {
            byte[] result;
            byte[] data;
            int idx = 0;
            try
            {
                data = base.Serialize(); // 11 bytes

                result = new byte[19];
                Array.Copy(data, 0, result, 0, data.Length);
                idx += data.Length;
                Array.Copy(csn, 0, result, idx, 8);

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
