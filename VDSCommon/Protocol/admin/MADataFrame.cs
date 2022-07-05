using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Protocol.admin
{
    public class MADataFrame
    {
        public byte Version;
        public byte RequestType;
        public byte OpCode;
        public byte[] TransactionId = new byte[20];

        public byte[] header = new byte[MADataFrameDefine.VDS_FRAME_HEADER];


        public UInt32 DataSize;
        public byte[] Data;

        public int ReadHeaderCount;
        public int ReadDataCount;
        public bool bHeaderCompleted;
        public bool bDataCompleted;

        public IOpData opDataFrame;


        public MADataFrame()
        {
            Init();
        }
        public void Init()
        {
            Version = MADataFrameDefine.VERSION;
            ReadHeaderCount = 0;
            ReadDataCount = 0;
            DataSize = 0;
            bHeaderCompleted = false;
            bDataCompleted = false;

            byte[] id = Utility.StringToByte(DateTime.Now.ToString("yyyyMMddHHmmssff"));
            Array.Copy(id, 0, TransactionId, 0, id.Length);

        }
        public int Deserialize(byte[] packet, int startIdx)
        {
            int i;
            i = startIdx;
            byte[] dataSize = new byte[4];
            int CopyCount = 0;

            if (bHeaderCompleted && bDataCompleted)
                return i;

            if (ReadHeaderCount < MADataFrameDefine.VDS_FRAME_HEADER)
            {

                if (MADataFrameDefine.VDS_FRAME_HEADER - ReadHeaderCount < packet.Length)
                {
                    CopyCount = MADataFrameDefine.VDS_FRAME_HEADER - ReadHeaderCount;
                }
                else
                {
                    CopyCount = packet.Length;
                }
                Array.Copy(packet, i, header, ReadHeaderCount, CopyCount);
                i += CopyCount;
                ReadHeaderCount += CopyCount;
            }

            if (!bHeaderCompleted && ReadHeaderCount == MADataFrameDefine.VDS_FRAME_HEADER)
            {
                // version 1byte
                Version = header[0];
                RequestType = header[1];
                OpCode = header[2];
                Array.Copy(header, 3, TransactionId, 0, 20);

                // data size
                Array.Copy(header, 23, dataSize, 0, 4);

                DataSize = Utility.toLittleEndianInt32(dataSize);
                Data = new byte[DataSize];

                bHeaderCompleted = true;
            }

            if (i < packet.Length && !bDataCompleted) // Header 완성되었을 경우
            {
                if (ReadDataCount < DataSize)
                {
                    //CopyCount = packet.Length - i;
                    if (DataSize - ReadDataCount < packet.Length - i)
                        CopyCount = (int)DataSize - ReadDataCount;
                    else
                        CopyCount = packet.Length - i;

                    Array.Copy(packet, i, Data, ReadDataCount, CopyCount);
                    ReadDataCount += CopyCount;
                    i += CopyCount;
                    if (ReadDataCount == DataSize)
                    {
                        opDataFrame = MADataFrameDefine.GetDataFrame(RequestType, OpCode, Data);
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
            byte[] dataSize = new byte[4];
            if (opDataFrame != null)
            {
                opData = opDataFrame.Serialize();
                DataSize = (UInt32)opData.Length;
                result = new byte[MADataFrameDefine.VDS_FRAME_HEADER + opData.Length];
                result[0] = Version;
                result[1] = RequestType;
                result[2] = OpCode;

                Array.Copy(TransactionId, 0, result, 3, 20);
                Array.Copy(Utility.toBigEndianInt32(DataSize), 0, result, 23, 4);
                Array.Copy(opData, 0, result, 27, DataSize);
            }
            return result;
        }

    }
}
