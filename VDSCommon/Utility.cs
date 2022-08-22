using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using VDSCommon.DataType;
using System.Runtime.InteropServices;
using VDSCommon.Protocol.admin;
using RestSharp;
using Newtonsoft.Json;
using VDSCommon.VitualKeyboard;
using System.Windows.Forms;
using System.Drawing;
using VDSCommon.API.Model;
using System.Reflection;

namespace VDSCommon
{

    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

   


    public delegate int AddLogDelegate(LOG_TYPE type, String _log);
    //public delegate int AddRadarData(RadarData radarData);
    public delegate int AddTrafficDataEvent(TrafficDataEvent trafficData);
    public delegate int FormAddLogDelegate(LOG_TYPE logType, String strLog);


    public delegate int AddMADataEvent(SessionContext session, MADataFrame frame);

    public delegate int ConnectCallback(SessionContext session, SOCKET_STATUS status);

    public delegate int FormAddTargetInfoDelegate(TrafficDataEvent target);
    public delegate int FormAddCommuDataDelegate(CommuData commuData);

    public delegate int ProcessRadarCallbackDelegate(Object[] _params);

    public delegate int SetRtspStreamingUrlDelegate(String[] rtspURL);

    public delegate int StartRTSPStreamingDelegate();

    public delegate int FormAddUDPData(String data);

    public enum SOCKET_STATUS
    {
        DISCONNECTED = 1,
        CONNECTING = 2,
        CONNECTED = 3,
        AUTHORIZED = 4,
        UNAUTHORIZE = 5
    };

    public enum CLIENT_TYPE
    {
        NONE = 0,
        VDS_CLIENT = 1,
        KICT_EVNT_CLIENT = 2, // KICT event   제어기->센터(10000)
        KICT_CTRL_CLIENT = 3, // Control Client 센터->제어기(12000)
        KICT_CLIB_CLIENT = 4 , // Calibration Client  센터->제어기(12000)
        VIDEO_VDS_CLIENT = 5

    }

    public enum REMOTE_CENTER_TYPE
    {
        CENTER_KICT = 1, // 안산센터
        CENTER_KOREX = 2 // 도로공사
    }

    public struct REPORT_INFO
    {
        public List<String> ids;
        public String REPORT_YN;
    }

    public struct SOCKET_MSG
    {
        public SessionContext session;
        public byte[] packet;
        public int size;
    }

    

    //LENGTH_CATEGORY[0] = 8 ; // 80 dm --> 8 m small
    //LENGTH_CATEGORY[1] = 12; // 120 dm --> 12 m mid 
    //LENGTH_CATEGORY[2] = 15; // 150 dm --> 15 m big

    public enum VEHICLE_LENGTH_CATEGORY
    {
        CATEGORY_SMALL = 0,
        CATEGORY_MIDDLE = 1,
        CATEGORY_LARGE = 2
    }


    public enum MOVE_DIRECTION
    {
        TO_RIGHT = 1, // 왼쪽에서 오른쪽으로 이동(<---)
        TO_LEFT = 2   // 오른쪽에서 왼쪽으로 이동(--->)
    }

    public struct CommuData
    {
        public SessionContext session;
        public int ProtocolType; // 1:ITS 2: korEx
        public int OpCode;
        public object data;
    }

    public static class Utility
    {
        [DllImport("kernel32.dll")]
        public extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        public static AddLogDelegate _addLog = null;

        public static Int32 msgId = 0;


        public static VirtualKeybardForm vkForm;

        public static int AddLog(LOG_TYPE logType, String strLog)
        {
            int nResult = 0;
            if (_addLog != null)
            {
                _addLog(logType, strLog);
                nResult = 1;
            }
            return nResult;
        }


