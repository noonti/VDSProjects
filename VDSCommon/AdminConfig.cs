using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public static class AdminConfig
    {

        /// <summary>
        /// 주소
        /// </summary>
        public static String ADMIN_ADDRESS = "127.0.0.1";

        /// <summary>
        ///  주소
        /// </summary>
        public static int ADMIN_PORT = 1234;


        public static int ADMIN_API_PORT = 8088;


        /// <summary>
        /// DB 주소
        /// </summary>
        public static String DB_ADDRESS = "127.0.0.1";

        /// <summary>
        /// DB 포트
        /// </summary>
        public static int DB_PORT = 3306;

        /// <summary>
        /// DB 명
        /// </summary>
        public static String DB_NAME = "vdsmanage";


        /// <summary>
        /// DB 사용자
        /// </summary>
        public static String DB_USER = "vdsadmin";

        /// <summary>
        /// DB 비밀번호
        /// </summary>
        public static String DB_PASSWD = "1234";

        public static String DB_CONN = String.Format($"Server={DB_ADDRESS};Port={DB_PORT};Database={DB_NAME};Uid={DB_USER};Pwd={DB_PASSWD};SSL Mode=None");

        public static String GetDBConn()
        {
            DB_CONN = String.Format($"Server={DB_ADDRESS};Port={DB_PORT};Database={DB_NAME};Uid={DB_USER};Pwd={DB_PASSWD};SSL Mode=None");
            return DB_CONN;
        }
    }
}
