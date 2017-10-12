using System;
using System.Threading;
using SimpleWebServer;

namespace AIS
{
    class Program
    {
        protected static bool _isActive = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Avatar Inventory System (AISv3) API Server 0.1  [{0} at {1}]", 
                    DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());

            // Allocate the new API server
            WebServerRouter _router = new WebServerRouter(8123);

            // Add inventory-related API methods
            InventoryMethods _inventoryMethods = new InventoryMethods();
            _inventoryMethods.AddRoutes(_router);

            // Now that the routes are added, start the router.
            _router.Run();

            Console.WriteLine("Server ready. Press Ctrl-C or Ctrl-Break to quit.");
            while (_router.IsRunning)
            {
                Thread.Sleep(50);
            }
            Console.WriteLine("Server shutting down.");
            Thread.Sleep(1000); // for now, give the last async request a chance to complete.
        }
    }
}



