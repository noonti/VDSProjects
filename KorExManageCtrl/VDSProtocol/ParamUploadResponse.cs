using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ParamUploadResponse : IExOPData
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

                if (param!=null)
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
            byte[] data;
            byte[] paramData = null;
            int idx = 0;
            try
            {
                if(param!=null)
                {
                    paramData = param.Serialize();
                    result = new byte[1 + paramData.Length];
                }
                else
                {
                    result = new byte[ 1 ];
                }
                result[idx++] = paramIndex;
                if(paramData != null)
                {
                    Array.Copy(paramData, 0, result, idx, paramData.Length);
                    idx += paramData.Length;
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
