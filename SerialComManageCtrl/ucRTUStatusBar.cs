using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SerialComManageCtrl.Protocol;
using VDSCommon;
using VDSCommon.API.Model;

namespace SerialComManageCtrl
{
    public partial class ucRTUStatusBar : UserControl
    {
        public ucRTUStatusBar()
        {
            InitializeComponent();

            ucBlinkLight.SetBlinkType(1);
            ledFrontGate.SetBlinkType(0);
            ledRearGate.SetBlinkType(0);
        }


        public int ProcessSerialDataFrame(SerialDataFrame dataFrame)
        {
            int nResult = 0;
            byte opCode = (byte)(dataFrame.OpCode - SerialDataFrameDefine.OPCODE_RESPONSE);
            switch (opCode)
            {
                case SerialDataFrameDefine.OPCODE_RTU_STATUS:
                    nResult = DisplayRTUStatus(VDSRackStatus.GetRackStatus());
                    break;
                
                    
            }
            ucBlinkLight.SetOn(1);
            return nResult;
        }

        public int DisplayRTUStatus(RackStatus rackStatus)
        {
            int nResult = 0;
            ledFrontGate.SetOn(rackStatus.IsFrontDoorOpen == 1 ? 1 : 0); //닫혔을 경우 On
            ledRearGate.SetOn(rackStatus.IsRearDoorOpen == 1 ? 1 : 0); //닫혔을 경우 On
            ledFan.SetOn(rackStatus.IsFanOn == 1 ? 1 : 0); //동작시 On
            ledHeater.SetOn(rackStatus.IsHeaterOn == 1 ? 1 : 0); //동작시 On
            ledAVR.SetOn(rackStatus.IsAVROn == 1 ? 1 : 0); //동작시 On
            lbTemperature.Text = String.Format($"{rackStatus.Temperature} °C");
            lbFanThreshold.Text = String.Format($"{rackStatus.FanThreshold}");
            lbAVRVolt.Text = String.Format($"{rackStatus.AVRVoltThreshold}");
            lbAVRAmp.Text = String.Format($"{rackStatus.AVRAmpThreshold}");
            lbHeaterThreshold.Text = String.Format($"{rackStatus.HeaterThreshold}");
            lbHumidity.Text = String.Format($"{rackStatus.HumitidyThreshold}");


            return nResult;

        }
    }
}
