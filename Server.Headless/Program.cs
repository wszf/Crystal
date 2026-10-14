using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Server;
using Server.MirEnvir;

namespace Server.Headless
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Packet.IsServer = true;

            // Configure log4net
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(Path.Combine(AppContext.BaseDirectory, "log4net.config")));

            var host = new ConsoleServerHost();
            Envir.Initialise(host);

            host.Log("Starting Legend of Mir Crystal Server (Headless Mode)...");

            try
            {
                // Loading a missing database creates an empty one. Fail clearly instead
                // when the CLI is launched from a directory without game data.
                if (!Server.File.Exists(Envir.DatabasePath) || !Server.Directory.Exists(Settings.MapPath)
                    || !Server.Directory.Exists(Settings.EnvirPath))
                {
                    host.Log($"[CRITICAL] Missing Server.MirDB, Maps or Envir in {System.IO.Directory.GetCurrentDirectory()}. Run from the server data directory.");
                    Environment.ExitCode = 1;
                    return;
                }

                Settings.Load();

                host.Log("Loading Database...");
                bool dbLoaded = Envir.Edit.LoadDB();
                if (!dbLoaded)
                {
                    host.Log("[CRITICAL] Failed to load server database. Exiting.");
                    Environment.ExitCode = 1;
                    return;
                }

                host.Log("Starting Server Environment...");
                Envir.Main.Start();

                host.Log("Server startup requested. Wait for the network-started message. Press Ctrl+C or type 'exit' to stop.");

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true; // Prevent immediate termination
                    host.Log("Shutdown signal received. Commencing graceful termination...");
                    cts.Cancel();
                };

                var commandProcessor = new CommandProcessor(host, cts);
                await commandProcessor.RunAsync();

                host.Log("Stopping Server Environment...");
                Envir.Main.Stop();

                host.Log("Saving configurations and database state...");
                Settings.Save();

                host.Log("Server shutdown complete.");
            }
            catch (Exception ex)
            {
                Logger.GetLogger().Error("Fatal execution error in Headless server host", ex);
                host.Log($"[FATAL] {ex}");
                Environment.ExitCode = 1;
            }
        }
    }
}
