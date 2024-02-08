using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitcrackRandomiser.Helpers;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace BitcrackRandomiser.Helpers
{
    internal class Share
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="_Settings"></param>
        public static async void Send(ResultType type, Settings settings, string? data = "")
        {
            /// Telegram Share
            if (settings.TelegramShare && settings.TelegramAccessToken.Length > 6 && settings.TelegramChatId.Length > 3)
            {
                string message = "";
                switch (type)
                {
                    case ResultType.workerStarted:
                        message = string.Format("[{0}].[{2}] started job for (Puzzle{1})", Helper.StringParser(settings.ParsedWalletAddress), settings.TargetPuzzle, settings.ParsedWorkerName);
                        break;
                    case ResultType.reachedOfKeySpace:
                        message = string.Format("[{0}].[{1}] reached of keyspace", Helper.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName);
                        break;
                    case ResultType.workerExited:
                        message = string.Format("[{0}].[{1}] goes offline.", Helper.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName);
                        break;
                    case ResultType.keyFound:
                        message = string.Format("[Key Found] Congratulations. Found by worker [{0}].[{2}] {1}", Helper.StringParser(settings.ParsedWalletAddress), data, settings.ParsedWorkerName);
                        break;
                    case ResultType.rangeScanned:
                        message = string.Format("[{0}] scanned by [{1}].[{2}]", data, Helper.StringParser(settings.ParsedWalletAddress), settings.ParsedWorkerName);
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
    }
}
