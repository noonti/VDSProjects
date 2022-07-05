using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ExResponse : IExOPData
    {
        public byte resultCode;
        public byte errorCode;

        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            int index = 0;
            try
            {
                if (packet.Length >0)
                {
                    
                    resultCode = packet[index++];
                    if(resultCode == ExDataFrameDefine.NAK_ERROR)
                    {
                        errorCode = packet[index++];
                    }
                    nResult = index;
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
            byte[] result = null;
            int index = 0;
            try
            {
                switch(resultCode)
                {
                    case ExDataFrameDefine.ACK_NORMAL:
                        result = new byte[1];
                        result[index++] = resultCode;
                        break;
                    case ExDataFrameDefine.NAK_ERROR:
                        result = new byte[2];
                        result[index++] = resultCode;
                        result[index++] = errorCode;
                        break;
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
