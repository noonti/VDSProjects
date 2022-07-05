using KorExManageCtrl.VDSProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol_v2._0
{
    public class RealTimeClock : IExOPData
    {
        public byte[] timeinfo = new byte[7]; // yyyymmddhhmmss : bcd 코드 
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            try
            {
                Array.Copy(packet, idx, timeinfo, 0, timeinfo.Length);
                idx += 2;

            }
            catch (Exception ex)
            {
                idx = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return idx;


        }

        public byte[] Serialize()
        {
            byte[] result;
            try
            {
                result = new byte[timeinfo.Length];
                Array.Copy(timeinfo, 0, result, 0, timeinfo.Length);
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
