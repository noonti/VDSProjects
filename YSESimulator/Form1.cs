using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;

namespace YSESimulator
{
    enum FRAME_STATE
    {
        NONE = 0,
        START_DLE,
        START_STX,
        ADDRESS_1,
        ADDRESS_2,
        ADDRESS_3,
        ADDRESS_4,
        OPCODE,
        DATA,
        END_DLE,
        END_ETX,
        CRC_1,
        CRC_2,
        COMPLETE
    };


    public delegate void DisplayDataDelegate(byte[] address, byte opcode,byte[] status, byte[] data);

    public partial class Form1 : Form
    {

        public UInt16[] crc16tab = new UInt16[] {
                                            0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
                                            0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
                                            0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
                                            0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
                                            0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
                                            0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
                                            0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
                                            0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
                                            0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
                                            0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
                                            0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
                                            0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
                                            0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
                                            0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
                                            0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
                                            0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
                                            0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
                                            0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
                                            0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
                                            0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
                                            0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
                                            0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
                                            0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
                                            0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
                                            0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
                                            0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
                                            0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
                                            0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
                                            0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
                                            0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
                                            0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
                                            0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
                                                                                };



        VDSClient _client;
        String ipAddress;
        int port;

        byte[] address = new byte[4];


        public const byte FRAME_DLE = 0x10;
        public const byte FRAME_STX = 0x02;
        public const byte FRAME_ETX = 0x03;


        public const byte OPCODE_SYNC_COMMAND = 0x01;
        public const byte OPCODE_DATA_COMMAND = 0x04;

        public const byte OPCODE_RESET_COMMAND = 0x0C;

        public const byte OPCODE_PARAM_DOWNLOAD_COMMAND = 0x0E;
        public const byte OPCODE_PARAM_UPLOAD_COMMAND = 0x0F;
        public const byte OPCODE_ONLINE_STATUS_COMMAND = 0x11;
        public const byte OPCODE_CHECK_MEMORY_COMMAND = 0x12;
        public const byte OPCODE_ECHO_MESSAGE_COMMAND = 0x13;
        public const byte OPCODE_CHECK_SEQUENCE_COMMAND = 0x14;
        public const byte OPCODE_CHECK_VERSION_COMMAND = 0x15;
        public const byte OPCODE_CHANGE_RTC_COMMAND = 0x18;

        public const byte OPCODE_CHECK_TEMPER_VOLT_COMMAND = 0x1E;

        public const byte OPCODE_ACK = 0x70;
        public const byte OPCODE_NAK = 0x71;

        System.Windows.Forms.Timer _pollingTimer = null;
        bool bPolling = false;
        DisplayDataDelegate _displayDataDelegate = null;
        ushort _groupNo = 230;
        ushort _ctrlNo = 121;

        UInt16 frame = 0;
        public Form1()
        {
            byte[] value = new byte[2];
            InitializeComponent();
            _client = new VDSClient();
            value = Utility.toBigEndianInt16(_groupNo);
            Array.Copy(value, 0, address, 0, 2);
            value = Utility.toBigEndianInt16(_ctrlNo);
            Array.Copy(value, 0, address, 2, 2);

            //address[0] = 0x01;
            //address[1] = 0x02;
            //address[2] = 0x03;
            //address[3] = 0x04;

            _displayDataDelegate = new DisplayDataDelegate(DisplayReceiveData);

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_client._status == SOCKET_STATUS.DISCONNECTED)
            {
                ipAddress = txtIPAddress.Text;
                port = int.Parse(txtPortNo.Text);

                int nResult = _client.SetAddress(ipAddress, port, CLIENT_TYPE.VDS_CLIENT, YSEConnectCallback, YSEReadCallback, YSESendCallback);
                //nResult = _client.SetAddress(ipAddress, port);
                if (nResult > 0)
                {
                    nResult = _client.StartConnect();
                }
                btnStart.Text = "Disconnect";
            }
            else
            {
                _client.Stop();
                btnStart.Text = "Connect";
            }


        }

