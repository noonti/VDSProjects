using MClavisRadarManageCtrl;
using MClavisRadarManageCtrl.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VDSDBHandler;
using VDSDBHandler.DBOperation;

namespace MClavisRadarController
{
   

    public partial class Form1 : Form
    {

        Socket radarServer;
        Socket radarClient;
        EndPoint localEndPoint;
        EndPoint remotePoint;
        byte[] buffer = new byte[8192];

        MClavisRadarManager mClavisRadarManager;
        private VDSLogger _Logger = new VDSLogger();

        FormAddUDPData _formAddUDPData = null;

        int serverPort = 45555;
        int clientPort = 45175;
         
        Timer _timer = null;

        List<RADAR_OBJECT_DATA> radarDataList = new List<RADAR_OBJECT_DATA>();
        List<MCLAVIS_MESSAGE> inverseMessageList = new List<MCLAVIS_MESSAGE>();

        int processIndex = 0;

        TrafficDataOperation db = new TrafficDataOperation(String.Format($"Server=127.0.0.1;Port=3306;Database=vdsdb;Uid=VDS;Pwd=1234;SSL Mode=None"));

        public Form1()
        {
            InitializeComponent();
            _Logger.SetManagerType(MANAGER_TYPE.VDS_CLIENT);
            Utility._addLog = _Logger.AddLog;

            _formAddUDPData = new FormAddUDPData(AddUDPData);

            radarDataList = db.GetRadarObjectDataList(new RADAR_OBJECT_DATA()
            { STATE = 9 }, out SP_RESULT spResult).ToList();

            foreach(var radarData in radarDataList)
            {
                inverseMessageList.Add(new MCLAVIS_MESSAGE()
                {
                    object_id = (byte)radarData.ID,
                    State = (byte)radarData.STATE,
                    Lane_Dir = (byte)radarData.DIRECTION,
                    Lane = (byte)radarData.LANE,
                    Velocity_Y = radarData.YY,
                    Velocity_X = radarData.XX ,
                    Range_Y = radarData.Y ,
                    Range_X = radarData.X ,
                    DETECT_TIME = radarData.DETECT_TIME,
                    processOutbreakYN = "N"

                });
            }
            VDSConfig.korExConfig = new VDSCommon.Config.KorExConfig();
            VDSConfig.korExConfig.inverseGapTime = 1;
            VDSConfig.korExConfig.inverseCheckDistance = 5;
            VDSConfig.korExConfig.inverseCheckTime = 5;

            VDSConfig.korExConfig.stopGapTime = 1;
            VDSConfig.korExConfig.stopGapDistance = 1;
            VDSConfig.korExConfig.stopCheckTime = 1;
            VDSConfig.korExConfig.stopMinTime = 3;
            

        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            byte[] test1 =
            {
                  0x30  ,0x01  ,0x00  ,0xCA  ,0xCB  ,0xCC  ,0xCD
                // ,0x05  ,0x00  ,0x08  ,0x33  ,0xC4  ,0x00  ,0x00  ,0x00  ,0x00  ,0x00  ,0x00
                // ,0x05  ,0x01  ,0x08  ,0xA5  ,0x38  ,0xFD  ,0xB4  ,0x22  ,0x50  ,0x03  ,0x00
                 ,0x05  ,0x4D  ,0x08  ,0xF5  ,0x4A  ,0x00  ,0x38  ,0xC8  ,0x03  ,0x60  ,0x09
                 ,0x05  ,0x22  ,0x08  ,0x48  ,0x45  ,0xFF  ,0x38  ,0xB7  ,0xF6  ,0xE0  ,0xD8
                // ,0x05  ,0x4D  ,0x08  ,0xF5  ,0x0A  ,0x00  ,0x38  ,0xC8  ,0x03  ,0xE0  ,0x6A
               //  ,0x05  ,0x22  ,0x08  ,0x48  ,0x45  ,0xFF  ,0x38  ,0xB7  ,0xF6  ,0xE0  ,0xD8
                 ,0x6C
                 ,0xEA  ,0xEB  ,0xEC  ,0xED
            };


            byte[] test2 =
            {
                0x30 ,0x01 ,0x00 ,0xCA ,0xCB ,0xCC ,0xCD ,
                0x05 ,0x00 ,0x08 ,0x36 ,0x59 ,0x00 ,0x14 ,0xFE ,0xFB ,0x10 ,0x00 ,
                0x05 ,0x01 ,0x08 ,0x01 ,0xE2 ,0xFF ,0xFE ,0x23 ,0xA2 ,0xC8 ,0x00 ,
                0x05 ,0x20 ,0x08 ,0x41 ,0x84 ,0x76 ,0x35 ,0xF7 ,0xE9 ,0xA0 ,0x53 ,
                0x82 ,
                0xEA ,0xEB ,0xEC ,0xED
            };


             

            //MClavisDataFrame data1 = new MClavisDataFrame();
            //data1.Deserialize(test1, 0);

            //byte[] num = { 0x05,0x04D, 0x08 ,0xF5 ,0x0A ,0x00 ,0x38 ,0xC8 ,0x03 ,0xE0 ,0x6A };
            //message.data = BitConverter.ToUInt64(, i);


            //data1.PrintMessageInfo(new MCLAVIS_MESSAGE()
            //{ msgId = new byte[2],
            //    DataSize = 8,
            //    //data = 0x6AE003C838000AF5
            //    data = 0xF54A0038C8036009
            //});



            if (rdgServer.Checked)
            {
                MakeServerUDPConnection(int.Parse(txtRemotePort.Text));
            }
            else
            {
                MakeUDPConnection(txtServerAddress.Text, int.Parse(txtRemotePort.Text), int.Parse(txtLocalPort.Text));

               
            }

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();

        }

