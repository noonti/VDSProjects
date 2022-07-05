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
        public int Direction { get; set; }// 1: TO Left  2: TO Right
        public List<LaneInfo> LaneList = new List<LaneInfo>();

        public int AddLaineInfo(LaneInfo lane)
        {
            LaneList.Add(lane);
            return LaneList.Count;
        }
    }
}
