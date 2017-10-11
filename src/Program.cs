using System;
using System.Net;
using System.Text;
using System.Threading;
using SimpleWebServer;

namespace AIS
{
    class Program
    {
        static bool _isActive = true;

        static WebServerRoute[] routes =
        {
            new WebServerRoute("GET", "/test/", GetTestResponse),
            new WebServerRoute("GET", "/test2/{arg}/", GetGenericResponse),
            new WebServerRoute("GET", "/test2/{arg}/{arg2}", GetGenericResponse),
            new WebServerRoute("GET", "/test2/{arg}/{arg2}/{arg3}", GetGenericResponse),
        };

        private static void GetTestResponse(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<HTML><BODY><h1>Sample Test Page</h1><p>{0}</p><p></p>", DateTime.Now);
            int x = 0;
            foreach(var part in requestParts)
            {
                sb.AppendFormat("<code>[{0}] is {1}</code>",x, part);
            }
            sb.Append("</BODY></HTML>");
            WebServer.SetResponse(response, sb.ToString());
        }

        private static void GetGenericResponse(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
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

        protected static WebServerRouter _router = null;
        protected static void CtrlBreakHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Shutdown requested.");
            args.Cancel = true;    // don't pass the key on
            _router.Server.Stop();
            _isActive = false;  // hop out of the console loop below
        }

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CtrlBreakHandler);

            _router = new WebServerRouter(8123, routes);

            // Console.WriteLine("HTTP server running. Press Ctrl-C to exit.");
            _router.Server.Run();

            Console.WriteLine("Server ready. Press Ctrl-C or Ctrl-Break to quit.");
            while (_isActive)
            {
                Thread.Sleep(50);
            }
            Console.WriteLine("Server shutting down.");
            Thread.Sleep(1000); // for now, give the last async request a chance to complete.
        }
    }
}



