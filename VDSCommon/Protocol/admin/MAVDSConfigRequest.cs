using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon.Config;

namespace VDSCommon.Protocol.admin
{
    public class MAVDSConfigRequest : IOpData
    {
        public String vdsControllerId { get; set; }

        public ControllerConfig  controllerConfig { get; set; }
        public KictConfig kictConfig { get; set; }
        public KorExConfig korExConfig { get; set; }

        public int Deserialize(byte[] packet)
        {

            int nResult = 0;
            try
            {
                String jsonString = Utility.ByteToString(packet);

                var data = JsonConvert.DeserializeObject<MAVDSConfigRequest>(jsonString);
                vdsControllerId = data.vdsControllerId;
                controllerConfig = data.controllerConfig;
                kictConfig = data.kictConfig;
                korExConfig = data.korExConfig;
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
                Console.WriteLine($"MAVDSConfigRequest:{jsonString}");
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
