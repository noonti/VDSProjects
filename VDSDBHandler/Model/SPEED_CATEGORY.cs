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
		
		public short FROM_VALUE { get; set; }
		public short TO_VALUE { get; set; }
		public DateTime REG_DATE { get; set; }
	}
}
