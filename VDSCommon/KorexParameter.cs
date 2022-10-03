using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class KorexParameter
    {
		public short SpeedAccuEnabled { get; set; }
		public short LengthAccuEnabled { get; set; }
		public short speedCalcuEnabled { get; set; }
		public short lengthCalcuEnabled { get; set; }
		public short reverseRunEnabled { get; set; }
		public short oscillationThreshold { get; set; }
		public short autoSyncPeriod { get; set; }

	}
}
