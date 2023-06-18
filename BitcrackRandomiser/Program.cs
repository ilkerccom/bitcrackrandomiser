using System.Diagnostics;
using System.Reflection.Emit;

namespace BitcrackRandomiser
{
    class Program
    {
        // Each scan
        public static int Attempts = 0;

        // Found or not
        public static bool Found = false;

        // Found private key
        public static string PrivateKey = string.Empty;

        // Is scan finished?
        public static bool IsFinished = false;

        // GPU Model name
        public static string GPUName = "-";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Get settings
            var AppSettings = Helpers.GetSettings();

            // Edit settings
            Helpers.WriteLine(string.Format("Press any key to edit settings or wait for {0} seconds to load app with <settings.txt>", 3));
            bool EditSettings = Task.Factory.StartNew(() => Console.ReadKey()).Wait(TimeSpan.FromSeconds(3));
            if (EditSettings)
            {
                AppSettings = Settings.SetSettings();
            }

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

            // Send worker start message to telegram if active
            if (settings.TelegramShare && Attempts == 0)
            {
                Helpers.ShareTelegram(string.Format("[{0}].[{2}] started job for (Puzzle{1})", Helpers.StringParser(settings.ParsedWalletAddress), settings.TargetPuzzle, settings.ParsedWorkerName), settings);
            }

            // Get random HEX value 
            string RandomHex = Requests.GetHex(settings).Result;
            string TargetAddress =
                settings.TargetPuzzle == "66" ? "13zb1hQbWVsc2S7ZTZnP2G4undNNpdh5so" :
                settings.TargetPuzzle == "67" ? "1BY8GQbnueYofwSuFAT3USAhGjPrkxDdW9" :
                settings.TargetPuzzle == "68" ? "1MVDYgVaSN6iKKEsbzRUAYFrYJadLYZvvZ" : "Unknown";

            // Cannot get HEX value
            if (RandomHex == "")
            {
                Helpers.WriteLine("Database connection error. Please wait...");
                Thread.Sleep(5000);
                RunBitcrack(settings);
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
                TargetAddress = "1HFUvT61q2bfT5tHvEfqLeicppkr5V1QAR";
                StartHex = "2FBE3AA";
                EndHex = "2FBE3AB";
            }

            // Write info
            Helpers.WriteLine(string.Format("Bitcrack starting... Puzzle: [{0}]", settings.TargetPuzzle), MessageType.normal, true);
            Helpers.WriteLine(string.Format("HEX range: {0}-{1}", StartHex, EndHex));
            Helpers.WriteLine(string.Format("Target address: {0}", TargetAddress));
            if (settings.TestMode) Helpers.WriteLine("Test mode is active.", MessageType.error);
            else Helpers.WriteLine("Test mode is passive.", MessageType.info);
            Helpers.WriteLine(string.Format("Scan type: {0}", settings.ScanType.ToString()), MessageType.info);
            Helpers.WriteLine(string.Format("Telegram share: {0}", settings.TelegramShare), MessageType.info);
            Helpers.WriteLine(string.Format("Untrusted computer: {0}", settings.UntrustedComputer), MessageType.info);
            Helpers.WriteLine(string.Format("Progress: {0}", "Visit the <btcpuzzle.info> for statistics."));
            Helpers.WriteLine(string.Format("Your wallet/worker name: {0}", settings.WalletAddress));

            // Get settings and arguments from setting.txt
            string BitcrackArguments = string.Format(settings.BitcrackArgs + " --keyspace {0}0000000000:{1}0000000000 {2}", StartHex, EndHex, TargetAddress);

            // Tcs
            var taskCompletionSource = new TaskCompletionSource<int>();

            // Proccess info
            var process = new Process
            {
                StartInfo = { FileName = settings.BitcrackPath, RedirectStandardError = true, RedirectStandardOutput = true, Arguments = BitcrackArguments },
                EnableRaisingEvents = true
            };

            // Output from BitCrack
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputReceivedHandler);
            process.OutputDataReceived += new DataReceivedEventHandler(OutputReceivedHandler);

            // Bitcrack exited
            process.Exited += (sender, args) =>
            {
                if (process.ExitCode == 0)
                {
                    // Check found or not
                    if (Found)
                    {
                        // Always send notification when key found
                        if (settings.TelegramShare)
                        {
                            Helpers.ShareTelegram(string.Format("[Key Found] Congratulations. Found by worker [{0}].[{2}] {1}", Helpers.StringParser(settings.ParsedWalletAddress), PrivateKey, settings.ParsedWorkerName), settings);
                        }

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
                    else if (IsFinished)
                    {
                        if (settings.TelegramShare && settings.TelegramShareEachKey)
                        {
                            Helpers.ShareTelegram(string.Format("[{0}] scanned by [{1}].[{2}]", StartHex, Helpers.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName), settings);
                        }

                        // Flag HEX as used
                        bool FlagUsed = Requests.SetHex(StartHex, settings.WalletAddress, GPUName, settings.TargetPuzzle).Result;

                        // Try flagging
                        int FlagTries = 1;
                        int MaxTries = 6;
                        while (!FlagUsed && FlagTries <= MaxTries)
                        {
                            FlagUsed = Requests.SetHex(StartHex, settings.WalletAddress, GPUName, settings.TargetPuzzle).Result;
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
                        IsFinished = false;
                        Thread.Sleep(10000);
                        RunBitcrack(settings);
                    }
                }
                else
                {
                    // User shuts down cuBitcrack.exe
                    Helpers.WriteLine("Bitcrack app exited with unknown code...");

                    // Worker goes to offline
                    if (settings.TelegramShare)
                    {
                        Helpers.ShareTelegram(string.Format("[{0}].[{1}] goes offline.", Helpers.StringParser(settings.WalletAddress), settings.ParsedWorkerName), settings);
                    }
                }

                taskCompletionSource.SetResult(process.ExitCode);
                process.Dispose();
            };

            Attempts++;

            // Start bitcrack app
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Check bitcrack app output
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private static void OutputReceivedHandler(object o, DataReceivedEventArgs e)
        {
            var Status = Helpers.CheckJobStatus(o, e);
            if (Status.OutputType == OutputType.finished)
            {
                IsFinished = true;
            }
            else if (Status.OutputType == OutputType.privateKeyFound)
            {
                Found = true;
                PrivateKey = Status.Content;
            }
            else if (Status.OutputType == OutputType.gpuModel)
            {
                GPUName = Status.Content;
            }
        }
    }
}
