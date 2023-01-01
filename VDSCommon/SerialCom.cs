using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class SerialCom
    {
        public SerialPort serialPort = null;
        public bool isOpened = false;

        //serialCom8.Init("COM8", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.None, System.IO.Ports.Handshake.None, new SerialDataReceivedEventHandler(SerialReceivedData8) );
        public int Init(String portName, int baudRate = 115200, Parity parity = System.IO.Ports.Parity.None, int dataBits = 8, StopBits stopBits = System.IO.Ports.StopBits.None, Handshake handShake = System.IO.Ports.Handshake.None, SerialDataReceivedEventHandler receivedHandler = null)
        {
            if (serialPort == null)
                serialPort = new SerialPort();

            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.Parity = parity;
            serialPort.DataBits = dataBits;
            //serialPort.StopBits = stopBits;
            serialPort.Handshake = handShake;
            if(receivedHandler!=null)
                serialPort.DataReceived += receivedHandler;

            serialPort.WriteTimeout = 500;
            return 1;

        }

        public int Open()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                serialPort.Open();
                isOpened = serialPort.IsOpen;
                nResult = 1;
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                nResult = 0;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int Close()
        {
            int nResult = 0;
            try
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
                isOpened = serialPort.IsOpen;
                serialPort = null;
                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }

        public int Send(byte[] data)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($" send Serial Data= {Utility.PrintHexaString(data, data.Length)} "));
                if(serialPort.IsOpen)
                {
                    serialPort.Write(data, 0, data.Length);
                    nResult = data.Length;
                }
                    
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                nResult = 0;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }
    }
}
