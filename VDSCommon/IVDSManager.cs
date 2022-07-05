using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon.DataType;

namespace VDSCommon
{
    public interface IVDSManager
    {
        int StartManager();
        int StopManager();
        //int SendTrafficData(TargetSummaryInfo target);

    }
}
