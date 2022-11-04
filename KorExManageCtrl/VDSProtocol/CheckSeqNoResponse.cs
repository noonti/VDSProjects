using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class CheckSeqNoResponse : IExOPData
    {
        public byte[] seqList;

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            int length = 0;
            length = packet.Length - idx;
            seqList = new byte[length];
            Array.Copy(packet, idx, seqList, 0, length);
            idx += length;
            return idx;
        }


        public byte[] Serialize()
        {
            byte[] result;

            int idx = 0;
            try
            {
                result = new byte[seqList.Length];
                Array.Copy(seqList, 0, result, idx, seqList.Length);
                idx += seqList.Length;

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
