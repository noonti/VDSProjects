using SerialComManageCtrl.Protocol;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;

namespace SerialComManageCtrl
{
    public class SerialComManager : IVDSManager
    {
        public SerialCom serialCom = null;
        public byte RTUStatus = 0x0;
        public byte RTUPFR = 0x0; // POWER FAIL RESET

        public FormSerialDataFrameDelegate _serialDataFrameDelegate = null;
        public Control _control = null;

        Timer _statusTimer = null;

        VDSClient _rtuClient;

        IRTUManager rtuManager;
        public int SetSerialPort(String portName, int baudRate = 115200, Parity parity = System.IO.Ports.Parity.None, int dataBits = 8, StopBits stopBits = System.IO.Ports.StopBits.None, Handshake handShake = System.IO.Ports.Handshake.None)
        {
            int nResult = 0;
            if(rtuManager!=null)
            {
                nResult = rtuManager.SetSerialPort(portName, baudRate, parity, dataBits, stopBits, handShake);
                rtuManager.Init(portName, baudRate, parity, dataBits, stopBits, handShake, new SerialDataReceivedEventHandler(SerialReceivedData));
            }

            if (serialCom == null)
                serialCom = new SerialCom();
            serialCom.Init(portName, baudRate, parity, dataBits, stopBits, handShake, new SerialDataReceivedEventHandler(SerialReceivedData));
            return nResult;
        }


        public int SetTCPPort(String address, int port)
        {
            if (rtuManager != null)
                rtuManager.SetTCPPort(address, port);

            //if (_rtuClient == null)
            //    _rtuClient = new VDSClient();
            
            //_rtuClient.SetAddress(address, port, CLIENT_TYPE.VDS_CLIENT, KorExConnectCallback, KorExReadCallback, SendCallback);
          
            return 1;
        }
        public int StartManager()
        {
            //if (rtuManager != null)
            //    rtuManager.StartManager();

            StartRTUStatusTimer();
            //if(_rtuClient==null)
            //{
            if (serialCom != null && serialCom.isOpened)
                serialCom.Close();
            serialCom.Open();
            //}
            //else
            //{
            //    _rtuClient.StartConnect();
            //}

            return 1;
        }

        public int StopManager()
        {
            StopRTUStatusTimer();
            //if (rtuManager != null)
            //    rtuManager.StopManager();



            //if (_rtuClient == null)
            //{
            if (serialCom != null && serialCom.isOpened)
                serialCom.Close();
            //}
            //else
            //{
            //   //
            //}


            return 1;
        }

        private int StartRTUStatusTimer()
        {
            if(_statusTimer == null)
            {
                _statusTimer = new Timer();
            }
            _statusTimer.Interval = 1000; // 1 초마다 상태 체크 
            _statusTimer.Tick += Status_Timer_Tick;
            _statusTimer.Start();
            return 1;
        }

        private int StopRTUStatusTimer()
        {
            if(_statusTimer!=null)
            {
                _statusTimer.Stop();
                _statusTimer = null;
            }
            return 1;
        }

        private void Status_Timer_Tick(object sender, EventArgs e)
        {
            RTUStatustRequest(); 
        }


        private void SerialReceivedData(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[VDSConfig.PACKET_SIZE];
            byte[] packet;
            if (serialCom.isOpened)
            {
                int readCount = serialCom.serialPort.Read(buffer, 0, VDSConfig.PACKET_SIZE);
                packet = new byte[readCount];
                Array.Copy(buffer, 0, packet, 0, readCount);
                ProcessReceivePacket(packet, readCount);
            }
        }


        private int ProcessReceivePacket(byte[] packet, int length)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            SerialDataFrame dataFrame = null;
            int i = 0;

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($" Serial Data= {Utility.PrintHexaString(packet, length)} length={length} "));

