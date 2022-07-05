using KorExManageCtrl.VDSProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol_v2._0
{
    public class SetTemperatureRequest : IExOPData
    {
        public byte fanTemperature;
        public byte heaterTemperature;

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            fanTemperature = packet[idx++];
            heaterTemperature = packet[idx++];
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[2];
            int idx = 0;
            try
            {

                result[idx++] = fanTemperature;
                result[idx++] = heaterTemperature;
               
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
