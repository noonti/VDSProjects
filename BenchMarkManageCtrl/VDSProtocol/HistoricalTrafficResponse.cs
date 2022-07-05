using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class HistoricalTrafficResponse : OPData, IOpData
    {
        public UInt16 TrafficDataCount;
        public List<TrafficData> trafficDataList = new List<TrafficData>();

        public HistoricalTrafficResponse()
        {
            _OPCode = DataFrameDefine.OP_HISTORIC_RES;
            TrafficDataCount = 0;
        }


        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                if (_OPCode == packet[0])
                {
                    TrafficDataCount =  packet[1];
                    //byte[] cnt = new byte[2];
                    //Array.Copy(packet, 1, cnt, 0, 2);
                    //TrafficDataCount = Utility.toLittleEndianInt16(cnt);
                    int i = 0;
                    int startIdx = 2;
                    while(i< TrafficDataCount &&  startIdx < packet.Length ) // 갯수 만큼 그리고 인덱스 체크 하며 루프 돈다
                    {
                        byte[] traffic = new byte[17];
                        Array.Copy(packet, startIdx, traffic, 0, 17);
                        TrafficData trafficData = new TrafficData();
                        trafficData.Deserialize(traffic);
                        trafficDataList.Add(trafficData);
                        startIdx += 17;
                        i++;
                    }
                    nResult = 1;
                }
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[1+1+ trafficDataList.Count * 17 ];
            try
            {
                result[0] = _OPCode;
                result[1] = (byte)trafficDataList.Count;
                int startIndex = 2;
                foreach(var trafficData in trafficDataList)
                {
                    Array.Copy(trafficData.Serialize(),0, result,startIndex,17);
                    startIndex += 17;
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
