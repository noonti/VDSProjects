using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ExRequest:IExOPData
    {
        //public byte[] transactionNo = new byte[8];

        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                //if (packet.Length >= 8 )
                //{
                //    Array.Copy(packet, 0, transactionNo, 0, 8);
                    
                //    nResult = 8;
                //}
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
            //byte[] result = new byte[8];
            try
            {
                //Array.Copy(transactionNo, 0, result, 0, 8);
            }
            catch (Exception ex)
            {
                //result = null;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return null;
        }


    }
}
