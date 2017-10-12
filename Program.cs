using System;
using System.Threading;
using SimpleAPIServer;
using log4net;
using log4net.Config;
using Nini.Config;

namespace AIS
{
    class Program
    {
        private static readonly ILog m_log = LogManager.GetLogger(typeof(Program));

        protected static bool _isActive = true;

        static void Main(string[] args)
        {
            ArgvConfigSource options = new ArgvConfigSource(args);

            // Initialize log4net
            XmlConfigurator.Configure();

            m_log.InfoFormat("Avatar Inventory System (AISv3) API Server 0.1  [{0} at {1}]", 
                    DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());

            // Allocate the new API server
            APIRouter _router = new APIRouter(8123);

            // Add inventory-related API methods
            InventoryAPI _inventoryMethods = new InventoryAPI(options);
            _inventoryMethods.AddRoutes(_router);

            // Now that the routes are added, start the router.
            _router.Run();

            m_log.Info("Server ready.");
            Console.WriteLine("Press Ctrl-C or Ctrl-Break to quit.");
            while (_router.IsRunning)
            {
                Thread.Sleep(50);
            }
            m_log.Info("Server shutting down.");
            Thread.Sleep(1000); // for now, give the last async request a chance to complete.
        }
    }
}



