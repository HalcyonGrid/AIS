using System;
using System.Net;
using System.Threading;
using System.Text;

namespace SimpleWebServer
{
    public delegate void WebHandler(HttpListenerRequest request, HttpListenerResponse response);

    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly WebHandler _handler;

        public WebServer(WebHandler handler, string[] prefixes)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Unsupported platform.");

            // URI prefixes (routes) are required.
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (handler == null)
                throw new ArgumentException("callback");

            Console.CancelKeyPress += new ConsoleCancelEventHandler(CtrlBreakHandler);

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _handler = handler;
        }

        static public void SetResponse(HttpListenerResponse response, HttpStatusCode status, string content)
        {
            response.StatusCode = (int)status;

            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
        }

        static public void SetResponse(HttpListenerResponse response, string content)
        {
            SetResponse(response, HttpStatusCode.OK, content);
        }

        public void SendGenericResponse(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<HTML><BODY><h1>Generic Handler</h1><p>{0}</p><p></p>", DateTime.Now);
            int x = 0;
            foreach (var part in requestParts)
            {
                sb.AppendFormat("<div><code>[{0}] is {1}</code></div>", x++, part);
            }
            sb.Append("</BODY></HTML>");
            WebServer.SetResponse(response, sb.ToString());
        }

        protected void CtrlBreakHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Shutdown requested.");
            args.Cancel = true;    // don't pass the key on
            Stop();
        }

        public bool IsRunning
        {
            get { return (_listener == null) ? false : _listener.IsListening; }
        }

        public void Run()
        {
            _listener.Start();

            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                _handler(ctx.Request, ctx.Response);

                                // Log the request to the console.
                                ConsoleColor prevColor = Console.ForegroundColor;
                                Console.ForegroundColor = (ctx.Response.StatusCode > 299) ? ConsoleColor.Red : ConsoleColor.Green;
                                Console.WriteLine("[" + ctx.Response.StatusCode + "]: " + ctx.Request.RawUrl);
                                Console.ForegroundColor = prevColor;
                            }
                            catch (Exception e) {
                                Console.WriteLine("Exception: " + e.Message);
                            } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
