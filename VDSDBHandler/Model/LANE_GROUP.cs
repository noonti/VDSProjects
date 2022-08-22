using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public class LANE_GROUP
	{
		public int ID { get; set; }

		public int CSN_NO { get; set; }


		public String LANE_GROUP_NAME { get; set; }


		public int LANE_SORT { get; set; }

		public int DIRECTION { get; set; }

		public int LANE_COUNT { get; set; }

		public String REG_DATE { get; set; }

		List<LANE_INFO> laneInfoList = new List<LANE_INFO>();

	}
}
