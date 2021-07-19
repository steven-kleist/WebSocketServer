using System;
using System.IO;
using System.Text;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
using System.Collections.Generic;
using CommandLine;
using HeyRed.Mime;

namespace WebSocketServer
{
    
    class Program
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
            public IEnumerable<string> Endpoints { get; set; }
        }


        protected static HttpServer httpsvc;
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);

        }

        static void RunOptions(Options options)
        {

            httpsvc = new HttpServer($"http://{options.Host}:{options.Port}/")
            {
                RootPath = options.StaticPath
            };


            httpsvc.OnGet += Httpsvc_OnGet;

            Console.WriteLine("Register services...");
            foreach (var item in options.Endpoints)
            {
                var parts = item.Split('=');
                if (parts.Length == 2)
                {
                    httpsvc.AddWebSocketService(parts[0], () => new LuaBehavior(parts[1]));
                    Console.WriteLine("  {0} => '{1}'", parts[0], parts[1]);
                }
                else
                {
                    Console.WriteLine($"Error: {item} is not well formed. Use '/chat=./Endpoints/chat' as an example.");
                }
                
            }

            httpsvc.Start();
            if (httpsvc.IsListening)
            {
                Console.WriteLine($"Listening on http://{httpsvc.Address}:{httpsvc.Port}, and providing WebSocket services:");
            }

            Console.WriteLine("\nPress Enter key to stop the server...");
            Console.ReadLine();

            httpsvc.Stop();
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            
        }


        /// <summary>
        /// EventHandler for sending static files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Httpsvc_OnGet(object sender, HttpRequestEventArgs e)
        {
            var req = e.Request;
            var res = e.Response;

            var path = req.RawUrl;
            if (path == "/")
                path += "index.html";
            
            byte[] contents;
            
            if (!File.Exists(httpsvc.RootPath + path))
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            contents = File.ReadAllBytes(httpsvc.RootPath + path);


            var FileExtensions = new List<string>
            {
                ".html",
                ".js"
            };
            if (FileExtensions.Contains(path.Substring(path.LastIndexOf("."))))
            {
                res.ContentEncoding = Encoding.UTF8;
            }

            var mimeType = MimeTypesMap.GetMimeType(path);

            res.ContentType = mimeType;
            res.ContentLength64 = contents.LongLength;
            res.Close(contents, true);
        }
    }
}
