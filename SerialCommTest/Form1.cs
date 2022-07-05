using SerialComManageCtrl;
using SerialComManageCtrl.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;

namespace SerialCommTest
{
    public partial class Form1 : Form
    {
        SerialComManager serialManager = new SerialComManager();
        SerialCom rtuPort = new SerialCom();
        private VDSLogger _Logger = new VDSLogger();

        SerialDataFrame _lastRequestDataFrame = null;

        public Form1()
        {
            InitializeComponent();
            _Logger.SetManagerType(MANAGER_TYPE.VDS_SERVER);
            Utility._addLog = _Logger.AddLog;
            DisplayRTUStatus();

            //ucBlinkLight.SetBlinkType(1);
            //ledFrontGate.SetBlinkType(0);
            //ledRearGate.SetBlinkType(0);
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            SerialDataFrame rtuStatus = new SerialDataFrame();

            rtuStatus.MakeRTSStatusFrameResponse();
            byte[] packet = rtuStatus.Serialize();

            SerialDataFrame rtuStatus2 = new SerialDataFrame();
            int i = rtuStatus2.Deserialize(packet, 0);
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            serialManager.CameraResetRequest();
        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            SerialDataFrame camResetResponse = new SerialDataFrame();
            camResetResponse.SetCameraResetFrameResponse();

            byte[] packet = camResetResponse.Serialize();

            SerialDataFrame camResetResponse2 = new SerialDataFrame();
            camResetResponse2.Deserialize(packet, 0);

        }

        private void darkButton5_Click(object sender, EventArgs e)
        {
            serialManager.FanCtrlRequest(1, 0);

        }


        private void darkButton7_Click(object sender, EventArgs e)
        {
            serialManager.HeaterCtrlRequest(1, 0);
        }


        private void darkButton9_Click(object sender, EventArgs e)
        {
            serialManager.PowerResetRequest();
        }

        private void darkButton8_Click(object sender, EventArgs e)
        {
            SerialDataFrame powerCtrlResponse = new SerialDataFrame();
            powerCtrlResponse.SetPowerResetFrameResponse(0);

            byte[] packet = powerCtrlResponse.Serialize();

            SerialDataFrame powerCtrlResponse2 = new SerialDataFrame();
            powerCtrlResponse2.Deserialize(packet, 0);
        }

        private void darkButton11_Click(object sender, EventArgs e)
        {
            serialManager.SetFanThresholdRequest(100);
        }

        private void darkButton10_Click(object sender, EventArgs e)
        {
            SerialDataFrame fanCtrlResponse = new SerialDataFrame();
            fanCtrlResponse.SetFanThresholdFrameResponse(100);

            byte[] packet = fanCtrlResponse.Serialize();

            SerialDataFrame fanCtrlResponse2 = new SerialDataFrame();
            fanCtrlResponse2.Deserialize(packet, 0);
        }

        private void darkButton13_Click(object sender, EventArgs e)
        {
            serialManager.SetHeaterThresholdRequest(200);
        }

        private void darkButton12_Click(object sender, EventArgs e)
        {
            SerialDataFrame heatCtrlResponse = new SerialDataFrame();
            heatCtrlResponse.SetHeatThresholdFrameResponse(100);

            byte[] packet = heatCtrlResponse.Serialize();

            SerialDataFrame heatCtrlResponse2 = new SerialDataFrame();
            heatCtrlResponse2.Deserialize(packet, 0);
        }

        private void darkButton14_Click(object sender, EventArgs e)
        {
            serialManager.SetSerialPort(txtPortName.Text);
            serialManager.SetFormSerialDataFrameDelegate(this, new FormSerialDataFrameDelegate(ucRTUStatus.ProcessSerialDataFrame));
            _Logger.StartManager();
            serialManager.StartManager();
        }


        private void OpenRTUPort()
        {
            if (rtuPort != null && rtuPort.isOpened)
                rtuPort.Close();
            rtuPort.Init(txtPortName.Text, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.None, System.IO.Ports.Handshake.None, new SerialDataReceivedEventHandler(SerialReceivedData));
            rtuPort.Open();

        }

        

        
        private void SerialReceivedData(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[VDSConfig.PACKET_SIZE];
            if (rtuPort.isOpened)
            {
                int readCount = rtuPort.serialPort.Read(buffer, 0, VDSConfig.PACKET_SIZE);
                ProcessReceivePacket(buffer, readCount);

            }
        }

