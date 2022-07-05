using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public interface IOpData
    {
        int Deserialize(byte[] packet);
        byte[] Serialize();
        //void SetAddLogDelegate(AddLogDelegate addLog);
        //int AddLog(LOG_TYPE logType, String strLog);

    }
}
