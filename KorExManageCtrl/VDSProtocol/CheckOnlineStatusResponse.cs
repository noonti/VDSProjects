using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class CheckOnlineStatusResponse : IExOPData
    {
        public UInt32 passedTime;
        public byte[] _passedTime = new byte[4];

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            Array.Copy(packet, idx, _passedTime, 0, 4);
            passedTime = Utility.toLittleEndianInt32(_passedTime);
            idx += 4;
            return idx;
        }


        public byte[] Serialize()
        {
            byte[] result;
            byte[] data;
            int idx = 0;
            try
            {

                result = new byte[_passedTime.Length];
                _passedTime = Utility.toBigEndianInt32(passedTime);
                Array.Copy(_passedTime, 0, result, idx, _passedTime.Length);
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
