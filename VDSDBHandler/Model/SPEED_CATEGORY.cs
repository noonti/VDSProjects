using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public  class SPEED_CATEGORY
    {
		public int ID { get; set; }

		public int CATEGORY_NO { get; set; }

		public String SPEED_UNIT { get; set; }	
		
		public float FROM_VALUE { get; set; }
		public float TO_VALUE { get; set; }
		public DateTime REG_DATE { get; set; }
	}
}
