using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class IndivTrafficData : IExOPData
    {
        public byte lane;
        public byte passTime;
        public byte speed;
        public UInt16 occupyTime;
        public byte category;


        public IndivTrafficData()
        {
            lane = 0;
            passTime = 0;
            speed = 0;
            occupyTime = 0;
            category = 0;

        }

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] data;
            lane = packet[idx++];
            passTime = packet[idx++];
            speed = packet[idx++];
            data = new byte[2];
            Array.Copy(packet, idx, data,0, data.Length);
            occupyTime = Utility.toLittleEndianInt16(data);
            idx += data.Length;
            category = packet[idx++];
            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            byte[] data;
            try
            {
                int idx = 0;
                result = new byte[6];
                result[idx++] = lane;
                result[idx++] = passTime;
                result[idx++] = speed;
                data = Utility.toBigEndianInt16(occupyTime);
                Array.Copy(data, 0, result, idx, data.Length);
                idx += data.Length;

                result[idx++] = category;

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
