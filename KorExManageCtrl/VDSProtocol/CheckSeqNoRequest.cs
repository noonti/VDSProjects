using KorExManageCtrl.VDSProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class CheckSeqNoRequest : IExOPData
    {
        public byte baseNumber;
        public byte counter;

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            baseNumber = packet[idx++];
            counter = packet[idx++];
            return idx;
        }


        public byte[] Serialize()
        {
            byte[] result;
            byte[] data;

            int idx = 0;
            try
            {
                result = new byte[2];
                result[idx++] = baseNumber;
                result[idx++] = counter;


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
