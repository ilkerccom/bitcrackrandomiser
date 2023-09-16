using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using static BitcrackRandomiser.Models;

namespace BitcrackRandomiser
{
    class Program
    {
        // Found private key
        public static string PrivateKey = "";

        // Is proof of work key
        public static bool IsProofKey = false;

        // Proof of work key
        public static List<ProofKey> ProofKey = new();

        // GPU Model name
        public static string GPUName = "-";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Get settings
            var AppSettings = Settings.GetSettings(args);

            // Edit settings
            Helpers.WriteLine(string.Format("Press <enter> to edit settings or wait for {0} seconds to load app with <settings.txt>", 3));
            if (!Console.IsInputRedirected)
            {
                bool EditSettings = Task.Factory.StartNew(() => Console.ReadLine()).Wait(TimeSpan.FromSeconds(3));
                if (EditSettings)
                {
                    AppSettings = Settings.SetSettings();
                }
            }

            // Send worker start message to telegram if active
            Helpers.ShareTelegram(string.Format("[{0}].[{2}] started job for (Puzzle{1})", Helpers.StringParser(AppSettings.ParsedWalletAddress), AppSettings.TargetPuzzle, AppSettings.ParsedWorkerName), AppSettings);

            // Send progress to api_share if active
            _ = Requests.SendApiShare(new ApiShare { Status = ApiShareStatus.workerStarted }, AppSettings);

