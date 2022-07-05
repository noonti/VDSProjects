using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class VDSVersionResponse : IExOPData
    {
        public byte[] version = new byte[4];

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            int length = 0;
            length = packet.Length - idx;
            Array.Copy(packet, idx, version, 0, version.Length);
            idx += version.Length;
            return idx;
        }


        public byte[] Serialize()
        {
            byte[] result;
            byte[] data;

            int idx = 0;
            try
            {
                result = new byte[version.Length];
                Array.Copy(version, 0, result, idx, version.Length);
                idx += version.Length;

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
