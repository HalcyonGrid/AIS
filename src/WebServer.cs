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

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _handler = handler;
            _listener.Start();
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

        public void Run()
        {
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
