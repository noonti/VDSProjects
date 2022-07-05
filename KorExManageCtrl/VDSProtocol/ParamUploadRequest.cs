using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ParamUploadRequest : IExOPData
    {
        public byte paramIndex;

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            try
            {
                paramIndex = packet[idx++];

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
            int idx = 0;
            try
            {
                result = new byte[1];
                result[idx++] = paramIndex;

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
