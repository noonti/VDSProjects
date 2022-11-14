using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MClavisRadarManageCtrl.Protocol
{

    public struct MCLAVIS_MESSAGE
    {
        public MCLAVIS_MESSAGE_TYPE msgType;
        public byte[] msgId ;
        public byte DataSize;
        public UInt64 data;
        public byte object_id ;
        public byte State ; // State 
        public byte Lane_Dir; // 0: 다가오는 방향 1:멀어지는 방향 
        public byte Lane ; // Lane 
        public double Velocity_Y ; // [km/h]
        public double Velocity_X ; // [km/h]
        public double Range_Y ; //[m]
        public double Range_X ; //[m]

        public String DETECT_TIME;

    }


    public enum MCLAVIS_MESSAGE_TYPE
    {
        NONE = 0,
        SENSOR_COMMAND = 1, // Command 
        SENSOR_CONTROL = 2, // STATUS
        OBJECT_HEADER = 3, // HEADER
        OBJECT_DATA = 4    // DATA
    }

    public static class MClavisDefine
    {
        public static byte[] STX = {0x03, 0x01,0x00 };
        public static byte[] START_SEQUENCE = { 0xCA, 0xCB, 0xCC, 0xCD };
        public static byte[] END_SEQUENCE = { 0xEA, 0xEB, 0xEC, 0xED };

        public static int MESSAGE_SIZE = 11;
    }

    public enum MCLAVIS_INVERSE_PHASE
    {
        INVERSE_PROGRESS = 1, // 역주행 진행중 
        INVERSE_COMPLETE = 2, // 역주행 완료
        INVSERSE_EXPIRE = 3 // 역주행 만료(역주행 아님)
        
    }
}
