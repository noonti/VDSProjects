using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class TrafficLane
    {
        public int lane; // 1 부터 시작...
        public int travel_direction; // 1: TO Right  2: TO Left
        public String laneName;

        public TrafficLane()
        {
            lane = 0;
            travel_direction = 0;
            laneName = String.Empty;
        }
    }
}
