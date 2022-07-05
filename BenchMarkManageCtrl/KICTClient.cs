using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class KICTClient : SessionContext
    {
        public DataFrame _prevDataFrame;

        public KICTClient()
        {
            _prevDataFrame = null;
        }
    }
}
