using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Helpers;
using System.Reflection;

namespace BitcrackRandomiser
{
    internal class Settings
    {
        /// <summary>
        /// Target puzzle number. [66,67,68 or 38]
        /// </summary>
        public string TargetPuzzle { get; set; } = "66";

        /// <summary>
        /// Which app will be used.
        /// </summary>
        public AppType AppType { get; set; } = AppType.vanitysearch;

        /// <summary>
        /// Bitcrack app folder path
        /// </summary>
        public string AppPath { get; set; } = string.Empty;

        /// <summary>
        /// Bitcrack args
        /// Example: -b 896 -t 256 -p 256
        /// </summary>
        public string AppArgs { get; set; } = "";

        /// <summary>
        /// User token value
        /// </summary>
        public string UserToken { get; set; } = "";

        /// <summary>
        /// Wallet address for worker
        /// </summary>
        string _WalletAddress = "";
        public string WalletAddress
        {
            get
            {
                if (_WalletAddress.Length < 6)
                    _WalletAddress = "Unknown";
                else if (!_WalletAddress.Contains('.'))
                    _WalletAddress = string.Format("{0}.worker{1}", _WalletAddress, new Random().Next(1000, 9999));

                return _WalletAddress;
            }
            set
            {
                _WalletAddress = value;
            }
        }

        /// <summary>
        /// GPU count. Max 16 GPUs
        /// </summary>
        int _GPUCount = 1;
        public int GPUCount
        {
            get
            {
                if (_GPUCount <= 0)
                    return 1;
                else if (_GPUCount > 16)
                    return 16;

                return _GPUCount;
            }
            set
            {
                _GPUCount = value;
            }
        }

        /// <summary>
        /// Active GPU for scanning.
        /// </summary>
        int _GPUIndex = 0;
        public int GPUIndex
        {
            get
            {
                if (_GPUIndex < 0 || _GPUIndex >= 16)
                    return 0;

                return _GPUIndex;
            }
            set
            {
                _GPUIndex = value;
            }
        }

        /// <summary>
        /// Parsed wallet address
        /// </summary>
        public string ParsedWalletAddress
        {
            get
            {
                return WalletAddress.Split('.')[0];
            }
        }

        /// <summary>
        /// Parsed worker name
        /// </summary>
        public string ParsedWorkerName
        {
            get
            {
                return WalletAddress.Split('.')[1];
            }
        }

        /// <summary>
        /// Scan type
        /// default: Default
        /// includeDefeatedRanges: Include defeated ranges to scan
        /// excludeIterated2: Exclude if HEX iterated 2
        /// excludeIterated3: Exclude if HEX iterated 3
        /// excludeIterated4: Exclude if HEX iterated 4 or more
        /// excludeStartsWithXX: Exclude if HEX starts with XX [1-2 chars]
        /// </summary>
        public string ScanType { get; set; } = "default";

        /// <summary>
        /// Scan for rewards in the pool.
        /// </summary>
        public bool ScanRewards { get; set; } = false;

        /// <summary>
        /// Custom ranges to scan
        /// Min length 2
        /// Max length 5
        /// Example [20,3FF,2DAB]
        /// </summary>
        public string CustomRange { get; set; } = "none";

        /// <summary>
        /// Send POST request on each key scanned/key found
        /// </summary>
        public string ApiShare { get; set; } = "";

