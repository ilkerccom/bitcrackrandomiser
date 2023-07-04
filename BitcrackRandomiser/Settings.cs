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
        /// default: Exclude defeated ranges
        /// includeDefeatedRanges: Include defeated ranges to scan
        /// </summary>
        public ScanType ScanType { get; set; } = ScanType.@default;

        /// <summary>
        /// Custom ranges to scan
        /// Min length 2
        /// Max length 5
        /// Example [20,3FF,2DAB]
        /// </summary>
        public string CustomRange { get; set; } = "none";

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
                            settings.AppPath = value;
                            break;
                        case "app_arguments":
                            settings.AppArgs = value;
                            break;
                        case "wallet_address":
                            settings.WalletAddress = value;
                            break;
                        case "scan_type":
                            ScanType _e = ScanType.@default;
                            _ = Enum.TryParse(value, true, out _e);
                            settings.ScanType = _e;
                            break;
                        case "custom_range":
                            settings.CustomRange = value;
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
            string _Folder = DetermineSettings("Enter app folder path", null, 6);

            // App arguments
            var _Arguments = DetermineSettings("Enter app arguments (can be empty)");

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

            // Settings
            ConsoleSettings.TargetPuzzle = _Puzzle;
            ConsoleSettings.AppPath = _Folder;
            ConsoleSettings.AppArgs = _Arguments;
            ConsoleSettings.WalletAddress = _WalletAddress;
            ConsoleSettings.ScanType = (ScanType)Enum.Parse(typeof(ScanType), _ScanType);
            ConsoleSettings.CustomRange = _CustomRange;
            ConsoleSettings.TelegramShare = bool.Parse(_TelegramShare);
            ConsoleSettings.TelegramAccessToken = _TelegramAccessToken;
            ConsoleSettings.TelegramChatId = _TelegramChatId;
            ConsoleSettings.TelegramShareEachKey = bool.Parse(_TelegramShareEachKey);
            ConsoleSettings.UntrustedComputer = bool.Parse(_UntrustedComputer);
            ConsoleSettings.TestMode = bool.Parse(_TestMode);

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
                    "wallet_address=" + ConsoleSettings.WalletAddress + Environment.NewLine +
                    "scan_type=" + ConsoleSettings.ScanType + Environment.NewLine +
                    "custom_range=" + ConsoleSettings.CustomRange + Environment.NewLine +
                    "telegram_share=" + ConsoleSettings.TelegramShare + Environment.NewLine +
                    "telegram_accesstoken=" + ConsoleSettings.TelegramAccessToken + Environment.NewLine +
                    "telegram_chatid=" + ConsoleSettings.TelegramChatId + Environment.NewLine +
                    "telegram_share_eachkey=" + ConsoleSettings.TelegramShareEachKey + Environment.NewLine +
                    "untrusted_computer=" + ConsoleSettings.UntrustedComputer + Environment.NewLine +
                    "test_mode=" + ConsoleSettings.TestMode;
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
    /// Scan type
    /// </summary>
    enum ScanType
    {
        @default = 0,
        includeDefeatedRanges = 1
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
