using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VDSCommon;
using VDSCommon.API.APIResponse;
using VDSCommon.API.Model;
using VDSDBHandler.DBOperation.VDSManage;
using VDSWebAPIServer.Common;

namespace VDSWebAPIServer.Controller
{
    public class CommonController : ApiController
    {
        static CommonOperation commonOp = new CommonOperation(AdminConfig.DB_CONN);

        [HttpPost]
        public APIResponse AddVDSGroups(VDS_GROUPS data)
        {
            APIResponse response = new APIResponse();
            SP_RESULT spResult;
            commonOp.AddVDSGroups(ref data, out spResult);
            if(spResult!=null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }

            return response;
        }

        [HttpPost]
        public APIResponse UpdateVDSGroups(VDS_GROUPS data)
        {
            APIResponse response = new APIResponse();

            commonOp.UpdateVDSGroups(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }

            return response;
        }

        [HttpPost]
        public APIResponse DeleteVDSGroups(VDS_GROUPS data)
        {
            APIResponse response = new APIResponse();

            commonOp.DeleteVDSGroups(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }

        [HttpPost]
        public VDSGroupsResponse GetVDSGroupsList(VDS_GROUPS data)
        {
            VDSGroupsResponse response = new VDSGroupsResponse();
            response.resultList = commonOp.GetVDSGroupsList(data, out SP_RESULT spResult).ToList();
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }


        [HttpPost]
        public VDSGroupsResponse GetVDSGroups(VDS_GROUPS data)
        {
            VDSGroupsResponse response = new VDSGroupsResponse();
            var vdsGroups = commonOp.GetVDSGroups(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            if (vdsGroups != null)
            {
                response.resultList.Add(vdsGroups);
            }
            return response;
        }
    }
}
