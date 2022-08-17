using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;
using VDSCommon.API.Model;
using VDSDBHandler.Model;

namespace VDSDBHandler.DBOperation
{
    public class TrafficDataOperation
    {
        private readonly DapperORM _dapperOrm;//= new DapperORM();

        public TrafficDataOperation()
        {
            _dapperOrm = new DapperORM();
        }
        public TrafficDataOperation(String address, int port, String dbName, String uid, String passwd)
        {
            _dapperOrm = new DapperORM(address, port, dbName, uid, passwd);
        }

        public TrafficDataOperation(String connString)
        {
            _dapperOrm = new DapperORM(connString);
        }


        public void AddTrafficData(TRAFFIC_DATA input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_CONTROLLER_ID = input.CONTROLLER_ID,
                I_VDS_TYPE = input.VDS_TYPE,
                I_LANE = input.LANE,
                I_DIRECTION = input.DIRECTION,
                I_LENGTH = input.LENGTH ,
                I_SPEED = input.SPEED,
                I_VEHICLE_CLASS = input.VEHICLE_CLASS,
                I_OCCUPY_TIME = input.OCCUPY_TIME,
                I_LOOP1_OCCUPY_TIME = input.LOOP1_OCCUPY_TIME,
                I_LOOP2_OCCUPY_TIME = input.LOOP2_OCCUPY_TIME,
                I_REVERSE_RUN_YN = input.REVERSE_RUN_YN,
                I_VEHICLE_GAP = input.VEHICLE_GAP,
                I_DETECT_TIME = input.DETECT_TIME,
                I_REPORT_YN = input.REPORT_YN,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_TRAFFIC_DATA", param, out spResult);
        }


        public void AddTrafficTestData(TRAFFIC_DATA input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_VDS_TYPE = input.VDS_TYPE,
                I_LANE = input.LANE,
                I_DIRECTION = input.DIRECTION,
                I_LENGTH = input.LENGTH,
                I_SPEED = input.SPEED,
                I_VEHICLE_CLASS = input.VEHICLE_CLASS,
                I_OCCUPY_TIME = input.OCCUPY_TIME,
                I_LOOP1_OCCUPY_TIME = input.LOOP1_OCCUPY_TIME,
                I_LOOP2_OCCUPY_TIME = input.LOOP2_OCCUPY_TIME,
                I_REVERSE_RUN_YN = input.REVERSE_RUN_YN,
                I_VEHICLE_GAP = input.VEHICLE_GAP,
                I_DETECT_TIME = input.DETECT_TIME,
                I_REPORT_YN = input.REPORT_YN,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_TRAFFIC_TEST_DATA", param, out spResult);
        }
        public void UpdateTrafficDataReportYN(String idList, String ReportYN, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_IDS = idList,
                I_REPORT_YN = ReportYN

            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_TRAFFIC_DATA_REPORT_YN", param, out spResult);
        }

