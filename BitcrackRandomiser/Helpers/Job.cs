using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Models;
using System.Diagnostics;

namespace BitcrackRandomiser.Helpers
{
    /// <summary>
    /// BitcrackRandomiser job class.
    /// </summary>
    internal class Job
    {
        /// <summary>
        /// Get current status of external app (Bitcrack or another)
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <param name="gpuIndex">GPU index</param>
        /// <param name="hex">HEX</param>
        /// <returns>
        /// [string] output message of external app
        /// </returns>
        public static Result GetStatus(object o, DataReceivedEventArgs e, int gpuIndex, string hex)
        {
            if (e.Data != null)
            {
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
                int consoleWidth = 720;
                if (Environment.UserInteractive && !Console.IsInputRedirected)
                    consoleWidth = Console.WindowWidth;

                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 9 + gpuIndex);

                if (e.Data.Contains("Reached") || e.Data.Contains("No targets remaining"))
                {
                    string finishedMessage = string.Format("[{0}] [Info] {1}", currentDate, "Scan completed.");
                    Console.Write(finishedMessage + new string(' ', consoleWidth - finishedMessage.Length));
                    return new Result { OutputType = OutputType.finished };
                }
                else if (e.Data.Contains("Address:"))
                    return new Result { OutputType = OutputType.address, Content = e.Data.Trim() };
                else if (e.Data.Contains("Private key:"))
                {
                    string key = e.Data.Replace(" ", "").ToLower().Trim().Replace("privatekey:", "").Trim().ToUpper();
                    return new Result { OutputType = OutputType.privateKeyFound, Content = key };
                }
                else if (e.Data.Contains("Initializing"))
                {
                    string gpuModel = e.Data[e.Data.IndexOf("Initializing")..].Replace("Initializing", "").Trim();
                    return new Result { OutputType = OutputType.gpuModel, Content = gpuModel };
                }
                else
                {
                    string data = e.Data.Trim();
                    if (data.Length > 0)
                    {
                        string message = string.Format("{0} [GPU={1}] [HEX={2}]", data.Length > 1 ? data : "...", gpuIndex, hex);
                        int totalLength = consoleWidth - message.Length;
                        string spaces = totalLength > 0 ? new string(' ', totalLength) : "";
                        Console.Write($"{message}{spaces}");
                    }
                    else
                    {
                        string loadingMessage = string.Format("[{0}] [Info] Running ... [GPU={2}] [HEX:{1}]", currentDate, hex, gpuIndex);
                        Console.Write(loadingMessage + new string(' ', consoleWidth - loadingMessage.Length));
                    }
                    return new Result { OutputType = OutputType.running };
                }
            }

            return new Result { OutputType = OutputType.unknown };
        }
    }
}
