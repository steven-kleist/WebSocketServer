using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebSocketServer
{

    public class LuaBehavior : WebSocketBehavior
    {
        protected readonly string BasePath;
        protected readonly string MainFile;

        private Dictionary<string, object> websocketObject;
        private Script _script = new Script();

        public LuaBehavior(string basepath, string mainfile = "main.lua")
        {
            BasePath = basepath;
            MainFile = mainfile;

            _script.Options.ScriptLoader = new FileSystemScriptLoader
            {
                ModulePaths = new string[] { Path.Combine(BasePath, "?.lua") }
            };

            Action<string> send = x => Send(x);
            Action<string> sendall = x => Sessions.Broadcast(x);

            websocketObject = new Dictionary<string, object> {
                { "send", send },
                { "sendall", sendall }
            };

            _script.Globals["websocket"] = websocketObject;

            try
            {
                _script.DoFile(Path.Combine(BasePath, MainFile));
            }
            catch (InterpreterException e)
            {
                ReportError(e);
            }
        }

        protected override void OnOpen()
        {
            var session = new Dictionary<string, object>
            {
                { "id", ID }
            };

            _script.Globals["session"] = session;
            
            try
            {
                _ = _script.Call(_script.Globals["on_open"]);
            }
            catch (InterpreterException err)
            {
                ReportError(err);
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            var eventData = new Dictionary<string, object>
            {
                { "code", e.Code },
                { "reason", e.Reason },
                { "was_clean", e.WasClean }
            };
           try
            {
                _ = _script.Call(_script.Globals["on_close"], eventData);
            }
            catch (InterpreterException err)
            {
                Error("Error while executing server function", err);
                ReportError(err);
            }
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            var message = new Dictionary<string, object>
            {
                { "data", e.Data },
                { "rawdata", e.RawData },
                { "is_binary", e.IsBinary },
                { "is_ping", e.IsPing },
                { "is_text", e.IsText }
            };

            try
            {
                _ = _script.Call(_script.Globals["on_message"], message);
            }
            catch (InterpreterException err)
            {
                ReportError(err);
            }
            

        }

        private static void ReportError(InterpreterException e)
        {
            Console.WriteLine("Error in Lua script:");
            Console.Write(e.DecoratedMessage);
        }
    }
}
