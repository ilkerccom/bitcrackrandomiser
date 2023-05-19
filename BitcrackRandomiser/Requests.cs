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
        /// <returns></returns>
        public static async Task<string> GetHex()
        {
            using var client = new HttpClient { BaseAddress = new Uri(ApiURL) };
            string Result = await client.GetAsync("hex/get").Result.Content.ReadAsStringAsync();
            return Result;
        }

        /// <summary>
        /// Get percentage
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetPercentage()
        {
            using var client = new HttpClient { BaseAddress = new Uri(ApiURL) };
            string Result = await client.GetAsync("hex/percentage").Result.Content.ReadAsStringAsync();
            return Result;
        }

        /// <summary>
        /// HEX will be flagged
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> SetHex(string HEX, string WalletAddress)
        {
            bool isSuccess = false;
            using var client = new HttpClient { BaseAddress = new Uri(ApiURL) };
            client.DefaultRequestHeaders.Add("HEX", HEX);
            client.DefaultRequestHeaders.Add("WalletAddress", WalletAddress);
            string Result = await client.PostAsync("hex/flag", null).Result.Content.ReadAsStringAsync();
            Boolean.TryParse(Result, out isSuccess);
            return isSuccess;
        }
    }
}