        private int YSEConnectCallback(SessionContext session, SOCKET_STATUS status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                if (status == SOCKET_STATUS.CONNECTED)
                {
                    session._socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(YSEReadCallback), session);

                    strLog = String.Format($"레이더 ({ipAddress}:{port})  접속 성공");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                }
                else if (status == SOCKET_STATUS.DISCONNECTED)
                {
                    strLog = String.Format($"레이더 ({ipAddress}:{port})  접속 실패");
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);
                    Console.WriteLine(strLog);

                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                _client._status = SOCKET_STATUS.DISCONNECTED;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private void YSEReadCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            try
            {
                // Retrieve the socket from the state object.  
                SessionContext session = (SessionContext)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesReceive = session._socket.EndReceive(ar);
                Console.WriteLine("receive {0} bytes to client1.", bytesReceive);

                ProcessPacket(session.buffer, bytesReceive);


                session._socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                new AsyncCallback(YSEReadCallback), session);



            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public int Send(byte[] data, int size)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                // Begin sending the data to the remote device.  
                if (_client._status == SOCKET_STATUS.CONNECTED)
                {
                    _client._sessionContext._socket.BeginSend(data, 0, size, 0,
                            new AsyncCallback(YSESendCallback), _client._sessionContext._socket);
                    //_client.SendAndReceive()
                    nResult = 1;
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private void YSESendCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            try
            {
                // Retrieve the socket from the state object.  
                Socket socket = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = socket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //int len = 0;
            //int len = 0;
            //int len = 0;
            ////byte[] data = MakeYSEFrame(Utility.StringToByte("avogadro12345678"), ref len);
            //byte[] data = new byte[8];
            //int index = 0;
            //data[index++] = FRAME_DLE;
            //data[index++] = FRAME_DLE;
            //data[index++] = FRAME_DLE;
            //data[index++] = 0x03;
            //data[index++] = 0x04;
            //data[index++] = 0x05;
            //data[index++] = 0x06;
            //data[index++] = FRAME_DLE;
            //byte[] data2 = MakeYSEFrame(data, ref len);

            //Send(data2, len);
        }

        private byte[] MakeDLEStuffingPacket(byte[] data, ref int len)
        {
            byte[] result = new byte[1024];
            int index = 0;
            len = 0;

            result[index++] = FRAME_DLE;
            result[index++] = FRAME_STX;

            // CRC 설정

            UInt16 crcNum = 0;
            crcNum = CaculateCRC16(data, (UInt16)data.Length, crcNum);
            byte[] crc = Utility.toBigEndianInt16(crcNum);

            for (int i = 0; i < data.Length; i++)
            {
                result[index++] = data[i];
                if (data[i] == FRAME_DLE)
                {
                    result[index++] = FRAME_DLE;
                }

            }
            result[index++] = FRAME_DLE;
            result[index++] = FRAME_ETX;

            result[index++] = crc[0]; // CRC
            result[index++] = crc[1]; // CRC
            len = index;
            return result;
        }

        private byte[] MakeYSEFrame(byte[] address, byte opcode, byte[] data, ref int len)
        {
            int index = 0;
            byte[] result;
            int dataLen = 0;

            if (data != null)
                dataLen = data.Length;

            byte[] buf = new byte[4 + 1 + dataLen];

            Array.Copy(address, 0, buf, index, 4);
            index += 4;

            buf[index++] = opcode;

            if (data != null)
            {
                Array.Copy(data, 0, buf, index, data.Length);
                index += data.Length;
            }

            DisplaySendFrame(lbxSend, address, opcode, data);

            result = MakeDLEStuffingPacket(buf, ref len);



            return result;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendSyncDataCommand();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SendVDSDataCommand();
        }

        public UInt16 CaculateCRC16(byte[] data, UInt16 size, UInt16 crc_sum)
        {
            UInt16 i, j;
            for (j = 0; j < size; j++)
            {
                i = (UInt16)((crc_sum ^ data[j]) & 0xFF);
                crc_sum = (UInt16)((crc_sum >> 8) ^ crc16tab[i]);
            }
            return crc_sum;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int len = 0;
            byte[] frameNo;
            UInt16 frame = 0x0A0B;
            frameNo = Utility.toBigEndianInt16(frame);
            byte[] data1 = MakeYSEFrame(address, OPCODE_SYNC_COMMAND, frameNo, ref len);

            byte[] data = new byte[len - 5];

            Array.Copy(data1, 0, data, 0, len - 5);
            Send(data, len - 5);
            Thread.Sleep(1000 * 5);

            Array.Copy(data1, len - 5, data, 0, 5);
            Send(data, 5);

            byte[] data2 = MakeYSEFrame(address, OPCODE_DATA_COMMAND, null, ref len);
            data = new byte[len];
            Array.Copy(data2, 0, data, 0, len);
            Send(data, len);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            int len = 0;
            byte[] data = MakeYSEFrame(address, OPCODE_RESET_COMMAND, null, ref len);
            Send(data, len);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] detector = new byte[3];

            detector[0] = 1;  // index 
            detector[1] = 0xFF;
            detector[2] = 0xFF;

            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, detector, ref len);
            Send(data, len);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[2];

