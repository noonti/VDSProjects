using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler
{
    public class RADAR_OBJECT_DATA
	{
        public int SEQ { get;set; }
		public String DETECT_TIME { get;set; }	
		public int ID { get; set; }
		public int STATE { get; set; }
		public int DIRECTION { get; set; }
		public int LANE { get; set; }
		public double YY { get; set; }
		public double XX { get; set; }
		public double Y { get; set; }
		public double X { get; set; }

	}
}
