using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class EchoBackResponse : OPData, IOpData
    {
        public byte[] EchoData = new byte[100];
        public EchoBackResponse()
        {
            _OPCode = DataFrameDefine.OP_ECHO_BACK_RES;
        }

        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                if (packet.Length == 101 && _OPCode == packet[0])
                {
                    Array.Copy(packet, 1, EchoData, 0, 100);
                    nResult = 1;
                }
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[101];
            try
            {
                result[0] = _OPCode;
                Array.Copy(EchoData, 0, result, 1, 100);
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
