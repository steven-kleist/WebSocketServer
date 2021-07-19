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
        private Script _script = new Script();
        public LuaBehavior(string basepath)
        {
            _script.Options.ScriptLoader = new FileSystemScriptLoader();
            BasePath = basepath;
            Action<string> send = x => Send(x);
            _script.Globals["send"] = send;

            Action<string> sendall = x => Sessions.Broadcast(x);
            _script.Globals["sendall"] = sendall;

            

            _script.DoFile(Path.Combine(BasePath, "main.lua"));
        }

        protected override void OnOpen()
        {
            var dict = new Dictionary<string, object>
            {
                { "session_id", ID }
            };
            
            try
            {
                _ = _script.Call(_script.Globals["on_open"], dict);
            }
            catch (ScriptRuntimeException err)
            {
                ReportError(err);
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            var dict = new Dictionary<string, object>
            {
                { "code", e.Code },
                { "reason", e.Reason },
                { "was_clean", e.WasClean },
                { "session_id", ID }
            };
           try
            {
                _ = _script.Call(_script.Globals["on_close"], dict);
            }
            catch (ScriptRuntimeException err)
            {
                ReportError(err);
            }
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            var dict = new Dictionary<string, object>
            {
                { "data", e.Data },
                { "rawdata", e.RawData },
                { "isbinary", e.IsBinary },
                { "isping", e.IsPing },
                { "istext", e.IsText },
                { "session_id", ID }
            };

            try
            {
                _ = _script.Call(_script.Globals["on_message"], dict);
            }
            catch (ScriptRuntimeException err)
            {
                ReportError(err);
            }
            

        }

        private static void ReportError(ScriptRuntimeException e)
        {
            Console.WriteLine("Error in Lua script:");
            Console.Write(e.DecoratedMessage);
        }
    }
}
