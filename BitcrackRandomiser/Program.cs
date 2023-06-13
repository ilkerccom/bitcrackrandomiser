using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime;
using System.Text;

namespace BitcrackRandomiser
{
    class Program
    {
        // Istesting
        public static bool IsTest = false;

        // Each scan
        public static int Attempts = 0;

        // Found or not
        public static bool Found = false;

        // Found private key
        public static string PrivateKey = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args.Contains("--mode-test"))
                {
                    // Test mode
                    IsTest = true;
                    Helpers.WriteLine("Please wait while app is starting in [Mode:Test Mode]...");
                    RunBitcrack();
                }
                else if (args.Contains("--mode-telegramtest"))
                {
                    // Telegram share test
                    Helpers.WriteLine("Telegram testing [Mode:Telegram Test Mode]");
                    Helpers.ShareTelegram("Test message from bitcrackrandomiser app.");
                    Helpers.WriteLine("Message sent to your telegram channel/group");
                }
            }
            else
            {
                // Run normally
                Helpers.WriteLine("Please wait while app is starting...");
                RunBitcrack();
            }

            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static Task<int> RunBitcrack()
        {
            // Get scan type. Include or exclude defeated ranges
            string BitcrackFolder = Helpers.GetSettings(0).Split('=')[1];
            string ScanType = Helpers.GetSettings(3).Split('=')[1].ToLower();
            string TelegramShare = Helpers.GetSettings(5).Split('=')[1].ToLower();
            string TelegramShareEachKey = Helpers.GetSettings(8).Split('=')[1].ToLower();
            string WalletAddress = Helpers.GetSettings(2).Split('=')[1];
            string UntrustedComputer = Helpers.GetSettings(9).Split('=')[1];
            string TargetPuzzle = Helpers.GetSettings(10).Split('=')[1];

            // Send start message to telegram if active
            if (TelegramShare == "true" && Attempts == 0)
            {
                Helpers.ShareTelegram(string.Format("[{0}] started job for (Puzzle{1})", Helpers.WalletParser(WalletAddress), TargetPuzzle));
            }

            // Is scan finished?
            bool IsFinished = false;

            // Get random HEX value 
            string RandomHex = Requests.GetHex(ScanType).Result;
            string TargetAddress = "13zb1hQbWVsc2S7ZTZnP2G4undNNpdh5so";

            // Cannot get HEX value
            if (RandomHex == "")
            {
                Helpers.WriteLine("Database connection error. Please wait...");
                Thread.Sleep(5000);
                RunBitcrack();
                return Task.FromResult(0);
            }

            // Add +1 to random HEX value
            int StartNumber = int.Parse(RandomHex, System.Globalization.NumberStyles.HexNumber);
            int EndNumber = StartNumber + 1;

            // Convert numbers to HEX
            string StartHex = RandomHex;
            string EndHex = EndNumber.ToString("X");

            // Testing
            if (IsTest)
            {
                TargetAddress = "1HFUvT61q2bfT5tHvEfqLeicppkr5V1QAR";
                StartHex = "2FBE3AA";
                EndHex = "2FBE3AB";
            }

            // Write info
            Helpers.WriteLine(string.Format("Bitcrack starting... Puzzle:{1} | TestMode: {0}", IsTest, TargetPuzzle), true);
            Helpers.WriteLine(string.Format("HEX range: {0}-{1}", StartHex, EndHex));
            Helpers.WriteLine(string.Format("Target address: {0}", TargetAddress));
            Helpers.WriteLine(string.Format("Progress: {0}", "Visit the <btcpuzzle.info> for statistics."));
            Helpers.WriteLine(string.Format("Your wallet address: {0}", WalletAddress));

            // Get settings and arguments from setting.txt
            string BitcrackArguments = string.Format(Helpers.GetSettings(1).Split('=')[1] + " -o {2}.txt --keyspace {0}0000000000:{1}0000000000 {2}", StartHex, EndHex, TargetAddress);
            if (UntrustedComputer == "true")
            {
                BitcrackArguments = string.Format(Helpers.GetSettings(1).Split('=')[1] + " --keyspace {0}0000000000:{1}0000000000 {2}", StartHex, EndHex, TargetAddress);
            }

            // Tcs
            var taskCompletionSource = new TaskCompletionSource<int>();

            // Proccess info
            var process = new Process
            {
                StartInfo = { FileName = BitcrackFolder, RedirectStandardError = true, RedirectStandardOutput = true, Arguments = BitcrackArguments },
                EnableRaisingEvents = true
            };

            // Output from BitCrack
            process.ErrorDataReceived += (object o, DataReceivedEventArgs e) =>
            {
                if (Helpers.CheckJobIsFinished(o, e))
                {
                    IsFinished = true;
                }

                // Check founded
                if(e.Data != null)
                {
                    if(e.Data.Contains("Private key:"))
                    {
                        Found = true;
                        PrivateKey = e.Data.Trim();
                    }
                }
            };

            // Output from BitCrack
            process.OutputDataReceived += (object o, DataReceivedEventArgs e) =>
            {
                if (Helpers.CheckJobIsFinished(o, e))
                {
                    IsFinished = true;
                }

                // Check founded
                if (e.Data != null)
                {
                    if (e.Data.Contains("Private key:"))
                    {
                        Found = true;
                        PrivateKey = e.Data.Trim();
                    }
                }
            };

            // Bitcrack exited
            process.Exited += (sender, args) =>
            {
                if(process.ExitCode == 0)
                {
                    int FlagTries = 1;

                    // Check winner
                    if (!Found)
                    {
                        Found = Helpers.CheckWinner(TargetAddress, StartHex);
                    }

                    // Run again if not found
                    if (Found)
                    {
                        if (TelegramShare == "true")
                        {
                            Helpers.ShareTelegram(string.Format("[Key Found] Congratulations. Found by worker {0}. {1}", Helpers.WalletParser(WalletAddress), PrivateKey));
                        }

                        Helpers.WriteLine("Congratulations. Key found. Please check your folder.");
                        if(UntrustedComputer == "false" && PrivateKey.Length > 16)
                        {
                            Helpers.WriteLine(PrivateKey);
                        }
                        Helpers.WriteLine("You can donate me; 1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw");
                        Console.ReadLine();
                    }
                    else if(IsFinished)
                    {
                        if (TelegramShare == "true" && TelegramShareEachKey == "true")
                        {
                            Helpers.ShareTelegram(string.Format("[{0}] scanned by [{1}]", StartHex, Helpers.WalletParser(WalletAddress)));
                        }

                        // Flag HEX as used
                        bool FlagUsed = Requests.SetHex(StartHex, WalletAddress).Result;

                        // Try flagging
                        while (!FlagUsed && FlagTries <= 3)
                        {
                            FlagUsed = Requests.SetHex(StartHex, WalletAddress).Result;
                            Helpers.WriteLine(string.Format("Flag error... Retrying... {0}/3", FlagTries));
                            Thread.Sleep(5000);
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
                        Thread.Sleep(10000);
                        RunBitcrack();
                    }
                }
                else
                {
                    // User shuts down cuBitcrack.exe
                    Helpers.WriteLine("Bitcrack app exited with unknown code...");

                    // Worker goes to offline
                    if (TelegramShare == "true")
                    {
                        Helpers.ShareTelegram(string.Format("[{0}] goes offline.", Helpers.WalletParser(WalletAddress)));
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
    }
}
