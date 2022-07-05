using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSController
{
    public class LaneInfo
    {
        public String LaneName { get; set; }
        public int Lane;
        public int Direction { get; set; }// 1: TO Right  2: TO Left
    }
}
