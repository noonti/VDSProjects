﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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
            int nResult = 0;
            try
            {
                serialPort.Open();
                isOpened = true;
                nResult = 1;
            }
            catch(Exception ex)
            {
                nResult = 0;
            }
            return nResult;
        }

        public int Close()
        {
            int nResult = 0;
            try
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
                serialPort = null;
                isOpened = false;
                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
            }
            return nResult;
        }

        public int Send(byte[] data)
        {
            int nResult = 0;
            try
            {
                serialPort.Write(data, 0, data.Length);
                nResult = data.Length;
            }
            catch(Exception ex)
            {
                nResult = 0;
            }
            return nResult;
        }
    }
}
