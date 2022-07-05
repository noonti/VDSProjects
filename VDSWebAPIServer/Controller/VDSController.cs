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
    public class VDSController : ApiController
    {
        static VDSControllerOperation vdsControllerOp = new VDSControllerOperation(AdminConfig.GetDBConn());

        [HttpPost]
        public APIResponse AddVDSController(VDS_CONTROLLER data)
        {
            APIResponse response = new APIResponse();

            vdsControllerOp.AddVDSController(ref data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }

            return response;
        }

        [HttpPost]
        public APIResponse UpdateVDSController(VDS_CONTROLLER data)
        {
            APIResponse response = new APIResponse();

            vdsControllerOp.UpdateVDSController(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }


        [HttpPost]
        public APIResponse UpdateVDSControllerConfig(VDS_CONTROLLER data)
        {
            APIResponse response = new APIResponse();

            vdsControllerOp.UpdateVDSControllerConfig(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }

        

        [HttpPost]
        public APIResponse DeleteVDSController(VDS_CONTROLLER data)
        {
            APIResponse response = new APIResponse();

            vdsControllerOp.DeleteVDSController(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }

        [HttpPost]
        public VDSControllerResponse GetVDSControllerList(VDS_CONTROLLER data)
        {
            VDSControllerResponse response = new VDSControllerResponse();
            response.resultList = vdsControllerOp.GetVDSControllerList(data, out SP_RESULT spResult).ToList();
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }

            return response;
        }

        [HttpPost]
        public VDSControllerResponse GetVDSController(VDS_CONTROLLER data)
        {
            VDSControllerResponse response = new VDSControllerResponse();
            var vdsController = vdsControllerOp.GetVDSController(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }

            if (vdsController != null)
            {
                response.resultList.Add(vdsController);
            }
            return response;
        }


        [HttpPost]
        public APIResponse AddVDSConfig(VDS_CONFIG data)
        {
            APIResponse response = new APIResponse();

            vdsControllerOp.AddVDSConfig(ref data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }

        [HttpPost]
        public APIResponse UpdateVDSConfig(VDS_CONFIG data)
        {
            APIResponse response = new APIResponse();

            vdsControllerOp.UpdateVDSConfig(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }


        [HttpPost]
        public APIResponse DeleteVDSConfig(VDS_CONFIG data)
        {
            APIResponse response = new APIResponse();

            vdsControllerOp.DeleteVDSConfig(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }
            return response;
        }


        [HttpPost]
        public VDSConfigResponse GetVDSConfig(VDS_CONFIG data)
        {
            VDSConfigResponse response = new VDSConfigResponse();
            var vdsConfig = vdsControllerOp.GetVDSConfig(data, out SP_RESULT spResult);
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }

            if (vdsConfig != null)
            {
                response.resultList.Add(vdsConfig);
            }
            return response;
        }
    }
}
