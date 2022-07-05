using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon.API.Model
{
    public class VDS_CONFIG
    {
        public int ID { get; set; }
        public String CONTROLLER_ID { get; set; }

        public int CENTER_TYPE { get; set; }
        public String IP_ADDRESS { get; set; }
        public int CTRL_PORT { get; set; }
        public int CALIB_PORT { get; set; }

        public String VDS_ID { get; set; }
        public String DB_ADDRESS { get; set; }
        public int DB_PORT { get; set; }
        public String DB_NAME { get; set; }
        public String DB_USER { get; set; }
        public String DB_PASSWD { get; set; }
        public String CENTER_ADDRESS { get; set; }
        public int CENTER_PORT { get; set; }
        public int VDS_TYPE { get; set; }
        public int SENSOR_COUNT { get; set; }
        public String DEVICE_ADDRESS { get; set; }
        public int DEVICE_PORT { get; set; }
        public int LOCAL_PORT { get; set; }
        public float CHECK_DISTANCE { get; set; }
        public String RTSP_STREAMING_URL { get; set; }
        public String USE_ANIMATION { get; set; }
        public String MODIFY_USER_ID { get; set; }
        public String REG_USER_ID { get; set; }
        public DateTime MODIFY_DATE { get; set; }
        public DateTime REG_DATE { get; set; }

        
    }
}