        /// <summary>
        /// Is API share is active
        /// </summary>
        public bool IsApiShare
        {
            get
            {
                if (Uri.IsWellFormedUriString(ApiShare, UriKind.Absolute))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Telegram share is active
        /// </summary>
        public bool TelegramShare { get; set; } = false;

        /// <summary>
        /// Telegram access token
        /// </summary>
        public string TelegramAccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Telegram chat id
        /// </summary>
        public string TelegramChatId { get; set; } = string.Empty;

        /// <summary>
        /// Send notification when each key scanned
        /// </summary>
        public bool TelegramShareEachKey { get; set; } = false;

        /// <summary>
        /// Leave true on untrusted computer
        /// Private key is not written to the file and console screen.
        /// Private key will be delivered to Telegram only.
        /// </summary>
        public bool UntrustedComputer { get; set; } = false;

        /// <summary>
        /// Test mode.
        /// If true, example private key will be found.
        /// </summary>
        public bool TestMode { get; set; } = false;

        /// <summary>
        /// Force continue to if key found
        /// If true, scan will continue until it is finished and will marked as scanned
        /// </summary>
        public bool ForceContinue { get; set; } = false;

        /// <summary>
        /// Private pool id
        /// </summary>
        public string PrivatePool { get; set; } = "none";

        /// <summary>
        /// Is private pool
        /// </summary>
        public bool IsPrivatePool
        {
            get
            {
                if (PrivatePool.Length == 8)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Hashed settings for untrusted computers.
        /// When the any setting changes, the hash value also changes.
        /// </summary>
        public string SecurityHash
        {
            get
            {
                string buildId = Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToString();
                string data = $"{TargetPuzzle}-{AppPath}-{AppArgs}-{ParsedWalletAddress}-{ApiShare}-{TelegramShare}-" +
                    $"{TelegramChatId}-{ScanType}-{CustomRange}-{UntrustedComputer}-{ForceContinue}-{UserToken}-{buildId}";
                return Helper.StringParser(value: Helper.SHA256Hash(data), length: 5, addDots: false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Settings GetSettings(string[] args)
        {
            Settings settings = new Settings();
            string path = AppDomain.CurrentDomain.BaseDirectory + "settings.txt";

            // From file
            string[] lines = File.ReadLines(path).ToArray();

            // From arguments
            if (args.Length > 0)
                lines = args;

            foreach (var line in lines)
            {
                if (line.Contains('='))
                {
                    string key = line.Split('=')[0];
                    string value = line.Split("=")[1];

                    switch (key)
                    {
                        case "target_puzzle":
                            settings.TargetPuzzle = value;
                            break;
                        case "app_type":
                            _ = Enum.TryParse(value, true, out AppType _at);
                            settings.AppType = _at;
                            break;
                        case "app_path":
                            if (value == "cuBitcrack" || value == "clBitcrack")
                            {
                                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                                    value = string.Format("{0}bitcrack\\{1}.exe", AppDomain.CurrentDomain.BaseDirectory, value);
                                else
                                    value = string.Format("{0}bitcrack/./{1}", AppDomain.CurrentDomain.BaseDirectory, value);
                            }
                            else if (value == "vanitysearch")
                            {
                                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                                    value = string.Format("{0}vanitysearch\\{1}.exe", AppDomain.CurrentDomain.BaseDirectory, value);
                                else
                                    value = string.Format("{0}vanitysearch/./{1}", AppDomain.CurrentDomain.BaseDirectory, value);
                            }
                            settings.AppPath = value;
                            break;
                        case "app_arguments":
                            settings.AppArgs = value;
                            break;
                        case "user_token":
                            settings.UserToken = value;
                            break;
                        case "wallet_address":
                            settings.WalletAddress = value;
                            break;
                        case "gpu_count":
                            _ = int.TryParse(value, out int _g);
                            settings.GPUCount = _g;
                            break;
                        case "gpu_index":
                            _ = int.TryParse(value, out int _gi);
                            settings.GPUIndex = _gi;
                            break;
                        case "scan_type":
                            settings.ScanType = value;
                            break;
                        case "scan_rewards":
                            _ = bool.TryParse(value, out bool _r);
                            settings.ScanRewards = _r;
                            break;
                        case "custom_range":
                            settings.CustomRange = value;
                            break;
                        case "api_share":
                            settings.ApiShare = value;
                            break;
                        case "telegram_share":
                            _ = bool.TryParse(value, out bool _v);
                            settings.TelegramShare = _v;
                            break;
                        case "telegram_accesstoken":
                            settings.TelegramAccessToken = value;
                            break;
                        case "telegram_chatid":
                            settings.TelegramChatId = value;
                            break;
                        case "telegram_share_eachkey":
                            _ = bool.TryParse(value, out bool _s);
                            settings.TelegramShareEachKey = _s;
                            break;
                        case "untrusted_computer":
                            _ = bool.TryParse(value, out bool _u);
                            settings.UntrustedComputer = _u;
                            break;
                        case "test_mode":
                            _ = bool.TryParse(value, out bool _t);
                            settings.TestMode = _t;
                            break;
                        case "force_continue":
                            _ = bool.TryParse(value, out bool _f);
                            settings.ForceContinue = _f;
                            break;
                        case "private_pool":
                            settings.PrivatePool = value;
                            break;
                    }
                }
            }
            return settings;
        }

        /// <summary>
        /// Create settings
        /// </summary>
        /// <returns></returns>
        public static Settings SetSettings()
        {
            Console.Clear();
            var consoleSettings = new Settings();

            // Select puzzle
            string _Puzzle = DetermineSettings("Select a puzzle number", new string[3] { "66", "67", "68" });

            // Select app path
            string _Folder = DetermineSettings("Enter app folder path [cuBitcrack, clBitcrack, vanitysearch or full path]", null, 6);

            // App arguments
            var _Arguments = DetermineSettings("Enter app arguments (can be empty)");

            // User token
            string _UserToken = DetermineSettings("Your user token value", null, 20);

            // Wallet address
            string _WalletAddress = DetermineSettings("Your BTC wallet address", null, 20);

            // GPU Count
            string _GPUCount = DetermineSettings("Enter your GPU count [Min:1, Max:16]", null, 1);

            // GPU Index
            string _GPUIndex = DetermineSettings("Enter your GPU Index [Default:0]", null, 1);

            // Scan type
            string _ScanType = DetermineSettings("Select a scan type", new string[2] { "default", "includeDefeatedRanges" });

            // Scan rewards
            string _ScanRewards = DetermineSettings("Scan for rewards of the pool?", new string[2] { "true", "false" });

            // Custom range
            string _CustomRange = DetermineSettings("Do you want scan custom range?", new string[2] { "yes", "no" });
            if (_CustomRange == "yes")
            {
                _CustomRange = DetermineSettings("Enter custom range (2B,3DFF or any)", null, 2);
            }
            else
            {
                _CustomRange = "none";
            }

            // API share
            string _ApiShare = DetermineSettings("Send API request to URL (can be empty)");

            // Telegram share
            string _TelegramShare = DetermineSettings("Will telegram sharing be enabled?", new string[2] { "true", "false" });

            // If telegram share will be enabled
            string _TelegramAccessToken = "0";
            string _TelegramChatId = "0";
            string _TelegramShareEachKey = "false";
            if (_TelegramShare == "true")
            {
                _TelegramAccessToken = DetermineSettings("Enter Telegram access token", null, 5);
                _TelegramChatId = DetermineSettings("Enter Telegram chat id", null, 5);
                _TelegramShareEachKey = DetermineSettings("Send notification when each key scanned", new string[2] { "true", "false" });
            }

            // Untrusted computer
            string _UntrustedComputer = DetermineSettings("Is this computer untrusted?", new string[2] { "true", "false" });

            // Test mode
            string _TestMode = DetermineSettings("Enable test mode", new string[2] { "true", "false" });

            // Force continue
            string _ForceContinue = DetermineSettings("Enable force continue", new string[2] { "true", "false" });

            // Private pool
            string _PrivatePool = DetermineSettings("Private pool id [none] or [pool_id]", null, 4);

            // Settings
            consoleSettings.TargetPuzzle = _Puzzle;
            consoleSettings.AppPath = _Folder;
            consoleSettings.AppArgs = _Arguments;
            consoleSettings.UserToken = _UserToken;
            consoleSettings.WalletAddress = _WalletAddress;
            consoleSettings.GPUCount = int.Parse(_GPUCount);
            consoleSettings.GPUIndex = int.Parse(_GPUIndex);
            consoleSettings.ScanType = _ScanType;
            consoleSettings.ScanRewards = bool.Parse(_ScanRewards);
            consoleSettings.CustomRange = _CustomRange;
            consoleSettings.ApiShare = _ApiShare;
            consoleSettings.TelegramShare = bool.Parse(_TelegramShare);
            consoleSettings.TelegramAccessToken = _TelegramAccessToken;
            consoleSettings.TelegramChatId = _TelegramChatId;
            consoleSettings.TelegramShareEachKey = bool.Parse(_TelegramShareEachKey);
            consoleSettings.UntrustedComputer = bool.Parse(_UntrustedComputer);
            consoleSettings.TestMode = bool.Parse(_TestMode);
            consoleSettings.ForceContinue = bool.Parse(_ForceContinue);
            consoleSettings.PrivatePool = _PrivatePool;

            // Will save settings
            string saveSettings = "";
            while (saveSettings != "yes" && saveSettings != "no")
            {
                Helper.Write("Do you want to save settings to settings.txt? (yes/no) : ", ConsoleColor.Cyan);
                saveSettings = Console.ReadLine() ?? "";
            }

            // Save settings
            if (saveSettings == "yes")
            {
                string savedSettings =
                    "target_puzzle=" + consoleSettings.TargetPuzzle + Environment.NewLine +
                    "app_path=" + consoleSettings.AppPath + Environment.NewLine +
                    "app_arguments=" + consoleSettings.AppArgs + Environment.NewLine +
                    "user_token=" + consoleSettings.UserToken + Environment.NewLine +
                    "wallet_address=" + consoleSettings.WalletAddress + Environment.NewLine +
                    "gpu_count=" + consoleSettings.GPUCount + Environment.NewLine +
                    "gpu_index=" + consoleSettings.GPUIndex + Environment.NewLine +
                    "scan_type=" + consoleSettings.ScanType + Environment.NewLine +
                    "scan_rewards=" + consoleSettings.ScanRewards + Environment.NewLine +
                    "custom_range=" + consoleSettings.CustomRange + Environment.NewLine +
                    "api_share=" + consoleSettings.ApiShare + Environment.NewLine +
                    "telegram_share=" + consoleSettings.TelegramShare + Environment.NewLine +
                    "telegram_accesstoken=" + consoleSettings.TelegramAccessToken + Environment.NewLine +
                    "telegram_chatid=" + consoleSettings.TelegramChatId + Environment.NewLine +
                    "telegram_share_eachkey=" + consoleSettings.TelegramShareEachKey + Environment.NewLine +
                    "untrusted_computer=" + consoleSettings.UntrustedComputer + Environment.NewLine +
                    "test_mode=" + consoleSettings.TestMode + Environment.NewLine +
                    "force_continue=" + consoleSettings.ForceContinue + Environment.NewLine +
                    "private_pool=" + consoleSettings.PrivatePool;
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(appPath, "settings.txt")))
                {
                    outputFile.WriteLine(savedSettings);
                }
                Helper.Write("\nSettings saved successfully. App starting ...", ConsoleColor.Green);
                Thread.Sleep(2000);
            }
            else
            {
                Helper.Write("\nApp starting ...", ConsoleColor.Green);
                Thread.Sleep(2000);
            }

            return consoleSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        private static string DetermineSettings(string message, string[]? values = null, int minLength = 0)
        {
            string value = "";
            string messageFormat = values == null ? string.Format("[Settings] {0} : ", message) : string.Format("[Settings] {0} ({1}) : ", message, string.Join('/', values));

            if (minLength > 0)
            {
                while (value.Length < minLength)
                {
                    Helper.Write(messageFormat);
                    value = Console.ReadLine() ?? "";
                }
            }
            else if (values != null)
            {
                while (!values.Contains(value))
                {
                    Helper.Write(messageFormat);
                    value = Console.ReadLine() ?? "";
                }
            }
            else
            {
                Helper.Write(messageFormat);
                value = Console.ReadLine() ?? "";
            }
            Helper.Write("-------------------------------\n");
            return value;
        }
    }
}
