using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;
using VDSCommon.API.Model;
using VDSDBHandler.DBOperation.VDSManage;
using VDSDBHandler.Model;

namespace VDSController.Global
{
    public static class GlobalCommonData
    {
        static CommonOperation commonOp = new CommonOperation();

        public static List<VDS_GROUPS> vdsGroupsList = new List<VDS_GROUPS>();
        public static List<VDS_TYPE> vdsTypeList = new List<VDS_TYPE>();
        public static List<KorexOffice> korexOfficeList = new List<KorexOffice>();
        public static List<LANE_GROUP> laneGroupList = new List<LANE_GROUP>();

        public static void GetCommonData()
        {
            vdsGroupsList = GetVDSGroups();
            vdsTypeList = GetVDSTypeList();
            GetKorexOfficeList();

        }

        public static List<VDS_GROUPS> GetVDSGroups()
        {
            return commonOp.GetVDSGroupsList(new VDS_GROUPS() { }, out SP_RESULT spResult).ToList();
        }

        public static List<VDS_TYPE> GetVDSTypeList()
        {
            return commonOp.GetVDSTypeList(out SP_RESULT spResult).ToList();
        }

        public static void GetKorexOfficeList()
        {
            var officeList = commonOp.GetKorexOfficeList(out SP_RESULT spResult);
            if (officeList != null)
            {
                foreach (var office in officeList)
                {
                    korexOfficeList.Add(new KorexOffice()
                    {
                        Id = office.ID,
                        OfficeCode = office.OFFICE_CODE,
                        OfficeName = office.OFFICE_NAME,
                        OfficerName = office.OFFICER_NAME,
                        TelNo = office.TEL_NO,
                        RegDate = office.REG_DATE

                    });

                }
            }
        }

    }
}
