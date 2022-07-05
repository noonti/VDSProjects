using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class TrafficDataEvent
    {
        public String id { get; set; }
        public String vds_type { get; set; }
        public byte lane { get; set; }
        public byte direction { get; set; }
        public int length { get; set; }    // cm 
        public double speed { get; set; }  // km/h

        [JsonProperty(PropertyName = "class")]
        public int vehicle_class { get; set; }

        public int occupyTime { get; set; } // milisecond 

        public int loop1OccupyTime { get; set; }
        public int loop2OccupyTime { get; set; }

        public String reverseRunYN { get; set; }

        public int vehicleGap { get; set; }

        public String detectTime { get; set; }

        public string reportYN { get; set; }
    }
}
