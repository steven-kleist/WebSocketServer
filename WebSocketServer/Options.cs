using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer
{
    public class Options
    {
        [Option('h', "host", Required = true, HelpText = "Host address to listening on")]
        public string Host { get; set; }

        [Option('p', "port", Required = true, HelpText = "Port to listening on")]
        public int Port { get; set; }

        [Option('s', "staticpath", Required = false, Default = "./Public", HelpText = "Path to serve static files from")]
        public string StaticPath { get; set; }

        [Option('e', "endpoints", Required = true)]
        public string Endpoints { get; set; }
    }
}