            // Run
            Helpers.WriteLine("Please wait while app is starting...", MessageType.normal, true);
            Parallel.For(0, AppSettings.GPUCount, i =>
            {
                ProofKey.Add(new ProofKey { Key = "", GPUIndex = i });
                RunBitcrack(AppSettings, i);
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
            string GetHex = Requests.GetHex(settings).Result;
            string TargetAddress =
                settings.TargetPuzzle == "38" ? "1HBtApAFA9B2YZw3G2YKSMCtb3dVnjuNe2" :
                settings.TargetPuzzle == "66" ? "13zb1hQbWVsc2S7ZTZnP2G4undNNpdh5so" :
                settings.TargetPuzzle == "67" ? "1BY8GQbnueYofwSuFAT3USAhGjPrkxDdW9" :
                settings.TargetPuzzle == "68" ? "1MVDYgVaSN6iKKEsbzRUAYFrYJadLYZvvZ" : "Unknown";

            // Cannot get HEX value
            if (GetHex == "")
            {
                Helpers.WriteLine("Database connection error. Please wait...", MessageType.error);
                Thread.Sleep(5000);
                RunBitcrack(settings, gpuIndex);
                return Task.FromResult(0);
            }

            // Invalid user token or wallet address
            if (GetHex == "INVALID_USER_TOKEN")
            {
                Helpers.WriteLine("Invalid user token value or wallet address.", MessageType.error);
                return Task.FromResult(0);
            }

            // Invalid user token or wallet address
            if (GetHex == "NOT_ELIGIBLE_FOR_FREE")
            {
                Helpers.WriteLine("You are not eligible for free tier. For more information, log in to your account at btcpuzzle.info", MessageType.error);
                return Task.FromResult(0);
            }

            // Invalid user for private pool
            if (GetHex == "INVALID_PRIVATE_POOL_USER")
            {
                Helpers.WriteLine("Only the user who created the private pool can join the private pool. Please check your user_token and wallet_address value.", MessageType.error);
                return Task.FromResult(0);
            }

            // Invalid user for private pool
            if (GetHex == "INVALID_PRIVATE_POOL")
            {
                Helpers.WriteLine("Invalid private pool. There is no such private pool. Check your private_pool value.", MessageType.error);
                return Task.FromResult(0);
            }

            // No ranges left to scan
            if (GetHex == "REACHED_OF_KEYSPACE")
            {
                Helpers.WriteLine("Reached of keyspace. No ranges left to scan.");
                Helpers.ShareTelegram(string.Format("[{0}].[{1}] reached of keyspace", Helpers.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName), settings);
                _ = Requests.SendApiShare(new ApiShare { Status = ApiShareStatus.reachedOfKeySpace }, settings);
                return Task.FromResult(0);
            }

            // Parse hex result
            string RandomHex = GetHex.Split(':')[0];
            List<string> ProofValues = GetHex.Split(':').Skip(1).ToList();
            if (ProofValues.Contains(TargetAddress))
            {
                // Impossible but, may be proof value == target address?
                PrivateKey = GetHex.Split(':')[2];
                JobFinished(TargetAddress, RandomHex, settings, KeyFound: true, gpuIndex);
                return Task.FromResult(0);
            }

            // Add +1 to random HEX value
            int StartNumber = int.Parse(RandomHex, System.Globalization.NumberStyles.HexNumber);
            int EndNumber = StartNumber + 1;

            // Convert numbers to HEX
            string StartHex = RandomHex;
            string EndHex = EndNumber.ToString("X");

            // Testing
            if (settings.TestMode)
            {
                // ~1min on 3090
                TargetAddress = "1Cnrx6rxiGvVNw1UroYM5hRjVvqPnWC7fR";
                StartHex = "2012E83";
                EndHex = "2012E84";

                // Test with custom settings
                string CustomTestFile = AppDomain.CurrentDomain.BaseDirectory + "customtest.txt";
                if (File.Exists(CustomTestFile))
                {
                    string[] lines = File.ReadAllLines(CustomTestFile);
                    if (lines.Length == 3)
                    {
                        TargetAddress = lines[0];
                        StartHex = lines[1];
                        EndHex = lines[2];
                    }
                }
            }

            // Write info
            if (gpuIndex == 0)
            {
                Helpers.WriteLine(string.Format("[v{1}] {2} starting... Puzzle: [{0}]", settings.TestMode ? "TEST" : settings.IsPrivatePool ? settings.PrivatePool : settings.TargetPuzzle, Assembly.GetEntryAssembly()?.GetName().Version, settings.AppType.ToString().ToUpper()), MessageType.normal, true);
                Helpers.WriteLine(string.Format("HEX range: {0}-{1} / Total GPU(s): {2}", StartHex, EndHex, settings.GPUCount));
                Helpers.WriteLine(string.Format("Target address: {0}", TargetAddress));
                if (settings.TestMode) Helpers.WriteLine("Test mode is active.", MessageType.error);
                else if (settings.TargetPuzzle == "38") Helpers.WriteLine("Test pool 38 is active.", MessageType.error);
                else Helpers.WriteLine("Test mode is passive.", MessageType.info);
                Helpers.WriteLine(string.Format("Scan type: {0}", settings.ScanType.ToString()), MessageType.info);
                Helpers.WriteLine(string.Format("API share: {0} / Telegram share: {1}", settings.IsApiShare, settings.TelegramShare), MessageType.info);
                Helpers.WriteLine(string.Format("Untrusted computer: {0}", settings.UntrustedComputer), MessageType.info);
                Helpers.WriteLine(string.Format("Progress: {0}", "Visit the <btcpuzzle.info> for statistics."));
                Helpers.WriteLine(string.Format("Your wallet/worker name: {0}", settings.WalletAddress));
            }

            // App arguments
            string AppArguments = "";
            if (settings.AppType == AppType.bitcrack)
            {
                string Zeros = (settings.TargetPuzzle == "38" ? new String('0', 8) : new String('0', 10));
                AppArguments = string.Format("{3} --keyspace {0}{4}:{1}{4} {2} {5} -d {6}", StartHex, EndHex, TargetAddress, settings.AppArgs, Zeros, string.Join(' ', ProofValues), gpuIndex);
            }

            // Tcs
            var taskCompletionSource = new TaskCompletionSource<int>();

            // Proccess info
            var process = new Process
            {
                StartInfo = { FileName = settings.AppPath, RedirectStandardError = true, RedirectStandardOutput = true, Arguments = AppArguments },
                EnableRaisingEvents = true
            };

            // Output from BitCrack
            process.ErrorDataReceived += (object o, DataReceivedEventArgs s) => OutputReceivedHandler(o, s, TargetAddress, ProofValues, StartHex, settings, process, gpuIndex);
            process.OutputDataReceived += (object o, DataReceivedEventArgs s) => OutputReceivedHandler(o, s, TargetAddress, ProofValues, StartHex, settings, process, gpuIndex);

            // App exited
            process.Exited += (sender, args) =>
            {
                if (process.ExitCode != 0)
                {
                    Helpers.ShareTelegram(string.Format("[{0}].[{1}] goes offline.", Helpers.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName), settings);
                    _ = Requests.SendApiShare(new ApiShare { Status = ApiShareStatus.workerExited }, settings);
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
        /// <param name="TargetAddress">Target address</param>
        /// <param name="HEX">HEX range</param>
        /// <param name="settings">Current settings</param>
        /// <param name="KeyFound">Key found or not</param>
        private static void JobFinished(string TargetAddress, string HEX, Settings settings, bool KeyFound = false, int GPUIndex = 0)
        {
            if (KeyFound)
            {
                // Always send notification when key found
                Helpers.ShareTelegram(string.Format("[Key Found] Congratulations. Found by worker [{0}].[{2}] {1}", Helpers.StringParser(settings.ParsedWalletAddress), PrivateKey, settings.ParsedWorkerName), settings);
                _ = Requests.SendApiShare(new ApiShare { Status = ApiShareStatus.keyFound, PrivateKey = PrivateKey, HEX = HEX }, settings);

                // Not on untrusted computer
                if (!settings.UntrustedComputer)
                {
                    Console.WriteLine(Environment.NewLine);
                    Helpers.WriteLine(PrivateKey, MessageType.success);
                    Helpers.SaveFile(PrivateKey, TargetAddress);
                }

                Helpers.WriteLine("Congratulations. Key found. Please check your folder.", MessageType.success);
                Helpers.WriteLine("You can donate me; 1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw", MessageType.success);
            }
            else
            {
                // Send request to custom API
                _ = Requests.SendApiShare(new ApiShare { Status = ApiShareStatus.rangeScanned, HEX = HEX }, settings);

                // Send notification each key scanned
                if (settings.TelegramShareEachKey)
                {
                    Helpers.ShareTelegram(string.Format("[{0}] scanned by [{1}].[{2}]", HEX, Helpers.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName), settings);
                }

                // Flag HEX as used
                FlagAsScanned(settings, HEX, GPUIndex);

                // Wait and restart
                ProofKey.FirstOrDefault(k => k.GPUIndex == GPUIndex).Key = "";
                IsProofKey = false;
                Thread.Sleep(5000);
                RunBitcrack(settings, GPUIndex);
            }
        }

        /// <summary>
        /// Flag HEX as scanned
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="HEX"></param>
        /// <returns></returns>
        public static bool FlagAsScanned(Settings settings, string HEX, int GPUIndex)
        {
            // Hash all proof keys with SHA256
            string HashedProofKey = Helpers.SHA256Hash(ProofKey.FirstOrDefault(p => p.GPUIndex == GPUIndex).Key);

            // Try flag
            string WalletAddress = settings.WalletAddress;
            if (settings.GPUCount > 1)
            {
                WalletAddress += "_" + GPUIndex;
            }
            bool FlagUsed = Requests.SetHex(HEX, WalletAddress, HashedProofKey, GPUName, settings.PrivatePool, settings.TargetPuzzle).Result;

            // Try flagging
            int FlagTries = 1;
            int MaxTries = 6;
            while (!FlagUsed && FlagTries <= MaxTries)
            {
                FlagUsed = Requests.SetHex(HEX, settings.WalletAddress, HashedProofKey, GPUName, settings.TargetPuzzle).Result;
                Helpers.WriteLine(string.Format("Flag error... Retrying... {0}/{1} [GPU{2}]", FlagTries, MaxTries, GPUIndex));
                Thread.Sleep(10000);
                FlagTries++;
            }

            // Info
            if (FlagUsed)
            {
                Helpers.WriteLine(string.Format("Range scanned and flagged successfully... Launching again... Please wait... [GPU{0}]", GPUIndex));
            }
            else
            {
                Helpers.WriteLine(string.Format("Range scanned with flag error... Launching again... Please wait... [GPU{0}] {1}", GPUIndex, HashedProofKey));
            }

            return FlagUsed;
        }

        /// <summary>
        /// Runs when output data received by external app
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <param name="TargetAddress">Target address</param>
        /// <param name="ProofValue">Proof value</param>
        /// <param name="HEX">Selected HEX range</param>
        /// <param name="settings">Current settings</param>
        /// <param name="process">Active proccess</param>
        public static void OutputReceivedHandler(object o, DataReceivedEventArgs e, string TargetAddress, List<string> ProofValues, string HEX, Settings settings, Process process, int GPUIndex)
        {
            var Status = Helpers.CheckJobStatus(o, e);
            if (Status.OutputType == OutputType.finished)
            {
                // Job finished normally
                JobFinished(TargetAddress, HEX, settings, KeyFound: false, GPUIndex);
            }
            else if (Status.OutputType == OutputType.address)
            {
                IsProofKey = ProofValues.Any(Status.Content.Contains);
                if (!IsProofKey)
                {
                    // Check again for known Bitcrack bug - Remove first 10 characters
                    List<string> ParsedProofValues = ProofValues.Select(x => x[10..]).ToList();
                    IsProofKey = ParsedProofValues.Any(Status.Content.Contains);
                }
            }
            else if (Status.OutputType == OutputType.privateKeyFound)
            {
                if (IsProofKey)
                {
                    ProofKey.FirstOrDefault(p => p.GPUIndex == GPUIndex).Key += Status.Content;
                }
                else
                {
                    if (settings.ForceContinue == false)
                    {
                        process.Kill();
                    }
                    PrivateKey = Status.Content;
                    JobFinished(TargetAddress, HEX, settings, KeyFound: true, GPUIndex);
                }
            }
            else if (Status.OutputType == OutputType.gpuModel)
            {
                GPUName = Status.Content;
            }
        }
    }
}
