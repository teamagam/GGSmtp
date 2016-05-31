using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GGSmtp
{
    [DataContract]
    internal class GGImageMetadata
    {
        private const string SOURCE_TYPE = "Sensor";

        [DataMember(Name = "timeStamp")]
        public long TimeStamp { get; set; }

        [DataMember(Name = "hasLocation")]
        public bool HasLocation { get; set; }

        [DataMember(Name = "sourceType")]
        public string SourceType
        {
            get { return SOURCE_TYPE; }
            set { }
        }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
