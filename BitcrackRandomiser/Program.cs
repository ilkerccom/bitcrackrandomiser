using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Helpers;

namespace BitcrackRandomiser
{
    class Program
    {
        /// <summary>
        /// Reward list
        /// </summary>
        public static List<string> rewardAddresses = new();

        /// <summary>
        /// Error logging
        /// </summary>
        public static bool isLoggingEnabled = false;

        /// <summary>
        /// Bitcrackrandomiser
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Get settings
            var appSettings = Settings.GetSettings(args);

            // Edit settings
            Helper.WriteLine($"Press <enter> to edit settings or wait for 3 seconds to load app with <settings.txt>");
            if (!Console.IsInputRedirected)
            {
                bool editSettings = Task.Factory.StartNew(() => Console.ReadLine()).Wait(TimeSpan.FromSeconds(3));
                if (editSettings)
                    appSettings = Settings.SetSettings();
            }

            // Logging status
            isLoggingEnabled = appSettings.EnableLogging;

            // App exit events
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Logger.LogError(null, $"App [{appSettings.AppType}] exited.");
                Share.Send(ResultType.workerExited, appSettings);
            };
            Thread.GetDomain().UnhandledException += (s, e) =>
            {
                Logger.LogError((Exception)e.ExceptionObject, $"App [{appSettings.AppType}] occured unhandled exception.");
                Share.Send(ResultType.workerExited, appSettings);
            };

            // Send worker start message to telegram or api if active
            Share.Send(ResultType.workerStarted, appSettings);

            // Get rewards from pool for current puzzle
            if (appSettings.ScanRewards)
                rewardAddresses = Requests.GetRewards(appSettings.TargetPuzzle).Result.Split("|")
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct().ToList();

            // Run
            Helper.WriteLine("Please wait while app is starting...", MessageType.normal, true);
            if (appSettings.AppType == AppType.bitcrack)
            {
                Parallel.For(0, appSettings.GPUCount, i =>
                {
                    Randomiser.Scan(appSettings, i);
                });
            }
            else if(appSettings.AppType == AppType.vanitysearch && appSettings.GPUSeperatedRange)
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
