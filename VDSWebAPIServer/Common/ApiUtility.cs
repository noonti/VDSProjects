using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon.API.Model;

namespace VDSWebAPIServer.Common
{
    public static class ApiUtility
    {
        public static void FillVDSGroupsComboBox(List<VDS_GROUPS> dataList, ComboBox cbData)
        {
            cbData.Items.Clear();

            foreach (var data in dataList)
            {
                cbData.Items.Add(data.TITLE);
            }
        }

        public static int GetTotalPageCount(int resultCount, int pageSize)
        {
            int result = 1;

            if (resultCount % pageSize == 0)
                result = (int)(resultCount / pageSize);
            else
                result = (int)(resultCount / pageSize) + 1;

            if (result == 0)
                result = 1;
            return result;
        }

    }
}
