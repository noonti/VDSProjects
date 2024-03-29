﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ReverseRunResponse : ExResponse, IExOPData
    {
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            idx = base.Deserialize(packet);
            return idx;

        }

        public byte[] Serialize()
        {
            byte[] result;
            int idx = 0;
            try
            {
                result = base.Serialize();

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
