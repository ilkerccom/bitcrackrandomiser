using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var Value = File.ReadLines(Path).ElementAt(Line);
            return Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Filename"></param>
        public static bool CheckWinner(string Filename, string HEX)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + Filename + ".txt";
            if (File.Exists(Path))
            {
                return true;
            }

            return false;
        }
    }
}
