using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class TrafficDataResponse : ExResponse, IExOPData
    {
        public byte frameNo;

        public DetectorInfo detector = new DetectorInfo();


        new public int Deserialize(byte[] packet)
        {
            int idx = 0;
            byte[] data;
            idx = base.Deserialize(packet);

            frameNo = packet[idx++];
            int cnt = packet.Length - idx;
            data = new byte[cnt];
            Array.Copy(packet, idx, data, 0, cnt);

            idx += detector.Deserialize(data);
            return idx;

        }


        new public byte[] Serialize()
        {
            byte[] result = null;//= new byte[9]; ;
            try
            {
                int idx = 0;
                byte[] data = base.Serialize();
                byte[] detectData = detector.Serialize();

                if(data.Length > 0 && detectData.Length >0)
                {
                    result = new byte[data.Length + detectData.Length+1];
                    Array.Copy(data, 0, result, idx, data.Length);
                    idx += data.Length;
                    result[idx++] = frameNo;
                    Array.Copy(detectData, 0, result, idx, detectData.Length);
                    idx += detectData.Length;

                }
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
