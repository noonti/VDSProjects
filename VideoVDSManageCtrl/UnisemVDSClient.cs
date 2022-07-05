using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;

namespace VideoVDSManageCtrl
{
    public class UnisemVDSClient : VDSClient
    {
        public String vdsControllerId { get; set; }
        public String rtspSourceURL { get; set; }
        public String rtspDetectionURL { get; set; }

        
        public String vdsType { get; set; }



    }
}
