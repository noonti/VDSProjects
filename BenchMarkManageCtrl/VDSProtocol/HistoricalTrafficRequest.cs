using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class HistoricalTrafficRequest : OPData, IOpData
    {

        public HistoricalTrafficRequest()
        {
            _OPCode = DataFrameDefine.OP_HISTORIC_REQ;
        
        }


        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                if (packet.Length == 1 && _OPCode == packet[0])
                {
                    
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
            byte[] result = new byte[1];
            try
            {
                result[0] = _OPCode;
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
