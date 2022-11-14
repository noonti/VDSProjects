using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace MClavisRadarManageCtrl.Protocol
{
    public class MClavisDataFrame
    {
        public List<MCLAVIS_MESSAGE> messageList = new List<MCLAVIS_MESSAGE>();
        int seq_start = -1;
        int seq_end = -1;
        public MClavisDataFrame()
        {
        }
        public int Deserialize(byte[] packet, int startIdx)
        {
            int i;
            i = startIdx;
            while(i < packet.Length)
            {
                seq_start = FindSequence(packet, i, MClavisDefine.START_SEQUENCE);
                seq_end = FindSequence(packet, seq_start, MClavisDefine.END_SEQUENCE);
                if (seq_start >= 0 && seq_end  >= 0)
                {
                    i += seq_end + MClavisDefine.END_SEQUENCE.Length;
                    int checkSum = packet[seq_end - 1]; // checkSum 
                    // 시작+4 , 종료-1 까지 데이터 영역 
                    int dataSize = seq_end - (seq_start + MClavisDefine.START_SEQUENCE.Length) - 1;
                    byte[] data = new byte[dataSize];
                    Array.Copy(packet, seq_start + 4, data,0, dataSize);
                    AddMClavisMessage(data);
                }
            }
            return i;
        }


        public int AddMClavisMessage(byte[] data)
        {
            int result = 0;
            int i = 0;
            MCLAVIS_MESSAGE message;
            if (data!=null && data.Length> 0)
            {
                while(i<data.Length)
                {
                    message = new MCLAVIS_MESSAGE();
                    message.DETECT_TIME = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
                    message.msgId = new byte[2];
                    Array.Copy(data, i, message.msgId, 0, 2);
                    i += 2;
                    message.DataSize = data[i++];
                    byte[] number = new byte[8];
                    Array.Copy(data, i, number, 0, 8);
                    message.data = Utility.toLittleEndianInt64(number);
                    SetMClavisMessageInfo(ref message);
                    messageList.Add(message);
                    i += 8;
                }

            }
            return result;
        }

        public int FindSequence(byte[] packet, int startIdx, byte[] sequence )
        {
            int result = -1;

            bool found = false;
            try
            {
                if (packet.Length < sequence.Length)
                    return result;

                for (int i = startIdx; i < packet.Length - sequence.Length + 1; i++)
                {
                    found = false;
                    for (int j = 0; j < sequence.Length; j++)
                    {
                        if (packet[i + j] == sequence[j])
                            found = true;
                        else
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        result = i;
                        break;
                    }
                }
            }catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                result = -1;
            }
            
            return result;
        }

        public int SetMClavisMessageInfo(ref MCLAVIS_MESSAGE message)
        {

            // Command Messaee : 0x03F2 : Command
            // Status  Message : ID0 0x0500 / ID1 0x0580 / ID2 0x0600 / ID3 0x0680
            // Object Header   : ID0 0x0501 / ID1 0x0581 / ID2 0x0601 / ID3 0x0681
            // Object Data     :  Object #0: ID0 0x0510 / ID1 0x0590 / ID2 0x0610 / ID3 0x0690
            //                    Object #1: ID0 0x0511 / ID1 0x0591 / ID2 0x0611 / ID3 0x0691
            //                    Object #2: ID0 0x0512 / ID1 0x0592 / ID2 0x0612 / ID3 0x0692
            //                    ...
            //                    Object #63:ID0 0x054F / ID1 0x05CF / ID2 0x064F / ID3 0x06CF

            if (message.msgId[0] == 0x03 && message.msgId[1] == 0xF2)
                message.msgType = MCLAVIS_MESSAGE_TYPE.SENSOR_COMMAND;    // Command 
            else if ((message.msgId[0] == 0x05 || message.msgId[0] == 0x06) && (message.msgId[1] == 0x00 || message.msgId[1] == 0x80))
                message.msgType = MCLAVIS_MESSAGE_TYPE.SENSOR_CONTROL;    // STATUS
            else if ((message.msgId[0] == 0x05 || message.msgId[0] == 0x06) && (message.msgId[1] == 0x01 || message.msgId[1] == 0x81)) // Header 
                message.msgType = MCLAVIS_MESSAGE_TYPE.OBJECT_HEADER;    // 
            else if ((message.msgId[0] == 0x05 || message.msgId[0] == 0x06)
                    && (message.msgId[1] >= 0x10 || message.msgId[1] >= 0x90))
                message.msgType = MCLAVIS_MESSAGE_TYPE.OBJECT_DATA;
            else
                message.msgType = MCLAVIS_MESSAGE_TYPE.NONE;

            switch(message.msgType)
            {
                case MCLAVIS_MESSAGE_TYPE.SENSOR_COMMAND:
                    break;
                case MCLAVIS_MESSAGE_TYPE.SENSOR_CONTROL:
                    SetSensorStatus(ref message);
                    break;
                case MCLAVIS_MESSAGE_TYPE.OBJECT_HEADER:
                    SetObjectHeader(ref message);
                    break;
                case MCLAVIS_MESSAGE_TYPE.OBJECT_DATA:
                    SetObjectData(ref message);
                    break;
            }

            

            return 1;
        }

        private int SetObjectHeader(ref MCLAVIS_MESSAGE message)
        {


            return 1;
        }

        private int SetSensorStatus(ref MCLAVIS_MESSAGE message)
        {
            

            return 1;
        }

        private int SetObjectData(ref MCLAVIS_MESSAGE message)
        {
            message.object_id = (byte)(message.data >> (4 + 4 + 11 + 11 + 14 + 14) & 0x3f);
            message.State = (byte)(message.data >> (4 + 11 + 11 + 14 + 14) & 0xf); // State 
            message.Lane_Dir = (byte)(message.data >> (3 + 11 + 11 + 14 + 14) & 0x1); // 0: 다가오는 방향 1:멀어지는 방향 
            message.Lane = (byte)(message.data >> (11 + 11 + 14 + 14) & 0x7); // Lane 
            //double Velocity_Y = ((double)(message.data >> (11 + 14 + 14) & 0x7ff) - 1024) * 0.1; // [m/s]
            message.Velocity_Y = ((double)(message.data >> (11 + 14 + 14) & 0x7ff) - 1024) * 0.1 * 3.6; // [km/h]
            // double Velocity_X = ((double)(message.data >> (14 + 14) & 0x07ff) - 1024) * 0.1; // [m/s]
            message.Velocity_X = ((double)(message.data >> (14 + 14) & 0x7ff) - 1024) * 0.1 * 3.6; // [km/h]
            message.Range_Y = ((double)(message.data >> (14) & 0x3fff) - 8192) * 0.064; //[m]
            message.Range_X = ((double)(message.data >> (0) & 0x3fff) - 8192) * 0.064; //[m]

            return 1;
        }
        public String GetMClavisMessageInfo(MCLAVIS_MESSAGE message)
        {
            String result = String.Empty;
            String msgId = String.Format("0x{0:X2}{1:X2}", message.msgId[0], message.msgId[1]);
            if(message.msgType ==  MCLAVIS_MESSAGE_TYPE.OBJECT_DATA)
                result = String.Format($"Message Type={message.msgType} ,message identifier={msgId} , object_id={message.object_id}, State={message.State}, Lane_Dir={message.Lane_Dir}, Lane={message.Lane}. Velocity_Y={message.Velocity_Y}, Velocity_X={message.Velocity_X}, Range_Y={message.Range_Y}, Range_X={message.Range_X}");
            else 
                result = String.Format($"Message Type={message.msgType} ,message identifier={msgId}");
            return result;
        }

    }
}
