using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace VideoVDSManageCtrl.Protocol
{
    public class TrafficData : TrafficDataEvent, IOpData
    {
        //public String id { get; set; }
        //public int lane { get; set; }
        //public int direction { get; set; }
        //public int length { get; set; }
        //public double speed { get;set;}

        //[JsonProperty(PropertyName = "class")]
        //public int vehicle_class { get;set;}
        
        //public int occupyTime { get; set; }

        //public int loop1OccupyTime { get; set; }
        //public int loop2OccupyTime { get; set; }

        //public String reverseRunYN { get; set; }

        //public int vehicleGap { get; set; }

        //public String detectTime { get; set; }


        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                String jsonString = Utility.ByteToString(packet);
                var data = JsonConvert.DeserializeObject<TrafficData>(jsonString);

                id = data.id;
                lane = data.lane;
                direction = data.direction;
                length = data.length;
                speed = data.speed;
                vehicle_class = data.vehicle_class;
                occupyTime = data.occupyTime;
                loop1OccupyTime = data.loop1OccupyTime;
                loop2OccupyTime = data.loop2OccupyTime;
                reverseRunYN = data.reverseRunYN;
                vehicleGap = data.vehicleGap;
                detectTime = data.detectTime;



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
