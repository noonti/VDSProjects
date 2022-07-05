using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;
using VDSCommon.API.Model;
using VDSDBHandler.DBOperation.VDSManage;

namespace VDSController.Global
{
    public static class GlobalCommonData
    {
        static CommonOperation commonOp = new CommonOperation();

        public static List<VDS_GROUPS> vdsGroupsList = new List<VDS_GROUPS>();
        public static List<VDS_TYPE> vdsTypeList = new List<VDS_TYPE>();

        public static void GetCommonData()
        {
            vdsGroupsList = GetVDSGroups();
            vdsTypeList = GetVDSTypeList();

        }

        public static List<VDS_GROUPS> GetVDSGroups()
        {
            return commonOp.GetVDSGroupsList(new VDS_GROUPS() { }, out SP_RESULT spResult).ToList();
        }

        public static List<VDS_TYPE> GetVDSTypeList()
        {
            return commonOp.GetVDSTypeList(out SP_RESULT spResult).ToList();
        }

    }
}
