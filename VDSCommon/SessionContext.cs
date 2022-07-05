using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class SessionContext
    {

        public CLIENT_TYPE _type ;
        // Client  socket.  
        public Socket _socket = null;
        // Size of receive buffer.  
        public const int BufferSize = VDSConfig.PACKET_SIZE;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        //public StringBuilder sb = new StringBuilder();

        public String Id = Guid.NewGuid().ToString();

        public SessionContext()
        {
            _type = CLIENT_TYPE.NONE;

        }
    }
}
