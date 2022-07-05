﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Protocol.admin
{
    public class MAResponse : IOpData
    {
        public int resultCode { get; set; }
        public String resultMessage { get; set; }

        public int Deserialize(byte[] packet)
        {

            int nResult = 0;
            try
            {
                String jsonString = Utility.ByteToString(packet);
                var data = JsonConvert.DeserializeObject<MAResponse>(jsonString);

                SetProperty(data);


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
                Console.WriteLine($"VDSAuthResponse:{jsonString}");
                result = Utility.StringToByte(jsonString);
            }
            catch (Exception ex)
            {
                result = null;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return result;
        }

        public void SetProperty(MAResponse data)
        {
            resultCode = data.resultCode;
            resultMessage = data.resultMessage;
        }
    }
}
