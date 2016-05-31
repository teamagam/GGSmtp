using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GGSmtp
{
    [DataContract]
    internal class GGMessage
    {
        private const string MESSAGE_TYPE = "Image";

        [DataMember(Name = "senderId")]
        public string SenderId { get; set; }

        [DataMember(Name = "type")]
        public string Type
        {
            get { return MESSAGE_TYPE; }
            set { }
        }

        [DataMember(Name = "content")]
        public GGImageMetadata Content { get; set; }
    }
}
