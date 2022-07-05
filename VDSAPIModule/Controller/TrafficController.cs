using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VDSCommon;
using VDSCommon.API.APIResponse;
using VDSCommon.API.Model;
using VDSDBHandler.DBOperation;
using VDSDBHandler.DBOperation.VDSManage;

namespace VDSAPIModule.Controller
{
    public class TrafficController : ApiController
    {
        
        // TRAFFIC_DATA
        [HttpPost]
        public TrafficDataResponse GetTrafficDataList(TRAFFIC_DATA data)
        {
            TrafficDataResponse response = new TrafficDataResponse();
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            response.resultList = db.GetTrafficDataList(data, out SP_RESULT spResult).ToList();
            if (spResult != null)
            {
                response.RESULT_CODE = spResult.RESULT_CODE;
                response.RESULT_COUNT = spResult.RESULT_COUNT;
                response.ERROR_MESSAGE = spResult.ERROR_MESSAGE;

            }

            return response;
        }

        [HttpPost]
        public TrafficDataResponse GetTrafficDataListByPage(TRAFFIC_DATA data)
        {
            TrafficDataResponse response = new TrafficDataResponse();
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            response.resultList = db.GetTrafficDataListByPage(data, out SP_RESULT spResult).ToList();
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
