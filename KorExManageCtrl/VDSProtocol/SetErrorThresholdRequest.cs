﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class SetErrorThresholdRequest : ExRequest, IExOPData
    {
        public byte threshold;
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            idx = base.Deserialize(packet);
            threshold = packet[idx++];
            return idx;

        }


        public byte[] Serialize()
        {
            byte[] result;
            byte[] data;
            int idx = 0;
            try
            {
                data = base.Serialize();
                result = new byte[data.Length + 1];
                Array.Copy(data, 0, result, 0, data.Length);
                idx += data.Length;
                result[idx++] = threshold;

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
