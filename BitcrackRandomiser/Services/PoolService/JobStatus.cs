using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Models;
using System.Diagnostics;

namespace BitcrackRandomiser.Services.PoolService
{
    /// <summary>
    /// BitcrackRandomiser job class.
    /// </summary>
    internal static class JobStatus
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
        public static Result GetStatus(object o, DataReceivedEventArgs e, int gpuIndex, string hex, AppType appType)
        {
            if (e.Data != null)
            {
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
                int consoleWidth = 720;
                if (Environment.UserInteractive && !Console.IsInputRedirected)
                    consoleWidth = Console.WindowWidth;

                if (!Program.isCloudSearchMode)
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(0, 9 + gpuIndex);
                }

                if (appType == AppType.bitcrack)
                {
                    /// Bitcrack
                    if (e.Data.Contains("Reached") || e.Data.Contains("No targets remaining"))
                    {
                        string finishedMessage = string.Format("[{0}] [Info] {1}", currentDate, "Scan completed.");
                        if (Program.isCloudSearchMode)
                            Console.WriteLine(finishedMessage + new string(' ', consoleWidth - finishedMessage.Length));
                        else
                            Console.Write(finishedMessage + new string(' ', consoleWidth - finishedMessage.Length));
                        return new Result { OutputType = OutputType.finished };
                    }
                    else if (e.Data.Contains("Address:"))
                    {
                        string address = e.Data.Split(':').Last().Trim();
                        return new Result { OutputType = OutputType.address, Content = address };
                    }
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
                            if (Program.isCloudSearchMode)
                                Console.WriteLine($"{message}{spaces}");
                            else
                                Console.Write($"{message}{spaces}");
                        }
                        else
                        {
                            string loadingMessage = string.Format("[{0}] [Info] Running ... [GPU={2}] [HEX:{1}]", currentDate, hex, gpuIndex);
                            if(Program.isCloudSearchMode)
                                Console.WriteLine(loadingMessage + new string(' ', consoleWidth - loadingMessage.Length));
                            else
                                Console.Write(loadingMessage + new string(' ', consoleWidth - loadingMessage.Length));
                        }
                        return new Result { OutputType = OutputType.running };
                    }
                }
                else if (appType == AppType.vanitysearch || appType == AppType.cpu)
                {
                    /// VanitySearch
                    if (e.Data.Contains("[EXIT]"))
                    {
                        string finishedMessage = string.Format("[{0}] [Info] {1}", currentDate, "Scan completed.");
                        if (Program.isCloudSearchMode)
                            Console.WriteLine(finishedMessage + new string(' ', consoleWidth - finishedMessage.Length));
                        else
                            Console.Write(finishedMessage + new string(' ', consoleWidth - finishedMessage.Length));
                        return new Result { OutputType = OutputType.finished };
                    }
                    else if (e.Data.Contains("Public Addr:"))
                    {
                        string address = e.Data.Split(':').Last().Trim();
                        return new Result { OutputType = OutputType.address, Content = address };
                    }
                    else if (e.Data.Contains("Priv (HEX):"))
                    {
                        string key = e.Data.Split(':').Last().Trim().Replace("0x", "").Trim();
                        if (key.Length != 64)
                            key = new string('0', 64 - key.Length) + key;
                        return new Result { OutputType = OutputType.privateKeyFound, Content = key };
                    }
                    else if (e.Data.Contains("GPU:"))
                    {
                        try
                        {
                            string gpuModel = e.Data.Split(':').Last().Trim().Replace("GPU", "");
                            gpuModel = gpuModel.Substring(gpuModel.IndexOf('#') + 2);
                            gpuModel = gpuModel.Remove(gpuModel.IndexOf('(')).Trim();
                            return new Result { OutputType = OutputType.gpuModel, Content = gpuModel };
                        }
                        catch
                        {
                            return new Result { OutputType = OutputType.gpuModel, Content = "-" };
                        }
                    }
                    else
                    {
                        string data = e.Data.Trim();
                        if (data.Length > 0)
                        {
                            string message = string.Format("{0} [HEX={2}]", data.Length > 1 ? data : "...", gpuIndex, hex);
                            int totalLength = consoleWidth - message.Length;
                            string spaces = totalLength > 0 ? new string(' ', totalLength) : "";

                            if(Program.isCloudSearchMode)
                                Console.WriteLine($"{message}{spaces}");
                            else
                                Console.Write($"{message}{spaces}");
                        }
                        else
                        {
                            string loadingMessage = string.Format("[{0}] [Info] Running ... [HEX:{1}]", currentDate, hex, gpuIndex);
                            if(Program.isCloudSearchMode)
                                Console.WriteLine(loadingMessage + new string(' ', consoleWidth - loadingMessage.Length));
                            else
                                Console.Write(loadingMessage + new string(' ', consoleWidth - loadingMessage.Length));
                        }
                        return new Result { OutputType = OutputType.running };
                    }
                }
            }

            return new Result { OutputType = OutputType.unknown };
        }
    }
}