            param[0] = 3;  // index 
            param[1] = 18;

            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[1 + 12];

            param[0] = 5;  // index 

            for (int i = 0; i < 12; i++)
            {
                param[i + 1] = (byte)((i + 1) * 10);
            }
            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[1 + 3];
            param[0] = 6;  // index 
            for (int i = 0; i < 3; i++)
            {
                param[i + 1] = (byte)((i + 1) * 10 + 1);
            }
            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[1 + 1];
            param[0] = 7;  // index 
            param[1] = 1;

            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[1 + 1];
            param[0] = 8;  // index 
            param[1] = 1;

            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[1 + 1];
            param[0] = 9;  // index 
            param[1] = 1;

            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[1 + 1];
            param[0] = 10;  // index 
            param[1] = 1;

            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[1 + 1];
            param[0] = 17;  // index 
            param[1] = 20;

            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[1 + 1];
            param[0] = 20;  // index 
            param[1] = 2;

            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_DOWNLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            int paramIndex = int.Parse((sender as Button).Tag.ToString());

            int len = 0;
            byte[] param = new byte[1];
            param[0] = (byte)paramIndex;  // index 

            byte[] data = MakeYSEFrame(address, OPCODE_PARAM_UPLOAD_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            int len = 0;
            byte[] data = MakeYSEFrame(address, OPCODE_ONLINE_STATUS_COMMAND, null, ref len);
            Send(data, len);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            int len = 0;
            byte[] data = MakeYSEFrame(address, OPCODE_CHECK_MEMORY_COMMAND, null, ref len);
            Send(data, len);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param;
            param = Utility.StringToByte("avogadro");
            byte[] data = MakeYSEFrame(address, OPCODE_ECHO_MESSAGE_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            int len = 0;

            byte[] param = new byte[2];
            param[0] = 100;
            param[1] = 10;
            byte[] data = MakeYSEFrame(address, OPCODE_CHECK_SEQUENCE_COMMAND, param, ref len);
            Send(data, len);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            int len = 0;
            byte[] data = MakeYSEFrame(address, OPCODE_CHECK_VERSION_COMMAND, null, ref len);
            Send(data, len);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            int len = 0;
            byte[] data = MakeYSEFrame(address, OPCODE_CHECK_TEMPER_VOLT_COMMAND, null, ref len);
            Send(data, len);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            int len = 0;

            String result = String.Empty;

            var date = dtDate.Value;//.ToString("yyyy-MM-dd");
            

            var time = dtTime.Value;//.ToString("HH:mm:ss.00");
            result = String.Format($"{date} {time}");


            Console.WriteLine("rtc time={0}", result);

            byte[] param = new byte[7];


            param[0] = Utility.ByteToBCD((byte)(date.Year / 100)); // (date.Year / 100); //2021 / 100  = 20 --> 2 0
            param[1] = Utility.ByteToBCD((byte)(date.Year % 100));  //(date.Year % 100); ;
            param[2] = Utility.ByteToBCD(byte.Parse(date.ToString("MM")));
            param[3] = Utility.ByteToBCD(byte.Parse(date.ToString("dd")));  //byte.Parse(time.ToString("dd")); 
            param[4] = Utility.ByteToBCD(byte.Parse(time.ToString("HH")));  //byte.Parse(time.ToString("HH")); 
            param[5] = Utility.ByteToBCD(byte.Parse(time.ToString("mm")));  //byte.Parse(time.ToString("mm")); 
            param[6] = Utility.ByteToBCD(byte.Parse(time.ToString("ss")));  // byte.Parse(time.ToString("ss"));

            //0x20 0x21 0x02 0x03 0x12 0x06 0x38  --> 테스트
            //param[0] = 0x20;
            //param[1] = 0x21;
            //param[2] = 0x02;
            //param[3] = 0x03;
            //param[4] = 0x12;
            //param[5] = 0x06;
            //param[6] = 0x38;



            byte[] data = MakeYSEFrame(address, OPCODE_CHANGE_RTC_COMMAND, param, ref len);
            Send(data, len);

        }

        public void StartPolling()
        {
            int polling = int.Parse(txtPollingTime.Text);
            if (bPolling)
            {
                _pollingTimer.Stop();
                bPolling = false;
                btnPolling.Text = "폴링시작";
                _pollingTimer = null;
            }
            else
            {
                _pollingTimer = new System.Windows.Forms.Timer();
                _pollingTimer.Interval = polling * 1000; // 1초마다 체크
                _pollingTimer.Tick += new EventHandler(Polling_Tick);
                _pollingTimer.Start();
                bPolling = true;
                btnPolling.Text = "폴링종료";
            }



        }

        private void Polling_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("polling....");
            SendSyncDataCommand();
            SendVDSDataCommand();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            StartPolling();
        }

