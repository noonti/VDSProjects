using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KorExManageCtrl.VDSProtocol
{
    public interface IExOPData
    {
        int Deserialize(byte[] packet);
        byte[] Serialize();
    }
}
