using BitcrackRandomiser.Models;

namespace BitcrackRandomiser
{
    internal class Requests
    {
        // API URL endpoint
        public static string apiURL = "https://api.btcpuzzle.info/";

        /// <summary>
        /// Get random HEX value from API
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static async Task<string> GetHex(Settings settings)
        {
            try
            {
                // CustomRange
                string startsWith = settings.CustomRange;
                string targetPuzzle = settings.TargetPuzzle;
                string scanType = settings.ScanType;
                using var client = new HttpClient { BaseAddress = new Uri(apiURL) };
                client.DefaultRequestHeaders.Add("UserToken", settings.UserToken);
                client.DefaultRequestHeaders.Add("WalletAddress", settings.WalletAddress);
                client.DefaultRequestHeaders.Add("PrivatePool", settings.PrivatePool);

                // Request
                var request = await client.GetAsync(string.Format("hex/getv3?startswith={0}&puzzlecode={1}&scantype={2}", startsWith, targetPuzzle, scanType));
                string result = await request.Content.ReadAsStringAsync();
                if (result.Length >= 6 && result.Length <= 160 && request.IsSuccessStatusCode)
                {
                    return result;
                }
                return "";
            }
            catch { return ""; }
        }

        /// <summary>
        /// HEX will be flagged as scanned
        /// </summary>
        /// <param name="HEX">Scanned HEX</param>
        /// <param name="WalletAddress">Worker wallet address</param>
        /// <param name="ProofKey">Proof key for marking</param>
        /// <param name="GPUName">Current GPU name</param>
        /// <param name="TargetPuzzle">Target Puzzle</param>
        /// <returns></returns>
        public static async Task<bool> SetHex(string hex, string walletAddress, string proofKey, string gpuName, string privatePool = "none", string targetPuzzle = "66")
        {
            try
            {
                bool isSuccess = false;
                using var client = new HttpClient { BaseAddress = new Uri(apiURL) };
                client.DefaultRequestHeaders.Add("HEX", hex);
                client.DefaultRequestHeaders.Add("WalletAddress", walletAddress);
                client.DefaultRequestHeaders.Add("PrivatePool", privatePool);
                client.DefaultRequestHeaders.Add("ProofKey", proofKey);
                client.DefaultRequestHeaders.Add("GPU", gpuName);
                string result = await client.PostAsync(string.Format("hex/flag?puzzlecode={0}", targetPuzzle), null).Result.Content.ReadAsStringAsync();
                Boolean.TryParse(result, out isSuccess);
                return isSuccess;
            }
            catch { return false; }
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
                    bool isSuccess = false;
                    using var client = new HttpClient { BaseAddress = new Uri(settings.ApiShare) };
                    client.DefaultRequestHeaders.Add("Status", apiShare.Status.ToString());
                    client.DefaultRequestHeaders.Add("HEX", apiShare.HEX);
                    client.DefaultRequestHeaders.Add("PrivateKey", apiShare.PrivateKey);
                    client.DefaultRequestHeaders.Add("TargetPuzzle", settings.TargetPuzzle);
                    client.DefaultRequestHeaders.Add("WorkerAddress", settings.ParsedWalletAddress);
                    client.DefaultRequestHeaders.Add("WorkerName", settings.ParsedWorkerName);
                    client.DefaultRequestHeaders.Add("ScanType", settings.ScanType.ToString());
                    string result = await client.PostAsync("", null).Result.Content.ReadAsStringAsync();
                    Boolean.TryParse(result, out isSuccess);
                    return isSuccess;
                }

                return false;
            }
            catch { return false; }
        }
    }
}
