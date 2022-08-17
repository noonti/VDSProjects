using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VDSCommon;
using VDSDBHandler.Model;

namespace VDSDBHandler.DBOperation
{
    public class TargetSummary
    {
        private readonly DapperORM _dapperOrm;//= new DapperORM();

        public TargetSummary()
        {
            _dapperOrm = new DapperORM();
        }
        public TargetSummary(String address, int port, String dbName, String uid, String passwd)
        {
            _dapperOrm = new DapperORM(address, port, dbName, uid, passwd);
        }

        public TargetSummary(String connString)
        {
            _dapperOrm = new DapperORM(connString);
        }

        public void AddTargetSummaryInfo(ref TARGET_SUMMARY_INFO input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID_0 = input.ID_0,
                I_ID_1 = input.ID_1,
                I_START_CYCLE_0 = input.START_CYCLE_0,
                I_START_CYCLE_1 = input.START_CYCLE_1,
                I_AGE_0 = input.AGE_0,
                I_AGE_1 = input.AGE_1,
                I_MAG_MAX_0 = input.MAG_MAX_0,
                I_MAG_MAX_1 = input.MAG_MAX_1,
                I_LANE =  input.LANE,
                I_TRAVEL_DIRECTION = input.TRAVEL_DIRECTION,
                I_LENGTH_X100 = input.LENGTH_X100,
                I_SPEED_X100 = input.SPEED_X100,
                I_RANGE_X100 = input.RANGE_X100,
                I_OCCUPY_TIME = input.OCCUPY_TIME,
                I_REPORT_YN = input.REPORT_YN,
                I_CREATE_TIME = input.CREATE_TIME
            });
            param.Add("@I_ID", dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_TARGET_SUMMARY_INFO", param, out spResult);
            if(spResult.IS_SUCCESS)
            {
                input.ID = param.Get<long>("@I_ID");

            }
        }

        public IEnumerable<TARGET_SUMMARY_INFO> GetTargetSummaryInfoList(TARGET_SUMMARY_INFO input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_SEARCH_TYPE = input.I_SEARCH_TYPE,
                I_ID = input.I_ID,
                I_START_DATE =  input.I_START_DATE,
                I_END_DATE =  input.I_END_DATE,
                I_REPORT_YN = input.I_REPORT_YN ,
                I_LIMIT_COUNT = input.I_LIMIT_COUNT

            });
            return _dapperOrm.ReturnList<TARGET_SUMMARY_INFO>("SP_GET_TARGET_SUMMARY_INFO_LIST", param, out spResult).ToList();

        }

        public IEnumerable<TARGET_SUMMARY_INFO> GetTargetSummaryByLaneList(TARGET_SUMMARY_INFO input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_START_DATE = input.I_START_DATE,
                I_END_DATE = input.I_END_DATE,
                I_LANE = input.I_LANE

            });
            return _dapperOrm.ReturnList<TARGET_SUMMARY_INFO>("SP_GET_TARGET_SUMMARY_BY_LANE_LIST", param, out spResult).ToList();

        }

        

        public void UpdateTargetReportYN(String idList,String ReportYN, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_IDS = idList, 
                I_REPORT_YN = ReportYN

            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_TARGET_REPORT_YN", param, out spResult);
        }


        public IEnumerable<TARGET_SUMMARY_STAT> GetTargetSummaryStat(TARGET_SUMMARY_INFO input, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_START_DATE = input.I_START_DATE,
                I_END_DATE = input.I_END_DATE,

            });
            return _dapperOrm.ReturnList<TARGET_SUMMARY_STAT>("SP_GET_TARGET_SUMMARY_STAT", param, out spResult).ToList();

        }

        //public IEnumerable<TRAFFIC_DATA_STAT> GetTrafficDataStat(TRAFFIC_DATA_STAT input, out SP_RESULT spResult)
        //{
        //    var param = new DynamicParameters();
        //    param.AddDynamicParams(new
        //    {
        //        I_START_DATE = input.I_START_DATE,
        //        I_END_DATE = input.I_END_DATE,

        //    });
        //    return _dapperOrm.ReturnList<TRAFFIC_DATA_STAT>("SP_GET_TRAFFIC_DATA_STAT", param, out spResult).ToList();

        //}

        public SPEED_DATA_STAT GetSpeedDataStat(SPEED_DATA_STAT input, byte [] speedCategory, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_START_DATE = input.I_START_DATE,
                I_END_DATE = input.I_END_DATE,
                I_LANE = input.LANE,
                I_CATEGORY_1 = speedCategory[0],
                I_CATEGORY_2 = speedCategory[1],
                I_CATEGORY_3 = speedCategory[2],
                I_CATEGORY_4 = speedCategory[3],
                I_CATEGORY_5 = speedCategory[4],
                I_CATEGORY_6 = speedCategory[5],
                I_CATEGORY_7 = speedCategory[6],
                I_CATEGORY_8 = speedCategory[7],
                I_CATEGORY_9 = speedCategory[8],
                I_CATEGORY_10 = speedCategory[9],
                I_CATEGORY_11 = speedCategory[10]

            });
            return _dapperOrm.ReturnSingle<SPEED_DATA_STAT>("SP_GET_SPEED_DATA_STAT", param, out spResult);

        }

        public LENGTH_DATA_STAT GetLengthDataStat(LENGTH_DATA_STAT input, byte[] lengthCategory, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_START_DATE = input.I_START_DATE,
                I_END_DATE = input.I_END_DATE,
                I_LANE = input.LANE,
                I_CATEGORY_1 = lengthCategory[0]/10,
                I_CATEGORY_2 = lengthCategory[1]/10,
                I_CATEGORY_3 = lengthCategory[2]/10

            });
            return _dapperOrm.ReturnSingle<LENGTH_DATA_STAT>("SP_GET_LENGTH_DATA_STAT", param, out spResult);

        }

        public ACCU_TRAFFIC_DATA_STAT GetAccuTrafficDataStat(ACCU_TRAFFIC_DATA_STAT input, byte[] lengthCategory, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_START_DATE = input.I_START_DATE,
                I_END_DATE = input.I_END_DATE

            });
            return _dapperOrm.ReturnSingle<ACCU_TRAFFIC_DATA_STAT>("SP_GET_ACCU_TRAFFIC_DATA_STAT", param, out spResult);

        }
        

    }
}
