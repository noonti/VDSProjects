﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class CSNCheckDataRequest : IExOPData //:ExRequest,
    {
         
        public int Deserialize(byte[] packet)
        {
            int idx = 0;
            //idx = base.Deserialize(packet);
            return idx;
        }


        public byte[] Serialize()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return null;
        }
    }
}
