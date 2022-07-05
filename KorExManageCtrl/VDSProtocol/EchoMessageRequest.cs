using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class EchoMessageRequest : IExOPData
    {
        public byte[] echoMessage;

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            int length = 0;
            length = packet.Length - idx;
            echoMessage = new byte[length];
            Array.Copy(packet, idx, echoMessage, 0, length);
            idx += length;
            return idx;
        }


        public byte[] Serialize()
        {
            byte[] result;
            byte[] data;
          
            int idx = 0;
            try
            {
                result = new byte[echoMessage.Length];
                Array.Copy(echoMessage, 0, result, idx, echoMessage.Length);
                idx += echoMessage.Length;

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
