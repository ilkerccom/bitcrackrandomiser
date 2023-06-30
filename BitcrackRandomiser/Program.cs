using System.Diagnostics;
using System.Reflection;

namespace BitcrackRandomiser
{
    class Program
    {
        // Found private key
        public static string PrivateKey = "";

        // Is proof of work key
        public static bool IsProofKey = false;

        // Proof of work key
        public static string ProofKey = "";

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
            Helpers.WriteLine(string.Format("Press any key to edit settings or wait for {0} seconds to load app with <settings.txt>", 3));
            bool EditSettings = Task.Factory.StartNew(() => { try { Console.ReadKey(); } catch { } }).Wait(TimeSpan.FromSeconds(3));
            if (EditSettings)
            {
                AppSettings = Settings.SetSettings();
            }

            // Send worker start message to telegram if active
            Helpers.ShareTelegram(string.Format("[{0}].[{2}] started job for (Puzzle{1})", Helpers.StringParser(AppSettings.ParsedWalletAddress), AppSettings.TargetPuzzle, AppSettings.ParsedWorkerName), AppSettings);

            // Run
            Helpers.WriteLine("Please wait while app is starting...", MessageType.normal, true);
            RunBitcrack(AppSettings);
            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static Task<int> RunBitcrack(Settings settings)
        {
            // Check important area
            if (!settings.TelegramShare && settings.UntrustedComputer)
            {
                Helpers.WriteLine("If the 'untrusted_computer' setting is 'true', the private key will only be sent to your Telegram address. Please change the 'telegram_share' to 'true' in settings.txt. Then enter your 'access token' and 'chat id'. Otherwise, even if the private key is found, you will not be able to see it anywhere!", MessageType.error, true);
                Thread.Sleep(30000);
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
                Helpers.WriteLine("Database connection error. Please wait...");
                Thread.Sleep(5000);
                RunBitcrack(settings);
                return Task.FromResult(0);
            }

            // No ranges left to scan
            if(GetHex == "REACHED_OF_KEYSPACE")
            {
                Helpers.WriteLine("Reached of keyspace. No ranges left to scan.");
                Helpers.ShareTelegram(string.Format("[{0}].[{1}] reached of keyspace", Helpers.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName), settings);
                return Task.FromResult(0);
            }

            // Parse hex result
            string RandomHex = GetHex.Split(':')[0];
            string ProofValue = GetHex.Split(':')[1];
            if(ProofValue == TargetAddress)
            {
                // Impossible but, may be proof value == target address?
                PrivateKey = GetHex.Split(':')[2];
                JobFinished(TargetAddress, RandomHex, settings, KeyFound: true);
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
                ProofValue = "1Hz8wCQp9j71j8NGuzFE5KN9SV7PeRguai";

                // Test with custom settings
                string CustomTestFile = AppDomain.CurrentDomain.BaseDirectory + "customtest.txt";
                if (File.Exists(CustomTestFile))
                {
                    string[] lines = File.ReadAllLines(CustomTestFile);
                    if(lines.Length == 4)
                    {
                        TargetAddress = lines[0];
                        StartHex = lines[1];
                        EndHex = lines[2];
                        ProofValue = lines[3];
                    }
                }
            }

            // Write info
            Helpers.WriteLine(string.Format("[v{1}] {2} starting... Puzzle: [{0}]", settings.TestMode ? "TEST" : settings.TargetPuzzle, Assembly.GetEntryAssembly()?.GetName().Version, settings.AppType.ToString().ToUpper()), MessageType.normal, true);
            Helpers.WriteLine(string.Format("HEX range: {0}-{1}", StartHex, EndHex));
            Helpers.WriteLine(string.Format("Target address: {0}", TargetAddress));
            if (settings.TestMode) Helpers.WriteLine("Test mode is active.", MessageType.error);
            else if (settings.TargetPuzzle == "38") Helpers.WriteLine("Test pool 38 is active.", MessageType.error);
            else Helpers.WriteLine("Test mode is passive.", MessageType.info);
            Helpers.WriteLine(string.Format("Scan type: {0}", settings.ScanType.ToString()), MessageType.info);
            Helpers.WriteLine(string.Format("Telegram share: {0}", settings.TelegramShare), MessageType.info);
            Helpers.WriteLine(string.Format("Untrusted computer: {0}", settings.UntrustedComputer), MessageType.info);
            Helpers.WriteLine(string.Format("Progress: {0}", "Visit the <btcpuzzle.info> for statistics."));
            Helpers.WriteLine(string.Format("Your wallet/worker name: {0}", settings.WalletAddress));

            // App arguments
            string AppArguments = "";
            if (settings.AppType == AppType.bitcrack)
            {
                string Zeros = (settings.TargetPuzzle == "38" ? new String('0', 8) : new String('0', 10));
                AppArguments = string.Format("{4} --keyspace {0}{5}:{1}{5} {2} {3}", StartHex, EndHex, TargetAddress, ProofValue, settings.AppArgs, Zeros);
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
            process.ErrorDataReceived += (object o, DataReceivedEventArgs s) => OutputReceivedHandler(o, s, TargetAddress, ProofValue, StartHex, settings, process);
            process.OutputDataReceived += (object o, DataReceivedEventArgs s) => OutputReceivedHandler(o, s, TargetAddress, ProofValue, StartHex, settings, process);

            // App exited
            process.Exited += (sender, args) =>
            {
                if (process.ExitCode != 0)
                {
                    Helpers.ShareTelegram(string.Format("[{0}].[{1}] goes offline.", Helpers.StringParser(settings.WalletAddress), settings.ParsedWorkerName), settings);
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
        private static void JobFinished(string TargetAddress, string HEX, Settings settings, bool KeyFound = false)
        {
            if (KeyFound)
            {
                // Always send notification when key found
                Helpers.ShareTelegram(string.Format("[Key Found] Congratulations. Found by worker [{0}].[{2}] {1}", Helpers.StringParser(settings.ParsedWalletAddress), PrivateKey, settings.ParsedWorkerName), settings);

                // Not on untrusted computer
                if (!settings.UntrustedComputer)
                {
                    Helpers.WriteLine(PrivateKey, MessageType.success);
                    Helpers.SaveFile(PrivateKey, TargetAddress);
                }

                Helpers.WriteLine("Congratulations. Key found. Please check your folder.", MessageType.success);
                Helpers.WriteLine("You can donate me; 1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw", MessageType.success);
                Console.ReadLine();
            }
            else
            {
                // Send notification each key scanned
                if (settings.TelegramShareEachKey)
                {
                    Helpers.ShareTelegram(string.Format("[{0}] scanned by [{1}].[{2}]", HEX, Helpers.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName), settings);
                }

                // Flag HEX as used
                bool FlagUsed = Requests.SetHex(HEX, settings.WalletAddress, ProofKey, GPUName, settings.TargetPuzzle).Result;

                // Try flagging
                int FlagTries = 1;
                int MaxTries = 6;
                while (!FlagUsed && FlagTries <= MaxTries)
                {
                    FlagUsed = Requests.SetHex(HEX, settings.WalletAddress, ProofKey, GPUName, settings.TargetPuzzle).Result;
                    Helpers.WriteLine(string.Format("Flag error... Retrying... {0}/{1}", FlagTries, MaxTries));
                    Thread.Sleep(10000);
                    FlagTries++;
                }

                // Info
                if (FlagUsed)
                {
                    Helpers.WriteLine("Range scanned and flagged successfully... Launching again... Please wait...");
                }
                else
                {
                    Helpers.WriteLine("Range scanned with flag error... Launching again... Please wait...");
                }

                // Wait and restart
                ProofKey = "";
                IsProofKey = false;
                Thread.Sleep(10000);
                RunBitcrack(settings);
            }
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
        private static void OutputReceivedHandler(object o, DataReceivedEventArgs e, string TargetAddress, string ProofValue, string HEX, Settings settings, Process process)
        {
            var Status = Helpers.CheckJobStatus(o, e);
            if (Status.OutputType == OutputType.finished)
            {
                // Job finished normally
                JobFinished(TargetAddress, HEX, settings);
            }
            else if (Status.OutputType == OutputType.address)
            {
                // Check founded address is proof key
                IsProofKey = Status.Content.Contains(ProofValue);
                if (!IsProofKey)
                {
                    // Check again for known Bitcrack bug - Remove first 10 characters
                    string ParsedProofValue = ProofValue[10..];
                    IsProofKey = Status.Content.Contains(ParsedProofValue);
                }
            }
            else if (Status.OutputType == OutputType.privateKeyFound)
            {
                if (IsProofKey)
                {
                    // Check if proof key
                    ProofKey = Status.Content;
                }
                else
                {
                    // If target address found
                    process.Kill();
                    PrivateKey = Status.Content;
                    JobFinished(TargetAddress, HEX, settings, KeyFound: true);
                }
            }
            else if (Status.OutputType == OutputType.gpuModel)
            {
                GPUName = Status.Content;
            }
        }
    }
}
