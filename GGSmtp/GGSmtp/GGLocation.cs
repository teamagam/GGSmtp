using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GGSmtp
{
    [DataContract]
    internal class GGLocation
    {
        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "altitude")]
        public double Altitude { get; set; }

        [DataMember(Name = "hasAltitude")]
        public bool HasAltitude { get; set; }
    }
}
