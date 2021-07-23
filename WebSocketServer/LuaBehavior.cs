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
        private Script _script = new Script();
        public LuaBehavior(string basepath, string mainfile)
        {
            BasePath = basepath;
            MainFile = mainfile;

            _script.Options.ScriptLoader = new FileSystemScriptLoader
            {
                ModulePaths = new string[] { Path.Combine(BasePath, "?.lua") }
            };
            
            Action<string> send = x => Send(x);
            _script.Globals["send"] = send;

            Action<string> sendall = x => Sessions.Broadcast(x);
            _script.Globals["sendall"] = sendall;

            

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
            var dict = new Dictionary<string, object>
            {
                { "session_id", ID }
            };
            
            try
            {
                _ = _script.Call(_script.Globals["on_open"], dict);
            }
            catch (InterpreterException err)
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
            catch (InterpreterException err)
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
                { "is_binary", e.IsBinary },
                { "is_ping", e.IsPing },
                { "is_text", e.IsText },
                { "session_id", ID }
            };

            try
            {
                _ = _script.Call(_script.Globals["on_message"], dict);
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
