using KorExManageCtrl.VDSProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol_v2._0
{
    public class LaneInfoEx : IExOPData
    {
        public byte largeTrafficCount;
        public byte middleTrafficCount;
        public byte smallTrafficCount;
        public byte speed;
        public UInt16 occupyRatio;
        public byte carLength;


        public double totalSpeed;
        public int totalOccupyTime;
        public int totalLength;

        public int lane;

        public LaneInfoEx()
        {
            largeTrafficCount = 0;
            middleTrafficCount = 0;
            smallTrafficCount = 0;
            speed = 0;
            occupyRatio = 0;
            carLength = 0;
            totalSpeed = 0;
            totalOccupyTime = 0;
            totalLength = 0;
            lane = 0;
        }

        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            largeTrafficCount = packet[idx++];
            middleTrafficCount = packet[idx++];
            smallTrafficCount = packet[idx++];
            speed = packet[idx++];

            // occupyRatio : 9999 100 으로 나눈 몫: , 나머지: 소수점 2자리 
            occupyRatio = (UInt16)(packet[idx++] * 100 + packet[idx++]);
            
            // 아래 평균값으로 점유율 계산한 경우 big endian 저장
            //byte[] value = new byte[2];
            //Array.Copy(packet, idx, value, 0, 2);
            //idx += 2;
            //occupyRatio = Utility.toLittleEndianInt16(value);

            carLength = packet[idx++];

            return idx;
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[7];
            int idx = 0;
            try
            {
                result[idx++] = largeTrafficCount;
                result[idx++] = middleTrafficCount;
                result[idx++] = smallTrafficCount;
                result[idx++] = speed;

                // occupyRatio : 9999 100 으로 나눈 몫: , 나머지: 소수점 2자리 
                result[idx++] = (byte)(occupyRatio/100);
                result[idx++] = (byte)(occupyRatio % 100);

                // 아래 평균값으로 점유율 계산한 경우 big endian 저장
                //byte[] value = Utility.toBigEndianInt16(occupyRatio);
                //Array.Copy(value, 0, result, idx, value.Length);
                //idx += value.Length;

                result[idx++] = carLength;
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
