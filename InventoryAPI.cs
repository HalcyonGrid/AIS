using System;
using System.Net;
using log4net;
using OpenMetaverse;
using OpenSim.Data;
using InWorldz.Data.Inventory.Cassandra;
using SimpleAPIServer;
using Nini.Config;

namespace AIS
{
    class InventoryAPI
    {
        private static readonly ILog m_log = LogManager.GetLogger(typeof(InventoryAPI));

        static private string _cluster = Properties.Settings.Default.cassandraCluster;
        static private string _connstring = Properties.Settings.Default.coreConnStr;

        private UUID _Id;

        private InventoryStorage _cassandraStorage;
        private CassandraMigrationProviderSelector _selector;
        private LegacyMysqlInventoryStorage _legacy;

        private IInventoryStorage _storage; // per-user specific storage reference

        public InventoryAPI(ArgvConfigSource options)
        {
            options.AddSwitch("Inventory", "local", "l");
            string useLocal = options.Configs["Inventory"].Get("local");
            if (useLocal != null)   // any value include "" will do
            {
                _cluster = Properties.LocalSettings.Default.cassandraCluster;
                _connstring = Properties.LocalSettings.Default.coreConnStr;
                m_log.Warn("Using LOCAL settings.");
            }


            try
            {
                _cassandraStorage = new InventoryStorage(_cluster);
                _legacy = new LegacyMysqlInventoryStorage(_connstring);
                _selector = new CassandraMigrationProviderSelector(true, _connstring, _cassandraStorage, _legacy);
                m_log.InfoFormat("Cassandra support on '{0}' enabled and ready.", _cluster);
            }
            catch (Exception e)
            {
                m_log.ErrorFormat("Unable to connect to cassandra cluster: {0}", e);
            }
        }

        public void AddRoutes(APIRouter router)
        {
            APIRoute[] inventoryRoutes =
            {
                new APIRoute("ALL", "/category/{category}",            HandleCategory),
                new APIRoute("ALL", "/category/{category}/children",   HandleCategoryChildren),
                new APIRoute("ALL", "/category/{category}/links",      HandleCategoryLinks),
                new APIRoute("ALL", "/category/{category}/items",      HandleCategoryItems),
                new APIRoute("ALL", "/category/{category}/categories", HandleCategoryCategories),
                new APIRoute("ALL", "/item/{item}",                    HandleItem),
            };

            router.AddRoutes(inventoryRoutes);
        }

        // "GET", "/category/{category}",           HandleGetCategory
        public void HandleGetCategory(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            APIServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public void HandleCategory(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                APIServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/category/{category}/children",  HandleGetCategoryChildren
        public void HandleGetCategoryChildren(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            APIServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public void HandleCategoryChildren(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategoryChildren(request, requestParts, response);
            else
                APIServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/category/{category}/links",     HandleGetCategoryLinks
        public void HandleGetCategoryLinks(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            APIServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public void HandleCategoryLinks(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                APIServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/category/{category}/items",     HandleGetCategoryItems
        public void HandleGetCategoryItems(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            APIServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public void HandleCategoryItems(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                APIServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/category/{category}/categories",HandleGetCategoryCategories
        public void HandleGetCategoryCategories(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            APIServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public void HandleCategoryCategories(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                APIServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/item/{item}",                   HandleGetItem
        public void HandleGetItem(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            APIServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public void HandleItem(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                APIServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }
    }
}