        public static String GetApplicationPath()
        {
            String result = String.Empty;
            try
            {

                result = System.Windows.Forms.Application.StartupPath;
            }
            catch (Exception ex)
            {
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return result;

        }

        public static String GetLogPath()
        {
            String result = String.Empty;
            try
            {
                result = System.IO.Path.Combine(GetApplicationPath(), "log");
            }
            catch(Exception ex)
            {
                result = String.Empty;
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return result;
        }


        public static bool IsConnected(Socket socket)
        {
            try
            {
                //return !(socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0);
                return !((socket.Poll(1000*3, SelectMode.SelectRead) && (socket.Available == 0)) );//SendTrafficData
            }
            catch (SocketException ex)
            {
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

                return false;
            }
        }

        public static String ByteToString(byte[] strByte)
        {
            String result = Encoding.Default.GetString(strByte);
            return result;
        }

        public static byte[] StringToByte(String str)
        {
            byte[] result = Encoding.UTF8.GetBytes(str);
            return result;
        }


        // Big Endian을 little엔디안으로 return UInt32
        public static UInt16 toLittleEndianInt16(byte [] bigNumber)
        {
            byte[] temp = new byte[2];
            bigNumber.CopyTo(temp, 0);
            Array.Reverse(temp);
            return BitConverter.ToUInt16(temp, 0);
        }

        


        // big엔디안으로 return Byte[]
        public static Byte[] toBigEndianInt16(UInt16 number)
        {
            byte[] temp = new byte[2];
            temp = BitConverter.GetBytes(number);
            Array.Reverse(temp);
            return temp;
        }


        // Big Endian을 little엔디안으로 return UInt32
        public static UInt32 toLittleEndianInt32(byte[] bigNumber)
        {
            byte[] temp = new byte[4];
            bigNumber.CopyTo(temp, 0);
            Array.Reverse(temp);
            return BitConverter.ToUInt32(temp, 0);
        }


        // Big Endian을 little엔디안으로 return UInt32
        public static UInt64 toLittleEndianInt64(byte[] bigNumber)
        {
            byte[] temp = new byte[8];
            bigNumber.CopyTo(temp, 0);
            Array.Reverse(temp);
            return BitConverter.ToUInt64(temp, 0);
        }

        // big엔디안으로 return Byte[]
        public static Byte[] toBigEndianInt32(UInt32 number)
        {
            byte[] temp = new byte[4];
            temp = BitConverter.GetBytes(number);
            Array.Reverse(temp);
            return temp;
        }


        public static byte[] GetDateTime(DateTime time)
        {
            byte[] result = new byte[8];
            int idx = 0;

            //0 1 year
            toBigEndianInt16((UInt16)time.Year).CopyTo(result, idx);
            idx += 2;
            //2 month
            result[idx++] = (byte)time.Month;
            //3 day
            result[idx++] = (byte)time.Day;
            //4 hour
            result[idx++] = (byte)time.Hour;
            result[idx++] = (byte)time.Minute;
            result[idx++] = (byte)time.Second;
            result[idx++] = (byte)(time.Millisecond / 10);
            return result;
        }

        //public static DateTime GetDateTime(byte[] time)
        //{
        //    String strDate = String.Empty;
        //    byte[] year = new byte[2];
        //    DateTime result = new DateTime();
        //    try
        //    {
        //        Array.Copy(time, 0, year, 0, 2);
        //        strDate = String.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}", toLittleEndianInt16(year), time[2], time[3], time[4], time[5], time[6], time[7]);
        //        result = DateTime.ParseExact(strDate, "yyyyMMddHHmmssff", CultureInfo.InvariantCulture);
        //    }
        //    catch(Exception ex)
        //    {
        //        AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
        //    }
        //    return result;
        //}


        public static bool Connect(ref Socket socket, String address, int port, int timeout)
        {
            //IPAddress ipAddress = IPAddress.Parse(address);// ipHostInfo.AddressList[0];
            //IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            bool bResult = false;
            try
            {
                IPAddress ipAddress = IPAddress.Parse(address);// ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                socket.Blocking = false;
                socket.Connect(remoteEP);
                bResult = true;

            }
            catch (SocketException ex)
            {

                if (ex.SocketErrorCode == SocketError.WouldBlock)
                {
                    ArrayList socketList = new ArrayList();
                    socketList.Add(socket);
                    Socket.Select(null, socketList, null, timeout * 1000*1000);
                    if (socketList.Count == 0)
                    {
                        //Trace.WriteLine(ex.ToString());
                        AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                        return false;
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                return false;
            }
            return bResult;
        }

        public static double GetOccupyTime(double velocity) // 단위: km/h
        {
            double Result = 0;
            try
            {
                Result = VDSConfig.controllerConfig.CheckDistance * 3600*1000 / ( velocity*1000); // 시간: 밀리세컨드, 거리:m 
                // 3600 : velocity = 
            }
            catch(Exception ex)
            {
                Result = 0;
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;
        }

        public static byte[] DateToByte(String strDate)
        {
            byte[] Result = null;
            DateTime date;
            try
            {
                if (!String.IsNullOrEmpty(strDate))
                {
                    date = DateTime.ParseExact(strDate, VDSConfig.RADAR_TIME_FORMAT, null);
                    Result = DateToByte(date);
                }
                else
                    Result = new byte[8];
            }
            catch (Exception ex)
            {
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;
        }
        public static byte[] DateToByte(DateTime date)
        {
            byte[] Result = null;
            int index = 0;
            try
            {
                Result = new byte[8];
                Result[index++] = ByteToBCD((byte)(date.Year / 100));
                Result[index++] = ByteToBCD((byte)(date.Year % 100));
                Result[index++] = ByteToBCD((byte)date.Month);
                Result[index++] = ByteToBCD((byte)date.Day);
                Result[index++] = ByteToBCD((byte)date.Hour);
                Result[index++] = ByteToBCD((byte)date.Minute);
                Result[index++] = ByteToBCD((byte)date.Second);
                //Result[index++] = ByteToBCD((byte)(date.Millisecond/100));
                Result[index++] = ByteToBCD((byte)((date.Millisecond / 10)%100));
                //time->wMilliseconds / 10)% 100

                Console.WriteLine($"DateToByte ..date.Year / 100 = {date.Year / 100}  date.Year % 100={date.Year % 100}, date.Month={date.Month} , date.Day={date.Day} ,date.Hour={date.Hour} ,date.Minute={date.Minute}, date.Second={date.Second}, (date.Millisecond / 10)%100={(date.Millisecond / 10) % 100}");
            }
            catch(Exception ex)
            {
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;
        }

        public static DateTime? ByteToDate(byte[] date)
        {
            DateTime? Result = null ;
            String strDate;
            int index = 0;
            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            int mili = 0;
            try
            {
                year = (int)BCDToByte(date[index++])*100 ;
                year += (int)BCDToByte(date[index++]);

                month = (int)BCDToByte(date[index++]);
                day = (int)BCDToByte(date[index++]);

                hour = (int)BCDToByte(date[index++]);
                minute = (int)BCDToByte(date[index++]);
                second = (int)BCDToByte(date[index++]);

                mili = (int)BCDToByte(date[index++]);
                if (mili >= 100 && mili < 1000)
                    mili = mili * 10;
                else if (mili >= 1000)
                    mili = mili / 100;

                strDate = String.Format("{0}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}.{6:D2}",year,month,day, hour,minute,second,mili);
                Result = DateTime.ParseExact(strDate, VDSConfig.RADAR_TIME_FORMAT , null);

            }
            catch (Exception ex)
            {
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;

        }
       
        public static byte ByteToBCD(byte value)
        {
            byte Result = 0;
            try
            {
                Result = (byte)( ((value / 10) << 4) + value % 10) ;

            }
            catch(Exception ex)
            {
                Result = 0;
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;
        }

        public static byte BCDToByte(byte value)
        {
            byte Result = 0;
            try
            {
                Result = (byte) ((value & 15) + (value >> 4) * 10);
            }
            catch (Exception ex)
            {
                Result = 0;
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;
        }

        public static byte[] BCDToBytes(byte[] values)
        {
            byte[] result = new byte[values.Length];
            int i = 0;
            foreach (var value in values)
            {
                result[i++] = BCDToByte(value);
            }
            return result;
        }

        public static byte[] SplitVelocity(double velocity) // km/h 
        {
            byte[] Result = new byte[2];
            try
            {
                double value = Math.Round(velocity, 3); // 2자리까지 표현
                int fraction = (int)Math.Truncate(value); // 정수 부분
                int remain = (int) ((value - fraction) * 100); // 소수 부분

                Result[0] = (byte)fraction;
                Result[1] = (byte)remain;

                
            }
            catch (Exception ex)
            {
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;
        }

        public static double MergeVelocity(byte[] velocity)
        {
            double Result = 0;
            try
            {
                if (velocity.Length == 2)
                {
                    Result = velocity[0] + ((double)velocity[1] / 100);
                }
            }
            catch (Exception ex)
            {
                Result = 0;
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return Result;
        }

        

        public static String MakeCSVData(List<String> idList)
        {
            String Result = String.Empty; // mili second 로 변환
            bool bFirst = false;
            try
            {
                foreach(var id in idList)
                {
                    if(!bFirst)
                    {
                        Result = id;
                        bFirst = true;
                    }
                    else
                    {
                        Result += "," + id;

                    }
                }

            }
            catch (Exception ex)
            {
                Result = String.Empty;
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return Result;
        }

        public static String PrintHexaString(byte [] data, int length)
        {
            String Result = String.Empty;
            for(int i=0;i<length;i++)
            {
                Result += String.Format("0x{0:X2} ", data[i]);
            }
            return Result;
        }

        public static String PrintOPCodeName(byte opCode)
        {
            //public const byte OP_FRAME_INITIAL_REQUEST = 0xA1;
            //public const byte OP_FRAME_INITIAL_RESPONSE = 0xA2;
            //public const byte OP_FRAME_RE_REQUEST = 0xA3;
            //public const byte OP_FRAME_RE_RESPONSE = 0xA4;

            String Result = String.Empty;

            switch (opCode)
            {
                case 0xA1:
                    Result = "정보 요청 프레임";
                    break;
                case 0xA2:
                    Result = "정보 응답 프레임";
                    break;
                case 0xA3:
                    Result = "정보 재전송 요청 프레임";
                    break;
                case 0xA4:
                    Result = "정보 재전송 응답 프레임";
                    break;
            }
            return Result;
        }

        public static UInt32 GetUTCSecond(int timezone)
        {
            UInt32 result = (UInt32)(DateTimeOffset.Now.ToUnixTimeSeconds() + 3600*timezone);
            return result;
        }

        public static UInt32 GetUTCSecond(DateTime date)
        {
            DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = (date - Epoch);
            return (UInt32)span.TotalSeconds;

        }

        public static byte[] IPAddressToKorExFormat(String ip)
        {
            byte[] Result = new byte[16];
            String[] values;
            int num;
            String temp = String.Empty;
            try
            {
                if(ip.Contains(".")) // IPv4
                {
                    values = ip.Split('.');
                    for(int i=0;i<values.Length;i++)
                    {
                        //172
                        temp += String.Format("{0:D3}",int.Parse(values[i]));
                        if (i < values.Length - 1)
                            temp += ".";
                        else
                            temp += "-";
                    }

                    if (!String.IsNullOrWhiteSpace(temp))
                    {
                        Result = StringToByte(temp);
                    }
                }
                else if(ip.Contains(":")) // IPv6
                {
                    values = ip.Split(':');
                    int startIndex = 0;
                    for (int i = 0; i < values.Length; i++)
                    {
                        num = Convert.ToInt32(values[i], 16);

                        Array.Copy(toBigEndianInt16((UInt16)num),0,Result, startIndex,2);
                        startIndex += 2;
                        
                    }
                }

                
            }
            catch (Exception ex)
            {
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;
        }

       

        public static byte GetLocalFrameNo()
        {
            byte result = 0x00;
            //30초당 1씩 
            result = (byte)((DateTime.Now.Minute * 60 + DateTime.Now.Second)/VDSConfig.korExConfig.localPollingPeriod + 1);
            return result;
        }

        public static Byte[] SplitNumber(double num)
        {
            byte[] result = new byte[2];
            byte intPart = 0;

            // 0: 정수 부분
            intPart = (byte)Math.Truncate(num);
            result[0] = intPart;
            // 1: 소수 부분
            result[1] = (byte)Math.Truncate((num - Math.Truncate(num)) * 100);

            return result;
        }

        //LENGTH_CATEGORY[0] = 8 ; // 80 dm --> 8 m small
        //LENGTH_CATEGORY[1] = 12; // 120 dm --> 12 m mid 
        //LENGTH_CATEGORY[2] = 15; // 150 dm --> 15 m big

        public enum VEHICLE_LENGTH_CATEGORY
        {
            CATEGORY_SMALL = 0,
            CATEGORY_MIDDLE = 1,
            CATEGORY_LARGE = 2
        }

        public static VEHICLE_LENGTH_CATEGORY GetVehicleLengthCategory(double length) //단위 : m
        {
            VEHICLE_LENGTH_CATEGORY result = VEHICLE_LENGTH_CATEGORY.CATEGORY_SMALL;

            if(length <= VDSConfig.LENGTH_CATEGORY[0]/10)
                result = VEHICLE_LENGTH_CATEGORY.CATEGORY_SMALL;
            else if(length <= VDSConfig.LENGTH_CATEGORY[1]/10)
                result = VEHICLE_LENGTH_CATEGORY.CATEGORY_MIDDLE;
            else
                result = VEHICLE_LENGTH_CATEGORY.CATEGORY_LARGE;

            return result;

        }

        public static int GetTotalPageCount(int totalCount, int pageSize)
        {
            int Result = 1;

            if (totalCount % pageSize == 0)
                Result = (int)(totalCount / pageSize);
            else
                Result = (int)(totalCount / pageSize) + 1;

            if (Result == 0)
                Result = 1;

            return Result;
        }

        public static byte GetLRCCode(byte[] packet, int startIndex, int endIndex)
        {
            byte result = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                result = (byte)((result + packet[i]) & 0xFF);

            }
            result = (byte)(((result ^ 0xFF) + 1) & 0xFF);
            return result;
        }

        public static int SetOsTime(DateTime date)
        {
            int nResult = 0;
            SYSTEMTIME systemTime;
            try
            {
                DateTime localTime = date.AddHours(-9);

                // 컴퓨터의 시간을 변경한다.
                systemTime = new SYSTEMTIME();

                systemTime.wYear = (ushort)localTime.Year;
                systemTime.wMonth = (ushort)localTime.Month;
                systemTime.wDay = (ushort)localTime.Day;
                systemTime.wHour = (ushort)localTime.Hour;
                systemTime.wMinute = (ushort)localTime.Minute;
                systemTime.wSecond = (ushort)localTime.Second;
                systemTime.wMilliseconds = (ushort)date.Millisecond;

                if (SetSystemTime(ref systemTime) != 0)
                    nResult = 1;

            }
            catch(Exception ex)
            {
                AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }


        public static byte GetThresholdToByte(int threshold)
        {
            byte result = 0;
            if (threshold < 0)
            {
                result = (byte)(0x80 + (byte)(-1 * threshold));
            }
            else
            {
                result = (byte)threshold;
            }
            return result;
        }

        public static int GetThresholdToInt(byte threshold)
        {
            int result = 0;
            //1000 0000
            byte signed = (byte)((threshold & 0x80) >> 7);
            result = (threshold & 0x7F);
            if (signed == 1)
                result = -1 * result;
            return result;
        }

        /// <summary>
        /// speed : km/h length: m distance : m
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="length"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static int GetOccupyTime(double speed, double length, double distance) // milisecond 
        {
            int result = 0;
            double occupyTime = 0;
            if (speed != 0)
            {
                // distance + length 거리를 가는 시간 계산
                double totalDistance = (distance + length) / 1000; // m ->  km 단위
                occupyTime = ((3600 * totalDistance / speed)) * 1000; // 시간 --> 밀리세컨드 단위
                result = (int)occupyTime;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] GetTransactionId()
        {
            byte[] result = new byte[8];
            Array.Copy(toBigEndianInt32(GetUTCSecond(DateTime.Now)), 0, result, 0, 4);
            Array.Copy(toBigEndianInt32((UInt32)msgId++), 0, result, 4, 4);
            if (msgId > 0x7FFFFFFF)
                msgId = 0;
            return result;
        }


        public static int GetSpeedCategory(double speed) //km/h
        {
            int result = 0;

            if (speed < VDSConfig.SPEED_CATEGORY[0])
                result = 1;
            else if (speed < VDSConfig.SPEED_CATEGORY[1])
                result = 2;
            else if (speed < VDSConfig.SPEED_CATEGORY[2])
                result = 3;
            else if (speed < VDSConfig.SPEED_CATEGORY[3])
                result = 4;
            else if (speed < VDSConfig.SPEED_CATEGORY[4])
                result = 5;
            else if (speed < VDSConfig.SPEED_CATEGORY[5])
                result = 6;
            else if (speed < VDSConfig.SPEED_CATEGORY[6])
                result = 7;
            else if (speed < VDSConfig.SPEED_CATEGORY[7])
                result = 8;
            else if (speed < VDSConfig.SPEED_CATEGORY[8])
                result = 9;
            else if (speed < VDSConfig.SPEED_CATEGORY[9])
                result = 10;
            else if (speed < VDSConfig.SPEED_CATEGORY[10])
                result = 11;
            else 
                result = 12;
            
            return result;
        }

        public static int GetResultCode(String resultCode)
        {
            int result = 100;
            if(!int.TryParse(resultCode, out result))
            {
                result = 500;
            }
            return result;
        }

        public static HttpStatusCode RequestWebAPI(String url,  object data, out String result)
        {
            result = String.Empty;
            String jsonData = JsonConvert.SerializeObject(data);
            var client = new RestClient(url);
            client.Timeout = 1000 * 5;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = response.Content;
            }
            return response.StatusCode;
        }


        public static void ShowVirtualKeyborad(Control control, Form parent)
        {

            if (vkForm == null)
            {
                vkForm = new VirtualKeybardForm();
            }
            vkForm.SetTargetControl(control, parent);
            vkForm.Show();
        }

        public static void HideVirtualKeyboard()
        {
            if (vkForm != null)
            {
                vkForm.SetTargetControl(null, null);
                vkForm.Close();
                vkForm = null;
            }
        }

        public static Point GetScreenPosition(Control control, Control parent)
        {
            Point result;
            int left = control.Left;
            int top = control.Top;
            Control currrent = control;
            
            while (parent!=null && currrent.Parent!=null && currrent.Parent!=parent)
            {
                left += currrent.Parent.Left;
                top += currrent.Parent.Top;

                currrent = currrent.Parent;
            }

            //left += currrent.Left;
            //top += currrent.Top;

            result = parent.PointToScreen(new Point(left, top));
            return result;

        }

        public static void FillVDSGroupsComboBox(List<VDS_GROUPS> dataList, ComboBox cbData)
        {
            cbData.Items.Clear();

            foreach (var data in dataList)
            {
                cbData.Items.Add(data.TITLE);
            }
            if (cbData.Items.Count > 0)
                cbData.SelectedIndex = 0;
        }

        public static void FillVDSTypeComboBox(List<VDS_TYPE> dataList, ComboBox cbData)
        {
            cbData.Items.Clear();

            foreach (var data in dataList)
            {
                cbData.Items.Add(data.VDS_TYPE_CODE);
            }
            if (cbData.Items.Count > 0)
                cbData.SelectedIndex = 0;
        }

        public static int GetCSN(ref byte[] csn)
        {
            int nResult = 0;
            try
            {
                Array.Copy(VDSConfig.korExConfig.csn, 0, csn, 0, VDSConfig.korExConfig.csn.Length);
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }


        public static bool GetCSN(byte[] csn, ref String vdsType, ref String vdsGroup, ref String vdsNo)
        {
            bool result = false;
            if (csn.Length==8)
            {
                vdsType = GetVDSTypeFromCSN(csn);
                vdsGroup = GetVDSGroupFromCSN(csn).ToString();
                vdsNo = BCDtoString(GetVDSNoFromCSN(csn));
                result = true;
            }
            return result;
        }

        public static byte[] GetCSN(String vdsType, String vdsGroup, String vdsNo)
        {
            byte[] result = new byte[8];
            int index = 0;
            Array.Copy(StringToByte(vdsType), 0, result, index, 2);
            index += 2;

            byte[] group = stringToBCD(vdsGroup);
            Array.Copy(group, 0, result, index, group.Length);
            index += group.Length;

            byte[] no = stringToBCD(vdsNo);
            Array.Copy(no, 0, result, index, no.Length);
            index += no.Length;


            return result;
        }

        public static String GetVDSTypeFromCSN(byte[] csn)
        {
            String result = String.Empty;
            byte[] vd = new byte[2];
            Array.Copy(csn, 0, vd, 0, 2); // 0~1: 'VD'
            result = ByteToString(vd);
            return result;
        }

        public static byte GetVDSGroupFromCSN(byte[] csn)
        {
            byte result;
            result = csn[2];
            return result;
        }

        public static byte[] GetVDSNoFromCSN(byte[] csn)
        {
            byte[] result = new byte[5];
            Array.Copy(csn, 3, result, 0, 5); // 0~1: 'VD'
            return result;
        }

        public static string BCDtoString(byte[] bcd)
        {
            StringBuilder temp = new StringBuilder(bcd.Length * 2);
            for (int i = 0; i < bcd.Length; i++)
            {
                temp.Append((byte)((bcd[i] & 0xf0) >> 4));
                temp.Append((byte)(bcd[i] & 0x0f));
            }

            return temp.ToString();
        }

        public static byte[] stringToBCD(string bcdString)
        {
            int length = bcdString.Length;

            if (length % 2 != 0)
            {
                length += 1;
                bcdString = "0" + bcdString;
            }

            byte[] bcd_Date = new byte[length / 2];

            int indexArray = 0;
            int bytearray = 0;
            for (int i = 0; i < bcdString.Length; i += 2)
            {
                bcd_Date[bytearray] = Convert.ToByte(bcdString[indexArray].ToString(), 16); indexArray++;
                bcd_Date[bytearray] <<= 4;
                bcd_Date[bytearray] |= Convert.ToByte(bcdString[indexArray].ToString(), 16); indexArray++;
                bytearray++;
            }
            return bcd_Date;
        }

        public static int SetOsTime(byte[] bcdTimeInfo) // yyyyMMddHHmmss
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                int year1 = (int)BCDToByte(bcdTimeInfo[0]);
                int year2 = (int)BCDToByte(bcdTimeInfo[1]);
                int month = (int)BCDToByte(bcdTimeInfo[2]);
                int day = (int)BCDToByte(bcdTimeInfo[3]);
                int hour = (int)BCDToByte(bcdTimeInfo[4]);
                int minute = (int)BCDToByte(bcdTimeInfo[5]);
                int second = (int)BCDToByte(bcdTimeInfo[6]); ;

                String time = String.Format("{0:D2}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}", year1, year2, month, day, hour, minute, second);
                DateTime osTime = DateTime.ParseExact(time, "yyyyMMddHHmmss", null);
                nResult = SetOsTime(osTime);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }

        public static CommuData GetCommuData(int protocolType, SessionContext session, int opCode,object data)
        {
            CommuData result = new CommuData();
            result.ProtocolType = protocolType;
            result.session = session;
            result.OpCode = opCode;
            result.data = data;
            return result;
        }


        public static int LaunchProcess(String filePath)
        {
            int result = 0;

            ApplicationLoader.PROCESS_INFORMATION processInfo;
            ApplicationLoader.StartProcessAndBypassUAC(filePath, out processInfo);
            return result;
        }

        public static String ToJson(object data)
        {
            String result = String.Empty;
            result = JsonConvert.SerializeObject(data);
            return result;
        }
    }
}

