using System.Collections.Generic;
using System.Net;

namespace SimpleWebServer
{
    public delegate void RouteHandler(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response);

    public class WebServerRoute
    {
        public string method { get; set; }
        public string path { get; set; }
        public RouteHandler handler { get; set; }

        public WebServerRoute(string m, string p, RouteHandler h)
        { method = m; path = p; handler = h; }
    }

    public class WebServerRouter
    {
        // URLs are of the form "http://+/api/command/arg" so the base path is typically "http://+" for all local hosts.
        private const string _default_scheme = "http";
        private const string _default_host = "+";       // all local hosts
        private const uint _default_port = 80;

        private char[] _separators = new char[] { '/' };

        private string _routeBase = null;
        private WebServerRoute[] _routes = null;
        private WebServer _server = null;

        public WebServerRouter(string routeBase, WebServerRoute[] routes)
        {
            if ((routes == null) || (routeBase == null))
                throw new System.ArgumentException("WebServerRouter");

            _routeBase = routeBase;
            _routes = routes;

            _server = new WebServer(Router, new string[] { _routeBase+"/" });
        }

        public WebServerRouter(string scheme, string host, uint port, WebServerRoute[] routes) : this($"{scheme}://{host}:{port}", routes) { }
        public WebServerRouter(string scheme, uint port, WebServerRoute[] routes) : this(scheme, _default_host, port, routes) { }
        public WebServerRouter(uint port, WebServerRoute[] routes) : this(_default_scheme, _default_host, port, routes) { }

        public WebServerRouter(WebServerRoute[] routes) : this(_default_scheme, _default_host, _default_port, routes) { }

        public WebServer Server
        {
            get { return _server; }
        }

        private string[] ParsePathForParts(string path)
        {
            // this does things like choping /api/command/arg into ["api","command","arg"]
            List<string> matches = new List<string>();
            string[] tempParts = path.Split(_separators);

            for (int x = 0; x < tempParts.Length; x++)
            {
                string tempPart = tempParts[x].Trim();
                if (tempPart.Length > 0)
                {
                    matches.Add(tempPart);
                }
            }
            return matches.ToArray();
        }

        private WebServerRoute RouteMatch(HttpListenerRequest request, out string[] matchesOut)
        {
            string[] urlParts = ParsePathForParts(request.RawUrl);
            foreach (var route in _routes)
            {
                // HTTP request method must match or be "ALL" in the route.
                if (string.Compare(route.method, "ALL", true) != 0)
                    if (string.Compare(request.HttpMethod, route.method, true) != 0)
                        continue;

                string[] routeParts = ParsePathForParts(route.path);
                if (routeParts.Length != urlParts.Length)
                    continue;

                bool isMatch = true;
                for (int x = 0; x < routeParts.Length; x++)
                {
                    string routePart = routeParts[x].Trim();
                    if (string.Compare(routePart, urlParts[x], true) == 0)
                    {
                        continue;  // this part is a match
                    }

                    // check if variable placeholder
                    if (routePart.StartsWith('{') && routePart.EndsWith('}'))
                    {
                        // it's a match on a variable part
                        continue;
                    }

                    // otherwise it's not a match
                    isMatch = false;
                    break;
                }
                if (isMatch)
                {
                    matchesOut = urlParts;
                    return route;
                }
            }

            matchesOut = new string[] { };
            return null;
        }

        public void Router(HttpListenerRequest request, HttpListenerResponse response)
        {
            string[] matches;
            WebServerRoute route = RouteMatch(request, out matches);
            if (route == null)
                WebServer.SetResponse(response, HttpStatusCode.NotFound, "");
            else
                route.handler(request, matches, response);
        }

    }

}
