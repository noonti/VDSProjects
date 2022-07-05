using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VDSCommon;
using VDSCommon.API.APIResponse;
using VDSCommon.API.Model;
using VDSDBHandler;
using VDSDBHandler.DBOperation.VDSManage;
using VDSWebAPIServer.Common;

namespace VDSWebAPIServer.Controller
{
    public class UserInfoController : ApiController
    {
        static UserInfoOperation userInfoOp = new UserInfoOperation(AdminConfig.GetDBConn());

        [HttpPost]
        public APIResponse AddUserInfo(USER_INFO data)
        {
            APIResponse response = new APIResponse();
             
            userInfoOp.AddUserInfo(ref data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }


        [HttpPost]
        public APIResponse UpdateUserInfo(USER_INFO data)
        {
            APIResponse response = new APIResponse();

            userInfoOp.UpdateUserInfo( data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }


        [HttpPost]
        public APIResponse DeleteUserInfo(USER_INFO data)
        {
            APIResponse response = new APIResponse();

            userInfoOp.DeleteUserInfo(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }


        [HttpPost]
        public UserInfoResponse GetUserInfo(USER_INFO data)
        {
            UserInfoResponse response = new UserInfoResponse();
            var userInfo = userInfoOp.GetUserInfo(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }

            if (userInfo!=null)
            {
                response.resultList.Add(userInfo);
            }
            return response;
        }


        [HttpPost]
        public APIResponse CheckLogin(USER_INFO data)
        {
            APIResponse response = new APIResponse();
            userInfoOp.CheckLogin(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;

        }
    }
}
