using System;
using System.Diagnostics;

namespace BitcrackRandomiser
{
    internal class Settings
    {
        /// <summary>
        /// Target puzzle number. [66,67,68]
        /// </summary>
        public string TargetPuzzle { get; set; } = "66";

        /// <summary>
        /// Which app will be used.
        /// </summary>
        public AppType AppType { get; set; } = AppType.bitcrack;

        /// <summary>
        /// Bitcrack app folder path
        /// </summary>
        public string AppPath { get; set; } = string.Empty;

        /// <summary>
        /// Bitcrack args
        /// Example: -b 896 -t 256 -p 256
        /// </summary>
        string _AppArgs = "";
        public string AppArgs
        {
            get
            {
                if (_AppArgs.Contains("-o"))
                {
                    return _AppArgs.Replace("-o", "x");
                }
                else if (_AppArgs.Contains("--keyspace"))
                {
                    return _AppArgs.Replace("--keyspace", "x");
                }
                return _AppArgs;
            }
            set
            {
                _AppArgs = value;
            }
        }

        /// <summary>
        /// User token value
        /// </summary>
        public string UserToken { get; set; } = "";

        /// <summary>
        /// Wallet address for worker
        /// </summary>
        string _WalletAddress = "";
        public string WalletAddress { 
            get
            {
                if(!_WalletAddress.Contains('.'))
                {
                    var Random = new Random();
                    _WalletAddress = string.Format("{0}.worker{1}", _WalletAddress, Random.Next(1000, 9999));
                }

                if (_WalletAddress.Length < 6)
                {
                    _WalletAddress = "Unknown";
                }

                return _WalletAddress;
            }
            set
            {
                _WalletAddress = value;
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
                if(Uri.IsWellFormedUriString(ApiShare, UriKind.Absolute))
                {
                    return true;
                }

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
                if(PrivatePool.Length == 8)
                {
                    return true;
                }

                return false;
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
            string Path = AppDomain.CurrentDomain.BaseDirectory + "settings.txt";

            // From file
            string[] Lines = File.ReadLines(Path).ToArray();

            // From arguments
            if(args.Length > 0)
            {
                Lines = args;
            }

            foreach (var line in Lines)
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
                            AppType _at = AppType.bitcrack;
                            _ = Enum.TryParse(value, true, out _at);
                            settings.AppType = _at;
                            break;
                        case "app_path":
                            if(value == "cuBitcrack" || value == "clBitcrack")
                            {
                                if(Environment.OSVersion.Platform == PlatformID.Win32NT)
                                {
                                    value = string.Format("{0}bitcrack\\{1}.exe", AppDomain.CurrentDomain.BaseDirectory, value);
                                }
                                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                                {
                                    value = string.Format("{0}bitcrack/./{1}", AppDomain.CurrentDomain.BaseDirectory, value);
                                }
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
                        case "scan_type":
                            settings.ScanType = value;
                            break;
                        case "custom_range":
                            settings.CustomRange = value;
                            break;
                        case "api_share":
                            settings.ApiShare = value;
                            break;
                        case "telegram_share":
                            bool _v;
                            _ = bool.TryParse(value, out _v);
                            settings.TelegramShare = _v;
                            break;
                        case "telegram_accesstoken":
                            settings.TelegramAccessToken = value;
                            break;
                        case "telegram_chatid":
                            settings.TelegramChatId = value;
                            break;
                        case "telegram_share_eachkey":
                            bool _s;
                            _ = bool.TryParse(value, out _s);
                            settings.TelegramShareEachKey = _s;
                            break;
                        case "untrusted_computer":
                            bool _u;
                            _ = bool.TryParse(value, out _u);
                            settings.UntrustedComputer = _u;
                            break;
                        case "test_mode":
                            bool _t;
                            _ = bool.TryParse(value, out _t);
                            settings.TestMode = _t;
                            break;
                        case "force_continue":
                            bool _f;
                            _ = bool.TryParse(value, out _f);
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
            var ConsoleSettings = new Settings();

            // Select puzzle
            string _Puzzle = DetermineSettings("Select a puzzle number", new string[3] { "66", "67", "68" });

            // Select app path
            string _Folder = DetermineSettings("Enter app folder path [cuBitcrack, clBitcrack or full path of Bitcrack]", null, 6);

            // App arguments
            var _Arguments = DetermineSettings("Enter app arguments (can be empty)");

            // User token
            string _UserToken = DetermineSettings("Your user token value", null, 20);

            // Wallet address
            string _WalletAddress = DetermineSettings("Your BTC wallet address", null, 20);

            // Scan type
            string _ScanType = DetermineSettings("Select a scan type", new string[2] { "default", "includeDefeatedRanges" });

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
            ConsoleSettings.TargetPuzzle = _Puzzle;
            ConsoleSettings.AppPath = _Folder;
            ConsoleSettings.AppArgs = _Arguments;
            ConsoleSettings.UserToken = _UserToken;
            ConsoleSettings.WalletAddress = _WalletAddress;
            ConsoleSettings.ScanType = _ScanType;
            ConsoleSettings.CustomRange = _CustomRange;
            ConsoleSettings.ApiShare = _ApiShare;
            ConsoleSettings.TelegramShare = bool.Parse(_TelegramShare);
            ConsoleSettings.TelegramAccessToken = _TelegramAccessToken;
            ConsoleSettings.TelegramChatId = _TelegramChatId;
            ConsoleSettings.TelegramShareEachKey = bool.Parse(_TelegramShareEachKey);
            ConsoleSettings.UntrustedComputer = bool.Parse(_UntrustedComputer);
            ConsoleSettings.TestMode = bool.Parse(_TestMode);
            ConsoleSettings.ForceContinue = bool.Parse(_ForceContinue);
            ConsoleSettings.PrivatePool = _PrivatePool;

            // Will save settings
            string _SaveSettings = "";
            while (_SaveSettings != "yes" && _SaveSettings != "no")
            {
                Helpers.Write("Do you want to save settings to settings.txt? (yes/no) : ", ConsoleColor.Cyan);
                _SaveSettings = Console.ReadLine() ?? "";
            }

            // Save settings
            if (_SaveSettings == "yes")
            {
                string SavedSettings = 
                    "target_puzzle=" + ConsoleSettings.TargetPuzzle + Environment.NewLine +
                    "app_path=" + ConsoleSettings.AppPath + Environment.NewLine +
                    "app_arguments=" + ConsoleSettings.AppArgs + Environment.NewLine +
                    "user_token=" + ConsoleSettings.UserToken + Environment.NewLine +
                    "wallet_address=" + ConsoleSettings.WalletAddress + Environment.NewLine +
                    "scan_type=" + ConsoleSettings.ScanType + Environment.NewLine +
                    "custom_range=" + ConsoleSettings.CustomRange + Environment.NewLine +
                    "api_share=" + ConsoleSettings.ApiShare + Environment.NewLine +
                    "telegram_share=" + ConsoleSettings.TelegramShare + Environment.NewLine +
                    "telegram_accesstoken=" + ConsoleSettings.TelegramAccessToken + Environment.NewLine +
                    "telegram_chatid=" + ConsoleSettings.TelegramChatId + Environment.NewLine +
                    "telegram_share_eachkey=" + ConsoleSettings.TelegramShareEachKey + Environment.NewLine +
                    "untrusted_computer=" + ConsoleSettings.UntrustedComputer + Environment.NewLine +
                    "test_mode=" + ConsoleSettings.TestMode + Environment.NewLine +
                    "force_continue=" + ConsoleSettings.ForceContinue + Environment.NewLine +
                    "private_pool=" + ConsoleSettings.PrivatePool;
                string AppPath = AppDomain.CurrentDomain.BaseDirectory;
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(AppPath, "settings.txt")))
                {
                    outputFile.WriteLine(SavedSettings);
                }
                Helpers.Write("\nSettings saved successfully. App starting ...", ConsoleColor.Green);
                Thread.Sleep(2000);
            }
            else
            {
                Helpers.Write("\nApp starting ...", ConsoleColor.Green);
                Thread.Sleep(2000);
            }

            return ConsoleSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Values"></param>
        /// <param name="MinLength"></param>
        /// <returns></returns>
        private static string DetermineSettings(string Message, string[]? Values = null, int MinLength = 0)
        {
            string _Value = "";
            string MessageFormat = Values == null ? string.Format("[Settings] {0} : ", Message) : string.Format("[Settings] {0} ({1}) : ", Message, string.Join('/', Values));

            if (MinLength > 0)
            {
                while (_Value.Length < MinLength)
                {
                    Helpers.Write(MessageFormat);
                    _Value = Console.ReadLine() ?? "";
                }
            }
            else if (Values != null)
            {
                while (!Values.Contains(_Value))
                {
                    Helpers.Write(MessageFormat);
                    _Value = Console.ReadLine() ?? "";
                }
            }
            else
            {
                Helpers.Write(MessageFormat);
                _Value = Console.ReadLine() ?? "";
            }
            Helpers.Write("-------------------------------\n");
            return _Value;
        }
    }

    /// <summary>
    /// Console message type
    /// </summary>
    enum MessageType
    {
        normal,
        success,
        info,
        error
    }

    /// <summary>
    /// Apps to scan
    /// </summary>
    enum AppType
    {
        bitcrack,
        keyhunt
    }
}
