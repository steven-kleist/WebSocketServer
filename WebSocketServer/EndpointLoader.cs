using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebSocketServer
{
    class EndpointLoader
    {
        public static List<EndpointConfiguration> Load(string searchpath)
        {
            var list = new List<EndpointConfiguration>();
            var files = Directory.GetFiles(searchpath, "endpoint.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileinfo = new FileInfo(file);
                var xs = new XmlSerializer(typeof(EndpointConfiguration));
                var sr = new StreamReader(fileinfo.FullName);
                var point = (EndpointConfiguration)xs.Deserialize(sr);
                point.BasePath = fileinfo.DirectoryName;
                sr.Close();

                list.Add(point);
            }

            return list;
        }
    }
}
