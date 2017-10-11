using System.Net;
using OpenMetaverse;
using OpenSim.Data;
using InWorldz.Data.Inventory.Cassandra;
using SimpleWebServer;

namespace AIS
{
    class InventoryMethods
    {
        private static UUID _Id;

        private static IInventoryStorage _storage;

        private static InventoryStorage _cassandraStorage;
        private static CassandraMigrationProviderSelector _selector;

        // "GET", "/category/{category}",           HandleGetCategory
        public static void HandleGetCategory(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            WebServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public static void HandleCategory(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                WebServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/category/{category}/children",  HandleGetCategoryChildren
        public static void HandleGetCategoryChildren(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            WebServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public static void HandleCategoryChildren(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategoryChildren(request, requestParts, response);
            else
                WebServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/category/{category}/links",     HandleGetCategoryLinks
        public static void HandleGetCategoryLinks(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            WebServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public static void HandleCategoryLinks(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                WebServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/category/{category}/items",     HandleGetCategoryItems
        public static void HandleGetCategoryItems(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            WebServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public static void HandleCategoryItems(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                WebServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/category/{category}/categories",HandleGetCategoryCategories
        public static void HandleGetCategoryCategories(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            WebServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public static void HandleCategoryCategories(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                WebServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }

        // "GET", "/item/{item}",                   HandleGetItem
        public static void HandleGetItem(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            WebServer.SetResponse(response, HttpStatusCode.OK, "OK " + request.RawUrl);
        }
        public static void HandleItem(HttpListenerRequest request, string[] requestParts, HttpListenerResponse response)
        {
            if (string.Compare(request.HttpMethod, "GET", true) == 0)
                HandleGetCategory(request, requestParts, response);
            else
                WebServer.SetResponse(response, HttpStatusCode.MethodNotAllowed, request.RawUrl);
        }
    }
}
