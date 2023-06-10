using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
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
        public static void WriteLine(string message, bool withClear = false)
        {
            if (withClear)
            {
                Console.Clear();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format("[{0}] [Info] " + message, DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")));
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetSettings(int Line = 0)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + "settings.txt";
            var Value = System.IO.File.ReadLines(Path).ElementAt(Line);
            return Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Filename"></param>
        public static bool CheckWinner(string Filename, string HEX)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + Filename + ".txt";
            if (System.IO.File.Exists(Path))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool CheckJobIsFinished(object o, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("Reached"))
                {
                    return true;
                }
                else
                {
                    try
                    {
                        Console.SetCursorPosition(0, 5);
                        Console.Write(e.Data + new string(' ', Console.WindowWidth - e.Data.Length));
                    }
                    catch { }
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        public static async void ShareTelegram(string Message)
        {
            try
            {
                var botClient = new TelegramBotClient(GetSettings(6).Split('=')[1]);

                Message _Message = await botClient.SendTextMessageAsync(
                chatId: GetSettings(7).Split('=')[1],
                text: Message);
            }
            catch { }
        }
    }
}
