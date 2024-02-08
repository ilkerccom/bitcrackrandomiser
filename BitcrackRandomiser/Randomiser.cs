using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Helpers;
using System.Diagnostics;
using System.Reflection;

namespace BitcrackRandomiser
{
    /// <summary>
    /// Main randomiser app functions.
    /// </summary>
    internal class Randomiser
    {
        // Found private key
        public static string privateKey = "";

        // Is proof of work key
        public static bool[] isProofKeys = new bool[16];

        // Proof of work keys list.
        public static string[] proofKeys = new string[16];

        // GPU Model names
        public static string[] gpuNames = new string[16];

        // Check if app started
        public static bool appStarted = false;

        /// <summary>
        /// Start scan!
        /// </summary>
        /// <param name="settings">Initial settings</param>
        /// <param name="gpuIndex">GPU index</param>
        /// <returns></returns>
        public static Task<int> Scan(Settings settings, int gpuIndex)
        {
            // Check important area
            if (!settings.TelegramShare && settings.UntrustedComputer && !settings.IsApiShare)
            {
                Helper.WriteLine("If the 'untrusted_computer' setting is 'true', the private key will only be sent to your Telegram address. Please change the 'telegram_share' to 'true' in settings.txt. Then enter your 'access token' and 'chat id'. Otherwise, even if the private key is found, you will not be able to see it anywhere!", MessageType.error, true);
                Thread.Sleep(10000);
            }

            // Get random HEX value from API
            string hex = Requests.GetHex(settings).Result;
            string targetAddress =
                settings.TargetPuzzle == "38" ? "1HBtApAFA9B2YZw3G2YKSMCtb3dVnjuNe2" :
                settings.TargetPuzzle == "66" ? "13zb1hQbWVsc2S7ZTZnP2G4undNNpdh5so" :
                settings.TargetPuzzle == "67" ? "1BY8GQbnueYofwSuFAT3USAhGjPrkxDdW9" :
                settings.TargetPuzzle == "68" ? "1MVDYgVaSN6iKKEsbzRUAYFrYJadLYZvvZ" : "Unknown";

            // Cannot get HEX value
            if (hex == "")
            {
                Helper.WriteLine("Database connection error. Please wait...", MessageType.error);
                Thread.Sleep(5000);
                Scan(settings, gpuIndex);
                return Task.FromResult(0);
            }

            // Check for errors
            switch (hex)
            {
                case "INVALID_USER_TOKEN":
                    Helper.WriteLine("Invalid user token value or wallet address.", MessageType.error);
                    return Task.FromResult(0);
                case "NOT_ELIGIBLE_FOR_FREE":
                    Helper.WriteLine("You are not eligible for free tier. For more information, log in to your account at btcpuzzle.info", MessageType.error);
                    return Task.FromResult(0);
                case "INVALID_PRIVATE_POOL_USER":
                    Helper.WriteLine("Only the user who created the private pool can join the private pool. Please check your user_token and wallet_address value.", MessageType.error);
                    return Task.FromResult(0);
                case "INVALID_PRIVATE_POOL":
                    Helper.WriteLine("Invalid private pool. There is no such private pool. Check your private_pool value.", MessageType.error);
                    return Task.FromResult(0);
                case "REACHED_OF_KEYSPACE":
                    Helper.WriteLine("Reached of keyspace. No ranges left to scan.");
                    Share.Send(ResultType.reachedOfKeySpace, settings);
                    return Task.FromResult(0);
                default:
                    break;
            }

            // Parse hex result
            string randomHex = hex.Split(':')[0];
            var proofValues = hex.Split(':').Skip(1).ToList();
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
                Helper.WriteLine(string.Format("[v{1}] <{2}> starting... Puzzle: [{0}]", settings.TestMode ? "TEST" : settings.IsPrivatePool ? settings.PrivatePool : settings.TargetPuzzle, Assembly.GetEntryAssembly()?.GetName().Version, settings.AppType.ToString()), MessageType.normal, true);
                Helper.WriteLine(string.Format("Target address: {0}", targetAddress));
                if (settings.TestMode) Helper.WriteLine("Test mode is active.", MessageType.error);
                else if (settings.TargetPuzzle == "38") Helper.WriteLine("Test pool 38 is active.", MessageType.error);
                else Helper.WriteLine("Test mode is passive.", MessageType.info);
                Helper.WriteLine(string.Format("Scan type(s): {0}",  $"[{settings.ScanType.Split(',').Length}] scan type(s) have been set.", MessageType.info));
                Helper.WriteLine(string.Format("API share: {0} / Telegram share: {1}", settings.IsApiShare, settings.TelegramShare), MessageType.info);
                Helper.WriteLine(string.Format("Untrusted computer: {0}", settings.UntrustedComputer), MessageType.info);
                Helper.WriteLine(string.Format("Progress: {0}", "Visit the <btcpuzzle.info> for statistics."));
                Helper.WriteLine(string.Format("Worker name: {0}", settings.WalletAddress));
                Helper.WriteLine("", MessageType.seperator);

                appStarted = true;
            }

