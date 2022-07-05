using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;
using VDSCommon.API.Model;
using VDSDBHandler.DBOperation.VDSManage;

namespace VDSWebAPIServer.Common
{
    public delegate void ChangeVDSGroupsEventDelegate(VDS_GROUPS vdsGroup);

    public static class GlobalCommonData
    {
        static CommonOperation commonOp = new CommonOperation();

        public static List<VDS_GROUPS> vdsGroupsList = new List<VDS_GROUPS>();

        public static int PAGE_SIZE = 50;

        public static void GetCommonData()
        {
            vdsGroupsList = GetVDSGroups();

        }

        public static List<VDS_GROUPS> GetVDSGroups()
        {
            return commonOp.GetVDSGroupsList(new VDS_GROUPS() { }, out SP_RESULT spResult).ToList();
        }

    }
}
