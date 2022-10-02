using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class TrafficCategory
    {
		public int Id { get; set; }

		public int CategoryNo { get; set; }

		public String CategoryUnit { get; set; }
		public byte FromValue { get; set; }
		public byte ToValue { get; set; }

		public int CategoryType { get; set; } // 1: Speed category 2: Length Category
		public DateTime REG_DATE { get; set; }
	}
}
