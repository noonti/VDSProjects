using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Protocol.admin
{ 
    public class MAControlVDSResponse : MAResponse
    {
        public String vdsControllerId { get; set; }
        public String commandName { get; set; }  // SERVICE, TRAFFIC DATA , ...
        public String operation { get; set; }    // START / STOP /....
        public String result { get; set; }

        public void SetProperty(MAControlVDSResponse data)
        {
            vdsControllerId = data.vdsControllerId;
            commandName = data.commandName;
            operation = data.operation;
            result = data.result;
            base.SetProperty(data);
        }

        public new int Deserialize(byte[] packet)
        {

            int nResult = 0;
            try
            {
                String jsonString = Utility.ByteToString(packet);
                var data = JsonConvert.DeserializeObject<MAControlVDSResponse>(jsonString);

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

        public new byte[] Serialize()
        {
            byte[] result;
            try
            {
                String jsonString = JsonConvert.SerializeObject(this);
                Console.WriteLine($"MAControlVDSResponse:{jsonString}");
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
