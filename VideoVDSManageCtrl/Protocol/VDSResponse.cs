using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoVDSManageCtrl.Protocol
{
    public class VDSResponse 
    {
        public int resultCode { get; set; }
        public String resultMessage { get; set; }

        public void SetProperty(VDSResponse data)
        {
            resultCode = data.resultCode;
            resultMessage = data.resultMessage;
        }
    }
}