        public int AddUDPData(String data)
        {
            if (lbxLog.Items.Count > 1000)
                lbxLog.Items.Clear();
            lbxLog.Items.Insert(0, data);
            return lbxLog.Items.Count;
        }


        public int MakeServerUDPConnection(int _remotePort)
        {
            radarServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            radarServer.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            serverPort = _remotePort;

            localEndPoint = new IPEndPoint(IPAddress.Any, serverPort);
            remotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), clientPort);

            radarServer.Bind(localEndPoint);

            //buffer = new byte[8192];
            radarServer.BeginReceiveFrom(buffer, 0, 8192, SocketFlags.None, ref remotePoint, new AsyncCallback(ReceiveCallback), buffer);

           


            return 1;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes("avogadro");
            if (rdgClient.Checked)
            {
                
                radarClient.SendTo(sendBuffer, sendBuffer.Length, SocketFlags.None, remotePoint);
            }
            else
            {
                radarServer.SendTo(sendBuffer, sendBuffer.Length, SocketFlags.None, remotePoint);
            }
        }

        /// <summary>
        /// UDP 클라이언트 소켓..
        /// </summary>
        /// <param name="_address"></param>
        /// <param name="_remotePort"></param>
        /// <param name="_localPort"></param>
        /// <returns></returns>
        public int MakeUDPConnection(String _address, int _remotePort, int _localPort)
        {
            radarClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            radarClient.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            clientPort = _localPort;
            serverPort = _remotePort;

            localEndPoint = new IPEndPoint(IPAddress.Any, clientPort);
            remotePoint = new IPEndPoint(IPAddress.Parse(_address), serverPort);
            radarClient.ReceiveBufferSize = 8192;
            radarClient.Bind(localEndPoint);

            
            //buffer = new byte[8192];
            radarClient.BeginReceiveFrom(buffer, 0, 8192, SocketFlags.None, ref remotePoint, new AsyncCallback(ReceiveCallback), buffer);

            return 1;
        }

        public void ReceiveCallback(IAsyncResult asyncResult)
        {
            int size = 0;
            try
            {
                size = radarClient.EndReceiveFrom(asyncResult, ref remotePoint);
                if(size > 0)
                {
                    byte[] receivedData;//= new byte[8192];
                    receivedData = (byte[])asyncResult.AsyncState;
                    String data;
                    if (rdgServer.Checked)
                    {
                        data = String.Format($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Server Mode: receive Size={size} bytes. Data={Utility.PrintHexaString(receivedData, size)}");
                        radarClient.SendTo(receivedData, size, SocketFlags.None, remotePoint);
                    }
                    else
                    {
                        data = String.Format($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Client Mode: receive Size={size} bytes. Data={Utility.PrintHexaString(receivedData, size)}");
                    }

                    BeginInvoke(_formAddUDPData, new object[] { data });
                    
                }

                // buffer = new byte[8192];
                radarClient.BeginReceiveFrom(buffer, 0, size, SocketFlags.None, ref remotePoint, new AsyncCallback(ReceiveCallback), buffer);
            }
            catch(Exception ex)
            {
                BeginInvoke(_formAddUDPData, new object[] { String.Format($"size={size}, {ex.StackTrace.ToString()}") });
                BeginInvoke(_formAddUDPData, new object[] { String.Format($"size={size}, {ex.Message.ToString()}") });
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _Logger.StartManager();
            mClavisRadarManager = new MClavisRadarManager();
            mClavisRadarManager.SetFormAddTargetInfoDelegate(this, new FormAddUDPData(AddUDPData));
            mClavisRadarManager.StartDevice(txtServerAddress.Text, int.Parse(txtRemotePort.Text), int.Parse(txtLocalPort.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (mClavisRadarManager != null)
                mClavisRadarManager.StopDevice();

            _Logger.StopManager();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //radarDataList
            mClavisRadarManager.ProcessRadarInverseDetect(inverseMessageList[processIndex++]);


        }

        private void button5_Click(object sender, EventArgs e)
        {
            mClavisRadarManager.ProcessInverseMessageList();
        }
    }
}
