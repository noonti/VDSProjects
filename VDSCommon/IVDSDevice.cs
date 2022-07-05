using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public interface IVDSDevice
    {
        int StartDevice(String address, int port, int localPort=0);
        int StopDevice();

        bool isService();
        int SetDeviceTime(Object callbackFunc, Object workData, DateTime? date);
        int CheckVDSStatus(ref byte[] status, ref byte[] checkTime);
        int SetAddTrafficDataEventDelegate(AddTrafficDataEvent addTrafficDataEvent);
        int AddTrafficDataEvent(TrafficDataEvent trafficDataEvent);

    }
}
