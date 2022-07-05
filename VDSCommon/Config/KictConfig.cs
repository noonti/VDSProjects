using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.Config
{
    /// <summary>
    /// 건기연 평가 센터 설정 
    /// </summary>
    public class KictConfig
    {

        /// <summary>
        /// 제어프로그램 대기 포트 
        /// </summary>
        public  int ctrlPort = 12000; // local  대기 포트 (축적데이터/상태/시각)



        /// <summary>
        /// CALIBRATION PORT 
        /// </summary>
        public  int calibPort = 11000; // local 디기 포트(검지 시작/종료, Echo)


        public  String csn = String.Empty;


        /// <summary>
        /// 안산 센터 아이피 주소
        /// </summary> 
        public  String centerAddress = "127.0.0.1"; // 수집서버(센터) 주소 

        /// <summary>
        /// 안산 센터 접속 포트
        /// </summary>
        public  int centerPort = 10000;

       

        /// <summary>
        /// 요청 완료 시간
        /// </summary>
        public  int requestTimeout = 1;  // 1초 이내에 전송 완료

        /// <summary>
        /// 응답 대기 시간
        /// </summary>
        public  int responseTimeout = 3; // 3초 이내에 응답 완료


        /// <summary>
        /// 응답 대기 시간 만료 시 재시도 횟수
        /// </summary>
        public  int retryCount = 3;      // 3번까지 재시도 


        /// <summary>
        /// 재시도 횟수 초과 시 재 요청 시도 시간
        /// </summary>
        public  int nextRetryTime = 5 * 60; // 5분 후 재시도 


        /// <summary>
        /// 접속 종료 시 재접속 시도 간격
        /// </summary>
        public  int reconnectInterval = 10;// 재접속 시도 간격(초)

        

        /// <summary>
        /// 센터 전송 주기(초)
        /// </summary>
        public  int centerPollingPeriod = 30;


        /// <summary>
        /// 현장 전송 주기(초)
        /// </summary>
        public  int localPollingPeriod = 30;

        public  int checkSessiontime = 5 * 60;// 5분 


        public const int PACKET_SIZE = 4096 * 2;
        public const int PAGE_SIZE = 20;

    }
}
