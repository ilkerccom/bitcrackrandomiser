using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Helpers;
using System.Reflection;

namespace BitcrackRandomiser
{
    class Program
    {
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

            // Send worker start message to telegram or api if active
            Share.Send(ResultType.workerStarted, appSettings);

            // Run
            Helper.WriteLine("Please wait while app is starting...", MessageType.normal, true);
            Parallel.For(0, appSettings.GPUCount, i =>
            {
                Randomiser.Scan(appSettings, i);
            });
            while (true) Console.ReadLine();
        }
    }
}
