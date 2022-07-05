using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class VDSSetTimeResponse : OPData, IOpData
    {
        public byte[] setTime = new byte[8];

        public VDSSetTimeResponse()
        {
            _OPCode = DataFrameDefine.OP_SET_TIME_RES;
        }

        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                if (packet.Length == 9 && _OPCode == packet[0])
                {
                    Array.Copy(packet, 1, setTime, 0, 8);
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
            byte[] result = new byte[9];
            try
            {
                result[0] = _OPCode;
                Array.Copy(setTime, 0, result, 1, 8);
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
