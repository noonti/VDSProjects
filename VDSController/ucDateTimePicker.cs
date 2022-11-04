using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VDSController
{
    public partial class ucDateTimePicker : UserControl
    {
        public ucDateTimePicker()
        {
            InitializeComponent();
        }

        public String GetDateTime()
        {
            String result =  String.Empty;
            var date = dtDate.Value.ToString("yyyyMMdd");
            var time = dtTime.Value.ToString("HHmmss");
            result = String.Format($"{date}{time}"); 
            return result;
        }

        public String GetDateTimeFormat()
        {
            String result = String.Empty;
            var date = dtDate.Value.ToString("yyyy-MM-dd");
            var time = dtTime.Value.ToString("HH:mm:ss.00");
            result = String.Format($"{date} {time}");
            return result;
        }

        public void SetDateTime(String date)
        {
            try
            {
                dtDate.Value = DateTime.ParseExact(date, "yyyyMMddHHmmss", null);
                dtTime.Value = DateTime.ParseExact(date, "yyyyMMddHHmmss", null);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }

        private void dtTime_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
