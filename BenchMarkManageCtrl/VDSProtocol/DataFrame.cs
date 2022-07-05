using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace BenchMarkManageCtrl
{
    public class DataFrame
    {
        public byte Version;
        public byte[] Addr = new byte[8];
        public byte OpCode;
        public UInt16 DataLength;

        public byte[] data;
        public byte[] header = new byte[DataFrameDefine.VDS_FRAME_HEADER];

        public int ReadHeaderCount;
        public int ReadDataCount;
        public bool bHeaderCompleted;
        public bool bDataCompleted;

        public IOpData opDataFrame;

        public DataFrame()
        {
            Init();
        }

        public DataFrame(byte opCode)
        {
            Init();
            SetResponseOpCode(opCode);
        }

        public void Init()
        {
            Version = DataFrameDefine.VERSION;
            ReadHeaderCount = 0;
            ReadDataCount = 0;
            DataLength = 0;
            bHeaderCompleted = false;
            bDataCompleted = false;
            Addr = Utility.StringToByte(VDSConfig.controllerConfig.ControllerId);
        }
        public int Deserialize(byte[] packet, int startIdx)
        {
            int i;
            i = startIdx;
            int CopyCount = 0;

            if (bHeaderCompleted && bDataCompleted)
                return i;

            if (ReadHeaderCount < DataFrameDefine.VDS_FRAME_HEADER)
            {

                if (DataFrameDefine.VDS_FRAME_HEADER - ReadHeaderCount < packet.Length)
                {
                    CopyCount = DataFrameDefine.VDS_FRAME_HEADER - ReadHeaderCount;
                }
                else
                {
                    CopyCount = packet.Length;
                }
                Array.Copy(packet, i, header, ReadHeaderCount, CopyCount);
                i += CopyCount;
                ReadHeaderCount += CopyCount;
            }

            if (!bHeaderCompleted && ReadHeaderCount == DataFrameDefine.VDS_FRAME_HEADER)
            {
                // version 1byte
                Version = header[0];
                // addr 8 byte 
                Array.Copy(header, 1, Addr, 0, 8);
                // 1 8 1 1 1 8 =
                // op code 
                OpCode = header[9];

                byte[] size = new byte[2];
                Array.Copy(header, 10, size, 0, 2);
                DataLength = Utility.toLittleEndianInt16(size);
                data = new byte[DataLength];
                bHeaderCompleted = true;
            }

            if (i < packet.Length && !bDataCompleted) // Header 완성되었을 경우
            {
                if (ReadDataCount < DataLength)
                {
                    //CopyCount = packet.Length - i;
                    if (DataLength - ReadDataCount < packet.Length - i)
                        CopyCount = DataLength - ReadDataCount;
                    else
                        CopyCount = packet.Length - i;

                    Array.Copy(packet, i, data, ReadDataCount, CopyCount);
                    ReadDataCount += CopyCount;
                    i += CopyCount;
                    if (ReadDataCount == DataLength)
                    {
                        opDataFrame = DataFrameDefine.GetDataFrame(data);
                        bDataCompleted = true;
                    }
                        
                }
            }
            return i;
        }

        public byte[] Serialize()
        {
            byte[] result = null;
            byte[] opData = null;
            if (opDataFrame != null)
            {
                opData = opDataFrame.Serialize();
                result = new byte[DataFrameDefine.VDS_FRAME_HEADER + opData.Length];
                result[0] = Version;
                Array.Copy(Addr, 0, result, 1, 8);
                result[9] = OpCode;
                DataLength = (ushort)opData.Length;
                Array.Copy(Utility.toBigEndianInt16(DataLength), 0, result, 10, 2);
                Array.Copy(opData, 0, result, 12, DataLength);

            }
            return result;
        }

        public int SetResponseOpCode(byte opCode)
        {
            int nResult = 0;
            try
            {
                //public const byte OP_FRAME_INITIAL_REQUEST = 0xA1;
                //public const byte OP_FRAME_INITIAL_RESPONSE = 0xA2;
                //public const byte OP_FRAME_RE_REQUEST = 0xA3;
                //public const byte OP_FRAME_RE_RESPONSE = 0xA4;
                if(opCode == DataFrameDefine.OP_FRAME_INITIAL_REQUEST ||
                   opCode == DataFrameDefine.OP_FRAME_RE_REQUEST)
                {
                    OpCode = (byte)(opCode + 1);// 0x
                    nResult = 1;
                }

                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"정보 요청  OPCode={opCode}: {Utility.PrintOPCodeName(opCode)} 에 대한 응답 OpCode={OpCode} :{Utility.PrintOPCodeName(OpCode)} "));

                Utility.AddLog(LOG_TYPE.LOG_INFO, $"");
            }
            catch(Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult ;
        }
    }
}
