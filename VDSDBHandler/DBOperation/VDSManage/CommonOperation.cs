using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;
using VDSCommon.API.Model;

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

    }
}
