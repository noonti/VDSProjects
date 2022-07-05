using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;

namespace SpeedLaneTester
{
    public partial class Form1 : Form
    {
        private VDSServer _ctrlServer = new VDSServer();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _ctrlServer.SetAddress(VDSConfig.IPADDRESS, 4693, CLIENT_TYPE.VDS_CLIENT, AcceptCtrlCallback);
            _ctrlServer.StartManager();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _ctrlServer.StopManager();
        }


        public void AcceptCtrlCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            String strLog;
            try
            {
                // Create the state object.  
                Socket serverSocket = (Socket)ar.AsyncState;
                Socket socket = serverSocket.EndAccept(ar);
                SessionContext session = new SessionContext();
                session._type = _ctrlServer._clientType;
                session._socket = socket;
                socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(ReadCallback), session);


                // start...send thread...

                new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            //Send(session._socket, "avogadro..send..");
                            Thread.Sleep(100);

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
                ).Start();

            }
            catch (Exception ex)
            {
                strLog = ex.StackTrace.ToString();
               
            }
            finally
            {
                _ctrlServer.SetAcceptProcessEvent();
            }
        }

        public void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;
            try
            {
                // Retrieve the socket from the state object.  
                

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

               

            }
            catch (Exception e)
            {

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                Console.WriteLine(e.ToString());
            }
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            String strLog;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            SessionContext session = (SessionContext)ar.AsyncState;
            Socket socket = session._socket;
            try
            {
                // Read data from the client socket.
                int bytesRead = socket.EndReceive(ar);
                Console.WriteLine("ReadCallback {0} byte", bytesRead);
                if (bytesRead > 0)
                {
                    // Not all data received. Get more.  
                    socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(ReadCallback), session);

                }
                else
                {
                    if (socket != null)
                    {
                        Console.WriteLine("Close socket");
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                strLog = ex.StackTrace.ToString();
            }
        }
    }
}
