using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Protocol.admin
{
    public interface IOpData
    {
        int Deserialize(byte[] packet);
        byte[] Serialize();
    }
}