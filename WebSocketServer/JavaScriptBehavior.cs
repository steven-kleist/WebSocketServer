using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebSocketServer
{
    class JavaScriptBehavior : WebSocketBehavior
    {
        protected readonly string BasePath;
        protected readonly string MainFile;
        private readonly Jurassic.ScriptEngine _script = new();
        public JavaScriptBehavior(string basepath, string mainfile)
        {
            BasePath = basepath;
            MainFile = mainfile;

            _script.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(_script));

            Action<string> send = x => Send(x);
            _script.SetGlobalFunction("send", send);

            Action<string> sendall = x => Sessions.Broadcast(x);
            _script.SetGlobalFunction("sendall", sendall);

            _script.SetGlobalFunction("log", new Action<object>(Console.WriteLine));

            _script.Execute(File.ReadAllText(Path.Combine(BasePath, MainFile)));
        }

        protected override void OnOpen()
        {
            var obj = _script.Object.Construct();
            obj["session_id"] = ID;

            try
            {
                _script.CallGlobalFunction("on_open", obj);
            }
            catch (Exception e)
            {
                ReportError(e);
            }
        }


        protected override void OnClose(CloseEventArgs e)
        {
            var obj = _script.Object.Construct();
            obj["code"] = e.Code;
            obj["reason"] = e.Reason;
            obj["was_clean"] = e.WasClean;
            obj["session_id"] = ID;

            try
            {
                _script.CallGlobalFunction("on_close", obj);
            }
            catch (Exception err)
            {
                ReportError(err);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var obj = _script.Object.Construct();
            obj["data"] = e.Data;
            obj["rawdata"] = e.RawData;
            obj["isbinary"] = e.IsBinary;
            obj["isping"] = e.IsPing;
            obj["istext"] = e.IsText;
            obj["session_id"] = ID;


            try
            {
                _script.CallGlobalFunction("on_message", obj);
            }
            catch (Exception err)
            {
                ReportError(err);
            }
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            base.OnError(e);
        }

        private static void ReportError(Exception e)
        {
            Console.WriteLine("Error in js script:");
            Console.Write(e.Message);
        }
    }
}
