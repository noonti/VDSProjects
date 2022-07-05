using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    [DataContract]
    [Serializable]
    public class SP_RESULT
    {
        /// <summary>
        /// 영향 받은 레코드 수
        /// </summary>
        public int RESULT_COUNT { get; set; }

        /// <summary>
        /// 결과 코드
        /// </summary>
        public string RESULT_CODE { get; set; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        public string ERROR_MESSAGE { get; set; }

        /// <summary>
        /// 현재페이지
        /// </summary>
        public int CURRENT_PAGE { get; set; }

        /// <summary>
        ///  세션타임아웃(초)
        /// </summary>
        public int SESSION_TIMEOUT { get; set; }

        /// <summary>
        /// 프로시저 성공 여부
        /// </summary>
        public bool IS_SUCCESS
        {
            get { return RESULT_CODE == "100" && RESULT_COUNT > 0; }
        }

        /// <summary>
        /// 중복체크(Insert) : 중복이면 True 반환
        /// </summary>
        public bool IS_REDUPLICATION
        {
            get
            {
                if (RESULT_CODE == "500")
                {
                    var strFrom = ERROR_MESSAGE.IndexOf("ERROR CODE=", StringComparison.Ordinal) + "ERROR CODE=".Length;
                    var strTo = ERROR_MESSAGE.LastIndexOf("MESSAGE=", StringComparison.Ordinal);
                    if (strFrom >= 0 && strTo >= 0) // 특정문자열을 찾았을 경우에만 동작
                    {
                        var eMessage = ERROR_MESSAGE.Substring(strFrom, strTo - strFrom).Trim();
                        //var eMessage = ERROR_MESSAGE.Split(new[] {"ERROR CODE="}, StringSplitOptions.None)[1].Split(new[] {"MESSAGE="}, StringSplitOptions.None)[0].Trim();

                        if (int.TryParse(eMessage, out var eCode))
                        {
                            //ERROR(sys.messages): 2601:Duplicated key(unique index), 2627:Unique constraint(primary key), 547:Constraint check
                            //return eCode == 2601 || eCode == 2627 || eCode == 547;
                            return eCode == 2601 || eCode == 2627;
                        }
                    }
                }

                return false;
            }
        }
    }
}
