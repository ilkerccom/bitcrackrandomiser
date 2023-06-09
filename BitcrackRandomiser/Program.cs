using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime;
using System.Text;

namespace BitcrackRandomiser
{
    class Program
    {
        // Found or not
        public static bool Found = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Helpers.WriteLine("Please wait while app is starting...");
            RunBitcrack();
            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static Task<int> RunBitcrack()
        {
            // Get scan type. Include or exclude defeated ranges
            string ScanType = Helpers.GetSettings(3).Split('=')[1].ToLower();
            string TelegramShare = Helpers.GetSettings(5).Split('=')[1].ToLower();
            string TelegramShareEachKey = Helpers.GetSettings(8).Split('=')[1].ToLower();

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
            string WalletAddress = Helpers.GetSettings(2).Split('=')[1];

            // Testing
            bool isTesting = false;
            if (isTesting)
            {
                TargetAddress = "1HFUvT61q2bfT5tHvEfqLeicppkr5V1QAR";
                StartHex = "2FBE3AA";
                EndHex = "2FBE3AB";
            }

            // Write info
            Helpers.WriteLine("Bitcrack starting...", true);
            Helpers.WriteLine(string.Format("HEX range: {0}-{1}", StartHex, EndHex));
            Helpers.WriteLine(string.Format("Target address: {0}", TargetAddress));
            Helpers.WriteLine(string.Format("Progress: {0}", "Visit the <btcpuzzle.info> for statistics."));
            Helpers.WriteLine(string.Format("Your wallet address: {0}", WalletAddress));

            // Get settings and arguments from setting.txt
            string BitcrackFolder = Helpers.GetSettings(0);
            string BitcrackArguments = string.Format(Helpers.GetSettings(1), StartHex, EndHex, TargetAddress);

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
            };

            // Output from BitCrack
            process.OutputDataReceived += (object o, DataReceivedEventArgs e) =>
            {
                if (Helpers.CheckJobIsFinished(o, e))
                {
                    IsFinished = true;
                }
            };

            // Bitcrack exited
            process.Exited += (sender, args) =>
            {
                if(process.ExitCode == 0 && IsFinished)
                {
                    int FlagTries = 1;

                    // Check winner
                    Found = Helpers.CheckWinner(TargetAddress, StartHex);

                    // Run again if not found
                    if (Found)
                    {
                        if (TelegramShare == "true")
                        {
                            Helpers.ShareTelegram(string.Format("Congratulations. Private Key Found in HEX range = {0}. Please check your folder or console screen to get key", StartHex));
                        }

                        Helpers.WriteLine("Congratulations. Key found. Please check your folder.");
                        Helpers.WriteLine("You can donate me; 1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw");
                        Console.ReadLine();
                    }
                    else
                    {
                        if (TelegramShare == "true" && TelegramShareEachKey == "true")
                        {
                            Helpers.ShareTelegram(string.Format("HEX {0} scanned in worker {1}", StartHex, WalletAddress));
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
                    Helpers.WriteLine("Bitcrack app exited with unknown code... Launching again... Please wait...");
                }

                taskCompletionSource.SetResult(process.ExitCode);
                process.Dispose();
            };

            // Start bitcrack app
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            return taskCompletionSource.Task;
        }
    }
}
