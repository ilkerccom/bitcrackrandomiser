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
#if (DEBUG)
        public static string ApiURL = "https://localhost:7141/";
#elif (RELEASE)
        public static string ApiURL = "https://api.btcpuzzle.info/";
#endif

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

                // Include defeated ranges
                bool IncludeDefeatedRanges = settings.ScanType == ScanType.includeDefeatedRanges;

                // Request
                Result = await client.GetAsync(string.Format("hex/get?startswith={0}&puzzlecode={1}&includedefeatedranges={2}", StartsWith, TargetPuzzle, IncludeDefeatedRanges)).Result.Content.ReadAsStringAsync();
                if (Result.Length >= 7 && Result.Length <= 46)
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
        /// <param name="HEX">Scanned HEX</param>
        /// <param name="WalletAddress">Worker wallet address</param>
        /// <param name="ProofKey">Proof key for marking</param>
        /// <param name="GPUName">Current GPU name</param>
        /// <param name="TargetPuzzle">Target Puzzle</param>
        /// <returns></returns>
        public static async Task<bool> SetHex(string HEX, string WalletAddress, string ProofKey, string GPUName, string TargetPuzzle = "66")
        {
            try
            {
                bool isSuccess = false;
                using var client = new HttpClient { BaseAddress = new Uri(ApiURL) };
                client.DefaultRequestHeaders.Add("HEX", HEX);
                client.DefaultRequestHeaders.Add("WalletAddress", WalletAddress);
                client.DefaultRequestHeaders.Add("ProofKey", ProofKey);
                client.DefaultRequestHeaders.Add("GPU", GPUName);
                string Result = await client.PostAsync(string.Format("hex/flag?puzzlecode={0}", TargetPuzzle), null).Result.Content.ReadAsStringAsync();
                Boolean.TryParse(Result, out isSuccess);
                return isSuccess;
            }
            catch { return false; }
        }
    }
}
