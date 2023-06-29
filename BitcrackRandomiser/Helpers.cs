using System.Diagnostics;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace BitcrackRandomiser
{
    internal class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string message, MessageType type = MessageType.normal, bool withClear = false)
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
            Console.WriteLine(string.Format("[{0}] [Info] {1}", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), message));
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
            Console.Write(string.Format("{0}",message));
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Result CheckJobStatus(object o, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("Reached"))
                {
                    Console.SetCursorPosition(0, 9);
                    string FinishedMessage = string.Format("[{0}] [Info] {1}", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), "Scan completed.");
                    Console.Write(FinishedMessage + new string(' ', Console.WindowWidth - FinishedMessage.Length));
                    return new Result { OutputType = OutputType.finished };
                }
                else if (e.Data.Contains("Address:"))
                {
                    return new Result { OutputType = OutputType.address, Content = e.Data.Trim() };
                }
                else if (e.Data.Contains("Private key:"))
                {
                    string Key = e.Data.Replace(" ", "").ToLower().Trim().Replace("privatekey:","").Trim().ToUpper();
                    return new Result { OutputType = OutputType.privateKeyFound, Content = Key };
                }
                else if (e.Data.Contains("Initializing"))
                {
                    string GpuModel = e.Data.Substring(e.Data.IndexOf("Initializing")).Replace("Initializing", "").Trim();
                    return new Result { OutputType = OutputType.gpuModel, Content = GpuModel };
                }
                else
                {
                    try
                    {
                        Console.SetCursorPosition(0, 9);
                        if (e.Data.Length > 0)
                        {
                            Console.Write(e.Data + new string(' ', Console.WindowWidth - e.Data.Length));
                        }
                        else
                        {
                            string LoadingMessage = string.Format("[{0}] [Info] {1}", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), "Running ...");
                            Console.Write(LoadingMessage + new string(' ', Console.WindowWidth - LoadingMessage.Length));
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
        public static async void ShareTelegram(string Message, Settings _Settings)
        {
            if (_Settings.TelegramShare && _Settings.TelegramAccessToken.Length > 6 && _Settings.TelegramChatId.Length > 3)
            {
                try
                {
                    var botClient = new TelegramBotClient(_Settings.TelegramAccessToken);
                    Message _Message = await botClient.SendTextMessageAsync(
                    chatId: _Settings.TelegramChatId,
                    text: Message);
                }
                catch { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WalletAddress"></param>
        /// <returns></returns>
        public static string StringParser(string Value, int Length = 8)
        {
            if (Value.Length > Length)
            {
                string Start = Value.Substring(0, Length);
                string End = Value.Substring(Value.Length - Length);
                return string.Format("{0}...{1}", Start, End);
            }
            return Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrivateKey"></param>
        /// <param name="Address"></param>
        public static void SaveFile(string PrivateKey, string Address)
        {
            // Save file
            string[] Lines = { PrivateKey, Address, DateTime.Now.ToLongDateString() };
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(AppPath, Address + ".txt")))
            {
                foreach (string Line in Lines)
                    outputFile.WriteLine(Line);
            }
        }
    }
}
