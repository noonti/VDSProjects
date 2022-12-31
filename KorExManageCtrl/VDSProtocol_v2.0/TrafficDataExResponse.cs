using KorExManageCtrl.VDSProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol_v2._0
{
    public class TrafficDataExResponse : IExOPData
    {
        public byte frameNo;
        public byte[] errorInfo = new byte[4];
        public byte laneCount;
        public List<LaneInfoEx> laneInfoList = new List<LaneInfoEx>();

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            frameNo = packet[idx++];
            Array.Copy(packet, idx, errorInfo, 0, 4);
            idx += 4;
            laneCount = packet[idx++];
            for (int i = 0; i < laneCount && idx < packet.Length; i++)
            {
                LaneInfoEx laneInfo = new LaneInfoEx();
                byte[] laneData = new byte[packet.Length - idx];
                Array.Copy(packet, idx, laneData, 0, packet.Length - idx);
                idx += laneInfo.Deserialize(laneData);
                laneInfoList.Add(laneInfo);
            }
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            try
            {
                int idx = 0;
                int totalLength = 1 + 4 + 1 + laneInfoList.Count * 7;
                result = new byte[totalLength];

                result[idx++] = frameNo;

                Array.Copy(errorInfo, 0, result, idx, 4);
                idx += 4;
                result[idx++] = (byte)laneInfoList.Count;

                foreach (var lane in laneInfoList.OrderBy(x=>x.lane).ToList())
                {
                    byte[] laneData = lane.Serialize();
                    Array.Copy(laneData, 0, result, idx, laneData.Length);
                    idx += laneData.Length;
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
