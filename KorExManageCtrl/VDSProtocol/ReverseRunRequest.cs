using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ReverseRunRequest : IExOPData
    {
        public  ContraflowInfo contraflow = new ContraflowInfo();
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] data;
            int cnt = packet.Length - idx;
            data = new byte[cnt];
            Array.Copy(packet, idx, data, 0, cnt);
            idx += contraflow.Deserialize(data);
            return idx;

        }

        public byte[] Serialize()
        {
            byte[] result;
            byte[] data;
            byte[] reverseInfo;
            int idx = 0;
            try
            {
                reverseInfo = contraflow.Serialize();
                result = new byte[reverseInfo.Length + 4];
                // contraflow info
                Array.Copy(reverseInfo, 0, result, idx, reverseInfo.Length);
                idx += reverseInfo.Length;

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
