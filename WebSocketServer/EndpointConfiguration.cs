using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebSocketServer
{
    [Serializable()]
    [XmlRoot("Endpoint")]
    public class EndpointConfiguration
    {
        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("engine")]
        public EndpointType.Type Engine { get; set; }

        [XmlElement("main")]
        public string MainScript { get; set; }

        public string BasePath { get; set; }
    }
}
