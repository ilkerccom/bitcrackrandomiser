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
        /// <param name="ScanType">default or includeDefeatedRanges</param>
        /// <returns></returns>
        public static async Task<string> GetHex(string ScanType = "default")
        {
            try
            {
                string Result = "";
                using var client = new HttpClient { BaseAddress = new Uri(ApiURL) };

                if (ScanType == "includeDefeatedRanges")
                {
                    Result = await client.GetAsync("hex/getall").Result.Content.ReadAsStringAsync();
                    return Result;
                }

                Result = await client.GetAsync("hex/get").Result.Content.ReadAsStringAsync();
                return Result;
            }
            catch { return ""; }
        }

        /// <summary>
        /// HEX will be flagged
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> SetHex(string HEX, string WalletAddress)
        {
            try
            {
                bool isSuccess = false;
                using var client = new HttpClient { BaseAddress = new Uri(ApiURL) };
                client.DefaultRequestHeaders.Add("HEX", HEX);
                client.DefaultRequestHeaders.Add("WalletAddress", WalletAddress);
                string Result = await client.PostAsync("hex/flag", null).Result.Content.ReadAsStringAsync();
                Boolean.TryParse(Result, out isSuccess);
                return isSuccess;
            }
            catch { return false; }
        }
    }
}
