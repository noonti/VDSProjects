using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace KorExManageCtrl.VDSProtocol
{
    public class ExDataFrame
    {
        public byte[] senderIP = new byte[16];
        public byte[] destinationIP = new byte[16];
        //public byte[] controllerKind = new byte[2];
        //public byte orgNo; // 지방청 번호
        //public byte[] controllerNum = new byte[5];
        public byte[] csn = new byte[8]; // 2: "VD" , 1: 지방청 코드 5: 제어기 번호
        //public UInt32 _csn;

        public byte[] totalLength = new byte[4];
        public int _totalLength;

        public byte opCode ;

        public int ReadHeaderCount;
        public int ReadDataCount;

        public byte[] data;
        public byte[] header = new byte[ExDataFrameDefine.HEADER_SIZE];


        public bool bHeaderCompleted;
        public bool bDataCompleted;

        public bool bRequstFrame;

        //
        public IExOPData opData;

        public ExDataFrame()
        {

            Init();
        }

        public ExDataFrame(ExDataFrame request)
        {
            Array.Copy(senderIP, 0, request.destinationIP, 0, 8);
            Array.Copy(destinationIP, 0, request.senderIP, 0, 8);
            Array.Copy(csn, 0, request.csn, 0, 8);



        }

        public void Init()
        {
            
            _totalLength = 0;

            Array.Copy(Utility.StringToByte("VD"),0, csn, 0,2) ;

            ReadHeaderCount = 0;
            ReadDataCount = 0;

            bHeaderCompleted = false;
            bDataCompleted = false;
            bRequstFrame = false;
        }


        public int Deserialize(byte[] packet, int startIdx)
        {
            int i;
            int headerIndex = 0;
            i = startIdx;
            int CopyCount = 0;

            if (bHeaderCompleted && bDataCompleted)
                return i;

            if (ReadHeaderCount < ExDataFrameDefine.HEADER_SIZE)
            {

                if (ExDataFrameDefine.HEADER_SIZE - ReadHeaderCount < packet.Length)
                {
                    CopyCount = ExDataFrameDefine.HEADER_SIZE - ReadHeaderCount;
                }
                else
                {
                    CopyCount = packet.Length;
                }
                Array.Copy(packet, i, header, ReadHeaderCount, CopyCount);
                i += CopyCount;
                ReadHeaderCount += CopyCount;
            }

            if (!bHeaderCompleted && ReadHeaderCount == ExDataFrameDefine.HEADER_SIZE)
            {

                // IP Setting 
                // Sender IP
                Array.Copy(header, headerIndex, senderIP, 0, 16);
                headerIndex += 16;

                // Destination IP
                Array.Copy(header, headerIndex, destinationIP, 0, 16);
                headerIndex += 16;

                // CSN (vds type , 지방청 코드, 제어기 번호)
                //// 지방청 번호
                //// 0x01:서울청, 0x02:원주청
                //// 0x03:대전청, 0x04:부산청
                //// 0x05:익산청
                Array.Copy(header, headerIndex, csn, 0, 8);
                headerIndex += 8;



                //// Controller Station Number  노선번호(2바이트) + 일련번호(2바이트)
                //Array.Copy(header, headerIndex, csn, 0, 4);
                //headerIndex += 4;
                //_csn = BitConverter.ToUInt32(csn, 0);

                Array.Copy(header, headerIndex, totalLength, 0, 4);
                _totalLength = (int)Utility.toLittleEndianInt32(totalLength);
                headerIndex += 4;

                opCode = header[headerIndex++];
               
                if(_totalLength - 1>0)
                {
                    data = new byte[_totalLength - 1];
                }
                else
                {
                    bDataCompleted = true; // 데이터가 없을 경우 
                }
                

                bHeaderCompleted = true;
            }

            if (i <= packet.Length && !bDataCompleted )// && opCode != ExDataFrameDefine.OP_CHECK_SESSION_COMMAND) // Header 완성되었을 경우
            {
               
                if (ReadDataCount < _totalLength-1)
                {
                    //CopyCount = packet.Length - i;
                    if (_totalLength - ReadDataCount < packet.Length - i)
                        CopyCount = _totalLength-1 - ReadDataCount;
                    else
                        CopyCount = packet.Length - i;

                    Array.Copy(packet, i, data, ReadDataCount, CopyCount);
                    ReadDataCount += CopyCount;
                    i += CopyCount;
                    if (ReadDataCount == _totalLength-1)
                    {
                        opData = ExDataFrameDefine.GetExOpData(bRequstFrame, opCode,data);
                        bDataCompleted = true;
                    }

                }
            }
            //else if(opCode == ExDataFrameDefine.OP_CHECK_SESSION_COMMAND)
            //{
            //    bRequstFrame = false;
            //    bDataCompleted = true;
            //    i = packet.Length;
            //}
            return i;
        }

        public byte[] Serialize()
        {
            byte[] result = null;
            byte[] data = null;
            int headerIndex = 0;
            if (opData != null)
            {
                data = opData.Serialize();
            }
            _totalLength = (data!=null?data.Length:0)  + 1;

            result = new byte[ExDataFrameDefine.HEADER_SIZE + _totalLength - 1];

            Array.Copy(senderIP, 0, result, headerIndex, 16);
            headerIndex += 16;


            Array.Copy(destinationIP, 0, result, headerIndex, 16);
            headerIndex += 16;

            Array.Copy(csn, 0, result, headerIndex, 8);
            headerIndex += 8;


            Array.Copy(Utility.toBigEndianInt32((UInt32)_totalLength), 0, result, headerIndex, 4);
            headerIndex += 4;

            result[headerIndex++] = opCode;

            if(data!=null)
                Array.Copy(data, 0, result, headerIndex, data.Length);

            
            return result;
        }

    }
}
