using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VideoVDSManageCtrl;
using VideoVDSManageCtrl.Protocol;

namespace VideoVDSController
{
    public partial class Form1 : Form
    {

        VideoVDSManager _videoManager = new VideoVDSManager();
        private VDSLogger _Logger = new VDSLogger();

        SerialCom serialCom8 = new SerialCom();
        SerialCom serialCom9 = new SerialCom();
        public Form1()
        {
            InitializeComponent();
            _Logger.SetManagerType(MANAGER_TYPE.VDS_CLIENT);
            Utility._addLog = _Logger.AddLog;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _Logger.StartManager();
            _videoManager.StartDevice(txtEventIPAddress.Text, int.Parse(txtEventPortNo.Text));

           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _videoManager.StopDevice();
            _Logger.StopManager();
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {


            VDSAuthRequest request = new VDSAuthRequest();
            request.vdsControllerId = "avogadro";

            DataFrame data1 = new DataFrame();
            data1.RequestType = DataFrameDefine.TYPE_REQUEST;
            data1.OpCode = DataFrameDefine.OPCODE_AUTH_VDS;
            byte[] id = Utility.StringToByte("transactionID");
            Array.Copy(id, 0, data1.TransactionId, 0, id.Length);
            data1.opDataFrame = request;

            var packet = data1.Serialize();

            DataFrame data2 = new DataFrame();
            var request2 = data2.Deserialize(packet, 0);
        }

        //private int SendAuthVDSRequest(SessionContext session, String vdsId)
        //{
        //    int nResult = 0;
        //    VDSAuthRequest request = new VDSAuthRequest();
        //    DataFrame frame = new DataFrame();
        //    frame.RequestType = DataFrameDefine.TYPE_REQUEST;
        //    frame.OpCode = DataFrameDefine.OPCODE_AUTH_VDS;

        //    request.vdsControllerId = vdsId;
        //    frame.opDataFrame = request;

        //    byte[] packet = frame.Serialize();

        //    nResult = Send(session, packet);

        //    return nResult;

        //}

        private void darkButton2_Click(object sender, EventArgs e)
        {
            VDSAuthResponse response = new VDSAuthResponse();
            response.resultCode = 100;
            response.resultMessage = "success";
            response.rtspSourceURL = "rtspSourceURL";
            response.rtspDetectionURL = "rtspDetectionURL";

            DataFrame data1 = new DataFrame();
            data1.RequestType = DataFrameDefine.TYPE_RESPONSE;
            data1.OpCode = DataFrameDefine.OPCODE_AUTH_VDS;
            byte[] id = Utility.StringToByte("transactionID");
            Array.Copy(id, 0, data1.TransactionId, 0, id.Length);
            data1.opDataFrame = response;

            var packet = data1.Serialize();

            DataFrame data2 = new DataFrame();
            var request2 = data2.Deserialize(packet, 0);
        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            VDSHeartBeatRequest request = new VDSHeartBeatRequest();
            request.vdsControllerId = "avogadro";
            request.heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);

            DataFrame data1 = new DataFrame();
            data1.RequestType = DataFrameDefine.TYPE_REQUEST;
            data1.OpCode = DataFrameDefine.OPCODE_HEARTBEAT;
            byte[] id = Utility.StringToByte("transactionID");
            Array.Copy(id, 0, data1.TransactionId, 0, id.Length);
            data1.opDataFrame = request;

            var packet = data1.Serialize();

            DataFrame data2 = new DataFrame();
            data2.Deserialize(packet, 0);
        }

        private void darkButton4_Click(object sender, EventArgs e)
        {
            
        }

        private void darkButton5_Click(object sender, EventArgs e)
        {
            VDSTrafficDataEvent trafficDataEvent = new VDSTrafficDataEvent();
            trafficDataEvent.totalCount = 3;
            trafficDataEvent.trafficDataList.Add(
                new TrafficData()
                {
                    id = "1",
                    lane = 1,
                    direction = 2,
                    length = 1200,
                    speed = 100,
                    vehicle_class = 2,
                    occupyTime = 100,
                    loop1OccupyTime = 200,
                    loop2OccupyTime = 300,
                    reverseRunYN = "N",
                    vehicleGap = 12,
                    detectTime = "2021-06-06 14:38:45.33",


                });

            trafficDataEvent.trafficDataList.Add(
                new TrafficData()
                {
                    id = "2",
                    lane = 2,
                    direction = 2,
                    length = 1200,
                    speed = 100,
                    vehicle_class = 2,
                    occupyTime = 100,
                    loop1OccupyTime = 200,
                    loop2OccupyTime = 300,
                    reverseRunYN = "N",
                    vehicleGap = 12,
                    detectTime = "2021-06-06 14:38:45.33",


                });

            trafficDataEvent.trafficDataList.Add(
                new TrafficData()
                {
                    id = "3",
                    lane = 3,
                    direction = 2,
                    length = 1200,
                    speed = 100,
                    vehicle_class = 2,
                    occupyTime = 100,
                    loop1OccupyTime = 200,
                    loop2OccupyTime = 300,
                    reverseRunYN = "N",
                    vehicleGap = 12,
                    detectTime = "2021-06-06 14:38:45.33",


                });

            DataFrame data1 = new DataFrame();
            data1.RequestType = DataFrameDefine.TYPE_COMMAND;
            data1.OpCode = DataFrameDefine.OPCODE_EVENT_TRAFFIC;
            byte[] id = Utility.StringToByte("transactionID");
            Array.Copy(id, 0, data1.TransactionId, 0, id.Length);
            data1.opDataFrame = trafficDataEvent;

            var packet = data1.Serialize();

            DataFrame data2 = new DataFrame();
            data2.Deserialize(packet, 0);
        }

