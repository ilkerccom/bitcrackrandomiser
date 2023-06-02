using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime;

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
            string ScanType = Helpers.GetSettings(3);

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
            string WalletAddress = Helpers.GetSettings(2);

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
            Helpers.WriteLine(string.Format("Your wallet address: {0}", Helpers.GetSettings(2)));

            // Get settings and arguments from setting.txt
            string BitcrackFolder = Helpers.GetSettings(0);
            string BitcrackArguments = string.Format(Helpers.GetSettings(1), StartHex, EndHex, TargetAddress);

            // Tcs
            var taskCompletionSource = new TaskCompletionSource<int>();

            // Proccess info
            var process = new Process
            {
                StartInfo = { FileName = BitcrackFolder, Arguments = BitcrackArguments },
                EnableRaisingEvents = true
            };

            // Bitcrack exited
            process.Exited += (sender, args) =>
            {
                if(process.ExitCode == 0)
                {
                    int FlagTries = 1;

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
                }
                else
                {
                    // User shuts down cuBitcrack.exe
                    Helpers.WriteLine("Bitcrack app exited with unknown code... Launching again... Please wait...");
                }

                // Check winner
                Found = Helpers.CheckWinner(TargetAddress, StartHex);

                // Run again
                if (!Found)
                {
                    Thread.Sleep(10000);
                    RunBitcrack();
                }
                else
                {
                    Helpers.WriteLine("Congratulations. Key found. Please check your folder.");
                    Helpers.WriteLine("You can donate me; 1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw");
                    Console.ReadLine();
                }

                taskCompletionSource.SetResult(process.ExitCode);
                process.Dispose();
            };

            // Start bitcrack app
            process.Start();
            return taskCompletionSource.Task;
        }
    }
}