        public void SendSyncDataCommand()
        {

            frame = GetFrameNo();
            // sync 명령 전송
            int len = 0;
            byte[] frameNo;
            frameNo = Utility.toBigEndianInt16(frame);
            byte[] data = MakeYSEFrame(address, OPCODE_SYNC_COMMAND, frameNo, ref len);
            Send(data, len);

            // data request 전송 
        }


        public void SendVDSDataCommand()
        {
            int len = 0;
            byte[] data = MakeYSEFrame(address, OPCODE_DATA_COMMAND, null, ref len);
            Send(data, len);
        }

        public void DisplaySendFrame(ListBox list, byte[] address, byte opcode, byte[] data)
        {
            int i;
            list.Items.Clear();
            String value;
            value = String.Format($"시간: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")} ");
            list.Items.Add(value);
            //temp = Utility.ByteToString(address);
            value = "address: ";
            for (i = 0; i < address.Length; i++)
            {
                value += String.Format("0x{0:X2} ", address[i]);

            }
            list.Items.Add(value);
            value = String.Format("opcode : 0x{0:X2}({1})", opcode, GetOpcodeName(opcode));
            list.Items.Add(value);

            DisplayDataFrame(list, opcode, data);

        }


        public void DisplayDataFrame(ListBox list, byte opcode, byte[] data)
        {
            int frameType = 0; // 0: send 1: receive
            if (list == lbxSend)
                frameType = 0;
            else
                frameType = 1;

            Console.WriteLine("frameType={0}", frameType);

            switch (opcode)
            {
                case OPCODE_SYNC_COMMAND:
                    DisplaySyncData(list, data);
                    //result = "제어동기화";
                    break;
                case OPCODE_DATA_COMMAND:
                    //result = "검지기 데이터 요구";
                    //DisplayData
                    if (frameType == 1)
                        DisplayVDSDataCommandResponse(data);
                    break;
                case OPCODE_RESET_COMMAND:
                    //result = "제어기 리셋";
                    //if (frameType == 1)
                    //    DisplayACKResponse(data);
                    break;
                case OPCODE_PARAM_DOWNLOAD_COMMAND:
                    //result = "파라메터 다운로드";
                    break;
                case OPCODE_PARAM_UPLOAD_COMMAND:
                    //result = "파라메터 업로드";
                    if (frameType == 1)
                        DisplayParamUploadCommandResponse(data);
                    break;
                case OPCODE_ONLINE_STATUS_COMMAND:
                    //result = "제어기 상태";
                    if (frameType == 1)
                        DisplayOnlineStatusCommandResponse(data);
                    break;
                case OPCODE_CHECK_MEMORY_COMMAND:
                    // result = "메모리 검사";
                    break;
                case OPCODE_ECHO_MESSAGE_COMMAND:
                    //result = "에코메시지";
                    if (frameType == 1)
                        DisplayEchoMessageCommandResposne(data);
                    break;
                case OPCODE_CHECK_SEQUENCE_COMMAND:
                    //result = "제어기 수치 처리 체크";
                    if (frameType == 1)
                        DisplaySEQCheckCommandResposne(data);
                    break;
                case OPCODE_CHECK_VERSION_COMMAND:
                    //result = "제어기 정보";
                    if (frameType == 1)
                        DisplayVersionCheckCommandResposne(data);
                    break;

                case OPCODE_CHANGE_RTC_COMMAND:
                    //result = "RTC변경";
                    if (frameType == 1)
                        DisplayRTCChangeCommandResposne(data);

                    break;
                case OPCODE_CHECK_TEMPER_VOLT_COMMAND:
                    //result = "온도/전압 체크";
                    if (frameType == 1)
                        DisplayTemperVoldCommandResposne(data);
                    break;
                case OPCODE_ACK:
                    list.Items.Add("ACK");
                    break;
                case OPCODE_NAK:
                    list.Items.Add("NAK");
                    break;

            }
        }

        public void DisplaySyncData(ListBox list, byte[] data)
        {
            // frame no
            String value;
            ushort frameNo = Utility.toLittleEndianInt16(data);
            value = String.Format("Frame No: {0}", frameNo);
            list.Items.Add(frameNo);
        }

        public String GetOpcodeName(byte opcode)
        {
            String result = String.Empty;
            switch (opcode)
            {
                case OPCODE_SYNC_COMMAND:
                    result = "제어동기화";
                    break;
                case OPCODE_DATA_COMMAND:
                    result = "검지기 데이터 요구";
                    break;
                case OPCODE_RESET_COMMAND:
                    result = "제어기 리셋";
                    break;
                case OPCODE_PARAM_DOWNLOAD_COMMAND:
                    result = "파라메터 다운로드";
                    break;
                case OPCODE_PARAM_UPLOAD_COMMAND:
                    result = "파라메터 업로드";
                    break;
                case OPCODE_ONLINE_STATUS_COMMAND:
                    result = "제어기 상태";
                    break;
                case OPCODE_CHECK_MEMORY_COMMAND:
                    result = "메모리 검사";
                    break;
                case OPCODE_ECHO_MESSAGE_COMMAND:
                    result = "에코메시지";
                    break;
                case OPCODE_CHECK_SEQUENCE_COMMAND:
                    result = "제어기 수치 처리 체크";
                    break;
                case OPCODE_CHECK_VERSION_COMMAND:
                    result = "제어기 정보";
                    break;

                case OPCODE_CHANGE_RTC_COMMAND:
                    result = "RTC변경";
                    break;
                case OPCODE_CHECK_TEMPER_VOLT_COMMAND:
                    result = "온도/전압 체크";
                    break;

            }
            return result;

        }

        public UInt16 GetFrameNo()
        {
            UInt16 result = (UInt16)((DateTime.Now.Minute * 60 + DateTime.Now.Second) % 255);
            Console.WriteLine("GetFrameNo return {0}", result);
            return result;
        }

        public void ProcessPacket(byte[] packet, int len)
        {

            int i = 0;
            bool prev_dle = false;
            byte data;
            byte[] frame = new byte[1024];
            int frameSize = 0;
            FRAME_STATE state = FRAME_STATE.NONE;
            while (i < len)
            {
                if (i == 60)
                    Console.WriteLine("aa");
                data = packet[i];
                switch (state)
                {
                    case FRAME_STATE.NONE:
                        if (data == FRAME_DLE)
                            state = FRAME_STATE.START_DLE;
                        else
                        {

                            frameSize = 0;
                        }
                        break;
                    case FRAME_STATE.START_DLE:
                        if (data == FRAME_STX)
                            state = FRAME_STATE.START_STX;
                        else
                        {

                            frameSize = 0;
                        }
                        break;
                    case FRAME_STATE.START_STX:
                        SetFrameData(ref frame, ref frameSize, ref state, data);
                        //set_frame_data(g_prevFrame, data);
                        break;
                    default:
                        if (data == FRAME_DLE)
                        {
                            if (state == FRAME_STATE.END_ETX) //
                                SetFrameData(ref frame, ref frameSize, ref state, data);
                            else if (prev_dle)
                            {
                                SetFrameData(ref frame, ref frameSize, ref state, data);
                                prev_dle = false;
                            }
                            else
                            {
                                prev_dle = true;
                            }
                        }
                        else
                        {
                            if (prev_dle) // DLE 가 있는 경우
                            {
                                if (data == FRAME_ETX)
                                    state = FRAME_STATE.END_ETX;
                                else
                                    frameSize = 0;
                                prev_dle = false;
                            }
                            else
                            {
                                SetFrameData(ref frame, ref frameSize, ref state, data);
                            }

                        }
                        break;
                }


                if (state == FRAME_STATE.COMPLETE)
                {
                    ProcessFrame(frame, frameSize);

                    state = FRAME_STATE.NONE;
                    frameSize = 0;
                    prev_dle = false;
                }
                i++;
            }
        }

        void SetFrameData(ref byte[] frame, ref int frameSize, ref FRAME_STATE state, byte data)
        {
            if (state < FRAME_STATE.START_STX)
                return;

            if (state != FRAME_STATE.DATA)
                state++;

            switch (state)
            {
                case FRAME_STATE.ADDRESS_1:
                    frame[frameSize++] = data;
                    break;
                case FRAME_STATE.ADDRESS_2:
                    frame[frameSize++] = data;
                    break;
                case FRAME_STATE.ADDRESS_3:
                    frame[frameSize++] = data;
                    break;
                case FRAME_STATE.ADDRESS_4:
                    frame[frameSize++] = data;
                    break;
                case FRAME_STATE.OPCODE:
                    frame[frameSize++] = data;
                    break;
                case FRAME_STATE.DATA:
                    frame[frameSize++] = data;
                    break;
                case FRAME_STATE.END_ETX:
                    // check timeout
                    break;
                case FRAME_STATE.CRC_1:
                    //frame->crc[0] = data;
                    state++;
                    break;
                case FRAME_STATE.CRC_2:
                    //frame->crc[1] = data;
                    //frame->state++;
                    state++;
                    break;
            }

        }

        void ProcessFrame(byte[] frame, int frameSize)
        {
            int i = 0;
            byte opcode = frame[4];
            byte[] address = new byte[4];
            byte[] status = new byte[2];

            byte[] data = new byte[frameSize - 7];

            Array.Copy(frame, 0, address, i, 4);
            i += 4;
            opcode = frame[i++];

            Array.Copy(frame, i, status, 0, 2);
            i += 2;

            Array.Copy(frame, i, data, 0, frameSize - 7);
            //DisplayReceiveFrame(lbxReceive, address, opcode, data);
            BeginInvoke(_displayDataDelegate, new object[] { address, opcode, status, data });
        }

        public void DisplayReceiveData(byte[] address, byte opcode, byte[] status, byte[] data)
        {
            DisplayReceiveFrame(lbxReceive, address, opcode, status, data);
        }

        public void DisplayReceiveFrame(ListBox list, byte[] address, byte opcode, byte[] status, byte[] data)
        {
            int i;
            list.Items.Clear();
            String value;
            byte[] temp = new byte[2];
            value = String.Format($"시간: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")} ");
            list.Items.Add(value);

            UInt16 groupNo;
            UInt16 ctrlNo;

            Array.Copy(address, 0, temp, 0, 2);
            groupNo = Utility.toLittleEndianInt16(temp);

            Array.Copy(address, 2, temp, 0, 2);
            ctrlNo = Utility.toLittleEndianInt16(temp);


            value = "address: ";
            for (i = 0; i < address.Length; i++)
            {
                value += String.Format("0x{0:X2} ", address[i]);

            }
            value += String.Format(" GroupNO={0}, CtrlNo={1}", groupNo, ctrlNo);
            list.Items.Add(value);
            value = String.Format("opcode : 0x{0:X2}({1})", opcode, GetOpcodeName(opcode));
            list.Items.Add(value);


            // status 출력
            // byte[] status = new byte[2];
            // Array.Copy(data, 0, status, 0, 2);
            UInt16 vds_status = Utility.toLittleEndianInt16(status);

            value = String.Format("제어기 RESET  : {0}", (vds_status & 0x2000) >> 13);
            list.Items.Add(value);

            value = String.Format("히터작동  : {0}", (vds_status & 0x1000) >> 12);
            list.Items.Add(value);

            value = String.Format("팬작동  : {0}", (vds_status & 0x0800) >> 11);
            list.Items.Add(value);

            value = String.Format("뒷문개방  : {0}", (vds_status & 0x0400) >> 10);
            list.Items.Add(value);


            value = String.Format("앞문개방  : {0}", (vds_status & 0x0200) >> 9);
            list.Items.Add(value);


            value = String.Format("자동SYNC  : {0}", (vds_status & 0x0100) >> 8);
            list.Items.Add(value);


            value = String.Format("전역주소 수신  : {0}", (vds_status & 0x0080) >> 7);
            list.Items.Add(value);


            value = String.Format("기본파라메터  : {0}", (vds_status & 0x0040) >> 6);
            list.Items.Add(value);


            value = String.Format("SHORT POWER FAIL  : {0}", (vds_status & 0x0010) >> 4);
            list.Items.Add(value);


            value = String.Format("LONG POWER FAIL  : {0}", (vds_status & 0x0008) >> 3);
            list.Items.Add(value);

            value = String.Format("UNKNOWN MESSAGE TYPE  : {0}", (vds_status & 0x0004) >> 2);
            list.Items.Add(value);

            value = String.Format("RESPONSE DATA NOT READY  : {0}", (vds_status & 0x0002) >> 1);
            list.Items.Add(value);


            value = String.Format("INVALID REQUEST  : {0}", (vds_status & 0x0001));
            list.Items.Add(value);


            DisplayDataFrame(list, opcode, data);

        }

        void DisplayVDSDataCommandResponse(byte[] data)
        {

            int i = 0;
            String value;
            byte[] temp = new byte[2];
            byte[] errorNo = new byte[4];
            byte[] traffic = new byte[7];

            // frame no 2byte
            Array.Copy(data, i, temp, 0, 2);
            i += 2;

            ushort frameNo = Utility.toLittleEndianInt16(temp);
            value = String.Format("frame no : {0}", frameNo);
            lbxReceive.Items.Add(value);


            // detector info

            // 장애 감지 정보 4 byte 
            Array.Copy(data, i, errorNo, 0, 4);
            i += 4;

            // 교통량 7 byte X 8차선
            int j = 0;
            while (i < data.Length)
            {
                Array.Copy(data, i, traffic, 0, 7);
                i += 7;

                value = String.Format("{0} 차선 --> 대형: {1} 중형: {2} 소형: {3}  평균속도:{4} 점유율: {5}.{6} 차두시간:{7} ", j + 1, traffic[0], traffic[1], traffic[2], traffic[3], traffic[4], traffic[5], traffic[6]);
                lbxReceive.Items.Add(value);
                j++;
            }
        }


        void DisplayParamUploadCommandResponse(byte[] data)
        {
            int i = 0;
            byte index = data[0];
            String value = String.Empty;

            switch (index)
            {
                case 1: // 검지기 지정
                    value = String.Format("검지기 지정: {0} {1}", data[1], data[2]);
                    break;
                case 2: // reserve
                    break;
                case 3: // polling cycle
                    value = String.Format("Polling Cycle: {0}", data[1]);
                    break;

                case 4: // reserved 
                    break;
                case 5: // 차량 속도 구분
                    value = "차량속도구분: ";
                    for (i = 0; i < 12; i++)
                    {
                        value += String.Format("category {0} - {1} km/h ", i, data[i + 1]);
                    }
                    break;
                case 6: // 차량 길이 구분
                    value = "차량길이구분: ";
                    for (i = 0; i < 3; i++)
                    {
                        value += String.Format("category {0} - {1} dm ", i, data[i + 1]);
                    }
                    break;
                case 7: // 속도누적치 
                    value = String.Format("속도누적치 계산:{0}", data[1]);
                    break;

                case 8: // 차량길이별 누적치 
                    value = String.Format("길이누적치 계산:{0}", data[1]);
                    break;
                case 9: // 속도계산가능여부
                    value = String.Format("속도계산가능:{0}", data[1]);
                    break;

                case 10: // 차량길이계산
                    value = String.Format("차량길이계산:{0}", data[1]);
                    break;
                case 17: // Oscillation threshold
                    value = String.Format("Oscillation threshold:{0}", data[1]);
                    break;
                case 20: // AutoSync 대기시간
                    value = String.Format("AutoSync 대기시간:{0}", data[1]);
                    break;




            }
            if (!String.IsNullOrEmpty(value))
                lbxReceive.Items.Add(value);

        }

        void DisplayOnlineStatusCommandResponse(byte[] data)
        {
            if(data.Length== 4)
            {
                uint passTime = Utility.toLittleEndianInt32(data);

                String value = String.Format("Pass Time : {0} 초", passTime);
                lbxReceive.Items.Add(value);
            }
            

        }

        void DisplayEchoMessageCommandResposne(byte[] data)
        {
            String msg = Utility.ByteToString(data);
            lbxReceive.Items.Add(String.Format("Echo Message: {0}",msg));

        }


        void DisplaySEQCheckCommandResposne(byte[] data)
        {
            String value = "SEQ Check";
            lbxReceive.Items.Add(value);
            for (int i =0;i<data.Length;i++)
            {
                value = String.Format("{0}", data[i]);
                lbxReceive.Items.Add(value);
            }
        }

        void DisplayVersionCheckCommandResposne(byte[] data)
        {
            String value = String.Format("Version 번호: {0}",data[0]);
            lbxReceive.Items.Add(value);

            value = String.Format("제조년도: {0}", data[1]);
            lbxReceive.Items.Add(value);

            value = String.Format("제조월: {0}", data[2]);
            lbxReceive.Items.Add(value);

            value = String.Format("제조일: {0}", data[3]);
            lbxReceive.Items.Add(value);

        }

        void DisplayRTCChangeCommandResposne(byte[] data)
        {
            if(data.Length == 7)
            {
                int year = (data[0] >> 4) * 1000 + (data[0] & 0x0F) * 100 + (data[1] >> 4) * 10 + (data[1] & 0x0F) * 1;
                int month = (data[2] >> 4) * 10 + (data[2] & 0x0F) * 1;
                int day = (data[3] >> 4) * 10 + (data[3] & 0x0F) * 1;
                int hour = (data[4] >> 4) * 10 + (data[4] & 0x0F) * 1;
                int min = (data[5] >> 4) * 10 + (data[5] & 0x0F) * 1;
                int sec = (data[6] >> 4) * 10 + (data[6] & 0x0F) * 1;
                String value = String.Format("RTC 시간: {0}-{1}-{2} {3}:{4}:{5}", year,month, day, hour, min, sec);
                lbxReceive.Items.Add(value);
            }

        }

        void DisplayTemperVoldCommandResposne(byte[] data)
        {
            if (data.Length == 3)
            {
                String value;
                value = String.Format("함체온도: {0}", data[0]);
                lbxReceive.Items.Add(value);


                value = String.Format("입력전압: {0}", data[1]);
                lbxReceive.Items.Add(value);


                value = String.Format("출력전압: {0}", data[2]);
                lbxReceive.Items.Add(value);
            }
        }

        void DisplayACKResponse(byte[] data)
        {

            int i = 0;
            String value;
            byte[] temp = new byte[2];
            byte[] errorNo = new byte[4];
            byte[] traffic = new byte[7];

            // frame no 2byte
            Array.Copy(data, i, temp, 0, 2);
            i += 2;

            ushort frameNo = Utility.toLittleEndianInt16(temp);
            value = String.Format("frame no : {0}", frameNo);
            lbxReceive.Items.Add(value);


            // detector info

            // 장애 감지 정보 4 byte 
            Array.Copy(data, i, errorNo, 0, 4);
            i += 4;

            // 교통량 7 byte X 8차선
            int j = 0;
            while (i < data.Length)
            {
                Array.Copy(data, i, traffic, 0, 7);
                i += 7;

                value = String.Format("{0} 차선 --> 대형: {1} 중형: {2} 소형: {3}  평균속도:{4} 점유율: {5}.{6} 차두시간:{7} ", j + 1, traffic[0], traffic[1], traffic[2], traffic[3], traffic[4], traffic[5], traffic[6]);
                lbxReceive.Items.Add(value);
                j++;
            }
        }
    }
}
