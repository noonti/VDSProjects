using KorExManageCtrl.VDSProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol_v2._0
{
    public class SystemStatus : IExOPData
    {
        public byte longPowerFail;
        public byte shortPowerFail;
        public byte defaultParameter;
        public byte frontStatus;
        public byte rearStatus;
        public byte fanStatus;
        public byte heaterStatus;
        public byte videoStatus;
        public byte IsReset;
        public byte temperature;
        public byte inputVoltage;
        public byte outputVoltage;


        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            longPowerFail = packet[idx++];
            shortPowerFail = packet[idx++];
            defaultParameter = packet[idx++];
            frontStatus = packet[idx++];
            rearStatus = packet[idx++];
            fanStatus = packet[idx++];
            heaterStatus = packet[idx++];
            videoStatus = packet[idx++];
            IsReset = packet[idx++];
            temperature = packet[idx++];
            inputVoltage = packet[idx++];
            outputVoltage = packet[idx++];
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[12];
            int idx = 0;
            try
            {

                result[idx++] = longPowerFail;
                result[idx++] = shortPowerFail;
                result[idx++] = defaultParameter;
                result[idx++] = frontStatus;

                result[idx++] = rearStatus;
                result[idx++] = fanStatus;
                result[idx++] = heaterStatus;
                result[idx++] = videoStatus;
                result[idx++] = IsReset;
                result[idx++] = temperature;
                result[idx++] = inputVoltage;
                result[idx++] = outputVoltage;
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