            // App arguments
            string appArguments = "";
            if (settings.AppType == AppType.bitcrack)
            {
                var totalZeros = settings.TargetPuzzle == "38" ? new String('0', 8) : new String('0', 10);
                var initialArgs = settings.AppArgs;
                var proofAddressList = string.Join(' ', proofValues);
                var currentGpuIndex = settings.GPUCount > 1 ? gpuIndex : settings.GPUIndex;

                appArguments = $"{initialArgs} --keyspace {startHex}{totalZeros}:{endHex}{totalZeros} {targetAddress} {proofAddressList} -d {currentGpuIndex}";
            }

            // Check app is exists
            if(!File.Exists(settings.AppPath))
            {
                Helper.WriteLine($"[{settings.AppType}] cannot find at path ({settings.AppPath}).", MessageType.error);
                return Task.FromResult(0);
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
                    Share.Send(ResultType.workerExited, settings);
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
        /// <param name="keyFound">Key found or not</param>
        /// <param name="gpuIndex">GPU index</param>
        private static void JobFinished(string targetAddress, string hex, Settings settings, bool keyFound = false, int gpuIndex = 0)
        {
            if (keyFound)
            {
                // Always send notification when key found
                Share.Send(ResultType.keyFound, settings, privateKey);

                // Not on untrusted computer
                if (!settings.UntrustedComputer)
                {
                    Console.WriteLine(Environment.NewLine);
                    Helper.WriteLine(privateKey, MessageType.success);
                    Helper.SaveFile(privateKey, targetAddress);
                }

                Helper.WriteLine("Congratulations. Key found. Please check your folder.", MessageType.success);
                Helper.WriteLine("You can donate me; 1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw", MessageType.success);
            }
            else
            {
                // Send notification each key scanned
                Share.Send(ResultType.rangeScanned, settings, hex);

                // Flag HEX as used
                Flagger.Flag(settings, hex, gpuIndex, proofKeys[gpuIndex], gpuNames[gpuIndex]);

                // Wait and restart
                proofKeys[gpuIndex] = "";
                isProofKeys[gpuIndex] = false;
                Thread.Sleep(5000);
                Scan(settings, gpuIndex);
            }
        }

        /// <summary>
        /// Handler for data received by external app
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <param name="targetAddress">Target address</param>
        /// <param name="proofValues">Proof values</param>
        /// <param name="hex">Selected HEX range</param>
        /// <param name="settings">Current settings</param>
        /// <param name="process">Active proccess</param>
        /// <param name="gpuIndex">GPU Index</param>
        public static void OutputReceivedHandler(object o, DataReceivedEventArgs e, string targetAddress, List<string> proofValues, string hex, Settings settings, Process process, int gpuIndex)
        {
            var status = Job.GetStatus(o, e, gpuIndex, hex);
            if (status.OutputType == OutputType.finished)
            {
                // Job finished normally and range scanned.
                JobFinished(targetAddress, hex, settings, keyFound: false, gpuIndex);
            }
            else if (status.OutputType == OutputType.address)
            {
                // An address found. Check it is proof key or target private key
                isProofKeys[gpuIndex] = proofValues.Any(status.Content.Contains);
                if (!isProofKeys[gpuIndex])
                {
                    // Check again for known Bitcrack bug - Remove first 10 characters
                    List<string> parsedProofValues = proofValues.Select(x => x[10..]).ToList();
                    isProofKeys[gpuIndex] = parsedProofValues.Any(status.Content.Contains);
                }
            }
            else if (status.OutputType == OutputType.privateKeyFound)
            {
                // A private key found
                if (isProofKeys[gpuIndex])
                    proofKeys[gpuIndex] += status.Content;
                else
                {
                    if (settings.ForceContinue == false)
                        process.Kill();
                    privateKey = status.Content;
                    JobFinished(targetAddress, hex, settings, keyFound: true, gpuIndex);
                }
            }
            else if (status.OutputType == OutputType.gpuModel)
                gpuNames[gpuIndex] = status.Content;
        }
    }
}
