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
            Helpers.WriteLine("Please wait while app is starting ...");
            RunBitcrack();
            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static Task<int> RunBitcrack()
        {
            // Get random HEX value 
            string RandomHex = Requests.GetHex().Result;
            string TargetAddress = "13zb1hQbWVsc2S7ZTZnP2G4undNNpdh5so";

            // Cannot get HEX value
            if (RandomHex == "")
            {
                Helpers.WriteLine("Database connection corrupted. Please try again later.");
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
            Helpers.WriteLine(string.Format("Percentage: {0}", Requests.GetPercentage().Result));
            Helpers.WriteLine(string.Format("Your BTC payment address: {0}", Helpers.GetSettings(2)));

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

            // Bitcrack exit
            process.Exited += (sender, args) =>
            {
                // Flag HEX as used
                bool FlagUsed = Requests.SetHex(StartHex, WalletAddress).Result;

                // Info
                if(FlagUsed)
                {
                    Helpers.WriteLine("Range scanned and flagged successfully ... Launching again ... Please wait ...");
                }
                else
                {
                    Helpers.WriteLine("Range scanned with flag error ... Launching again ... Please wait ...");
                }

                // Check winner
                Found = Helpers.CheckWinner(TargetAddress, StartHex);

                // Run again
                if (!Found)
                {
                    Thread.Sleep(15000);
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