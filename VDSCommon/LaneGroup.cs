using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSController
{
    public class LaneGroup
    {


        public String LaneGroupName { get; set; }
        public int LaneSort { get; set; } // 1: 오름차순, 2: 내림차순
        public int Direction { get; set; }// 1: TO Right  2: TO Left 
        public List<LaneInfo> LaneList = new List<LaneInfo>();

        public int AddLaneInfo(LaneInfo lane)
        {
            LaneList.Add(lane);
            return LaneList.Count;
        }

        public byte GetLaneConfig()
        {
            byte result = 0;
            int bitCount = 0;
            /*
             0 bit : 1,9  차로 설치
             1 bit : 2,10 차로 설치
             2 bit : 3,11 차로 설치
             3 bit : 4,12 차로 설치
             4 bit : 5,13 차로 설치
             5 bit : 6,14 차로 설치
             6 bit : 7,15 차로 설치
             7 bit : 8,16 차로 설치

            */

            foreach (var lane in LaneList)
            {
                bitCount = (lane.Lane - 1) % 8;
                result |= (byte)(0x01 << bitCount);
            }
            return result;
        }

        public int SetLaneConfig(byte config)
        {
            int result = 0;
            int i = 0;

            //LaneList.Clear();

            for(i=0;i<8;i++)
            {
                if( ((config >> i) & 0x01) == 0x01 ) // 해당 차선 사용인 경우
                {
                    Console.WriteLine($"차선 사용...설정 Direction={Direction}, 차선= {i + 1}");
                }

            }
            return result;
        }
    }
}
