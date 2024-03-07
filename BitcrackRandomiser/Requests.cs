using BitcrackRandomiser.Models;

namespace BitcrackRandomiser
{
    /// <summary>
    /// API requests only
    /// </summary>
    internal class Requests
    {
        /// <summary>
        /// API URL endpoint
        /// </summary>
        public static string apiURL = "https://api.btcpuzzle.info/";

        /// <summary>
        /// Get random HEX value from API
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static async Task<string> GetHex(Settings settings, int gpuIndex)
        {
            try
            {
                // Wallet address
                string walletAddress = settings.WalletAddress;
                if (settings.AppType == Enums.AppType.bitcrack && settings.GPUCount > 1)
                    walletAddress += "_" + gpuIndex;

                // CustomRange
                string startsWith = settings.CustomRange;
                string targetPuzzle = settings.TargetPuzzle;
                string scanType = settings.ScanType;
                using var client = new HttpClient { BaseAddress = new Uri(apiURL) };
                client.DefaultRequestHeaders.Add("UserToken", settings.UserToken);
                client.DefaultRequestHeaders.Add("WalletAddress", walletAddress);
                client.DefaultRequestHeaders.Add("PrivatePool", settings.PrivatePool);
                client.DefaultRequestHeaders.Add("SecurityHash", settings.SecurityHash);

                // Request
                var request = await client.GetAsync($"hex/getv3?startswith={startsWith}&puzzlecode={targetPuzzle}&scantype={scanType}");
                string result = await request.Content.ReadAsStringAsync();
                if (result.Length >= 6 && result.Length <= 160 && request.IsSuccessStatusCode)
                    return result;
                return "";
            }
            catch { return ""; }
        }

        /// <summary>
        /// HEX will be flagged as scanned
        /// </summary>
        /// <param name="hex">Scanned HEX</param>
        /// <param name="walletAddress">Worker wallet address</param>
        /// <param name="proofKey">Proof key for marking</param>
        /// <param name="gpuName">Current GPU name</param>
        /// <param name="privatePool">Private pool ID</param>
        /// <param name="targetPuzzle">Target Puzzle</param>
        /// <param name="gpuCount">GPU count</param>
        /// <returns></returns>
        public static async Task<bool> SetHex(string hex, string walletAddress, string proofKey, string gpuName, string privatePool = "none", string targetPuzzle = "66", int gpuCount = 1)
        {
            try
            {
                using var client = new HttpClient { BaseAddress = new Uri(apiURL) };
                client.DefaultRequestHeaders.Add("HEX", hex);
                client.DefaultRequestHeaders.Add("WalletAddress", walletAddress);
                client.DefaultRequestHeaders.Add("PrivatePool", privatePool);
                client.DefaultRequestHeaders.Add("ProofKey", proofKey);
                client.DefaultRequestHeaders.Add("GPU", gpuName ?? "-");
                client.DefaultRequestHeaders.Add("GPUCount", gpuCount.ToString());
                string result = await client.PostAsync($"hex/flag?puzzlecode={targetPuzzle}", null).Result.Content.ReadAsStringAsync();
                _ = bool.TryParse(result, out bool isSuccess);
                return isSuccess;
            }
            catch { return false; }
        }

        /// <summary>
        /// Get rewards from pool
        /// </summary>
        /// <param name="targetPuzzle">Target puzzle code</param>
        /// <returns></returns>
        public static async Task<string> GetRewards(string targetPuzzle = "66")
        {
            try
            {
                using var client = new HttpClient { BaseAddress = new Uri(apiURL), Timeout = TimeSpan.FromSeconds(10) };
                string result = await client.GetAsync($"hex/rewards?puzzlecode={targetPuzzle}").Result.Content.ReadAsStringAsync();
                return result;
            }
            catch { return ""; }
        }

        /// <summary>
        /// Share progress to your API
        /// </summary>
        /// <param name="apiShare"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static async Task<bool> SendApiShare(ApiShare apiShare, Settings settings)
        {
            try
            {
                if (settings.IsApiShare)
                {
                    using var client = new HttpClient { BaseAddress = new Uri(settings.ApiShare) };
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("Status", apiShare.Status.ToString());
                    client.DefaultRequestHeaders.Add("HEX", apiShare.HEX);
                    client.DefaultRequestHeaders.Add("PrivateKey", apiShare.PrivateKey);
                    client.DefaultRequestHeaders.Add("TargetPuzzle", settings.TargetPuzzle);
                    client.DefaultRequestHeaders.Add("WorkerAddress", settings.ParsedWalletAddress);
                    client.DefaultRequestHeaders.Add("WorkerName", settings.ParsedWorkerName);
                    client.DefaultRequestHeaders.Add("ScanType", settings.ScanType.ToString());
                    string result = await client.PostAsync("", null).Result.Content.ReadAsStringAsync();
                    _ = bool.TryParse(result, out bool isSuccess);
                    return isSuccess;
                }

                return false;
            }
            catch { return false; }
        }
    }
}
