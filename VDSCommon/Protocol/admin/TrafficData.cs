using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Protocol.admin
{
    public class TrafficData : TrafficDataEvent, IOpData
    {

        public void SetTrafficDataEvent(TrafficDataEvent dataEvent)
        {
            id = dataEvent.id;
            vds_type = dataEvent.vds_type;
            lane = dataEvent.lane;
            direction = dataEvent.direction;
            length = dataEvent.length;
            speed = dataEvent.speed;
            vehicle_class = dataEvent.vehicle_class;
            occupyTime = dataEvent.occupyTime;
            loop1OccupyTime = dataEvent.loop1OccupyTime;
            loop2OccupyTime = dataEvent.loop2OccupyTime;
            reverseRunYN = dataEvent.reverseRunYN;
            vehicleGap = dataEvent.vehicleGap;

            detectTime = dataEvent.detectTime;
            reportYN = dataEvent.reportYN;

    }
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
