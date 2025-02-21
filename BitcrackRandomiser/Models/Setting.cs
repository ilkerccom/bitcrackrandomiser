using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Services;
using System.Reflection;

namespace BitcrackRandomiser.Models
{
    class Setting
    {
        /// <summary>
        /// Target puzzle number
        /// </summary>
        public string? TargetPuzzle { get; set; }

        /// <summary>
        /// Which app will be used.
        /// </summary>
        public AppType AppType { get; set; } = AppType.vanitysearch;

        /// <summary>
        /// Bitcrack app folder path
        /// </summary>
        public string? AppPath { get; set; }

        /// <summary>
        /// Bitcrack args
        /// Example: -b 896 -t 256 -p 256
        /// </summary>
        public string? AppArgs { get; set; }

        /// <summary>
        /// User token value
        /// </summary>
        public string? UserToken { get; set; }

        /// <summary>
        /// Worker name
        /// </summary>
        string _workerName = "";
        public string WorkerName
        {
            get => string.IsNullOrEmpty(_workerName) || _workerName.Length < 1 || _workerName.Length > 16
                ? _workerName = $"worker{new Random().Next(1000, 9999)}"
                : _workerName;
            set => _workerName = value;
        }

        /// <summary>
        /// GPU count. Max 32 GPUs
        /// </summary>
        int _gpuCount = 1;
        public int GPUCount
        {
            get => Math.Clamp(_gpuCount, 1, 32);
            set => _gpuCount = value;
        }

        /// <summary>
        /// Active GPU for scanning.
        /// </summary>
        int _gpuIndex = 0;
        public int GPUIndex
        {
            get => (_gpuIndex < 0 || _gpuIndex >= 16) ? 0 : _gpuIndex;
            set => _gpuIndex = value;
        }

        /// <summary>
        /// VanitySearch only. When running multiple graphics cards, it uses each graphics card as a separate worker.
        /// </summary>
        public bool GPUSeperatedRange { get; set; } = false;

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
        public string? ApiShare { get; set; }

        /// <summary>
        /// Is API share is active
        /// </summary>
        public bool IsApiShare => Uri.IsWellFormedUriString(ApiShare, UriKind.Absolute);

        /// <summary>
        /// Telegram share is active
        /// </summary>
        public bool TelegramShare { get; set; }

        /// <summary>
        /// Telegram access token
        /// </summary>
        public string? TelegramAccessToken { get; set; }

        /// <summary>
        /// Telegram chat id
        /// </summary>
        public string? TelegramChatId { get; set; }

        /// <summary>
        /// Send notification when each key scanned
        /// </summary>
        public bool TelegramShareEachKey { get; set; }

        /// <summary>
        /// Leave true on untrusted computer
        /// Private key is not written to the file and console screen.
        /// Private key will be delivered to Telegram only.
        /// </summary>
        public bool UntrustedComputer { get; set; }

        /// <summary>
        /// Test mode.
        /// If true, example private key will be found.
        /// </summary>
        public bool TestMode { get; set; }

        /// <summary>
        /// Force continue to if key found
        /// If true, scan will continue until it is finished and will marked as scanned
        /// </summary>
        public bool ForceContinue { get; set; }

        /// <summary>
        /// Hashed settings for untrusted computers.
        /// When the any setting changes, the hash value also changes.
        /// </summary>
        public string SecurityHash
        {
            get
            {
                string buildId = Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToString();
                string data = $"{TargetPuzzle}-{AppPath}-{AppArgs}-{WorkerName}-{ApiShare}-{TelegramShare}-" +
                    $"{TelegramChatId}-{CustomRange}-{UntrustedComputer}-{ForceContinue}-{UserToken}-{buildId}";
                return Helper.StringParser(value: Helper.SHA256Hash(data), length: 5, addDots: false);
            }
        }
    }
}
