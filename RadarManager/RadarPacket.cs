using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace RadarManageCtrl
{
    public class RadarPacket
    {
        public bool bCompleted;
        public RECEIVER_STATE state;
        public byte[] data = new byte[VDSConfig.PACKET_SIZE];
        public int dataSize;
        int nMalformedPckt;
        public RadarPacket()
        {
            bCompleted = false;
            nMalformedPckt = 0;
            dataSize = 0;
            state = RECEIVER_STATE.S_BEGIN;
        }

        public int SetRadarPacket(byte[] packet, int startIdx,  int size)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            int nProcessCount = startIdx;
            byte c;
            while(nProcessCount < size && state!= RECEIVER_STATE.S_FAIL)
            {
                c = packet[nProcessCount++];
                switch(state)
                {
                    case RECEIVER_STATE.S_FAIL:
                    break;

                    case RECEIVER_STATE.S_BEGIN:
                        //crc = 0;
                        if (c == PacketDefine.MARK_BEGIN)
                            state = RECEIVER_STATE.S_MARKB;
                        else
                        {
                            //foreignByte(c)  // 아무것도 안함.
                            state = RECEIVER_STATE.S_FAIL;
                        }
                        break;

                    case RECEIVER_STATE.S_MARKB:
                        if (c == PacketDefine.ESC)
                            state = RECEIVER_STATE.S_ESC;
                        else if (c == PacketDefine.MARK_BEGIN)
                        {
                            nMalformedPckt++;
                            state = RECEIVER_STATE.S_MARKB;
                        }
                        else if (c == PacketDefine.MARK_END)
                        {
                            nMalformedPckt++;
                            state = RECEIVER_STATE.S_BEGIN;
                        }
                        else
                        {
                            state = RECEIVER_STATE.S_MSG;
                            data[dataSize++] = c;
                        }
                        break;
                    case RECEIVER_STATE.S_ESC:
                        if (c == PacketDefine.ESC_MARK_BEGIN)
                        {
                            data[dataSize++] = PacketDefine.MARK_BEGIN;
                            state = RECEIVER_STATE.S_MSG;
                        }
                        else if (c == PacketDefine.ESC_MARK_END)
                        {
                            data[dataSize++] = PacketDefine.MARK_END;
                            state = RECEIVER_STATE.S_MSG;
                        }
                        else if (c == PacketDefine.ESC_ESC)
                        {
                            data[dataSize++] = PacketDefine.ESC;
                            state = RECEIVER_STATE.S_MSG;
                        }
                        else
                        {
                            nMalformedPckt++;
                            state = RECEIVER_STATE.S_BEGIN;
                        }
                        break;
                    case RECEIVER_STATE.S_MSG:
                        if (c == PacketDefine.MARK_BEGIN)
                        {
                            nMalformedPckt++;
                            state = RECEIVER_STATE.S_BEGIN;
                            break;
                        }
                        else if (c == PacketDefine.ESC)
                        {
                            state = RECEIVER_STATE.S_ESC;
                            break;
                        }
                        else if (c == PacketDefine.MARK_END)
                        {
                            if (dataSize > sizeof(UInt16))
                            {
                                UInt16 crc1, crc2;

                                //crc1 = (UInt16)(data[dataSize - 2]);
                                crc1 =   (UInt16) ( (data[dataSize - 2]) | ((data[dataSize - 1]) << 8) );
                                crc2 = PacketDefine.GetCRCValue(data, dataSize - sizeof(UInt16));
                                if (crc1 == crc2)
                                {
                                    bCompleted = true;
                                    return nProcessCount;    /* done, out -----> */
                                }
                                else
                                {
                                    state = RECEIVER_STATE.S_BEGIN;
                                    break;
                                }
                            }
                            else
                            {

                                nMalformedPckt++;
                                state = RECEIVER_STATE.S_FAIL;
                                break;
                            }
                        }
                        else
                            data[dataSize++] = c;
                        break;
                    default:
                        state = RECEIVER_STATE.S_BEGIN;
                        break;

                }
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nProcessCount;
        }
    }
}