            while (i < length)
            {
                if (dataFrame == null)
                {
                    dataFrame = new SerialDataFrame();
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"미완성 패킷 후속 처리 "));
                }

                i = dataFrame.Deserialize(packet, i);
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"시리얼 패킷 데이터 완성 여부: dataFrame.bHeaderCompleted={dataFrame.bHeaderCompleted}, dataFrame.bDataCompleted = {dataFrame.bDataCompleted} "));
                if (dataFrame.bDataCompleted)
                {
                    // processDataFrame....
                    if(dataFrame.isLRCOK)
                    {
                        ProcessDataFrame(dataFrame);
                    }
                    else // LRC error
                    {
                        Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"DataFrame 패킷 LRC 체크 실패 i={i}, packet.Length={packet.Length}"));
                    }
                        
                    dataFrame = null;
                    nResult++;
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"DataFrame 패킷 미완성 i={i}, packet.Length={packet.Length}"));
                }
            }
            // Not all data received. Get more.  
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }
         
        private int ProcessDataFrame(SerialDataFrame dataFrame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            byte opCode = (byte)(dataFrame.OpCode - SerialDataFrameDefine.OPCODE_RESPONSE);
            switch (opCode)
            {
                case SerialDataFrameDefine.OPCODE_RTU_STATUS:
                    nResult = ProcessRTUStatus(dataFrame);
                    break;
                //case SerialDataFrameDefine.OPCODE_CAMERA_RESET:
                //    nResult = ProcessCameraResetResponse(dataFrame);
                //    break;

                case SerialDataFrameDefine.OPCODE_CTRL_POWER:
                    nResult = ProcessACPowerResetResponse(dataFrame);
                    break;


                case SerialDataFrameDefine.OPCODE_FAN_CTRL:
                    nResult = ProcessFanCtrlResponse(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_HEATER_CTRL:
                    nResult = ProcessHeaterCtrlResponse(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_POWER_RESET:
                    nResult = ProcessPowerResetResponse(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_FAN_THRESHOLD:
                    nResult = ProcessSetFanThresholdResponse(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_HEATER_THRESHOLD:
                    nResult = ProcessSetHeaterThresholdResponse(dataFrame);
                    break;
            }
            //_serialDataFrameDelegate?.Invoke(dataFrame);
            if (_control != null)
            {
                _control.BeginInvoke(_serialDataFrameDelegate, new object[] { dataFrame});
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int ProcessRTUStatus(SerialDataFrame dataFrame)
        {
            int nResult = 0;
            RTUStatus = dataFrame.Status;
            RTUPFR = dataFrame.Data[0]; // 0bit  PFR ( 0: 정상, 1: RTU 모듈 리셋)
            /* bit
             * 0 : FRONT DOOR   (Close : 0, Open : 1) 
             * 1 : REAR  DOOR   (Close : 0, Open : 1) 
             * 2 : FAN          (OFF : 0, ON : 1) 
             * 3 : HEATER       (OFF : 0, ON : 1) 
             * 4 : AVR          (OFF : 0, ON : 1) 
             * 5 : HEATER 동작 모드(임계치에 의한 동작모드0 , 강제제어모드: 1)
             * 6 : FAN 동작 모드(임계치에 의한 동작모드0 , 강제제어모드: 1)
             */
            VDSRackStatus.SetRTSStatus(RTUStatus, dataFrame.Data);
            return nResult;
        }

        /// <summary>
        /// RUT 상태 요청
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int RTUStatustRequest(byte data = 0x0)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                if(serialCom!=null)
                {
                    SerialDataFrame rtuStatusRequest = new SerialDataFrame();
                    rtuStatusRequest.SetRTUStatusFrameRequest();
                    byte[] packet = rtuStatusRequest.Serialize();
                    nResult = serialCom.Send(packet);

                }


            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        /// <summary>
        /// 카메라 전원 리셋 요청
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int CameraResetRequest(byte data=0x0)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                SerialDataFrame camResetRequest = new SerialDataFrame();
                camResetRequest.SetCameraResetFrameRequest(data);
                byte[] packet = camResetRequest.Serialize();
                nResult = serialCom.Send(packet);
                

            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        /// <summary>
        /// 카메라 전원 리셋 응답
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private int ProcessCameraResetResponse(SerialDataFrame dataFrame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, "ProcessCameraResetResponse....");
                RTUPFR = dataFrame.Status;
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            
            return nResult;
        }


        /// <summary>
        /// AC 전원 리셋 요청
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int ACPowerResetRequest(byte ac1, byte ac2, byte ac3, byte ac4)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            byte data = 0x00;
            try
            {
                data = (byte)( ac1==1?0x01:0 + ac2==1?0x02:0 + ac3==1?0x04:0 + ac4==1?0x08:0  );
                SerialDataFrame camResetRequest = new SerialDataFrame();
                camResetRequest.SetCameraResetFrameRequest(data);
                byte[] packet = camResetRequest.Serialize();
                nResult = serialCom.Send(packet);


            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        /// <summary>
        /// AC 전원 리셋 응답
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private int ProcessACPowerResetResponse(SerialDataFrame dataFrame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, "ProcessACPowerResetResponse....");
                RTUPFR = dataFrame.Status;
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        /// <summary>
        ///  FAN 제어 요청 (0: 구동, 1: 정지)
        /// </summary>
        /// <param name="fan1"></param>
        /// <param name="fan2"></param>
        /// <returns></returns>
        public int FanCtrlRequest(byte fan1, byte fan2)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                SerialDataFrame fanCtrlRequest = new SerialDataFrame();
                fanCtrlRequest.SetFanControlFrameRequest(fan1, fan2);
                byte[] packet = fanCtrlRequest.Serialize();
                nResult = serialCom.Send(packet);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        /// <summary>
        /// FAN 제어 응답
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private int ProcessFanCtrlResponse(SerialDataFrame dataFrame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, "ProcessFanCtrlResponse....");
                RTUPFR = dataFrame.Status;
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        /// <summary>
        ///  HEATER 제어 요청 (0: 구동, 1: 정지)
        /// </summary>
        /// <param name="heat1"></param>
        /// <param name="heat2"></param>
        /// <returns></returns>
        public int HeaterCtrlRequest(byte heat1, byte heat2)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                SerialDataFrame heatCtrlRequest = new SerialDataFrame();
                heatCtrlRequest.SetHeatControlFrameRequest(heat1, heat2);
                byte[] packet = heatCtrlRequest.Serialize();
                nResult = serialCom.Send(packet);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        /// <summary>
        /// HEATER 제어 응답
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private int ProcessHeaterCtrlResponse(SerialDataFrame dataFrame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, "ProcessHeaterCtrlResponse....");
                RTUPFR = dataFrame.Status;
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        /// <summary>
        ///  Power Reset 요청 (0: 구동, 1: 정지)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int PowerResetRequest(byte data=0x0)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                SerialDataFrame powerResetRequest = new SerialDataFrame();
                powerResetRequest.SetPowerResetFrameRequest(data);
                byte[] packet = powerResetRequest.Serialize();
                nResult = serialCom.Send(packet);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        /// <summary>
        /// Power Reset 응답
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private int ProcessPowerResetResponse(SerialDataFrame dataFrame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, "ProcessPowerResetResponse....");
                RTUPFR = dataFrame.Status;
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        /// <summary>
        /// FAN 임계값 설정 요청
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int SetFanThresholdRequest(int data)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                SerialDataFrame fanThresholdRequest = new SerialDataFrame();
                fanThresholdRequest.SetFanThresholdFrameRequest(data);
                byte[] packet = fanThresholdRequest.Serialize();
                nResult = serialCom.Send(packet);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        /// <summary>
        /// /// FAN 임계값 설정 응답
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private int ProcessSetFanThresholdResponse(SerialDataFrame dataFrame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, "ProcessSetFanThresholdResponse....");
                RTUPFR = dataFrame.Status;
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        /// <summary>
        /// HEATER 임계값 설정 요청
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int SetHeaterThresholdRequest(int data)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                SerialDataFrame heaterThresholdRequest = new SerialDataFrame();
                heaterThresholdRequest.SetHeatThresholdFrameRequest(data);
                byte[] packet = heaterThresholdRequest.Serialize();
                nResult = serialCom.Send(packet);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        /// <summary>
        /// HEATER 임계값 설정 응답
        /// </summary>
        /// <param name="dataFrame"></param>
        /// <returns></returns>
        private int ProcessSetHeaterThresholdResponse(SerialDataFrame dataFrame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, "ProcessSetHeaterThresholdResponse....");
                RTUPFR = dataFrame.Status;
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }

        public void SetFormSerialDataFrameDelegate(Control control, FormSerialDataFrameDelegate serialDataFrameDelegate)
        {
            if (_serialDataFrameDelegate == null)
                _serialDataFrameDelegate = serialDataFrameDelegate;
            _control = control;
        }

        public void RTUStatustTest(byte[] packet, int length)
        {
            ProcessReceivePacket(packet, length);
        }
    }

}
