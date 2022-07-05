using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class IncidentDetectThreshold : IExOPData
    {
        public byte incidentCycle;
        public byte period ;
        public byte algorithm; // 1: 점유율, 2: 속도, 3: 교통량, 4: 점유율 및 교통량 
        public UInt16 kfactor_1;
        public UInt16 kfactor_2;

        public UInt16[] threshold = new UInt16[32];


        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] data = new byte[2];
            try
            {
                if (packet.Length == 71)
                {

                    incidentCycle = packet[idx++];
                    period = packet[idx++];
                    algorithm = packet[idx++];

                    Array.Copy(packet, idx, data, 0, 2);
                    idx += 2;
                    kfactor_1 = Utility.toLittleEndianInt16(data);

                    Array.Copy(packet, idx, data, 0, 2);
                    idx += 2;
                    kfactor_2 = Utility.toLittleEndianInt16(data);

                    for (int i = 0; i < 32; i++)
                    {
                        Array.Copy(packet, idx, data, 0, 2);
                        idx += 2;
                        threshold[i] = Utility.toLittleEndianInt16(data);
                    }
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
            byte[] result = new byte[71];
            int idx = 0;
            byte[] data;
            try
            {
                result[idx++] = incidentCycle;
                result[idx++] = period;
                result[idx++] = algorithm;

                data = Utility.toBigEndianInt16(kfactor_1);
                Array.Copy(data, 0, result, idx, 2);
                idx += 2;

                data = Utility.toBigEndianInt16(kfactor_2);
                Array.Copy(data, 0, result, idx, 2);
                idx += 2;

                for(int i=0;i<32;i++)
                {
                    data = Utility.toBigEndianInt16(threshold[i]);
                    Array.Copy(data, 0, result, idx, 2);
                    idx += 2;
                }


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
