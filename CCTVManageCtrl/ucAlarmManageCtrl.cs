using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CCTVManageCtrl
{
    public partial class ucAlarmManageCtrl : UserControl
    {
        private String _address = String.Empty;
        private int _port = 8081;

        private bool m_bInitSDK = false;
        private Int32 m_lRealHandle = -1;
        private Int32 m_lUserID = -1;
        private Int32 m_lAlarmHandle = -1;

        private uint iLastErr = 0;

        private Int32 iListenHandle = -1;
        private CHCNetSDK.MSGCallBack_V31 m_falarmData = null;

        private bool initializeSDK()
        {
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK)
            {
                CHCNetSDK.NET_DVR_SetLogToFile(3, ".\\log\\", true);
                if (m_falarmData == null)
                {
                    m_falarmData = new CHCNetSDK.MSGCallBack_V31(MsgCallback);
                }
                CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(m_falarmData, IntPtr.Zero);


            }
            return m_bInitSDK;
        }

        private void releaseSDK()
        {
            if (m_lRealHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
                m_lRealHandle = -1;
            }

            //鬧饋되쩌 Logout the device
            if (m_lUserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
                m_lUserID = -1;
            }
            SetAlarmClose();
            CHCNetSDK.NET_DVR_Cleanup();
        }

        public ucAlarmManageCtrl()
        {
            InitializeComponent();

            if(initializeSDK())
                Disposed += OnDispose;
        }

        private void OnDispose(object sender, EventArgs e)
        {
            releaseSDK();
        }

        public bool CCTVLogin(String address, int port, String userId, String passwd)
        {
            bool result = false;
            try
            {
                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
                m_lUserID = CHCNetSDK.NET_DVR_Login_V30(address, port, userId, passwd, ref DeviceInfo);
                if (m_lUserID < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    return false;
                }
                else
                {
                    SetAlarmOpen();
                    result = true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());

            }

            return result;

        }
        

        public void PlayPreview()
        {
            if (m_lUserID >= 0)
            {
                if (m_lRealHandle < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo.hPlayWnd = CCTVPlayWindow.Handle;//渡응눗왯
                    lpPreviewInfo.lChannel = 1 ; //Int16.Parse(textBoxChannel.Text);//渡te응돨구繫돛
                    lpPreviewInfo.dwStreamType = 0;//쯤직잚謹：0-寮쯤직，1-綾쯤직，2-쯤직3，3-쯤직4，鹿늪잚股
                    lpPreviewInfo.dwLinkMode = 0;//젯쌈렘駕：0- TCP렘駕，1- UDP렘駕，2- 뜩꺄렘駕，3- RTP렘駕，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo.bBlocked = true; //0- 렷羸힘혤직，1- 羸힘혤직
                    //lpPreviewInfo.dwDisplayBufNum = 1; //꺄렴욋꺄렴뻠녑혐離댕뻠녑煉鑒
                    lpPreviewInfo.byProtoType = 0;
                    lpPreviewInfo.byPreviewMode = 0;

                    IntPtr pUser = new IntPtr();//痰빵鑒앴

                    //댔역渡응 Start live view 
                    m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                    if (m_lRealHandle < 0)
                    {
                        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        String str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //渡응呵겨，渴놔댄轎뵀
                        Console.WriteLine(str);
                        return;
                    }
                }
                else
                {
                    if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
                    {
                        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        String str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                        Console.WriteLine(str);
                        return;
                    }
                    m_lRealHandle = -1;
                }
                return;
            }
        }

        private void chkPreview_Click(object sender, EventArgs e)
        {
            PlayPreview();
        }

        public void SetAlarmAddress(String address, int port)
        {
            _address = address;
            _port = port;
        }

        //public bool StartAlarmListening(String address, int port)
        //{
        //    bool result = false;
        //    try
        //    {
        //        if (m_falarmData == null)
        //        {
        //            m_falarmData = new CHCNetSDK.MSGCallBack(MsgCallback);
        //        }

        //        iListenHandle = CHCNetSDK.NET_DVR_StartListen_V30(address, (ushort)port, m_falarmData, IntPtr.Zero);
        //        if (iListenHandle < 0)
        //        {
        //            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
        //            String strErr = "Failed to start listening, Error code:" + iLastErr; //撤防失败，输出错误号
        //            Console.WriteLine(strErr);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Start listening successfully！");
        //            result = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message.ToString());

        //    }

        //    return result;
        //}

        //public void StopAlarmListening()
        //{
        //    if (!CHCNetSDK.NET_DVR_StopListen_V30(iListenHandle))
        //    {
        //        iLastErr = CHCNetSDK.NET_DVR_GetLastError();
        //        String strErr = "Failed to stop listening, Error code:" + iLastErr; //撤防失败，输出错误号
        //        Console.WriteLine(strErr);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Stop listening successfully！");
        //    }
        //}

        public bool MsgCallback(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //수신된 경보 메시지 유형을 lCommand로 판단하는 데 서로 다른 lCommand 대응 pAlarmInfo 내용 通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            ProcessAlarmMessage(lCommand, ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
            return true;
        }

        public void ProcessAlarmMessage(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //수신된 경보 메시지 유형을 lCommand로 판단하는 데 서로 다른 lCommand 대응 pAlarmInfo 내용 通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            switch (lCommand)
            {
                case CHCNetSDK.COMM_ALARM: //(DS-8000 오래된 장비) 모바일 탐지, 비디오 분실, 차폐, IO 신호량 등 알람 메시지 (DS-8000老设备)移动侦测、视频丢失、遮挡、IO信号量等报警信息
                    AddAlarmInfo("CHCNetSDK.COMM_ALARM");
                    break;
                case CHCNetSDK.COMM_ALARM_V30://모바일 탐지, 비디오 분실, 차폐, IO 신호량 등 알람 메시지 移动侦测、视频丢失、遮挡、IO信号量等报警信息
                    AddAlarmInfo("CHCNetSDK.COMM_ALARM_V30");
                    break;
                case CHCNetSDK.COMM_ALARM_RULE://출입 구역, 침입, 배회, 인원집합 등의 행위 분석 알람 정보 进出区域、入侵、徘徊、人员聚集等行为分析报警信息
                    AddAlarmInfo("CHCNetSDK.COMM_ALARM_RULE");
                    ProcessCommAlarm_RULE(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_UPLOAD_PLATE_RESULT://교통 스냅샷 결과 업로드(오래된 경보 메시지 유형) 交通抓拍结果上传(老报警信息类型)
                    AddAlarmInfo("CHCNetSDK.COMM_UPLOAD_PLATE_RESULT");
                    break;
                case CHCNetSDK.COMM_ITS_PLATE_RESULT://교통 스냅샷 결과 업로드(오래된 경보 메시지 유형)交通抓拍结果上传(新报警信息类型)
                    AddAlarmInfo("CHCNetSDK.COMM_ITS_PLATE_RESULT");
                    break;
                case CHCNetSDK.COMM_ALARM_PDC://객체 트래픽 통계 경보 메시지 客流量统计报警信息
                    AddAlarmInfo( "CHCNetSDK.COMM_ALARM_PDC");
                    break;
                case CHCNetSDK.COMM_ITS_PARK_VEHICLE://객체 트래픽 통계 경보 메시지 客流量统计报警信息
                    AddAlarmInfo( "CHCNetSDK.COMM_ITS_PARK_VEHICLE");
                    break;
                case CHCNetSDK.COMM_DIAGNOSIS_UPLOAD://VQD 경보 메시지 VQD报警信息
                    AddAlarmInfo( "CHCNetSDK.COMM_DIAGNOSIS_UPLOAD");
                    break;
                case CHCNetSDK.COMM_UPLOAD_FACESNAP_RESULT://사람 얼굴 캡처 결과 정보 人脸抓拍结果信息
                    AddAlarmInfo( "CHCNetSDK.COMM_UPLOAD_FACESNAP_RESULT");
                    break;
                case CHCNetSDK.COMM_SNAP_MATCH_ALARM://사람 얼굴 대 결과 정보 人脸比对结果信息
                    AddAlarmInfo( "CHCNetSDK.COMM_SNAP_MATCH_ALARM");
                    break;
                case CHCNetSDK.COMM_ALARM_FACE_DETECTION://사람의 얼굴에서 경보 메시지를 탐지하다. 人脸侦测报警信息
                    AddAlarmInfo( "CHCNetSDK.COMM_ALARM_FACE_DETECTION");
                    break;
                case CHCNetSDK.COMM_ALARMHOST_CID_ALARM://경보 호스트 CID 경보 업로드 报警主机CID报警上传
                    AddAlarmInfo( "CHCNetSDK.COMM_ALARMHOST_CID_ALARM");
                    break;
                case CHCNetSDK.COMM_ALARM_ACS://게이트 금지 호스트 경보 업로드 门禁主机报警上传
                    AddAlarmInfo( "CHCNetSDK.COMM_ALARM_ACS");
                    break;
                case CHCNetSDK.COMM_ID_INFO_ALARM://신분증 카드 정보 업로드 身份证刷卡信息上传
                    AddAlarmInfo( "CHCNetSDK.COMM_ID_INFO_ALARM");
                    break;
                //default:
                //    {
                //        Console.WriteLine("ProcessAlarmMessage..default");
                //        //경보 디바이스 IP 주소 报警设备IP地址
                //        string strIP = pAlarmer.sDeviceIP;

                //        //경보 메시지 유형 报警信息类型
                //        string stringAlarm = "upload alarm，alarm message type：" + lCommand;

                //        if (InvokeRequired)
                //        {
                //            object[] paras = new object[3];
                //            paras[0] = DateTime.Now.ToString(); //현재 PC 시스템 시간 当前PC系统时间
                //            paras[1] = strIP;
                //            paras[2] = stringAlarm;
                //            //listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
                //        }
                //        else
                //        {
                //            //이 컨트롤의 마스터 노드 직접 업데이트 정보 목록 만들기 创建该控件的主线程直接更新信息列表 
                //            //UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
                //        }
                //    }
                    //break;
            }
        }

        private void ProcessCommAlarm_RULE(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_VCA_RULE_ALARM struRuleAlarmInfo = new CHCNetSDK.NET_VCA_RULE_ALARM();
            struRuleAlarmInfo = (CHCNetSDK.NET_VCA_RULE_ALARM)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_VCA_RULE_ALARM));

            //경보 메시지 报警信息
            string stringAlarm = "";
            uint dwSize = (uint)Marshal.SizeOf(struRuleAlarmInfo.struRuleInfo.uEventParam);


            //switch (struRuleAlarmInfo.struRuleInfo.wEventTypeEx)
            //{
            //    case (ushort)CHCNetSDK.VCA_RULE_EVENT_TYPE_EX.ENUM_VCA_EVENT_TRAVERSE_PLANE:
            //        IntPtr ptrTraverseInfo = Marshal.AllocHGlobal((Int32)dwSize);
            //        Marshal.StructureToPtr(struRuleAlarmInfo.struRuleInfo.uEventParam, ptrTraverseInfo, false);
            //        m_struTraversePlane = (CHCNetSDK.NET_VCA_TRAVERSE_PLANE)Marshal.PtrToStructure(ptrTraverseInfo, typeof(CHCNetSDK.NET_VCA_TRAVERSE_PLANE));
            //        stringAlarm = "Line crossing，Object ID：" + struRuleAlarmInfo.struTargetInfo.dwID;
            //        //경계면 사이드라인 시작점 좌표 警戒面边线起点坐标: (m_struTraversePlane.struPlaneBottom.struStart.fX, m_struTraversePlane.struPlaneBottom.struStart.fY)
            //        //경계면 사이드라인 시작점 좌표 警戒面边线终点坐标: (m_struTraversePlane.struPlaneBottom.struEnd.fX, m_struTraversePlane.struPlaneBottom.struEnd.fY)
            //        break;
            //    case (ushort)CHCNetSDK.VCA_RULE_EVENT_TYPE_EX.ENUM_VCA_EVENT_ENTER_AREA:
            //        IntPtr ptrEnterInfo = Marshal.AllocHGlobal((Int32)dwSize);
            //        Marshal.StructureToPtr(struRuleAlarmInfo.struRuleInfo.uEventParam, ptrEnterInfo, false);
            //        m_struVcaArea = (CHCNetSDK.NET_VCA_AREA)Marshal.PtrToStructure(ptrEnterInfo, typeof(CHCNetSDK.NET_VCA_AREA));
            //        stringAlarm = "Target entering area，Object ID：" + struRuleAlarmInfo.struTargetInfo.dwID + " struRuleAlarmInfo.struRuleInfo.byRuleID: " + struRuleAlarmInfo.struRuleInfo.byRuleID;
            //        //m_struVcaArea.struRegion 다각형 영역 좌표 多边形区域坐标
            //        break;
            //    case (ushort)CHCNetSDK.VCA_RULE_EVENT_TYPE_EX.ENUM_VCA_EVENT_EXIT_AREA:
            //        IntPtr ptrExitInfo = Marshal.AllocHGlobal((Int32)dwSize);
            //        Marshal.StructureToPtr(struRuleAlarmInfo.struRuleInfo.uEventParam, ptrExitInfo, false);
            //        m_struVcaArea = (CHCNetSDK.NET_VCA_AREA)Marshal.PtrToStructure(ptrExitInfo, typeof(CHCNetSDK.NET_VCA_AREA));
            //        stringAlarm = "Target leaving area，Object ID：" + struRuleAlarmInfo.struTargetInfo.dwID;
            //        //m_struVcaArea.struRegion 다각형 영역 좌표 多边形区域坐标
            //        break;
            //    case (ushort)CHCNetSDK.VCA_RULE_EVENT_TYPE_EX.ENUM_VCA_EVENT_INTRUSION:
            //        //IntPtr ptrIntrusionInfo = Marshal.AllocHGlobal((Int32)dwSize);
            //        //Marshal.StructureToPtr(struRuleAlarmInfo.struRuleInfo.uEventParam, ptrIntrusionInfo, false);
            //        //m_struIntrusion = (CHCNetSDK.NET_VCA_INTRUSION)Marshal.PtrToStructure(ptrIntrusionInfo, typeof(CHCNetSDK.NET_VCA_INTRUSION));

            //        //int i = 0;
            //        //string strRegion = "";
            //        //for (i = 0; i < m_struIntrusion.struRegion.dwPointNum; i++)
            //        //{
            //        //    strRegion = strRegion + "(" + m_struIntrusion.struRegion.struPos[i].fX + "," + m_struIntrusion.struRegion.struPos[i].fY + ")";
            //        //}
            //        stringAlarm = "Intrusion detection，Object ID：" + struRuleAlarmInfo.struTargetInfo.dwID + "，Region range：" + strRegion;
            //        //m_struIntrusion.struRegion 다각형 영역 좌표 多边形区域坐标
            //        break;
            //    default:
            //        stringAlarm = "other behaviour analysis alarm，Object ID：" + struRuleAlarmInfo.struTargetInfo.dwID;
            //        break;
            //}

            //알람 시간: 연월일 시 분초 报警时间：年月日时分秒
            string strTimeYear = ((struRuleAlarmInfo.dwAbsTime >> 26) + 2000).ToString();
            string strTimeMonth = ((struRuleAlarmInfo.dwAbsTime >> 22) & 15).ToString("d2");
            string strTimeDay = ((struRuleAlarmInfo.dwAbsTime >> 17) & 31).ToString("d2");
            string strTimeHour = ((struRuleAlarmInfo.dwAbsTime >> 12) & 31).ToString("d2");
            string strTimeMinute = ((struRuleAlarmInfo.dwAbsTime >> 6) & 63).ToString("d2");
            string strTimeSecond = ((struRuleAlarmInfo.dwAbsTime >> 0) & 63).ToString("d2");
            string strTime = strTimeYear + "-" + strTimeMonth + "-" + strTimeDay + " " + strTimeHour + ":" + strTimeMinute + ":" + strTimeSecond;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetAlarmOpen();
            //StartAlarmListening(_address, _port);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetAlarmClose();
            //StopAlarmListening();
        }



        public void SetAlarmOpen()
        {

            CHCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam = new CHCNetSDK.NET_DVR_SETUPALARM_PARAM();
            struAlarmParam.dwSize = (uint)Marshal.SizeOf(struAlarmParam);
            struAlarmParam.byLevel = 1; //0- 1단계 방어, 1- 2단계 방어 0- 一级布防,1- 二级布防
            struAlarmParam.byAlarmInfoType = 1;//스마트 교통 장비 유효, 새 경보 메시지 유형 智能交通设备有效，新报警信息类型
            struAlarmParam.byFaceAlarmDetection = 1;//1-얼굴 조사 1-人脸侦测

            m_lAlarmHandle = CHCNetSDK.NET_DVR_SetupAlarmChan_V41(m_lUserID, ref struAlarmParam);
            if (m_lAlarmHandle < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                String strErr = "Failed to arm, Error code:" + iLastErr; //패브릭 실패, 출력 오류 신호 布防失败，输出错误号
                Console.WriteLine(strErr);
            }
            else
            {
                Console.WriteLine("Arm successfully");
            }

        }
        public void SetAlarmClose()
        {
            if (m_lAlarmHandle >= 0)
            {
                if (!CHCNetSDK.NET_DVR_CloseAlarmChan_V30(m_lAlarmHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    String strErr = "Failed to disarm, Error code:" + iLastErr; //디텍터 제거 실패, 잘못된 신호 출력 撤防失败，输出错误号
                    Console.WriteLine(strErr);
                }
                else
                {
                    Console.WriteLine("Disarmed");
                    m_lAlarmHandle = -1;
                }
            }

        }

        private void AddAlarmInfo(String info)
        {
            AddAlarmInfo( info);
        }
    }
}
