using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialComManageCtrl.Protocol
{
    public static class SerialDataFrameDefine
    {
        public const byte FRAME_STX = 0x7E;
        public const byte FRAME_HEADER_SIZE = 3; // 3bytes header

        public const byte OPCODE_RESPONSE = 0x80;
        public const byte OPCODE_RTU_STATUS = 0x01;
        public const byte OPCODE_CAMERA_RESET = 0x02;
        public const byte OPCODE_CTRL_POWER = 0x02; // AC 전원 제어
        public const byte OPCODE_FAN_CTRL = 0x03;
        public const byte OPCODE_HEATER_CTRL = 0x04;
        public const byte OPCODE_POWER_RESET = 0x05;
        public const byte OPCODE_FAN_THRESHOLD = 0x06;
        public const byte OPCODE_HEATER_THRESHOLD = 0x07;

    }
}
