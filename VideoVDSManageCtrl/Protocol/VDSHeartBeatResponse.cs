using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace VideoVDSManageCtrl.Protocol
{
    public class VDSHeartBeatResponse : VDSResponse, IOpData
    {
        public String vdsControllerId { get; set; }
        public String heartBeatTime { get; set; }

        public void SetProperty(VDSHeartBeatResponse data)
        {
            vdsControllerId = data.vdsControllerId;
            heartBeatTime = data.heartBeatTime;
            base.SetProperty(data);
        }

        public int Deserialize(byte[] packet)
        {

            int nResult = 0;
            try
            {
                String jsonString = Utility.ByteToString(packet);

                var data = JsonConvert.DeserializeObject<VDSHeartBeatResponse>(jsonString);
                SetProperty(data);

                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;

        }

        public byte[] Serialize()
        {
            byte[] result;
            try
            {
                String jsonString = JsonConvert.SerializeObject(this);
                result = Utility.StringToByte(jsonString);
            }
            catch (Exception ex)
            {
                result = null;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return result;
        }
    }
}
