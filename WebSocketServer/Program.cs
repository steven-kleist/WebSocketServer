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
            var endpoints = EndpointLoader.Load(options.Endpoints);


            foreach (var e in endpoints)
            {

                if (e.Engine == EndpointType.Type.Lua)
                {
                    httpsvc.AddWebSocketService(e.Url, () => new LuaBehavior(e.BasePath, e.MainScript));
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
