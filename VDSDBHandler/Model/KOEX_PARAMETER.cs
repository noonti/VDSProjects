using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSDBHandler.Model
{
    public class KOREX_PARAMETER
	{

		public short SPEED_ACCU_ENABLED { get; set; }
		public short LENGTH_ACCU_ENABLED { get; set; }
		public short SPEED_CALCU_ENABLED { get; set; }
		public short LENGTH_CALCU_ENABLED { get; set; }
		public short REVERSE_RUN_ENABLED { get; set; }
		public short OSCILLATION_THRESHOLD { get; set; }
		public short AUTO_SYNC_PERIOD { get; set; }


	}
}
