using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public interface IRTUManager
    {
        int StartManager();
        int StopManager();
        int SetSerialPort(String portName, int baudRate = 115200, Parity parity = System.IO.Ports.Parity.None, int dataBits = 8, StopBits stopBits = System.IO.Ports.StopBits.None, Handshake handShake = System.IO.Ports.Handshake.None);
        int Init(String portName, int baudRate = 115200, Parity parity = System.IO.Ports.Parity.None, int dataBits = 8, StopBits stopBits = System.IO.Ports.StopBits.None, Handshake handShake = System.IO.Ports.Handshake.None, SerialDataReceivedEventHandler receivedHandler = null);
        int SetTCPPort(String address, int port);


        /// <summary>
        /// 카메라 전원 리셋 요청
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int CameraResetRequest(byte data = 0x0);

        /// <summary>
        ///  FAN 제어 요청 (0: 구동, 1: 정지)
        /// </summary>
        /// <param name="fan1"></param>
        /// <param name="fan2"></param>
        /// <returns></returns>
        int FanCtrlRequest(byte fan1, byte fan2);


        /// <summary>
        ///  HEATER 제어 요청 (0: 구동, 1: 정지)
        /// </summary>
        /// <param name="heat1"></param>
        /// <param name="heat2"></param>
        /// <returns></returns>
        int HeaterCtrlRequest(byte heat1, byte heat2);


        /// <summary>
        ///  Power Reset 요청 (0: 구동, 1: 정지)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int PowerResetRequest(byte data = 0x0);


        /// <summary>
        /// FAN 임계값 설정 요청
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int SetFanThresholdRequest(int data);


        /// <summary>
        /// HEATER 임계값 설정 요청
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int SetHeaterThresholdRequest(int data);

    }
}
