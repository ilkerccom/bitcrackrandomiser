using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcrackRandomiser
{
    internal class Settings
    {
        /// <summary>
        /// Target puzzle number. [66,67,68]
        /// </summary>
        public string TargetPuzzle { get; set; } = "66";

        /// <summary>
        /// Bitcrack app folder path
        /// </summary>
        public string BitcrackPath { get; set; } = string.Empty;

        /// <summary>
        /// Bitcrack args
        /// Example: -b 896 -t 256 -p 256
        /// </summary>
        public string BitcrackArgs { get; set; } = string.Empty;

        /// <summary>
        /// Wallet address for worker
        /// </summary>
        public string WalletAddress { get; set; } = string.Empty;

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
        public string CustomRange { get; set; } = string.Empty;

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
}
