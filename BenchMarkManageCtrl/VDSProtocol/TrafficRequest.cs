using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class TrafficRequest : OPData, IOpData
    {
        //public byte lane;
        //public byte direction;
        //public byte[] checkTime = new byte[8];
        //public UInt16 velocity ;
        //public UInt16 occupyTime ;
        //public UInt16 carLength ;

        public TrafficData trafficData = new TrafficData();

        //public long id;
        //public byte lane;
        //public byte direction;
        //public byte[] checkTime = new byte[8];
        //public double velocity;   // km/h
        //public UInt16 occupyTime; // msec
        //public UInt16 carLength;  // cm 
        //public byte _Reserved;




        public TrafficRequest()
        {
            _OPCode = DataFrameDefine.OP_TRAFFIC_REQ;
            
        }


        public int Deserialize(byte[] packet)
        {
            int nResult = 0;
            try
            {
                if (packet.Length == 18 && _OPCode == packet[0])
                {
                    //data1[idx++] = 0xB0; // Op 0 
                    //data1[idx++] = 0x01; //1차선 1
                    //data1[idx++] = 0x01; //상행  2
                    //data1[idx++] = 0x01; //년    3 
                    //data1[idx++] = 0x01; //년
                    //data1[idx++] = 0x04; //월
                    //data1[idx++] = 0x15; //일
                    //data1[idx++] = 0x0A; //시
                    //data1[idx++] = 0x0B; //분
                    //data1[idx++] = 0x0C; //초
                    //data1[idx++] = 0x01; //ms       

                    //data1[idx++] = 0x01; //속도  11
                    //data1[idx++] = 0x02; //속도 


                    //data1[idx++] = 0x03; //점유시간 13
                    //data1[idx++] = 0x04; //점유시간

                    //data1[idx++] = 0x05; //차량길이 15
                    //data1[idx++] = 0x06; //차량길이

                    //data1[idx++] = 0x00; //처리상태 17
                    byte[] traffic = new byte[17];
                    Array.Copy(packet, 1, traffic, 0, 17);
                    trafficData.Deserialize(traffic);
                    //lane = packet[1];
                    //direction = packet[2];
                    //Array.Copy(packet, 3, checkTime, 0, 8);

                    //byte[] value = new byte[2];
                    //Array.Copy(packet, 11, value, 0, 2);
                    //velocity = Utility.toLittleEndianInt16(value);

                   

                    //Array.Copy(packet, 13, value, 0, 2);
                    //occupyTime = Utility.toLittleEndianInt16(value);

                    //Array.Copy(packet, 15, value, 0, 2);
                    //carLength = Utility.toLittleEndianInt16(value);

                    //_Reserved = packet[17];
                    nResult = 1;

                }
            }
            catch(Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[18];
            try
            {
                result[0] = _OPCode;
                byte [] traffic = trafficData.Serialize();
                Array.Copy(traffic,0, result, 1, traffic.Length);

                //result[1] = lane;
                //result[2] = direction;
                //Array.Copy(checkTime, 0, result, 3, 8);

                //byte[] value = new byte[2];

                //value = Utility.toBigEndianInt16(velocity);
                //Array.Copy(value, 0, result, 11, 2);


                //value = Utility.toBigEndianInt16(occupyTime);
                //Array.Copy(value, 0, result, 13, 2);


                //value = Utility.toBigEndianInt16(carLength);
                //Array.Copy(value, 0, result, 15, 2);

                //result[17] = _Reserved;
            }
            catch(Exception ex)
            {
                result = null;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return result;
        }
    }

}


