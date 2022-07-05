using KorExManageCtrl.VDSProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol_v2._0
{
    public class SpeedDataExResponse : IExOPData
    {
        public byte laneCount;
        public List<SpeedData> speedDataList = new List<SpeedData>();

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            laneCount = packet[idx++];
            for (int i = 0; i < laneCount && idx < packet.Length; i++)
            {
                SpeedData speedDataInfo = new SpeedData();
                byte[] speedData = new byte[packet.Length - idx];
                Array.Copy(packet, idx, speedData, 0, packet.Length - idx);
                idx += speedDataInfo.Deserialize(speedData);
                speedDataList.Add(speedDataInfo);
            }
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            try
            {
                int idx = 0;
                int totalLength = 1 + speedDataList.Count * 2*12;
                result = new byte[totalLength];
                result[idx++] = (byte)speedDataList.Count;
                foreach (var speed in speedDataList)
                {
                    byte[] speedData = speed.Serialize();
                    Array.Copy(speedData, 0, result, idx, speedData.Length);
                    idx += speedData.Length;
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