        private void darkButton6_Click(object sender, EventArgs e)
        {
            VDSHistoricTrafficDataRequest request = new VDSHistoricTrafficDataRequest();
            request.startDateTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT); ;
            request.endDateTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT); ;

            request.pageSize = 100;
            request.pageIndex = 1;


            DataFrame data1 = new DataFrame();
            data1.RequestType = DataFrameDefine.TYPE_REQUEST;
            data1.OpCode = DataFrameDefine.OPCODE_HISTORIC_TRAFFIC;

            byte[] id = Utility.StringToByte("transactionID");
            Array.Copy(id, 0, data1.TransactionId, 0, id.Length);
            data1.opDataFrame = request;

            var packet = data1.Serialize();

            DataFrame data2 = new DataFrame();
            data2.Deserialize(packet, 0);
        }

        private void darkButton7_Click(object sender, EventArgs e)
        {
            VDSHistoricTrafficDataResponse trafficDataResponse = new VDSHistoricTrafficDataResponse();
            trafficDataResponse.resultCode = 100;
            trafficDataResponse.resultMessage = "success";

            trafficDataResponse.startDateTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
            trafficDataResponse.endDateTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);

            trafficDataResponse.pageSize = 100;
            trafficDataResponse.pageIndex = 1;


            trafficDataResponse.totalCount = 1120;

            trafficDataResponse.trafficDataList.Add(
                new TrafficData()
                {
                    id = "1",
                    lane = 1,
                    direction = 2,
                    length = 1200,
                    speed = 100,
                    vehicle_class = 2,
                    occupyTime = 100,
                    loop1OccupyTime = 200,
                    loop2OccupyTime = 300,
                    reverseRunYN = "N",
                    vehicleGap = 12,
                    detectTime = "2021-06-06 14:38:45.33",


                });

            trafficDataResponse.trafficDataList.Add(
                new TrafficData()
                {
                    id = "2",
                    lane = 2,
                    direction = 2,
                    length = 1200,
                    speed = 100,
                    vehicle_class = 2,
                    occupyTime = 100,
                    loop1OccupyTime = 200,
                    loop2OccupyTime = 300,
                    reverseRunYN = "N",
                    vehicleGap = 12,
                    detectTime = "2021-06-06 14:38:45.33",


                });

            trafficDataResponse.trafficDataList.Add(
                new TrafficData()
                {
                    id = "3",
                    lane = 3,
                    direction = 2,
                    length = 1200,
                    speed = 100,
                    vehicle_class = 2,
                    occupyTime = 100,
                    loop1OccupyTime = 200,
                    loop2OccupyTime = 300,
                    reverseRunYN = "N",
                    vehicleGap = 12,
                    detectTime = "2021-06-06 14:38:45.33",


                });

            DataFrame data1 = new DataFrame();
            data1.RequestType = DataFrameDefine.TYPE_RESPONSE;
            data1.OpCode = DataFrameDefine.OPCODE_HISTORIC_TRAFFIC;
            byte[] id = Utility.StringToByte("transactionID");
            Array.Copy(id, 0, data1.TransactionId, 0, id.Length);
            data1.opDataFrame = trafficDataResponse;

            var packet = data1.Serialize();

            DataFrame data2 = new DataFrame();
            data2.Deserialize(packet, 0);
        }

        private void darkButton8_Click(object sender, EventArgs e)
        {
            serialCom8.Init("COM8", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.None, System.IO.Ports.Handshake.None, new SerialDataReceivedEventHandler(SerialReceivedData8) );
            serialCom8.Open();
        }

        private void SerialReceivedData8(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[VDSConfig.PACKET_SIZE];
            if (serialCom8.isOpened)
            {
                int readCount = serialCom8.serialPort.Read(buffer,0, VDSConfig.PACKET_SIZE);
                Console.WriteLine($"serial 8 receive...{readCount} bytes");
            }
        }

        private void darkButton9_Click(object sender, EventArgs e)
        {
            serialCom8.Close();
        }

        private void darkButton11_Click(object sender, EventArgs e)
        {
            serialCom9.Init("COM7", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.None, System.IO.Ports.Handshake.None, new SerialDataReceivedEventHandler(SerialReceivedData9));
            serialCom9.Open();
        }

        private void darkButton12_Click(object sender, EventArgs e)
        {
            serialCom8.Send(Utility.StringToByte("avogadro"));
        }

        private void SerialReceivedData9(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[VDSConfig.PACKET_SIZE];
            if (serialCom9.isOpened)
            {
                int readCount = serialCom9.serialPort.Read(buffer, 0, VDSConfig.PACKET_SIZE);
                byte[] data = new byte[readCount];
                Array.Copy(buffer, 0, data, 0,readCount);
                String str = Utility.ByteToString(data);


                Console.WriteLine($"{str}");
            }
        }

        private void darkButton13_Click(object sender, EventArgs e)
        {
            serialCom9.Send(Utility.StringToByte("avog"));
        }

        private void darkButton14_Click(object sender, EventArgs e)
        {
            SerialComManageCtrl.Protocol.SerialDataFrame rtuStatus = new SerialComManageCtrl.Protocol.SerialDataFrame();

            rtuStatus.MakeRTSStatusFrameResponse();
            byte[] packet = rtuStatus.Serialize();

            SerialComManageCtrl.Protocol.SerialDataFrame rtuStatus2 = new SerialComManageCtrl.Protocol.SerialDataFrame();
            int i = rtuStatus2.Deserialize(packet, 0);
        }
    }
}
