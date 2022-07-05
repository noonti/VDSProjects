using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace VDSDBHandler
{

    /// <summary>
    /// 저장프로시저 처리결과 코드 정의
    /// </summary>
    public static class SP_RESULT_CODE
    {
        /// <summary>
        /// 정상처리
        /// </summary>
        public static string SUCCESS = "100";

        /// <summary>
        /// 실패(코드상에서 임시처리용으로 사용)
        /// </summary>
        public static string DEFAULT_FAIL = "-999";
    }


    //public class SP_RESULT
    //{
    //    /// <summary>
    //    /// 영향 받은 레코드 수
    //    /// </summary>
    //    public int RESULT_COUNT { get; set; }

    //    /// <summary>
    //    /// 결과 코드
    //    /// </summary>
    //    public string RESULT_CODE { get; set; }

    //    /// <summary>
    //    /// 에러 메시지
    //    /// </summary>
    //    public string ERROR_MESSAGE { get; set; }

    //    /// <summary>
    //    /// 현재페이지
    //    /// </summary>
    //    public int CURRENT_PAGE { get; set; }

    //    /// <summary>
    //    ///  세션타임아웃(초)
    //    /// </summary>
    //    public int SESSION_TIMEOUT { get; set; }

    //    /// <summary>
    //    /// 프로시저 성공 여부
    //    /// </summary>
    //    public bool IS_SUCCESS
    //    {
    //        get { return RESULT_CODE == "100" && RESULT_COUNT > 0; }
    //    }

    //    /// <summary>
    //    /// 중복체크(Insert) : 중복이면 True 반환
    //    /// </summary>
    //    public bool IS_REDUPLICATION
    //    {
    //        get
    //        {
    //            if (RESULT_CODE == "500")
    //            {
    //                var strFrom = ERROR_MESSAGE.IndexOf("ERROR CODE=", StringComparison.Ordinal) + "ERROR CODE=".Length;
    //                var strTo = ERROR_MESSAGE.LastIndexOf("MESSAGE=", StringComparison.Ordinal);
    //                if (strFrom >= 0 && strTo >= 0) // 특정문자열을 찾았을 경우에만 동작
    //                {
    //                    var eMessage = ERROR_MESSAGE.Substring(strFrom, strTo - strFrom).Trim();
    //                    //var eMessage = ERROR_MESSAGE.Split(new[] {"ERROR CODE="}, StringSplitOptions.None)[1].Split(new[] {"MESSAGE="}, StringSplitOptions.None)[0].Trim();

    //                    if (int.TryParse(eMessage, out var eCode))
    //                    {
    //                        //ERROR(sys.messages): 2601:Duplicated key(unique index), 2627:Unique constraint(primary key), 547:Constraint check
    //                        //return eCode == 2601 || eCode == 2627 || eCode == 547;
    //                        return eCode == 2601 || eCode == 2627;
    //                    }
    //                }
    //            }

    //            return false;
    //        }
    //    }
    //}
    public class DapperORM
    {
        static string ConnString = "Server=127.0.0.1;Database=vdsdb;Uid=VDS;Pwd=1234;SSL Mode=None";
        //static string ConnString = "Server=127.0.0.1;Database=radarvdsdb;Uid=VDS;Pwd=1234;SSL Mode=None";

        public DapperORM()
        {

        }

        public DapperORM(String address, int port, String dbName, String uid, String passwd)
        {
            ConnString = String.Format($"Server={address};Port={port};Database={dbName};Uid={uid};Pwd={passwd};SSL Mode=None");
        }

        public DapperORM(String conn)
        {
            ConnString = conn ;
        }


        private static IDbConnection _connection
        {
            get { return new MySql.Data.MySqlClient.MySqlConnection(ConnString); }
        }

        public IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param, out SP_RESULT spResult)
        {
            using (_connection)
            {
                param.Add("@RESULT_COUNT", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@RESULT_CODE", dbType: DbType.String, direction: ParameterDirection.Output, size: 10);
                param.Add("@ERROR_MESSAGE", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
                var list = _connection.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
                spResult = new SP_RESULT
                {
                    RESULT_COUNT = param.Get<int>("@RESULT_COUNT"),
                    RESULT_CODE = param.Get<string>("@RESULT_CODE"),
                    ERROR_MESSAGE = param.Get<string>("@ERROR_MESSAGE")
                };
                return list;
            }
        }


        public IEnumerable<T> ReturnListPage<T>(string procedureName, DynamicParameters param, out SP_RESULT spResult)
        {
            using (_connection)
            {
                param.Add("@RESULT_COUNT", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@RESULT_CODE", dbType: DbType.String, direction: ParameterDirection.Output, size: 10);
                param.Add("@ERROR_MESSAGE", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
                var list = _connection.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
                spResult = new SP_RESULT
                {
                    CURRENT_PAGE = param.Get<int>("@CURRENT_PAGE"),
                    RESULT_COUNT = param.Get<int>("@RESULT_COUNT"),
                    RESULT_CODE = param.Get<string>("@RESULT_CODE"),
                    ERROR_MESSAGE = param.Get<string>("@ERROR_MESSAGE")
                };
                return list;
            }
        }

        public T ReturnSingle<T>(string procedureName, DynamicParameters param, out SP_RESULT spResult) where T : class
        {
            using (_connection)
            {
                param.Add("@RESULT_COUNT", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@RESULT_CODE", dbType: DbType.String, direction: ParameterDirection.Output, size: 10);
                param.Add("@ERROR_MESSAGE", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
                var value = _connection.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure).SingleOrDefault();
                spResult = new SP_RESULT
                {
                    RESULT_COUNT = param.Get<int>("@RESULT_COUNT"),
                    RESULT_CODE = param.Get<string>("@RESULT_CODE"),
                    ERROR_MESSAGE = param.Get<string>("@ERROR_MESSAGE")
                };
                return value;
            }
        }

        /// <summary>
        /// 스토어드 프로시저 DML : Return Void
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="spResult"></param>
        /// <param name="param"></param>
        public void ExecuteWithoutReturn(string procedureName, DynamicParameters param, out SP_RESULT spResult)
        {
            using (_connection)
            {
                param.Add("@RESULT_COUNT", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@RESULT_CODE", dbType: DbType.String, direction: ParameterDirection.Output, size: 10);
                param.Add("@ERROR_MESSAGE", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
                _connection.Execute(procedureName, param,null,60, commandType: CommandType.StoredProcedure);
                spResult = new SP_RESULT
                {
                    RESULT_COUNT = param.Get<int>("@RESULT_COUNT"),
                    RESULT_CODE = param.Get<string>("@RESULT_CODE"),
                    ERROR_MESSAGE = param.Get<string>("@ERROR_MESSAGE")
                };
            }
        }
    }
}
