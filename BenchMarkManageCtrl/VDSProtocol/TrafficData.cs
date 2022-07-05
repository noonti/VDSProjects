using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class TrafficData
    {
        //public String targetID;
        public String id;
        public byte lane;
        public byte direction;
        public byte[] checkTime = new byte[8];
        public double velocity;   // km/h
        public UInt16 occupyTime; // msec
        public UInt16 carLength;  // cm 
        public byte _Reserved;

        public TrafficData()
        {
            //targetID = String.Empty;
            velocity = 0;
            occupyTime = 0;
            carLength = 0;
            _Reserved = 0x00;
        }


        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                if (packet.Length == 17 ) //&& _OPCode == packet[0])
                {
                    lane = packet[0];
                    direction = packet[1];
                    Array.Copy(packet, 2, checkTime, 0, 8);

                    byte[] value = new byte[2];
                    Array.Copy(packet, 10, value, 0, 2);

                    // byte --> speed 데이터로 변환..
                    velocity = Utility.MergeVelocity(value); //Utility.toLittleEndianInt16(value);

                    Array.Copy(packet, 12, value, 0, 2);
                    occupyTime = Utility.toLittleEndianInt16(value);

                    Array.Copy(packet, 14, value, 0, 2);
                    carLength = Utility.toLittleEndianInt16(value);
                    _Reserved = packet[16];
                    nResult = 1;
                }
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
            byte[] result = new byte[17];
            try
            {
                result[0] = lane;
                result[1] = direction;
                Array.Copy(checkTime, 0, result, 2, 8);

                byte[] value = new byte[2];



                value = Utility.SplitVelocity(velocity);
                Array.Copy(value, 0, result, 10, 2);


                value = Utility.toBigEndianInt16(occupyTime);
                Array.Copy(value, 0, result, 12, 2);


                value = Utility.toBigEndianInt16(carLength);
                Array.Copy(value, 0, result, 14, 2);
                result[16] = _Reserved;
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
