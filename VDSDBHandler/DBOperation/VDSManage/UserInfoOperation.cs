using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;
using VDSCommon.API.Model;


namespace VDSDBHandler.DBOperation.VDSManage
{
    public class UserInfoOperation
    {
        private readonly DapperORM _dapperOrm;//= new DapperORM();

        public UserInfoOperation()
        {
            _dapperOrm = new DapperORM();
        }
        public UserInfoOperation(String address, int port, String dbName, String uid, String passwd)
        {
            _dapperOrm = new DapperORM(address, port, dbName, uid, passwd);
        }

        public UserInfoOperation(String connString)
        {
            _dapperOrm = new DapperORM(connString);
        }


        public void AddUserInfo(ref USER_INFO data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_USER_ID = data.USER_ID,
                I_PASSWD = data.PASSWD,
                I_USER_NAME = data.USER_NAME,
                I_USER_TYPE = data.USER_TYPE,
                I_DEPT_NAME = data.DEPT_NAME,
                I_APPROVE_YN = data.APPROVE_YN,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_USER_INFO", param, out spResult);
        }


        public void UpdateUserInfo(USER_INFO data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_USER_ID = data.USER_ID,
                I_PASSWD = data.PASSWD,
                I_USER_NAME = data.USER_NAME,
                I_USER_TYPE = data.USER_TYPE,
                I_DEPT_NAME = data.DEPT_NAME,
                I_APPROVE_YN = data.APPROVE_YN,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_USER_INFO", param, out spResult);
        }

        public void DeleteUserInfo(USER_INFO data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_USER_ID = data.USER_ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_DELETE_USER_INFO", param, out spResult);
        }

        public IEnumerable<USER_INFO> GetUserInfoList(USER_INFO data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_USER_ID = data.USER_ID,
                I_USER_NAME = data.USER_NAME,
                I_USER_TYPE = data.USER_TYPE,
                I_APPROVE_YN = data.APPROVE_YN,
                I_PAGE_NO = data.CURRENT_PAGE,
                I_PAGE_SIZE = data.PAGE_SIZE

            });
            return _dapperOrm.ReturnList<USER_INFO>("SP_GET_USER_INFO_LIST", param, out spResult).ToList();
        }

        public USER_INFO GetUserInfo(USER_INFO data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_USER_ID = data.USER_ID

            });
            return _dapperOrm.ReturnSingle<USER_INFO>("SP_GET_USER_INFO", param, out spResult);

        }


        public void ApproveUserInfo(USER_INFO data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_USER_ID = data.USER_ID,
                I_APPROVE_YN = data.APPROVE_YN
            });

            _dapperOrm.ExecuteWithoutReturn("SP_APPROVE_USER_INFO", param, out spResult);
        }

        public void CheckLogin(USER_INFO data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_USER_ID = data.USER_ID,
                I_PASSWD = data.PASSWD,

            });
            _dapperOrm.ExecuteWithoutReturn("SP_CHECK_LOGIN", param, out spResult);
        }
    }
}
