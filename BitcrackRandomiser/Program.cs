using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Services;
using BitcrackRandomiser.Services.Randomiser;
using BitcrackRandomiser.Services.SettingsService;
using BitcrackRandomiser.Services.ShareService;

namespace BitcrackRandomiser
{
    static class Program
    {
        /// <summary>
        /// App is working in any cloud service
        /// </summary>
        public static bool isCloudSearchMode = false;

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Get settings
            var appSettings = SettingsService.GetSettings(args);

            // Edit settings
            Helper.WriteLine($"Press <enter> to edit settings or wait for 3 seconds to load app with <settings.txt>");
            if (!Console.IsInputRedirected)
            {
                bool editSettings = Task.Factory.StartNew(() => Console.ReadLine()).Wait(TimeSpan.FromSeconds(3));
                if (editSettings)
                    appSettings = SettingsService.SetSettings();

                isCloudSearchMode = appSettings.CloudSearchMode;
            }

            // App exit events
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Logger.LogError(null, $"App [{appSettings.AppType}] exited.");
                ShareService.Send(ResultType.workerExited, appSettings);
            };
            Thread.GetDomain().UnhandledException += (s, e) =>
            {
                Logger.LogError((Exception)e.ExceptionObject, $"App [{appSettings.AppType}] occured unhandled exception.");
                ShareService.Send(ResultType.workerExited, appSettings);
            };

            // Send worker start message to telegram or api if active
            ShareService.Send(ResultType.workerStarted, appSettings);

            // Run
            Helper.WriteLine("Please wait while app is starting...", MessageType.normal, true);
            if (appSettings.AppType == AppType.bitcrack)
            {
                Parallel.For(0, appSettings.GPUCount, i =>
                {
                    Randomiser.Scan(appSettings, i);
                });
            }
            else if (appSettings.AppType == AppType.vanitysearch && appSettings.GPUSeperatedRange)
            {
                Parallel.For(0, appSettings.GPUCount, i =>
                {
                    Randomiser.Scan(appSettings, i);
                });
            }
            else if (appSettings.AppType == AppType.vanitysearch || appSettings.AppType == AppType.cpu)
                Randomiser.Scan(appSettings, 0);

            while (true) Console.ReadLine();
        }
    }
}
