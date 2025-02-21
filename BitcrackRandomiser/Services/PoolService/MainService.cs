using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Models;
using BitcrackRandomiser.Services.PoolService.Models;
using Newtonsoft.Json;

namespace BitcrackRandomiser.Services.PoolService
{
    internal static class MainService
    {
        /// <summary>
        /// API URL endpoint
        /// </summary>
        public static readonly string _apiURL = "https://api.btcpuzzle.info/";

        /// <summary>
        /// Get random HEX value from API
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static async Task<(bool isSuccess, GetHexResponse? data, string? error)> GetHex(Setting settings, int gpuIndex)
        {
            try
            {
                // Wallet address
                string workerName = settings.WorkerName;
                if ((settings.AppType == AppType.bitcrack && settings.GPUCount > 1) || (settings.AppType == AppType.vanitysearch && settings.GPUSeperatedRange))
                    workerName += "_" + gpuIndex;

                // CustomRange
                string startsWith = settings.CustomRange;
                string targetPuzzle = settings!.TargetPuzzle!;
                using var client = new HttpClient { BaseAddress = new Uri(_apiURL) };
                client.DefaultRequestHeaders.Add("UserToken", settings.UserToken);
                client.DefaultRequestHeaders.Add("WorkerName", workerName);
                client.DefaultRequestHeaders.Add("CustomRange", settings.CustomRange);
                client.DefaultRequestHeaders.Add("SecurityHash", settings.SecurityHash);

                // Request
                var request = await client.GetAsync($"puzzle/{settings.TargetPuzzle}/range");
                string content = await request.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetHexResponse>(content);
                if (request.StatusCode == System.Net.HttpStatusCode.OK)
                    return (true, result, null);
                else if (request.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    return (false, null, content);

                Logger.LogError(null, $"Database connection error. Request: {request}, Content:{result}, Headers:{client.DefaultRequestHeaders}");
                return (false, null, null);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Database connection error");
                return (false, null, null);
            }
        }

        /// <summary>
        /// HEX will be flagged as scanned
        /// </summary>
        /// <param name="hex">Scanned HEX</param>
        /// <param name="workerName">Worker wallet address</param>
        /// <param name="proofKey">Proof key for marking</param>
        /// <param name="targetPuzzle">Target Puzzle</param>
        /// <returns></returns>
        public static async Task<bool> FlagHex(string userToken, string hex, string workerName, string hashedProofKey, string proofKey, string gpuName, int gpuCount, bool isForceContinue, string targetPuzzle)
        {
            try
            {
                using var client = new HttpClient { BaseAddress = new Uri(_apiURL) };
                client.DefaultRequestHeaders.Add("HEX", hex);
                client.DefaultRequestHeaders.Add("WorkerName", workerName);
                client.DefaultRequestHeaders.Add("GPUName", gpuName);
                client.DefaultRequestHeaders.Add("GPUCount", gpuCount.ToString());
                client.DefaultRequestHeaders.Add("IsForceContinue", isForceContinue.ToString());
                client.DefaultRequestHeaders.Add("HashedProofKey", hashedProofKey);
                client.DefaultRequestHeaders.Add("UserToken", userToken);
                var result = await client.PutAsync($"puzzle/{targetPuzzle}/range", null);
                bool isSuccess = result.IsSuccessStatusCode;

                if (!isSuccess)
                    Logger.LogError(null, $"HEX [{hex}] flag error. Puzzle: {targetPuzzle}, Proof (SHA256): {hashedProofKey}, Worker:{workerName}, Proof: {proofKey}");

                return isSuccess;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"HEX [{hex}] flag error.");
                return false;
            }
        }

        /// <summary>
        /// Get rewards from pool
        /// </summary>
        /// <param name="targetPuzzle">Target puzzle code</param>
        /// <returns></returns>
        public static async Task<string> GetRewards(string targetPuzzle)
        {
            try
            {
                using var client = new HttpClient { BaseAddress = new Uri(_apiURL), Timeout = TimeSpan.FromSeconds(10) };
                string result = await client.GetAsync($"puzzle/{targetPuzzle}/rewards").Result.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Cannot pull data from reward. Puzzle:{targetPuzzle}");
                return "";
            }
        }
    }
}
