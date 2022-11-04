using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ParamDownloadRequest : IExOPData
    {
        public byte paramIndex;
        public IExOPData param;

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            try
            {
               
                paramIndex = packet[idx++];
                param = ExDataFrameDefine.CreateParam(paramIndex);

                byte[] data = new byte[packet.Length - idx];
                Array.Copy(packet, idx, data, 0, packet.Length - idx);
                idx += param.Deserialize(data);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return idx;
        }


        public byte[] Serialize()
        {
            byte[] result;
            byte[] paramData;
            int idx = 0;
            try
            {
              
                paramData = param.Serialize();

                result = new byte[paramData.Length+1];
                
                result[idx++] = paramIndex;
                Array.Copy(paramData, 0, result, idx, paramData.Length);

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
