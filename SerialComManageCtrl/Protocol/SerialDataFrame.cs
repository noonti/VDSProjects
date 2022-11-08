using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace SerialComManageCtrl.Protocol
{
    public delegate int FormSerialDataFrameDelegate(SerialDataFrame dataFrame);

    public class SerialDataFrame
    {
        public byte Stx;
        public byte Size ;
        public byte OpCode;
        public byte Status;
        public byte[] Data;
        public byte LRC;


        public byte[] header = new byte[SerialDataFrameDefine.FRAME_HEADER_SIZE];
        public byte DataSize;
        public int ReadHeaderCount;
        public int ReadDataCount;
        public bool bHeaderCompleted;
        public bool bStatusCompleted;
        public bool bDataCompleted;

        public bool isResponse;
        public bool isLRCOK;

        public byte[] packetData = null;

        public SerialDataFrame()
        {
            Init();
        }

        
        public void Init()
        {
            Stx = SerialDataFrameDefine.FRAME_STX;
            DataSize = 0;
            ReadHeaderCount = 0;
            ReadDataCount = 0;
            bHeaderCompleted = false;
            bDataCompleted = false;
            bStatusCompleted = false;
            isResponse = false;
            isLRCOK = false;
        }


        public bool IsResponse(byte opcode)
        {
            bool result = false;
            if (((opcode & SerialDataFrameDefine.OPCODE_RESPONSE) >> 7) == 1)
                result = true;
            return result;
        }
        public int Deserialize(byte[] packet, int startIdx)
        {
            int i;
            i = startIdx;
            int CopyCount = 0;

            if (bHeaderCompleted && bDataCompleted)
                return i;

            if (ReadHeaderCount < SerialDataFrameDefine.FRAME_HEADER_SIZE)
            {

                if (SerialDataFrameDefine.FRAME_HEADER_SIZE - ReadHeaderCount < packet.Length)
                {
                    CopyCount = SerialDataFrameDefine.FRAME_HEADER_SIZE - ReadHeaderCount;
                }
                else
                {
                    CopyCount = packet.Length;
                }
                Array.Copy(packet, i, header, ReadHeaderCount, CopyCount);
                i += CopyCount;
                ReadHeaderCount += CopyCount;
            }

            if (!bHeaderCompleted && ReadHeaderCount == SerialDataFrameDefine.FRAME_HEADER_SIZE)
            {
                Stx = header[0];
                Size = header[1];
                OpCode = header[2];
                isResponse = IsResponse(OpCode);
                if(isResponse)
                    DataSize = (byte)(Size - 5); 
                else
                    DataSize = (byte)(Size - 4);
                Data = new byte[DataSize]; // header(3) + LRC(1) 뺀 나머지 할당 
                bHeaderCompleted = true;
            }
            if (i < packet.Length && isResponse && !bStatusCompleted)
            {
                Status = packet[i++];
                bStatusCompleted = true;
            }
                

            if (i < packet.Length && !bDataCompleted) // Header 완성되었을 경우
            {
                
                if (ReadDataCount < DataSize)
                {
                    //CopyCount = packet.Length - i;
                    if (DataSize - ReadDataCount < packet.Length - i)
                        CopyCount = DataSize - ReadDataCount;
                    else
                        CopyCount = packet.Length - i;

                    Array.Copy(packet, i, Data, ReadDataCount, CopyCount);
                    ReadDataCount += CopyCount;
                    i += CopyCount;
                    if (ReadDataCount == DataSize)
                    {
                        bDataCompleted = true;
                    }
                    if(i < packet.Length && bDataCompleted) // LRC
                    {
                        LRC = packet[i];
                        packetData = GetPacketData(LRC);
                        isLRCOK = CheckLRCCode(packetData, 0,packetData.Length -2, LRC);
                        i++;

                    }

                }
            }
            return i;
        }


        public byte[] Serialize()
        {
            byte[] result;
            int i = 0;
            try
            {
                isResponse = IsResponse(OpCode);

                Size = (byte)(Data.Length + 4);
                if (isResponse)
                {
                    Size += 1; // Status 값 추가 
                }
                result = new byte[Size];
                result[i++] = Stx;
                result[i++] = Size;
                result[i++] = OpCode;
                if(isResponse)
                    result[i++] = Status;
                Array.Copy(Data, 0, result, i, Data.Length);
                i+= Data.Length;

                LRC = Utility.GetLRCCode(result,0, i);
                result[i++] = LRC;
            }
            catch (Exception ex)
            {
                result = null;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return result;
        }
        

        public byte GetLRCCode(byte[] buffer, int len)
        {
            return 1;
        }

        //CheckLRCCode(packet, startIdx, i-1, LRC);
        public bool CheckLRCCode(byte[] packet, int startIndex, int endIndex, byte lrc)
        {

            bool result = false;

            // 아래 정보 조합하여 LRC 체크 한다. 

            //public byte Stx;
            //public byte Size;
            //public byte OpCode;
            //public byte Status;
            //public byte[] Data;
            //public byte LRC;
            // LRC 와 lrc 값 비교
            byte value = Utility.GetLRCCode(packet, startIndex, endIndex);
            Console.WriteLine($"GetLRCCode = {value}");
            if (lrc == value)
                result = true;
            return result;
        }
        /// <summary>
        /// 편의상 상태 정보 생성..
        /// </summary>
        /// <returns></returns>
        public int MakeRTSStatusFrameResponse()
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_RESPONSE + SerialDataFrameDefine.OPCODE_RTU_STATUS;
                DataSize = 8;
                Status = 0x01; // 상태 정보 
                Data = new byte[DataSize];
                Data[0] = 0x02;// 검지기 상태 정보 
                Data[1] = 0x03;// 온도 계측값
                Data[2] = 0x04;// FAN  동작 임계값 
                Data[3] = 0x05;// AVR 출력 전압
                Data[4] = 0x06;// AVR 출력 전류
                Data[5] = 0x07;// HETER 동작 임계값
                Data[6] = 0x08;// 습도 계측값
                Data[7] = 0x09;// 예비용

                nResult = 1;

            }
            catch(Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }

        public int SetRTUStatusFrameRequest()
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_RTU_STATUS;
                Size = 5;
                DataSize = 1;
                Data = new byte[DataSize];
                Data[0] = 0x0;
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }


        /// <summary>
        /// 카메라 리셋 요청
        /// </summary>
        /// <returns></returns>
        public int SetCameraResetFrameRequest(byte data)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_CAMERA_RESET;
                Size = 5;
                DataSize = 1;
                Data = new byte[DataSize];
                Data[0] = data;
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }

        /// <summary>
        /// AC 파워 리셋 요청
        /// </summary>
        /// <returns></returns>
        public int SetACPowerResetFrameRequest(byte data)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_CTRL_POWER;
                Size = 5;
                DataSize = 1;
                Data = new byte[DataSize];
                Data[0] = data;
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }


        /// <summary>
        /// FAN 제어 요청 (1:  fan 구동, 0:fan  정지)
        /// </summary>
        /// <returns></returns>
        public int SetFanControlFrameRequest(byte fan1, byte fan2)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_FAN_CTRL;
                Size = 5;
                DataSize = 1;
                Data = new byte[DataSize];
                Data[0] = GetControlData(fan1, fan2);// (byte) (  ((fan2 & 0x01) << 4) + (fan1 & 0x01)  ) ;
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }


        /// <summary>
        /// Heater 제어 요청 (1:  Heater 구동, 0:Heater  정지)
        /// </summary>
        /// <returns></returns>
        public int SetHeatControlFrameRequest(byte heat1, byte heat2)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_HEATER_CTRL;
                Size = 5;
                DataSize = 1;
                Data = new byte[DataSize];
                Data[0] = GetControlData(heat1, heat2);// (byte) (  ((fan2 & 0x01) << 4) + (fan1 & 0x01)  ) ;
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }

        /// <summary>
        /// Power Fail Reset요청 
        /// </summary>
        /// <returns></returns>
        public int SetPowerResetFrameRequest(byte data)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_POWER_RESET;
                Size = 5;
                DataSize = 1;
                Data = new byte[DataSize];
                Data[0] = data;
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }


        /// <summary>
        ///  FAN 임계값 요청  
        /// </summary>
        /// <returns></returns>
        public int SetFanThresholdFrameRequest(int threshold)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_FAN_THRESHOLD;
                Size = 5;
                DataSize = 1;
                Data = new byte[DataSize];
                Data[0] = Utility.GetThresholdToByte(threshold);
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }

        /// <summary>
        ///  Heater 임계값 요청  
        /// </summary>
        /// <returns></returns>
        public int SetHeatThresholdFrameRequest(int threshold)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_HEATER_THRESHOLD;
                Size = 5;
                DataSize = 1;
                Data = new byte[DataSize];
                Data[0] = Utility.GetThresholdToByte(threshold);
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }


        /// <summary>
        /// 카메라 리셋 응답
        /// </summary>
        /// <returns></returns>
        public int SetCameraResetFrameResponse()
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_RESPONSE + SerialDataFrameDefine.OPCODE_CAMERA_RESET;
                DataSize = 1;
                Status = 0x01; // 상태 정보 
                Data = new byte[DataSize];
                Data[0] = 0x00;// 검지기 상태 정보 
                nResult = 1;
                
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }

        /// <summary>
        /// FAN 제어 응답
        /// </summary>
        /// <returns></returns>
        public int SetFanControlFrameResponse(byte data)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_RESPONSE + SerialDataFrameDefine.OPCODE_FAN_CTRL;
                DataSize = 1;
                Status = 0x01; // 상태 정보 
                Data = new byte[DataSize];
                Data[0] = data;// GetControlData(fan1,fan2);// (byte)(((fan2 & 0x01) << 4) + (fan1 & 0x01));
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }


        /// <summary>
        /// FAN 제어 응답
        /// </summary>
        /// <returns></returns>
        public int SetHeatControlFrameResponse(byte data)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_RESPONSE + SerialDataFrameDefine.OPCODE_HEATER_CTRL;
                DataSize = 1;
                Status = 0x01; // 상태 정보 
                Data = new byte[DataSize];
                Data[0] = data;// GetControlData(heat1, heat2);// (byte)(((fan2 & 0x01) << 4) + (fan1 & 0x01));
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }

        /// <summary>
        /// Power Fail Reset 응답
        /// </summary>
        /// <returns></returns>
        public int SetPowerResetFrameResponse(byte data)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_RESPONSE + SerialDataFrameDefine.OPCODE_POWER_RESET;
                DataSize = 1;
                Status = 0x01; // 상태 정보 
                Data = new byte[DataSize];
                Data[0] = data;
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }


        /// <summary>
        /// FAn 임계값 응답
        /// </summary>
        /// <returns></returns>
        public int SetFanThresholdFrameResponse(byte data)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_RESPONSE + SerialDataFrameDefine.OPCODE_FAN_THRESHOLD;
                DataSize = 1;
                Status = 0x01; // 상태 정보 
                Data = new byte[DataSize];
                Data[0] = data;
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }


        /// <summary>
        /// HEATER 임계값 응답
        /// </summary>
        /// <returns></returns>
        public int SetHeatThresholdFrameResponse(byte data)
        {
            int nResult = 0;
            try
            {
                OpCode = SerialDataFrameDefine.OPCODE_RESPONSE + SerialDataFrameDefine.OPCODE_HEATER_THRESHOLD;
                DataSize = 1;
                Status = 0x01; // 상태 정보 
                Data = new byte[DataSize];
                Data[0] = data;
                nResult = 1;

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }


            return nResult;
        }

        public byte GetControlData(byte op1, byte op2)
        {
           return  (byte)(((op2 & 0x01) << 4) + (op1 & 0x01));
        }


        public byte[] GetPacketData(byte lrc)
        {
            byte[] result = null;
            int index = 0;
            int size = SerialDataFrameDefine.FRAME_HEADER_SIZE + DataSize + 1 + (isResponse == true ? 1 : 0);
            result = new byte[size];
            Array.Copy(header, 0, result, index, header.Length);
            index += header.Length;
            if (isResponse)
                result[index++] = Status;
            Array.Copy(Data, 0, result, index, DataSize);
            index += DataSize;
            result[index++] = lrc;
            return result;
        }


    }
}
