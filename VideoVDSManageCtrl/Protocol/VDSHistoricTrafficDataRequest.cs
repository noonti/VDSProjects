using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace VideoVDSManageCtrl.Protocol
{
    public class VDSHistoricTrafficDataRequest : IOpData
    {
        public String startDateTime { get; set; }
        public String endDateTime { get; set; }

        public int pageSize { get; set; }
        public int pageIndex { get; set; }


        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                String jsonString = Utility.ByteToString(packet);
                var data = JsonConvert.DeserializeObject<VDSHistoricTrafficDataRequest>(jsonString);

                startDateTime = data.startDateTime;
                endDateTime = data.endDateTime;

                pageSize = data.pageSize;
                pageIndex = data.pageIndex;



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
                Console.WriteLine($"VDSHistoricTrafficDataRequest:{jsonString}");
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
