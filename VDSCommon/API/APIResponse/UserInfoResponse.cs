using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VDSCommon.API.Model;

namespace VDSCommon.API.APIResponse
{
    [DataContract]
    [Serializable]
    public class UserInfoResponse : APIResponse
    {
        [DataMember(Name = "RESULT_LIST", Order = 3)]
        public List<USER_INFO> resultList = new List<USER_INFO>();
    }
}
