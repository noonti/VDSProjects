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
    public class CommonOperation
    {
        private readonly DapperORM _dapperOrm;//= new DapperORM();

        public CommonOperation()
        {
            _dapperOrm = new DapperORM();
        }
        public CommonOperation(String address, int port, String dbName, String uid, String passwd)
        {
            _dapperOrm = new DapperORM(address, port, dbName, uid, passwd);
        }

        public CommonOperation(String connString)
        {
            _dapperOrm = new DapperORM(connString);
        }



        public void AddVDSGroups(ref VDS_GROUPS data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_GROUP_CODE = data.GROUP_CODE,
                I_PARENT_ID = data.PARENT_ID,
                I_DEPTH = data.DEPTH,
                I_TITLE = data.TITLE,
                I_OFFICER_NAME = data.OFFICER_NAME,
                I_TEL_NO = data.TEL_NO,
                I_USE_YN = data.USE_YN,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_VDS_GROUPS", param, out spResult);
        }


        public void UpdateVDSGroups(VDS_GROUPS data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = data.ID,
                I_GROUP_CODE = data.GROUP_CODE,
                I_PARENT_ID = data.PARENT_ID,
                I_DEPTH = data.DEPTH,
                I_TITLE = data.TITLE,
                I_OFFICER_NAME = data.OFFICER_NAME,
                I_TEL_NO = data.TEL_NO,
                I_USE_YN = data.USE_YN,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_VDS_GROUPS", param, out spResult);
        }

        public void DeleteVDSGroups(VDS_GROUPS data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = data.ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_DELETE_VDS_GROUPS", param, out spResult);
        }

        public IEnumerable<VDS_GROUPS> GetVDSGroupsList(VDS_GROUPS data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_PARENT_ID = data.PARENT_ID,
                I_USE_YN = data.USE_YN,

            });
            return _dapperOrm.ReturnList<VDS_GROUPS>("SP_GET_VDS_GROUPS_LIST", param, out spResult).ToList();
        }

        public VDS_GROUPS GetVDSGroups(VDS_GROUPS data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = data.ID,

            });
            return _dapperOrm.ReturnSingle<VDS_GROUPS>("SP_GET_VDS_GROUPS", param, out spResult);
        }

        public IEnumerable<VDS_TYPE> GetVDSTypeList(out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
            });
            return _dapperOrm.ReturnList<VDS_TYPE>("SP_GET_VDS_TYPE_LIST", param, out spResult).ToList();
        }




        public void AddSpeedCategory(SPEED_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CATEGORY_NO = data.CATEGORY_NO,
                I_SPEED_UNIT = data.SPEED_UNIT,
                I_FROM_VALUE = data.FROM_VALUE,
                I_TO_VALUE = data.TO_VALUE
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_SPEED_CATEGORY", param, out spResult);
        }


        public void UpdateSpeedCategory(SPEED_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = data.ID,
                I_CATEGORY_NO = data.CATEGORY_NO,
                I_SPEED_UNIT = data.SPEED_UNIT,
                I_FROM_VALUE = data.FROM_VALUE,
                I_TO_VALUE = data.TO_VALUE
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_SPEED_CATEGORY", param, out spResult);
        }

        public void DeleteSpeedCategory(SPEED_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = data.ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_DELETE_SPEED_CATEGORY", param, out spResult);
        }

        public IEnumerable<SPEED_CATEGORY> GetSpeedCategoryList(SPEED_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {

            });
            return _dapperOrm.ReturnList<SPEED_CATEGORY>("SP_GET_SPEED_CATEGORY_LIST", param, out spResult).ToList();
        }

        public SPEED_CATEGORY GetSpeedCategory(SPEED_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = data.ID,

            });
            return _dapperOrm.ReturnSingle<SPEED_CATEGORY>("SP_GET_SPEED_CATEGORY", param, out spResult);
        }


        public void AddLengthCategory(LENGTH_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_CATEGORY_NO = data.CATEGORY_NO,
                I_LENGTH_UNIT = data.LENGTH_UNIT,
                I_FROM_VALUE = data.FROM_VALUE,
                I_TO_VALUE = data.TO_VALUE
            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_LENGTH_CATEGORY", param, out spResult);
        }


        public void UpdateLengthCategory(LENGTH_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = data.ID,
                I_CATEGORY_NO = data.CATEGORY_NO,
                I_LENGTH_UNIT = data.LENGTH_UNIT,
                I_FROM_VALUE = data.FROM_VALUE,
                I_TO_VALUE = data.TO_VALUE
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_LENGTH_CATEGORY", param, out spResult);
        }

        public void DeleteLengthCategory(LENGTH_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = data.ID,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_DELETE_LENGTH_CATEGORY", param, out spResult);
        }

        public IEnumerable<LENGTH_CATEGORY> GetLengthCategoryList(LENGTH_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {

            });
            return _dapperOrm.ReturnList<LENGTH_CATEGORY>("SP_GET_LENGTH_CATEGORY_LIST", param, out spResult).ToList();
        }

        public LENGTH_CATEGORY GetLengthCategory(LENGTH_CATEGORY data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_ID = data.ID,

            });
            return _dapperOrm.ReturnSingle<LENGTH_CATEGORY>("SP_GET_LENGTH_CATEGORY", param, out spResult);
        }


        public void AddKorexParameter(KOREX_PARAMETER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_SPEED_ACCU_ENABLED = data.SPEED_ACCU_ENABLED,
                I_LENGTH_ACCU_ENABLED = data.LENGTH_ACCU_ENABLED,
                I_SPEED_CALCU_ENABLED = data.SPEED_CALCU_ENABLED,
                I_LENGTH_CALCU_ENABLED = data.LENGTH_CALCU_ENABLED,
                I_REVERSE_RUN_ENABLED = data.REVERSE_RUN_ENABLED,
                I_OSCILLATION_THRESHOLD = data.OSCILLATION_THRESHOLD,
                I_AUTO_SYNC_PERIOD = data.AUTO_SYNC_PERIOD,

            });
            _dapperOrm.ExecuteWithoutReturn("SP_ADD_KOREX_PARAMETER", param, out spResult);
        }


        public void UpdateKorexParameter(KOREX_PARAMETER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
                I_SPEED_ACCU_ENABLED = data.SPEED_ACCU_ENABLED,
                I_LENGTH_ACCU_ENABLED = data.LENGTH_ACCU_ENABLED,
                I_SPEED_CALCU_ENABLED = data.SPEED_CALCU_ENABLED,
                I_LENGTH_CALCU_ENABLED = data.LENGTH_CALCU_ENABLED,
                I_REVERSE_RUN_ENABLED = data.REVERSE_RUN_ENABLED,
                I_OSCILLATION_THRESHOLD = data.OSCILLATION_THRESHOLD,
                I_AUTO_SYNC_PERIOD = data.AUTO_SYNC_PERIOD,
            });
            _dapperOrm.ExecuteWithoutReturn("SP_UPDATE_KOREX_PARAMETER", param, out spResult);
        }

        public KOREX_PARAMETER GetKorexParameter(KOREX_PARAMETER data, out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {

            });
            return _dapperOrm.ReturnSingle<KOREX_PARAMETER>("SP_GET_KOREX_PARAMETER", param, out spResult);
        }

        public IEnumerable<KOREX_OFFICE> GetKorexOfficeList(out SP_RESULT spResult)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(new
            {
            });
            return _dapperOrm.ReturnList<KOREX_OFFICE>("SP_GET_KOREX_OFFICE_LIST", param, out spResult).ToList();
        }
    }
}
