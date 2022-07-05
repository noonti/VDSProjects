using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;
using VDSCommon.API.Model;
using VDSDBHandler.Model;

namespace VDSDBHandler.DBOperation.VDSManage
{
    public class VDSControllerOperation
    {
        private readonly DapperORM _dapperOrm;

        public VDSControllerOperation()
        {
            _dapperOrm = new DapperORM();
        }
        public VDSControllerOperation(String address, int port, String dbName, String uid, String passwd)
        {
            _dapperOrm = new DapperORM(address, port, dbName, uid, passwd);
        }

        public VDSControllerOperation(String connString)
        {
            _dapperOrm = new DapperORM(connString);
        }


        public void AddVDSController(ref VDS_CONTROLLER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,
                I_GROUP_ID = data.GROUP_ID,
                I_CONTROLLER_NAME = data.CONTROLLER_NAME,
                I_IP_ADDRESS = data.IP_ADDRESS,
                I_USE_YN = data.USE_YN,
                I_PROTOCOL = data.PROTOCOL,
                I_VDS_TYPE = data.VDS_TYPE,
                I_MODIFY_USER_ID = data.REG_USER_ID,
                I_REG_USER_ID = data.REG_USER_ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_VDS_CONTROLLER", param, out spResult);
        }


        public void UpdateVDSController(VDS_CONTROLLER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,
                I_GROUP_ID = data.GROUP_ID,
                I_CONTROLLER_NAME = data.CONTROLLER_NAME,
                I_IP_ADDRESS = data.IP_ADDRESS,
                I_USE_YN = data.USE_YN,
                I_PROTOCOL = data.PROTOCOL,
                I_VDS_TYPE = data.VDS_TYPE,
                I_MODIFY_USER_ID = data.MODIFY_USER_ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_VDS_CONTROLLER", param, out spResult);
        }


        public void UpdateVDSControllerConfig(VDS_CONTROLLER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,
                I_VDS_CONFIG = data.VDS_CONFIG,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_VDS_CONTROLLER_CONFIG", param, out spResult);
        }

        
        public void DeleteVDSController(VDS_CONTROLLER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_DELETE_VDS_CONTROLLER", param, out spResult);
        }


        public void UpdateVDSHeartBeatTime(VDS_CONTROLLER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,
               
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_VDS_HEARTBEAT_TIME", param, out spResult);
        }

        
        public IEnumerable<VDS_CONTROLLER> GetVDSControllerList(VDS_CONTROLLER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_GROUP_ID = data.GROUP_ID,
                I_CONTROLLER_NAME = data.CONTROLLER_NAME,
                I_IP_ADDRESS = data.IP_ADDRESS,
                I_USE_YN = data.USE_YN,
                I_PROTOCOL = data.PROTOCOL,
                I_VDS_TYPE = data.VDS_TYPE,
                I_PAGE_NO = data.CURRENT_PAGE,
                I_PAGE_SIZE = data.PAGE_SIZE,

            });
            return _dapperOrm.ReturnList<VDS_CONTROLLER>("SP_GET_VDS_CONTROLLER_LIST", param, out spResult).ToList();
        }



        public VDS_CONTROLLER GetVDSController(VDS_CONTROLLER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,

            });
            return _dapperOrm.ReturnSingle<VDS_CONTROLLER>("SP_GET_VDS_CONTROLLER", param, out spResult);

        }


        public void AddVDSConfig(ref VDS_CONFIG data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,
                I_CENTER_TYPE = data.CENTER_TYPE,
                I_IP_ADDRESS = data.IP_ADDRESS,
                I_CTRL_PORT = data.CTRL_PORT,
                I_CALIB_PORT = data.CALIB_PORT ,
                I_VDS_ID = data.VDS_ID,
                I_DB_ADDRESS = data.DB_ADDRESS,
                I_DB_PORT = data.DB_PORT,
                I_DB_NAME = data.DB_NAME,
                I_DB_USER = data.DB_USER,
                I_DB_PASSWD = data.DB_PASSWD,
                I_CENTER_ADDRESS = data.CENTER_ADDRESS ,
                I_CENTER_PORT = data.CENTER_PORT ,
                I_VDS_TYPE = data.VDS_TYPE,
                I_SENSOR_COUNT = data.SENSOR_COUNT ,
                I_DEVICE_ADDRESS = data.DEVICE_ADDRESS,
                I_DEVICE_PORT = data.DEVICE_PORT,
                I_LOCAL_PORT = data.LOCAL_PORT,
                I_CHECK_DISTANCE = data.CHECK_DISTANCE,
                I_RTSP_STREAMING_URL = data.RTSP_STREAMING_URL,
                I_USE_ANIMATION = data.USE_ANIMATION,
                I_REG_USER_ID = data.REG_USER_ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_VDS_CONFIG", param, out spResult);
        }


        public void UpdateVDSConfig(VDS_CONFIG data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,
                I_CENTER_TYPE = data.CENTER_TYPE,
                I_IP_ADDRESS = data.IP_ADDRESS,
                I_CTRL_PORT = data.CTRL_PORT,
                I_CALIB_PORT = data.CALIB_PORT,
                I_VDS_ID = data.VDS_ID,
                I_DB_ADDRESS = data.DB_ADDRESS,
                I_DB_PORT = data.DB_PORT,
                I_DB_NAME = data.DB_NAME,
                I_DB_USER = data.DB_USER,
                I_DB_PASSWD = data.DB_PASSWD,
                I_CENTER_ADDRESS = data.CENTER_ADDRESS,
                I_CENTER_PORT = data.CENTER_PORT,
                I_VDS_TYPE = data.VDS_TYPE,
                I_SENSOR_COUNT = data.SENSOR_COUNT,
                I_DEVICE_ADDRESS = data.DEVICE_ADDRESS,
                I_DEVICE_PORT = data.DEVICE_PORT,
                I_LOCAL_PORT = data.LOCAL_PORT,
                I_CHECK_DISTANCE = data.CHECK_DISTANCE,
                I_RTSP_STREAMING_URL = data.RTSP_STREAMING_URL,
                I_USE_ANIMATION = data.USE_ANIMATION,
                I_MODIFY_USER_ID = data.MODIFY_USER_ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_VDS_CONFIG", param, out spResult);
        }

        public void DeleteVDSConfig(VDS_CONFIG data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_DELETE_VDS_CONFIG", param, out spResult);
        }

        public VDS_CONFIG GetVDSConfig(VDS_CONFIG data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,

            });
            return _dapperOrm.ReturnSingle<VDS_CONFIG>("SP_GET_VDS_CONFIG", param, out spResult);

        }


        public VDS_CONTROLLER CheckVDSController(VDS_CONTROLLER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CONTROLLER_ID = data.CONTROLLER_ID,

            });
            return _dapperOrm.ReturnSingle<VDS_CONTROLLER>("SP_CHECK_VDS_CONTROLLER", param, out spResult);

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
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_TRAFFIC_DATA", param, out spResult);
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
    }
}
