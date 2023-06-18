using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BitcrackRandomiser
{
    internal class Requests
    {
        // API URL endpoint
        public static string ApiURL = "https://api.btcpuzzle.info/";

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
                string StartsWith = settings.CustomRange;
                string TargetPuzzle = settings.TargetPuzzle;
                string Result = "";
                using var client = new HttpClient { BaseAddress = new Uri(ApiURL) };

                // Include defeated ranges to scan
                if (settings.ScanType == ScanType.includeDefeatedRanges)
                {
                    Result = await client.GetAsync(string.Format("hex/getall?startswith={0}&puzzlecode={1}", StartsWith, TargetPuzzle)).Result.Content.ReadAsStringAsync();
                    if (Result.Length >= 7 && Result.Length <= 10)
                    {
                        return Result;
                    }
                    return "";
                }

                // Default ranges
                Result = await client.GetAsync(string.Format("hex/get?startswith={0}&puzzlecode={1}", StartsWith, TargetPuzzle)).Result.Content.ReadAsStringAsync();
                if (Result.Length >= 7 && Result.Length <= 10)
                {
                    return Result;
                }
                return "";
            }
            catch { return ""; }
        }

        /// <summary>
        /// HEX will be flagged as scanned
        /// </summary>
        /// <param name="HEX"></param>
        /// <param name="WalletAddress"></param>
        /// <param name="TargetPuzzle"></param>
        /// <returns></returns>
        public static async Task<bool> SetHex(string HEX, string WalletAddress, string GPUName, string TargetPuzzle = "66")
        {
            try
            {
                bool isSuccess = false;
                using var client = new HttpClient { BaseAddress = new Uri(ApiURL) };
                client.DefaultRequestHeaders.Add("HEX", HEX);
                client.DefaultRequestHeaders.Add("WalletAddress", WalletAddress);
                client.DefaultRequestHeaders.Add("GPU", GPUName);
                string Result = await client.PostAsync(string.Format("hex/flag?puzzlecode={0}", TargetPuzzle), null).Result.Content.ReadAsStringAsync();
                Boolean.TryParse(Result, out isSuccess);
                return isSuccess;
            }
            catch { return false; }
        }
    }
}