        private int ProcessReceivePacket(byte[] packet, int length)
        {
            int nResult = 0;
            SerialDataFrame dataFrame = null;
            int i = 0;
            while (i < packet.Length)
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
                if (dataFrame.bDataCompleted)
                {
                    // processDataFrame....
                    ProcessDataFrame(dataFrame);
                    dataFrame = null;
                    nResult++;
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"DataFrame 패킷 미완성 i={i}, packet.Length={packet.Length}"));
                }
            }
            return nResult;
        }

        private int ProcessDataFrame(SerialDataFrame dataFrame)
        {
            int nResult = 0;
            switch (dataFrame.OpCode)
            {
                case SerialDataFrameDefine.OPCODE_CAMERA_RESET:
                    nResult = ProcessCameraResetRequest(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_FAN_CTRL:
                    nResult = ProcessFanCtrlRequest(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_HEATER_CTRL:
                    nResult = ProcessHeaterCtrlRequest(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_POWER_RESET:
                    nResult = ProcessPowerResetRequest(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_FAN_THRESHOLD:
                    nResult = ProcessSetFanThresholdRequest(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_HEATER_THRESHOLD:
                    nResult = ProcessSetHeaterThresholdRequest(dataFrame);
                    break;
            }

            return nResult;
        }

        private int ProcessCameraResetRequest(SerialDataFrame request)
        {
            SerialDataFrame response = new SerialDataFrame();
            response.SetCameraResetFrameResponse();
            response.Status = 0x01; // PFR set 
            byte[] packet = response.Serialize();
            return rtuPort.Send(packet);
        }

        private int ProcessFanCtrlRequest(SerialDataFrame request)
        {
            SerialDataFrame response = new SerialDataFrame();
            response.SetFanControlFrameResponse(request.Data[0]);
            response.Status = 0x01; // PFR set 
            byte[] packet = response.Serialize();
            return rtuPort.Send(packet);
        }

        private int ProcessHeaterCtrlRequest(SerialDataFrame request)
        {
            SerialDataFrame response = new SerialDataFrame();
            response.SetHeatControlFrameResponse(request.Data[0]);
            response.Status = 0x01; // PFR set 
            byte[] packet = response.Serialize();
            return rtuPort.Send(packet);
        }

        private int ProcessPowerResetRequest(SerialDataFrame request)
        {
            SerialDataFrame response = new SerialDataFrame();
            response.SetPowerResetFrameResponse(request.Data[0]);
            response.Status = 0x01; // PFR set 
            byte[] packet = response.Serialize();
            return rtuPort.Send(packet);
        }


        private int ProcessSetFanThresholdRequest(SerialDataFrame request)
        {
            SerialDataFrame response = new SerialDataFrame();
            response.SetFanThresholdFrameResponse(request.Data[0]);
            response.Status = 0x01; // PFR set 
            byte[] packet = response.Serialize();
            return rtuPort.Send(packet);
        }
        private int ProcessSetHeaterThresholdRequest(SerialDataFrame request)
        {
            SerialDataFrame response = new SerialDataFrame();
            response.SetHeatThresholdFrameResponse(request.Data[0]);
            response.Status = 0x01; // PFR set 
            byte[] packet = response.Serialize();
            return rtuPort.Send(packet);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte value = Utility.GetThresholdToByte(-96);
            int threshold = Utility.GetThresholdToInt(value);

            ////        lrc:= 0
            ////for each byte b in the buffer do
            ////                lrc:= (lrc + b) and 0xFF
            ////lrc:= (((lrc XOR 0xFF) +1) and 0xFF)
            //byte[] packet = new byte[3];
            //int i = 0;
            //packet[i++] = 0x33;
            //packet[i++] = 0x37;
            //packet[i++] = 0x31;
            ////packet[i++] = 0x67;
            ////packet[i++] = 0x61;
            ////packet[i++] = 0x64;
            ////packet[i++] = 0x72;
            ////packet[i++] = 0x6f;
            //byte code = GetLRCCode(packet);

        }

        private byte GetLRCCode(byte[] packet)
        {
            byte result = 0;
            for(int i=0;i<packet.Length;i++)
            {
                result = (byte)((result + packet[i]) & 0xFF);

            }
            result = (byte) (((result ^ 0xFF) + 1) & 0xFF);
            return result;
        }

        private void ledBulb1_Click(object sender, EventArgs e)
        {
            ((LedBulb)sender).On = !((LedBulb)sender).On; 
        }

        private void darkButton3_Click_1(object sender, EventArgs e)
        {
            
            //ucBlinkLight.SetOn(1);
            //ucLight.SetOn(1);
        }

        private int ProcessSerialDataFrame(SerialDataFrame dataFrame)
        {
            int nResult = 0;
            byte opCode = (byte)(dataFrame.OpCode - SerialDataFrameDefine.OPCODE_RESPONSE);
            switch (opCode)
            {
                case SerialDataFrameDefine.OPCODE_RTU_STATUS:
                    break;
                default:
                    nResult = DisplayRequestResponse(dataFrame);
                    break;
            }
            //ucBlinkLight.SetOn(1);
            return nResult;
        }

        private int DisplayRTUStatus()
        {
            int nResult = 0;
            ucRTUStatus.DisplayRTUStatus(VDSRackStatus.GetRackStatus());
            //ledFrontGate.SetOn(VDSRackStatus.IsFrontDoorOpen==1 ? 1 : 0); //닫혔을 경우 On
            //ledRearGate.SetOn(VDSRackStatus.IsRearDoorOpen == 1 ? 1 : 0); //닫혔을 경우 On
            //ledFan.SetOn(VDSRackStatus.IsFanOn == 1 ? 1 : 0); //동작시 On
            //ledHeater.SetOn(VDSRackStatus.IsHeaterOn == 1 ? 1 : 0); //동작시 On
            //ledAVR.SetOn(VDSRackStatus.IsAVROn == 1 ? 1 : 0); //동작시 On
            //lbTemperature.Text = String.Format($"{VDSRackStatus.Temperature} °C");
            //lbFanThreshold.Text = String.Format($"{VDSRackStatus.FanThreshold}");
            //lbAVRVolt.Text = String.Format($"{VDSRackStatus.AVRVoltThreshold}");
            //lbAVRAmp.Text = String.Format($"{VDSRackStatus.AVRAmpThreshold}");
            //lbHeaterThreshold.Text = String.Format($"{VDSRackStatus.HeaterThreshold}");
            //lbHumidity.Text = String.Format($"{VDSRackStatus.HumitidyThreshold}");


            return nResult;

        }

        private int DisplayRequestResponse(SerialDataFrame dataFrame)
        {
            int nResult = 0;
            byte opCode = (byte)(dataFrame.OpCode - SerialDataFrameDefine.OPCODE_RESPONSE);
            switch (opCode)
            {
                case SerialDataFrameDefine.OPCODE_CAMERA_RESET:
                    DisplayCameraResetResponse(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_FAN_CTRL:
                    DisplayFanCtrlResponse(dataFrame);
                    break;
                case SerialDataFrameDefine.OPCODE_HEATER_CTRL:
                    DisplayHeaterCtrlResponse(dataFrame);
                    
                    break;
                case SerialDataFrameDefine.OPCODE_POWER_RESET:
                    DisplayPowerResetResponse(dataFrame);
                    
                    break;
                case SerialDataFrameDefine.OPCODE_FAN_THRESHOLD:
                    DisplaySetFanThreshHoldResponse(dataFrame);
                    
                    break;
                case SerialDataFrameDefine.OPCODE_HEATER_THRESHOLD:
                    DisplaySetHeaterThreshHoldResponse(dataFrame);
                    break;
            }

            return nResult;

        }
        

        private void tbFan_Scroll(object sender, EventArgs e)
        {
            lbFanValue.Text = tbFan.Value.ToString();
        }

        private void tbHeater_Scroll(object sender, EventArgs e)
        {
            lbHeaterValue.Text = tbHeater.Value.ToString();
        }

        private void darkButton3_Click_2(object sender, EventArgs e)
        {
            byte fan1, fan2;
            String strInfo;

            fan1 = (byte) (rdgFan1On.Checked ? 0 : 1); // 0: 구동, 1: 정지
            fan2 = (byte) (rdgFan2On.Checked ? 0 : 1); // 0: 구동, 1: 정지
            serialManager.FanCtrlRequest(fan1, fan2);

            strInfo = String.Format($"FAN1={fan1} FAN2={fan2} 요청");
            SetRequestInfo(strInfo);
        }

        private void darkButton4_Click(object sender, EventArgs e)
        {
            String strInfo = String.Empty;
            if(rdgCamera.Checked)
            {
                strInfo = String.Format("카메라 Reset 요청");
                serialManager.CameraResetRequest();

            }
            else if(rdgPower.Checked)
            {
                strInfo = String.Format("Power Fail Reset 요청");
                serialManager.PowerResetRequest();
            }
            SetRequestInfo(strInfo);
        }

        private void darkButton10_Click_1(object sender, EventArgs e)
        {
            byte heater1, heater2;

            heater1 = (byte)(rdgHeater1On.Checked ? 0 : 1); // 0: 구동, 1: 정지
            heater2 = (byte)(rdgHeater2On.Checked ? 0 : 1); // 0: 구동, 1: 정지
            serialManager.HeaterCtrlRequest(heater1, heater2);

            String strInfo = String.Format($"Heater1 = {heater1} Heater2 ={heater2} 요청");
            SetRequestInfo(strInfo);
        }

        private void darkButton6_Click(object sender, EventArgs e)
        {
            int threshold = tbFan.Value;
            serialManager.SetFanThresholdRequest(threshold);

            String strInfo = String.Format($"FAN 임계값({threshold}) 요청");
            SetRequestInfo(strInfo);
        }

        private void darkButton8_Click_1(object sender, EventArgs e)
        {
            int threshold = tbHeater.Value;
            serialManager.SetHeaterThresholdRequest(threshold);

            String strInfo = String.Format($"Heater 임계값({threshold}) 요청");
            SetRequestInfo(strInfo);

        }

        private void DisplayCameraResetResponse(SerialDataFrame dataFrame)
        {
            String info;
            info = String.Format($"카메라 전원 리셋 응답. PFR={dataFrame.Status}");
            SetResponseInfo(info);

        }

        private void DisplayPowerResetResponse(SerialDataFrame dataFrame)
        {
            String info;
            info = String.Format($"Power Fail Reset 응답 ");
            SetResponseInfo(info);

        }


        private void DisplayFanCtrlResponse(SerialDataFrame dataFrame)
        {
            String info;
            info = String.Format($"Fan 동작 제어 응답. PFR={dataFrame.Status}, FAN1={dataFrame.Data[0] & 0x01}, FAN2={(dataFrame.Data[0] & 0x10)>>4}");
            SetResponseInfo(info);
        }

        private void DisplayHeaterCtrlResponse(SerialDataFrame dataFrame)
        {
            String info;
            info = String.Format($"Heater 동작 제어 응답. PFR={dataFrame.Status}, Heater1={dataFrame.Data[0] & 0x01}, Heater2={(dataFrame.Data[0] & 0x10) >> 4}");
            SetResponseInfo(info);
        }


        private void DisplaySetFanThreshHoldResponse(SerialDataFrame dataFrame)
        {
            String info;
            info = String.Format($"Fan 임계값 설정 응답. PFR={dataFrame.Status}, 임계값={Utility.GetThresholdToInt(dataFrame.Data[0]) }");
            SetResponseInfo(info);
        }

        private void DisplaySetHeaterThreshHoldResponse(SerialDataFrame dataFrame)
        {
            String info;
            info = String.Format($"Heater 임계값 설정 응답. PFR={dataFrame.Status}, 임계값={Utility.GetThresholdToInt(dataFrame.Data[0]) }");
            SetResponseInfo(info);
        }
        



        private void SetRequestInfo(String info)
        {
            lbRequest.Text = String.Format($"[{DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT)}] {info}");
        }

        private void SetResponseInfo(String info)
        {
            lbResult.Text = String.Format($"[{DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT)}] {info}");
        }

        private void ucRTUStatusBar1_Load(object sender, EventArgs e)
        {

        }
    }


}
