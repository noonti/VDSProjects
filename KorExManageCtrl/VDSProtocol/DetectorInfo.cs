using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class DetectorInfo : IExOPData
    {
        public byte[] errorInfo = new byte[8];
        public byte[] accidentInfo = new byte[4];
        public byte sensorCount;

        public List<DetectInfo> detectInfoList = new List<DetectInfo>();

        public byte laneCount;

        public List<LaneInfo> laneInfoList = new List<LaneInfo>();




        public int Deserialize(byte[] packet)
        {
            int idx = 0;

            Array.Copy(packet, idx, errorInfo, 0, 8);
            idx += 8;

            Array.Copy(packet, idx, accidentInfo, 0, 4);
            idx += 4;

            sensorCount = packet[idx++];

            for(int i = 0;i< sensorCount && idx < packet.Length ;i++)
            {
                DetectInfo loop = new DetectInfo();
                byte[] data = new byte[packet.Length - idx];
                Array.Copy(packet, idx, data, 0, packet.Length - idx);
                idx += loop.Deserialize(data);
                detectInfoList.Add(loop);
            }

            laneCount = packet[idx++];

            for (int i = 0; i < laneCount && idx < packet.Length; i++)
            {
                LaneInfo lane = new LaneInfo();
                byte[] data = new byte[packet.Length - idx];
                Array.Copy(packet, idx, data, 0, packet.Length - idx);
                idx += lane.Deserialize(data);

                laneInfoList.Add(lane);
            }
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            try
            {
                int idx = 0;
                int totalLength = 8 + 4 + 1 + 1 + detectInfoList.Count * 3 + laneInfoList.Count * 2;
                result = new byte[totalLength];

                Array.Copy(errorInfo, 0, result, idx, 8);
                idx += 8;

                Array.Copy(accidentInfo, 0, result, idx, 4);
                idx += 4;

                result[idx++] = sensorCount;
                foreach(var loop in detectInfoList)
                {
                    result[idx++] = loop.trafficCount;
                    Array.Copy(loop.occupyTime, 0, result, idx, 2);
                    idx+=2;
                }

                result[idx++] = laneCount;
                foreach (var lane in laneInfoList)
                {
                    result[idx++] = lane.averageSpeed;
                    result[idx++] = lane.averageLength;
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
