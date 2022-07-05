using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class VDSStatusResponse : OPData, IOpData
    {

        public byte[] checkTime = new byte[8];
        public byte[] VDSStatus = new byte[2];

        public VDSStatusResponse()
        {
            _OPCode = DataFrameDefine.OP_VDS_STATUS_RES;
        }

        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                if (packet.Length == 11 && _OPCode == packet[0])
                {
                    Array.Copy(packet, 1, checkTime, 0, 8);
                    Array.Copy(packet, 9, VDSStatus, 0, 2);
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
            byte[] result = new byte[11];
            try
            {
                result[0] = _OPCode;
                Array.Copy(checkTime, 0, result, 1, 8);
                Array.Copy(VDSStatus, 0, result, 9, 2);
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
