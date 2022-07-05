using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;

namespace VDSController
{
    public partial class tabLogVIewer : UserControl
    {
        public tabLogVIewer()
        {
            InitializeComponent();
        }


        public int AddLog(LOG_TYPE logType, String strLog)
        {
            if (LogList.Items.Count > 1000)
                LogList.Items.RemoveAt(0); // 오래된 것부터 삭제하면서 추가 한다 
            LogList.Items.Add(strLog);
            LogList.SelectedIndex = LogList.Items.Count - 1;
            return LogList.Items.Count;
        }
    }
}
