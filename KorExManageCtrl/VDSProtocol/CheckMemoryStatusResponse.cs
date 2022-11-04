using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class CheckMemoryStatusResponse : ExResponse, IExOPData
    {
       

        new public int Deserialize(byte[] packet)
        {
            int idx = base.Deserialize(packet);
            return idx;
        }


        new public byte[] Serialize()
        {
            byte[] result;
            try
            {
                result = base.Serialize();

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

