using System.Diagnostics;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Text;
using System.Security.Cryptography;
using BitcrackRandomiser.Models;
using BitcrackRandomiser.Enums;

namespace BitcrackRandomiser
{
    internal class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string message, MessageType type = MessageType.normal, bool withClear = false, int gpuIndex = 0)
        {
            // Clear console
            if (withClear)
            {
                Console.Clear();
            }

            // Message type
            if (type == MessageType.error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (type == MessageType.success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (type == MessageType.info)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            // Write message
            if (type == MessageType.externalApp)
            {
                Console.SetCursorPosition(0, 10 + gpuIndex);
                Console.Write(string.Format("[{0}] [Info] {1}", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), message));
            }
            else
            {
                Console.WriteLine(string.Format("[{0}] [Info] {1}", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), message));
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Color"></param>
        public static void Write(string message, ConsoleColor Color = ConsoleColor.Blue)
        {
            // Write message
            Console.ForegroundColor = Color;
            Console.Write(string.Format("{0}", message));
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Result CheckJobStatus(object o, DataReceivedEventArgs e, int gpuIndex, string hex)
        {
            if (e.Data != null)
            {
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
                int consoleWidth = 720;
                if (Environment.UserInteractive && !Console.IsInputRedirected)
                    consoleWidth = Console.WindowWidth;

                if (e.Data.Contains("Reached") || e.Data.Contains("No targets remaining"))
                {
                    Console.SetCursorPosition(0, 10 + gpuIndex);
                    string finishedMessage = string.Format("[{0}] [Info] {1}", currentDate, "Scan completed.");
                    Console.Write(finishedMessage + new string(' ', consoleWidth - finishedMessage.Length));
                    return new Result { OutputType = OutputType.finished };
                }
                else if (e.Data.Contains("Address:"))
                {
                    return new Result { OutputType = OutputType.address, Content = e.Data.Trim() };
                }
                else if (e.Data.Contains("Private key:"))
                {
                    string key = e.Data.Replace(" ", "").ToLower().Trim().Replace("privatekey:", "").Trim().ToUpper();
                    return new Result { OutputType = OutputType.privateKeyFound, Content = key };
                }
                else if (e.Data.Contains("Initializing"))
                {
                    string gpuModel = e.Data.Substring(e.Data.IndexOf("Initializing")).Replace("Initializing", "").Trim();
                    return new Result { OutputType = OutputType.gpuModel, Content = gpuModel };
                }
                else
                {
                    try
                    {
                        Console.SetCursorPosition(0, 10 + gpuIndex);
                        string data = e.Data.Trim();
                        if (data.Length > 0)
                        {
                            string message = string.Format("{0} [GPU{1}] [HEX:{2}]", data.Length > 1 ? data : "...", gpuIndex, hex);
                            Console.Write(message + new string(' ', consoleWidth - message.Length));
                        }
                        else
                        {
                            string loadingMessage = string.Format("[{0}] [Info] {1} [HEX:{2}]", currentDate, "Running ...", hex);
                            Console.Write(loadingMessage + new string(' ', consoleWidth - loadingMessage.Length));
                        }
                    }
                    catch { }
                    return new Result { OutputType = OutputType.running };
                }
            }

            return new Result { OutputType = OutputType.unknown };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="_Settings"></param>
        public static async void ShareData(ResultType type, Settings settings, string? data = "")
        {
            /// Telegram Share
            if (settings.TelegramShare && settings.TelegramAccessToken.Length > 6 && settings.TelegramChatId.Length > 3)
            {
                string message = "";
                switch (type)
                {
                    case ResultType.workerStarted:
                        message = string.Format("[{0}].[{2}] started job for (Puzzle{1})", Helpers.StringParser(settings.ParsedWalletAddress), settings.TargetPuzzle, settings.ParsedWorkerName);
                        break;
                    case ResultType.reachedOfKeySpace:
                        message = string.Format("[{0}].[{1}] reached of keyspace", Helpers.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName);
                        break;
                    case ResultType.workerExited:
                        message = string.Format("[{0}].[{1}] goes offline.", Helpers.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName);
                        break;
                    case ResultType.keyFound:
                        message = string.Format("[Key Found] Congratulations. Found by worker [{0}].[{2}] {1}", Helpers.StringParser(settings.ParsedWalletAddress), data, settings.ParsedWorkerName);
                        break;
                    case ResultType.rangeScanned:
                        message = string.Format("[{0}] scanned by [{1}].[{2}]", data, Helpers.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName);
                        break;
                    default:
                        break;
                }

                try
                {
                    var botClient = new TelegramBotClient(settings.TelegramAccessToken);
                    Message _Message = await botClient.SendTextMessageAsync(
                    chatId: settings.TelegramChatId,
                    text: message);
                }
                catch { }
            }

            /// API Share
            if (settings.IsApiShare)
            {
                switch (type)
                {
                    case ResultType.keyFound:
                        _ = Requests.SendApiShare(new ApiShare { Status = type, PrivateKey = data, HEX = data }, settings);
                        break;
                    case ResultType.rangeScanned:
                        _ = Requests.SendApiShare(new ApiShare { Status = type, HEX = data }, settings);
                        break;
                    default:
                        _ = Requests.SendApiShare(new ApiShare { Status = type }, settings);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WalletAddress"></param>
        /// <returns></returns>
        public static string StringParser(string value, int length = 8)
        {
            if (value.Length > length)
            {
                string start = value.Substring(0, length);
                string end = value.Substring(value.Length - length);
                return string.Format("{0}...{1}", start, end);
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrivateKey"></param>
        /// <param name="Address"></param>
        public static void SaveFile(string privateKey, string address)
        {
            string[] lines = { privateKey, address, DateTime.Now.ToLongDateString() };
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(appPath, address + ".txt")))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }

        /// <summary>
        /// SHA256 Hashing
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static string SHA256Hash(string data)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder sb = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
