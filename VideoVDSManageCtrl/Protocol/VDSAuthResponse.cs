using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace VideoVDSManageCtrl.Protocol
{
    public class VDSAuthResponse : VDSResponse,  IOpData
    {
        public String rtspSourceURL { get; set; }
        public String rtspDetectionURL { get; set; }


        public void SetProperty(VDSAuthResponse data)
        {
            rtspSourceURL = data.rtspSourceURL;
            rtspDetectionURL = data.rtspDetectionURL;
            base.SetProperty(data);
        }

        public int Deserialize(byte[] packet)
        {

            int nResult = 0;
            try
            {
                String jsonString = Utility.ByteToString(packet);
                var data = JsonConvert.DeserializeObject<VDSAuthResponse>(jsonString);

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
                Console.WriteLine($"VDSAuthResponse:{jsonString}");
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
