using BitcrackRandomiser.Models;

namespace BitcrackRandomiser.Services
{
    internal static class CustomAPIService
    {
        /// <summary>
        /// Send info to your custom API
        /// </summary>
        /// <param name="apiShare"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static async Task<bool> SendApiShare(ApiShare apiShare, Setting settings)
        {
            try
            {
                if (settings.IsApiShare)
                {
                    using var client = new HttpClient { BaseAddress = new Uri(settings?.ApiShare!) };
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("Status", apiShare.Status.ToString());
                    client.DefaultRequestHeaders.Add("HEX", apiShare.HEX);
                    client.DefaultRequestHeaders.Add("PrivateKey", apiShare.PrivateKey);
                    client.DefaultRequestHeaders.Add("TargetPuzzle", settings?.TargetPuzzle);
                    client.DefaultRequestHeaders.Add("WorkerName", settings?.WorkerName);
                    client.DefaultRequestHeaders.Add("CustomRange", settings?.CustomRange);
                    string result = await client.PostAsync("", null).Result.Content.ReadAsStringAsync();
                    _ = bool.TryParse(result, out bool isSuccess);

                    if (!isSuccess)
                        Logger.LogError(null, $"API share failed. Url:{settings?.ApiShare}, Puzzle:{settings?.TargetPuzzle}, Wallet:{settings?.WorkerName}");
                    else
                        Logger.LogInformation($"API share successfully. Url:{settings?.ApiShare}");

                    return isSuccess;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"API share failed. Url:{settings.ApiShare}");
                return false;
            }
        }
        
    }
}
