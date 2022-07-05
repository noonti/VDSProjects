using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class VDSStartResponse : OPData, IOpData
    {
        public byte VDSFlag;
        public VDSStartResponse()
        {
            _OPCode = DataFrameDefine.OP_START_STOP_RES;
            VDSFlag = 0;
        }

        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                if (packet.Length == 2 && _OPCode == packet[0])
                {
                    VDSFlag = packet[1];
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
            byte[] result = new byte[2];
            try
            {
                result[0] = _OPCode;
                result[1] = VDSFlag;
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
