using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SimpleWebServer
{
    public delegate void RouteHandler(HttpListenerRequest request, List<string> requestParts, HttpListenerResponse response);

    public class WebServerRoute
    {
        public String method { get; set; }
        public String path { get; set; }
        public RouteHandler handler { get; set; }

        public WebServerRoute(String m, String p, RouteHandler h)
        { method = m; path = p; handler = h; }
    }

    public class WebServerRouter
    {
        private const String _default_scheme = "http";
        private const String _default_host = "+";       // all local hosts
        private const uint _default_port = 80;


        private String _routeBase = null;
        private WebServerRoute[] _routes = null;
        private WebServer _server = null;
        private char[] _separators = new char[] { '/' };

        public WebServerRouter(String routeBase, WebServerRoute[] routes)
        {
            if ((routes == null) || (routeBase == null))
                throw new System.ArgumentException("WebServerRouter");

            _routeBase = routeBase;
            _routes = routes;

            /* now just passing the routeBase as the only route, so we can use the Router method.
            List<String> paths = new List<string>();
            foreach (var route in routes)
            {
                string[] routeParts = route.path.Split(_separators);
                StringBuilder sb = new StringBuilder(_routeBase + "/");
                for (int x = 0; x < routeParts.Length; x++)
                {
                    String routePart = routeParts[x].Trim();
                    if (routePart.Length > 0)
                    {
                        if (routePart.StartsWith('{') && routePart.EndsWith('}')) // variable part
                            sb.Append("*");
                        else
                            sb.Append(routePart + "/");
                        sb.Append("/");
                    }
                }
                paths.Add(sb.ToString());
            }
            ***/

            _server = new WebServer(Router, new String[] { _routeBase+"/" });
        }

        public WebServerRouter(String scheme, String host, uint port, WebServerRoute[] routes) : this($"{scheme}://{host}:{port}", routes) { }
        public WebServerRouter(String scheme, uint port, WebServerRoute[] routes) : this(scheme, _default_host, port, routes) { }
        public WebServerRouter(uint port, WebServerRoute[] routes) : this(_default_scheme, _default_host, port, routes) { }

        public WebServerRouter(WebServerRoute[] routes) : this(_default_scheme, _default_host, _default_port, routes) { }

        public WebServer Server
        {
            get { return _server; }
        }

        private WebServerRoute RouteMatch(HttpListenerRequest request, out List<string> matchesOut)
        {
            List<string> matches = new List<string>();
            string[] urlParts = request.RawUrl.Split(_separators);
            foreach (var route in _routes)
            {
                string[] routeParts = route.path.Split(_separators);
                if (routeParts.Length != urlParts.Length)
                    continue;
                if (String.Compare(request.HttpMethod, route.method, true) != 0)
                    continue;

                bool isMatch = true;
                matches.Clear();
                for (int x = 0; x < routeParts.Length; x++)
                {
                    String routePart = routeParts[x].Trim();
                    if (String.Compare(routePart, urlParts[x], true) == 0)
                    {
                        if (routePart.Length > 0)
                            matches.Add(routePart);
                        continue;  // this part is a match
                    }

                    // check if variable placeholder
                    if (routePart.StartsWith('{') && routePart.EndsWith('}'))
                    {
                        // it's a match on a variable part
                        // matches[routePart.Substring(1,routePart.Length-2)] = urlParts[x];
                        matches.Add(urlParts[x]);
                        continue;
                    }

                    // otherwise it's not a match
                    isMatch = false;
                    break;
                }
                if (isMatch)
                {
                    matchesOut = matches;
                    return route;
                }
            }

            matches.Clear();
            matchesOut = matches;
            return null;
        }

        public void Router(HttpListenerRequest request, HttpListenerResponse response)
        {
            List<string> matches;
            WebServerRoute route = RouteMatch(request, out matches);
            if (route == null)
                WebServer.SetResponse(response, HttpStatusCode.NotFound, "");
            else
                route.handler(request, matches, response);
        }

    }

}
