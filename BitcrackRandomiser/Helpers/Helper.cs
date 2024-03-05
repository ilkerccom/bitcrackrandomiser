using System.Text;
using System.Security.Cryptography;
using BitcrackRandomiser.Enums;

namespace BitcrackRandomiser.Helpers
{
    /// <summary>
    /// BitcrackRandomiser helper class.
    /// </summary>
    internal class Helper
    {
        /// <summary>
        /// Write message to console screen
        /// </summary>
        /// <param name="message">Message to show.</param>
        /// <param name="type">Message type. [normal, success, info, error, externalApp]</param>
        /// <param name="withClear">Clear message before write.</param>
        /// <param name="gpuIndex">GPU index</param>
        public static void WriteLine(string message, MessageType type = MessageType.normal, bool withClear = false, int gpuIndex = 0)
        {
            // Console width
            int consoleWidth = 720;
            if (Environment.UserInteractive && !Console.IsInputRedirected)
                consoleWidth = Console.WindowWidth;

            // Clear console
            if (withClear)
                Console.Clear();

            // Message type
            if (type == MessageType.error)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (type == MessageType.success)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (type == MessageType.info)
                Console.ForegroundColor = ConsoleColor.Blue;
            else
                Console.ForegroundColor = ConsoleColor.Yellow;

            // Write message
            if (type == MessageType.externalApp)
            {
                // Add spaces to message
                message += new string(' ', consoleWidth - message.Length);

                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 9 + gpuIndex);
                Console.Write(string.Format("[{0}] [Info] {1}", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), message));
            }
            else if(type == MessageType.seperator)
            {
                Console.WriteLine(new string('-', consoleWidth));
            }
            else
                Console.WriteLine(string.Format("[{0}] [Info] {1}", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), message));
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Write message to console.
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="color">Console.Color color</param>
        public static void Write(string message, ConsoleColor color = ConsoleColor.Blue)
        {
            // Write message
            Console.ForegroundColor = color;
            Console.Write(string.Format("{0}", message));
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// String parser. Example; 1eosEvve...m1TLFBtw
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <param name="length">Max. length of string</param>
        /// <returns></returns>
        public static string StringParser(string value, int length = 8, bool addDots = true)
        {
            if (value.Length > length)
            {
                string start = value[..length];
                string end = value[^length..];

                if(!addDots) return $"{start}{end}";
                return $"{start}...{end}";
            }
            return value;
        }

        /// <summary>
        /// Save private key to file
        /// </summary>
        /// <param name="privateKey">Private key (HEX)</param>
        /// <param name="address">Wallet address</param>
        public static void SaveFile(string privateKey, string address)
        {
            string[] lines = { privateKey, address, DateTime.Now.ToLongDateString() };
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            using StreamWriter outputFile = new(Path.Combine(appPath, address + ".txt"));
            foreach (string line in lines)
                outputFile.WriteLine(line);
        }

        /// <summary>
        /// Save address for vanity
        /// </summary>
        /// <param name="lines"></param>
        public static bool SaveAddressVanity(List<string> lines)
        {
            string appPath = string.Format("{0}", AppDomain.CurrentDomain.BaseDirectory);
            using StreamWriter outputFile = new(Path.Combine(appPath, "vanitysearch.txt"));
            foreach (string line in lines)
                outputFile.WriteLine(line);

            return true;
        }

        /// <summary>
        /// SHA256 Hashing
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SHA256Hash(string data)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder sb = new();
            for (int i = 0; i < bytes.Length; i++)
            sb.Append(bytes[i].ToString("x2"));
            return sb.ToString();
        }
    }
}
