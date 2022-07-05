using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class SimulationTemplate : IExOPData
    {
        public byte dataStreamNo1;
        public byte simulationEnabled1;
        public byte vehicleLength1;
        public byte speed1;
        public byte headway1;
        public byte distance1;
        public byte loopLength1;

        public byte dataStreamNo2;
        public byte simulationEnabled2;
        public byte vehicleLength2;
        public byte speed2;
        public byte headway2;
        public byte distance2;
        public byte loopLength2;

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            try
            {
                if (packet.Length == 14)
                {

                    dataStreamNo1 = packet[idx++];
                    simulationEnabled1 = packet[idx++];
                    vehicleLength1 = packet[idx++];
                    speed1 = packet[idx++];
                    headway1 = packet[idx++];
                    distance1 = packet[idx++];
                    loopLength1 = packet[idx++];

                    dataStreamNo2 = packet[idx++];
                    simulationEnabled2 = packet[idx++];
                    vehicleLength2 = packet[idx++];
                    speed2 = packet[idx++];
                    headway2 = packet[idx++];
                    distance2 = packet[idx++];
                    loopLength2 = packet[idx++];


                }
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
            byte[] result = new byte[14];
            int idx = 0;
            try
            {
                result[idx++] = dataStreamNo1;
                result[idx++] = simulationEnabled1;
                result[idx++] = vehicleLength1;

                result[idx++] = speed1;
                result[idx++] = headway1;
                result[idx++] = distance1;
                result[idx++] = loopLength1;

                result[idx++] = dataStreamNo2;
                result[idx++] = simulationEnabled2;
                result[idx++] = vehicleLength2;

                result[idx++] = speed2;
                result[idx++] = headway2;
                result[idx++] = distance2;
                result[idx++] = loopLength2;

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


