using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.API.Model
{
    public class VDS_CONTROLLER
    {
        public int ID { get; set; }
	    public String CONTROLLER_ID { get; set; }
        public int GROUP_ID { get; set; }
        public String GROUP_NAME { get; set; }

        public String CONTROLLER_NAME { get; set; }
        public String IP_ADDRESS { get; set; }

        public int STATUS { get; set; }
        public String STATUS_NAME { get; set; }

        public String USE_YN { get; set; }

        public int PROTOCOL { get; set; }
        public String PROTOCOL_NAME { get; set; }

        public int VDS_TYPE { get; set; }
        public String VDS_TYPE_NAME { get; set; }

        public String MODIFY_USER_ID { get; set; }
        public String REG_USER_ID { get; set; }
        public String MODIFY_DATE { get; set; }
        public String REG_DATE { get; set; }

        public String LAST_HEARTBEAT_TIME { get; set; }

        public String VDS_CONFIG { get; set; }

        public int CURRENT_PAGE { get; set; }
        public int PAGE_SIZE { get; set; }


        public RackStatus rackStatus { get; set; }

        public SessionContext sessionContext { get; set; }




        public void SetVDSController(VDS_CONTROLLER controller)
        {

            GROUP_ID = controller.GROUP_ID;
            GROUP_NAME = controller.GROUP_NAME;

            CONTROLLER_NAME = controller.CONTROLLER_NAME;
            IP_ADDRESS = controller.IP_ADDRESS;

            STATUS = controller.STATUS;
            STATUS_NAME = controller.STATUS_NAME;

            USE_YN = controller.USE_YN;

            PROTOCOL = controller.PROTOCOL;
            PROTOCOL_NAME = controller.PROTOCOL_NAME;

            VDS_TYPE = controller.VDS_TYPE;
            VDS_TYPE_NAME = controller.VDS_TYPE_NAME;

        }

    }
}