        public IEnumerable<TRAFFIC_DATA> GetTrafficDataList(TRAFFIC_DATA input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_START_DATE = input.I_START_DATE,
                I_END_DATE = input.I_END_DATE,
                I_LANE = input.LANE,
                I_DIRECTION = input.DIRECTION,
                I_REPORT_YN = input.I_REPORT_YN,

            });
            return _dapperOrm.ReturnList<TRAFFIC_DATA>("SP_GET_TRAFFIC_DATA_LIST", param, out spResult).ToList();

        }

        public IEnumerable<TRAFFIC_DATA> GetTrafficDataListByPage(TRAFFIC_DATA input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_START_DATE = input.I_START_DATE,
                I_END_DATE = input.I_END_DATE,
                I_LANE = input.LANE,
                I_DIRECTION = input.DIRECTION,
                I_REPORT_YN = input.I_REPORT_YN,
                I_PAGE_NO = input.I_PAGE_NO,
                I_PAGE_SIZE = input.I_PAGE_SIZE

            });
            return _dapperOrm.ReturnList<TRAFFIC_DATA>("SP_GET_TRAFFIC_DATA_LIST_BY_PAGE", param, out spResult).ToList();

        }

        

        public TRAFFIC_DATA GetLastTrafficData(out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
            });
            return _dapperOrm.ReturnSingle<TRAFFIC_DATA>("SP_GET_LAST_TRAFFIC_DATA", param, out spResult);

        }


        public void AddTrafficStat(TRAFFIC_STAT input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_DETECT_DATE = input.DETECT_DATE,
                I_FRAME_NO = input.FRAME_NO,
                I_ERROR_INFO = input.ERROR_INFO,
                I_LANE_COUNT = input.LANE_COUNT,
                I_LANE_INFO = input.LANE_INFO,
                I_REPORT_YN = input.REPORT_YN
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_TRAFFIC_STAT", param, out spResult);
        }

        public void UpdateTrafficStat(TRAFFIC_STAT input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_DETECT_DATE = input.DETECT_DATE,
                I_FRAME_NO = input.FRAME_NO,
                I_ERROR_INFO = input.ERROR_INFO,
                I_LANE_COUNT = input.LANE_COUNT,
                I_LANE_INFO = input.LANE_INFO,
                I_REPORT_YN = input.REPORT_YN
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_TRAFFIC_STAT", param, out spResult);
        }



        public void AddTrafficStatDetail(TRAFFIC_STAT_DETAIL input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_PARENT_ID = input.PARENT_ID,
                I_LANE = input.LANE,
                I_LARGE_COUNT = input.LARGE_COUNT,
                I_MIDDLE_COUNT = input.MIDDLE_COUNT,
                I_SMALL_COUNT = input.SMALL_COUNT,
                I_SPEED = input.SPEED,
                I_OCCUPY = input.OCCUPY,
                I_CAR_LENGTH = input.CAR_LENGTH
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_TRAFFIC_STAT_DETAIL", param, out spResult);
        }

        public void UpdateTrafficStatDetail(TRAFFIC_STAT_DETAIL input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_PARENT_ID = input.PARENT_ID,
                I_LANE = input.LANE,
                I_LARGE_COUNT = input.LARGE_COUNT,
                I_MIDDLE_COUNT = input.MIDDLE_COUNT,
                I_SMALL_COUNT = input.SMALL_COUNT,
                I_SPEED = input.SPEED,
                I_OCCUPY = input.OCCUPY,
                I_CAR_LENGTH = input.CAR_LENGTH
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_TRAFFIC_STAT_DETAIL", param, out spResult);
        }


        public void AddSpeedStat(SPEED_STAT input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_DETECT_DATE = input.DETECT_DATE,
                I_LANE_COUNT = input.LANE_COUNT,
                I_SPEED_INFO = input.SPEED_INFO,
                I_REPORT_YN = input.REPORT_YN
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_SPEED_STAT", param, out spResult);
        }

        public void UpdateSpeedStat(SPEED_STAT input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_DETECT_DATE = input.DETECT_DATE,
                I_LANE_COUNT = input.LANE_COUNT,
                I_SPEED_INFO = input.SPEED_INFO,
                I_REPORT_YN = input.REPORT_YN
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_SPEED_STAT", param, out spResult);
        }

        public void AddSpeedStatDetail(SPEED_STAT_DETAIL input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_PARENT_ID = input.PARENT_ID,
                I_LANE = input.LANE,
                I_CATEGORY_1_COUNT = input.CATEGORY_1_COUNT,
                I_CATEGORY_2_COUNT = input.CATEGORY_2_COUNT,
                I_CATEGORY_3_COUNT = input.CATEGORY_3_COUNT,
                I_CATEGORY_4_COUNT = input.CATEGORY_4_COUNT,
                I_CATEGORY_5_COUNT = input.CATEGORY_5_COUNT,
                I_CATEGORY_6_COUNT = input.CATEGORY_6_COUNT,
                I_CATEGORY_7_COUNT = input.CATEGORY_7_COUNT,
                I_CATEGORY_8_COUNT = input.CATEGORY_8_COUNT,
                I_CATEGORY_9_COUNT = input.CATEGORY_9_COUNT,
                I_CATEGORY_10_COUNT = input.CATEGORY_10_COUNT,
                I_CATEGORY_11_COUNT = input.CATEGORY_11_COUNT,
                I_CATEGORY_12_COUNT = input.CATEGORY_12_COUNT,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_SPEED_STAT_DETAIL", param, out spResult);
        }

        public void UpdateSpeedStatDetail(SPEED_STAT_DETAIL input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = input.ID,
                I_PARENT_ID = input.PARENT_ID,
                I_LANE = input.LANE,
                I_CATEGORY_1_COUNT = input.CATEGORY_1_COUNT,
                I_CATEGORY_2_COUNT = input.CATEGORY_2_COUNT,
                I_CATEGORY_3_COUNT = input.CATEGORY_3_COUNT,
                I_CATEGORY_4_COUNT = input.CATEGORY_4_COUNT,
                I_CATEGORY_5_COUNT = input.CATEGORY_5_COUNT,
                I_CATEGORY_6_COUNT = input.CATEGORY_6_COUNT,
                I_CATEGORY_7_COUNT = input.CATEGORY_7_COUNT,
                I_CATEGORY_8_COUNT = input.CATEGORY_8_COUNT,
                I_CATEGORY_9_COUNT = input.CATEGORY_9_COUNT,
                I_CATEGORY_10_COUNT = input.CATEGORY_10_COUNT,
                I_CATEGORY_11_COUNT = input.CATEGORY_11_COUNT,
                I_CATEGORY_12_COUNT = input.CATEGORY_12_COUNT,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_SPEED_STAT_DETAIL", param, out spResult);
        }
    }
}
