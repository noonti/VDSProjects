using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class VDSLog
    {
        public LOG_TYPE _logType { get; set; }
        public DateTime _logTime { get; set; }
        public String _log { get; set; }

        public VDSLog()
        {
            _logTime = DateTime.Now;
        }
    }
}
