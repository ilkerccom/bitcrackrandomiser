using BitcrackRandomiser.Enums;
using System.Diagnostics;
using System.Reflection;

namespace BitcrackRandomiser
{
    class Program
    {
        // Found private key
        public static string privateKey = "";

        // Is proof of work key
        public static bool isProofKey = false;

        // Proof of work keys list.
        public static string[] proofKeys = new string[16] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        // GPU Model name
        public static string gpuName = "-";

        // Check if app started
        public static bool appStarted = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Get settings
            var appSettings = Settings.GetSettings(args);

            // Edit settings
            Helpers.WriteLine(string.Format("Press <enter> to edit settings or wait for {0} seconds to load app with <settings.txt>", 3));
            if (!Console.IsInputRedirected)
            {
                bool editSettings = Task.Factory.StartNew(() => Console.ReadLine()).Wait(TimeSpan.FromSeconds(3));
                if (editSettings)
                    appSettings = Settings.SetSettings();
            }

            // Send worker start message to telegram or api if active
            Helpers.ShareData(ResultType.workerStarted, appSettings);

            // Run
            Helpers.WriteLine("Please wait while app is starting...", MessageType.normal, true);
            Parallel.For(0, appSettings.GPUCount, i =>
            {
                RunBitcrack(appSettings, i);
            });
            while (true)
            {
                Console.ReadLine();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static Task<int> RunBitcrack(Settings settings, int gpuIndex)
        {
            // Check important area
            if (!settings.TelegramShare && settings.UntrustedComputer && !settings.IsApiShare)
            {
                Helpers.WriteLine("If the 'untrusted_computer' setting is 'true', the private key will only be sent to your Telegram address. Please change the 'telegram_share' to 'true' in settings.txt. Then enter your 'access token' and 'chat id'. Otherwise, even if the private key is found, you will not be able to see it anywhere!", MessageType.error, true);
                Thread.Sleep(10000);
            }

            // Get random HEX value 
            string hex = Requests.GetHex(settings).Result;
            string targetAddress =
                settings.TargetPuzzle == "38" ? "1HBtApAFA9B2YZw3G2YKSMCtb3dVnjuNe2" :
                settings.TargetPuzzle == "66" ? "13zb1hQbWVsc2S7ZTZnP2G4undNNpdh5so" :
                settings.TargetPuzzle == "67" ? "1BY8GQbnueYofwSuFAT3USAhGjPrkxDdW9" :
                settings.TargetPuzzle == "68" ? "1MVDYgVaSN6iKKEsbzRUAYFrYJadLYZvvZ" : "Unknown";

            // Cannot get HEX value
            if (hex == "")
            {
                Helpers.WriteLine("Database connection error. Please wait...", MessageType.error);
                Thread.Sleep(5000);
                RunBitcrack(settings, gpuIndex);
                return Task.FromResult(0);
            }

            // Check for errors
            switch (hex)
            {
                case "INVALID_USER_TOKEN":
                    Helpers.WriteLine("Invalid user token value or wallet address.", MessageType.error);
                    return Task.FromResult(0);
                case "NOT_ELIGIBLE_FOR_FREE":
                    Helpers.WriteLine("You are not eligible for free tier. For more information, log in to your account at btcpuzzle.info", MessageType.error);
                    return Task.FromResult(0);
                case "INVALID_PRIVATE_POOL_USER":
                    Helpers.WriteLine("Only the user who created the private pool can join the private pool. Please check your user_token and wallet_address value.", MessageType.error);
                    return Task.FromResult(0);
                case "INVALID_PRIVATE_POOL":
                    Helpers.WriteLine("Invalid private pool. There is no such private pool. Check your private_pool value.", MessageType.error);
                    return Task.FromResult(0);
                case "REACHED_OF_KEYSPACE":
                    Helpers.WriteLine("Reached of keyspace. No ranges left to scan.");
                    Helpers.ShareData(ResultType.reachedOfKeySpace, settings);
                    return Task.FromResult(0);
                default:
                    break;
            }

            // Parse hex result
            string randomHex = hex.Split(':')[0];
            List<string> proofValues = hex.Split(':').Skip(1).ToList();
            if (proofValues.Contains(targetAddress))
            {
                // Impossible but, may be proof value == target address?
                privateKey = hex.Split(':')[2];
                JobFinished(targetAddress, randomHex, settings, keyFound: true, gpuIndex);
                return Task.FromResult(0);
            }

            // Add +1 to random HEX value
            int startNumber = int.Parse(randomHex, System.Globalization.NumberStyles.HexNumber);
            int endNumber = startNumber + 1;

            // Convert numbers to HEX
            string startHex = randomHex;
            string endHex = endNumber.ToString("X");

            // Testing
            if (settings.TestMode)
            {
                // ~1min on 3090
                targetAddress = "1Cnrx6rxiGvVNw1UroYM5hRjVvqPnWC7fR";
                startHex = "2012E83";
                endHex = "2012E84";

                // Test with custom settings
                string customTestFile = AppDomain.CurrentDomain.BaseDirectory + "customtest.txt";
                if (File.Exists(customTestFile))
                {
                    string[] lines = File.ReadAllLines(customTestFile);
                    if (lines.Length == 3)
                    {
                        targetAddress = lines[0];
                        startHex = lines[1];
                        endHex = lines[2];
                    }
                }
            }

            // Write info
            if (!appStarted)
            {
                Helpers.WriteLine(string.Format("[v{1}] {2} starting... Puzzle: [{0}]", settings.TestMode ? "TEST" : settings.IsPrivatePool ? settings.PrivatePool : settings.TargetPuzzle, Assembly.GetEntryAssembly()?.GetName().Version, settings.AppType.ToString().ToUpper()), MessageType.normal, true);
                //Helpers.WriteLine(string.Format("HEX range: {0}-{1} / Total GPU(s): {2}", startHex, endHex, settings.GPUCount));
                Helpers.WriteLine(string.Format("Target address: {0}", targetAddress));
                if (settings.TestMode) Helpers.WriteLine("Test mode is active.", MessageType.error);
                else if (settings.TargetPuzzle == "38") Helpers.WriteLine("Test pool 38 is active.", MessageType.error);
                else Helpers.WriteLine("Test mode is passive.", MessageType.info);
                Helpers.WriteLine(string.Format("Scan type: {0}", settings.ScanType.ToString()), MessageType.info);
                Helpers.WriteLine(string.Format("API share: {0} / Telegram share: {1}", settings.IsApiShare, settings.TelegramShare), MessageType.info);
                Helpers.WriteLine(string.Format("Untrusted computer: {0}", settings.UntrustedComputer), MessageType.info);
                Helpers.WriteLine(string.Format("Progress: {0}", "Visit the <btcpuzzle.info> for statistics."));
                Helpers.WriteLine(string.Format("Your wallet/worker name: {0}", settings.WalletAddress));

                appStarted = true;
            }

            // App arguments
            string appArguments = "";
            if (settings.AppType == AppType.bitcrack)
            {
                string Zeros = (settings.TargetPuzzle == "38" ? new String('0', 8) : new String('0', 10));
                appArguments = string.Format("{3} --keyspace {0}{4}:{1}{4} {2} {5} -d {6}", startHex, endHex, targetAddress, settings.AppArgs, Zeros, string.Join(' ', proofValues), settings.GPUCount > 1 ? gpuIndex : settings.GPUIndex);
            }

            // Tcs
            var taskCompletionSource = new TaskCompletionSource<int>();

            // Proccess info
            var process = new Process
            {
                StartInfo = { FileName = settings.AppPath, RedirectStandardError = true, RedirectStandardOutput = true, Arguments = appArguments },
                EnableRaisingEvents = true
            };

            // Output from BitCrack
            process.ErrorDataReceived += (object o, DataReceivedEventArgs s) => OutputReceivedHandler(o, s, targetAddress, proofValues, startHex, settings, process, gpuIndex);
            process.OutputDataReceived += (object o, DataReceivedEventArgs s) => OutputReceivedHandler(o, s, targetAddress, proofValues, startHex, settings, process, gpuIndex);

            // App exited
            process.Exited += (sender, args) =>
            {
                if (process.ExitCode != 0)
                {
                    Helpers.ShareData(ResultType.workerExited, settings);
                }
                taskCompletionSource.SetResult(process.ExitCode);
                process.Dispose();
            };

            // Start the app
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Runs on scan completed or private key found
        /// </summary>
        /// <param name="targetAddress">Target address</param>
        /// <param name="hex">HEX range</param>
        /// <param name="settings">Current settings</param>
        /// <param name="KeyFound">Key found or not</param>
        private static void JobFinished(string targetAddress, string hex, Settings settings, bool keyFound = false, int gpuIndex = 0)
        {
            if (keyFound)
            {
                // Always send notification when key found
                Helpers.ShareData(ResultType.keyFound, settings, privateKey);

                // Not on untrusted computer
                if (!settings.UntrustedComputer)
                {
                    Console.WriteLine(Environment.NewLine);
                    Helpers.WriteLine(privateKey, MessageType.success);
                    Helpers.SaveFile(privateKey, targetAddress);
                }

                Helpers.WriteLine("Congratulations. Key found. Please check your folder.", MessageType.success);
                Helpers.WriteLine("You can donate me; 1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw", MessageType.success);
            }
            else
            {
                // Send notification each key scanned
                Helpers.ShareData(ResultType.rangeScanned, settings, hex);

                // Flag HEX as used
                FlagAsScanned(settings, hex, gpuIndex);

                // Wait and restart
                proofKeys[gpuIndex] = "";
                isProofKey = false;
                Thread.Sleep(5000);
                RunBitcrack(settings, gpuIndex);
            }
        }

        /// <summary>
        /// Flag HEX as scanned
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="HEX"></param>
        /// <returns></returns>
        public static bool FlagAsScanned(Settings settings, string hex, int gpuIndex)
        {
            // Hash all proof keys with SHA256
            string hashedProofKey = Helpers.SHA256Hash(proofKeys[gpuIndex]);

            // Try flag
            string walletAddress = settings.WalletAddress;
            if (settings.GPUCount > 1)
                walletAddress += "_" + gpuIndex;
            bool flagUsed = Requests.SetHex(hex, walletAddress, hashedProofKey, gpuName, settings.PrivatePool, settings.TargetPuzzle).Result;

            // Try flagging
            int flagTries = 1;
            int maxTries = 6;
            while (!flagUsed && flagTries <= maxTries)
            {
                flagUsed = Requests.SetHex(hex, settings.WalletAddress, hashedProofKey, gpuName, settings.TargetPuzzle).Result;
                Helpers.WriteLine(string.Format("Flag error... Retrying... {0}/{1} [GPU{2}]", flagTries, maxTries, gpuIndex), MessageType.externalApp, gpuIndex: gpuIndex);
                Thread.Sleep(10000);
                flagTries++;
            }

            // Info
            if (flagUsed)
                Helpers.WriteLine(string.Format("[{1}] scanned successfully... Launching again... [GPU{0}]", gpuIndex, hex), MessageType.externalApp, gpuIndex: gpuIndex);
            else
                Helpers.WriteLine(string.Format("[{1}] scanned with flag error... Launching again... [GPU{0}]", gpuIndex, hex), MessageType.externalApp, gpuIndex: gpuIndex);

            return flagUsed;
        }

        /// <summary>
        /// Runs when output data received by external app
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <param name="targetAddress">Target address</param>
        /// <param name="proofValues">Proof values</param>
        /// <param name="HEX">Selected HEX range</param>
        /// <param name="settings">Current settings</param>
        /// <param name="process">Active proccess</param>
        public static void OutputReceivedHandler(object o, DataReceivedEventArgs e, string targetAddress, List<string> proofValues, string hex, Settings settings, Process process, int gpuIndex)
        {
            var status = Helpers.CheckJobStatus(o, e, gpuIndex, hex);
            if (status.OutputType == OutputType.finished)
            {
                // Job finished normally
                JobFinished(targetAddress, hex, settings, keyFound: false, gpuIndex);
            }
            else if (status.OutputType == OutputType.address)
            {
                isProofKey = proofValues.Any(status.Content.Contains);
                if (!isProofKey)
                {
                    // Check again for known Bitcrack bug - Remove first 10 characters
                    List<string> ParsedProofValues = proofValues.Select(x => x[10..]).ToList();
                    isProofKey = ParsedProofValues.Any(status.Content.Contains);
                }
            }
            else if (status.OutputType == OutputType.privateKeyFound)
            {
                if (isProofKey)
                {
                    proofKeys[gpuIndex] += status.Content;
                }
                else
                {
                    if (settings.ForceContinue == false)
                    {
                        process.Kill();
                    }
                    privateKey = status.Content;
                    JobFinished(targetAddress, hex, settings, keyFound: true, gpuIndex);
                }
            }
            else if (status.OutputType == OutputType.gpuModel)
            {
                gpuName = status.Content;
            }
        }
    }
}
