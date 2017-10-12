using System;
using System.Collections.Generic;
using System.Net;
using log4net;

namespace SimpleAPIServer
{
    public delegate void RouteHandler(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response);

    public class APIRoute
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public RouteHandler Handler { get; set; }

        public APIRoute(string m, string p, RouteHandler h)
        { Method = m; Path = p; Handler = h; }
    }

    public class APIRouter
    {
        private static readonly ILog m_log = LogManager.GetLogger(typeof(APIRouter));

        // URLs are of the form "http://+/api/command/arg" so the base path is typically "http://+" for all local hosts.
        private const string _default_scheme = "http";
        private const string _default_host = "+";       // all local hosts
        private const uint _default_port = 80;
        private string _routeBase = null;
        private List<APIRoute> _routes = new List<APIRoute>();
        private APIServer _server = null;

        private char[] _separators = new char[] { '/' };

        public APIRouter(string routeBase)
        {
            if (routeBase == null)
                throw new ArgumentException("routeBase");

            _routeBase = routeBase;
            _server = new APIServer(Router, new string[] { _routeBase + "/" });
        }

        public APIRouter(string scheme, string host, uint port) : this($"{scheme}://{host}:{port}") { }
        public APIRouter(string scheme, uint port) : this(scheme, _default_host, port) { }
        public APIRouter(uint port) : this(_default_scheme, _default_host, port) { }
        public APIRouter() : this(_default_scheme, _default_host, _default_port) { }

        public APIServer Server
        {
            get { return _server; }
        }

        public void AddRoutes(APIRoute[] routes)
        {
            if (routes == null)
                throw new System.ArgumentException("routes");

            foreach (var route in routes)
                _routes.Add(route);
        }
        public void AddRoutes(List<APIRoute> routes)
        {
            if (routes == null)
                throw new System.ArgumentException("routes");

            foreach (var route in routes)
                _routes.Add(route);
        }
        public void AddRoute(APIRoute route)
        {
            _routes.Add(route);
        }
        public void AddRoute(string method, string path, RouteHandler handler)
        {
            AddRoute(new APIRoute(method, path, handler));
        }

        private void AddTestRoutes()
        {
            // Some routes for testing the REST API
            AddRoute("GET", "/test/", _server.SendGenericResponse);
            AddRoute("GET", "/test/{arg}", _server.SendGenericResponse);
            AddRoute("GET", "/test/{arg}/{arg2}", _server.SendGenericResponse);
            AddRoute("GET", "/test/{arg}/{arg2}/{arg3}", _server.SendGenericResponse);
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

        private APIRoute RouteMatch(HttpListenerRequest request, out string[] matchesOut)
        {
            int queryStart = request.RawUrl.IndexOf('?');
            string pathToUse = (queryStart < 0) ? request.RawUrl : request.RawUrl.Substring(0, queryStart);
            string[] urlParts = ParsePathForParts(pathToUse);

            foreach (var route in _routes)
            {
                // HTTP request method must match or be "ALL" in the route.
                if (string.Compare(route.Method, "ALL", true) != 0)
                    if (string.Compare(request.HttpMethod, route.Method, true) != 0)
                        continue;

                string[] routeParts = ParsePathForParts(route.Path);
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
                    if (routePart.StartsWith("{") && routePart.EndsWith("}"))
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

        public bool IsRunning
        {
            get { return (_server == null) ? false : _server.IsRunning; }
        }

        public void Run()
        {
            AddTestRoutes();

            m_log.Info("Starting server with routes:");
            foreach (var route in _routes)
                m_log.Info("  " + route.Method + " " + route.Path);
            _server.Run();
        }

        public void Router(HttpListenerRequest request, HttpListenerResponse response)
        {
            string[] matches;
            APIRoute route = RouteMatch(request, out matches);
            if (route == null)
                APIServer.SetResponse(response, HttpStatusCode.NotFound, "");
            else
                route.Handler(request, matches, response);
        }
    }
}
