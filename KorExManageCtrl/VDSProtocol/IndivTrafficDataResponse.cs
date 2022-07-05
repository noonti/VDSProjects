using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class IndivTrafficDataResponse : IExOPData
    {
        public byte timeFrameNo;
        public UInt16 totalCount;
        public List<IndivTrafficData> trafficDataList = new List<IndivTrafficData>();

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] data;
            timeFrameNo = packet[idx++];

            // total Count 
            data = new byte[2];
            Array.Copy(packet, idx, data, 0, 2);
            idx += 2;
            totalCount = Utility.toLittleEndianInt16(data);

            for (int i = 0; i < totalCount && idx < packet.Length; i++)
            {
                IndivTrafficData trafficData = new IndivTrafficData();
                data = new byte[packet.Length - idx];
                Array.Copy(packet, idx, data, 0, packet.Length - idx);
                idx += trafficData.Deserialize(data);
                trafficDataList.Add(trafficData);
            }
            return idx;

        }


        public byte[] Serialize()
        {
            byte[] result;
            byte[] data;
            int idx = 0;
            try
            {
                int totalLength = 1 + 2 + trafficDataList.Count * 6  ;
                result = new byte[totalLength];

                // frame no
                result[idx++] = timeFrameNo;

                // 차량 대수 
                totalCount = (UInt16) trafficDataList.Count;
                data = Utility.toBigEndianInt16(totalCount);
                Array.Copy(data, 0, result, idx, data.Length);
                idx += data.Length;

                for(int i =0;i<totalCount;i++)
                {
                    IndivTrafficData trafficData = trafficDataList[i];
                    data = trafficData.Serialize();

                    Array.Copy(data, 0, result, idx, data.Length);
                    idx += data.Length;
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
