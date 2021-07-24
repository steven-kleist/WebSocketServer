using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebSocketServer
{
    [Serializable()]
    public class EndpointType
    {
        public enum Type
        {
            [XmlEnum("lua")]
            Lua
        }
    }
}
