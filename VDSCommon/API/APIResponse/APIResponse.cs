using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.API.APIResponse
{
    [DataContract]
    [Serializable]
    public class APIResponse
    {
        /// <summary>
        /// API 호출 결과
        /// </summary>
        //public SP_RESULT spResult;

        /// <summary>
        /// 영향 받은 레코드 수
        /// </summary>
        [DataMember(Name = "RESULT_COUNT", Order = 0)]
        public int RESULT_COUNT { get; set; }


        /// <summary>
        /// 결과 코드
        /// </summary>
        [DataMember(Name = "RESULT_CODE", Order = 1)]
        public string RESULT_CODE { get; set; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        [DataMember(Name = "ERROR_MESSAGE", Order = 2)]
        public string ERROR_MESSAGE { get; set; }
    }
}
