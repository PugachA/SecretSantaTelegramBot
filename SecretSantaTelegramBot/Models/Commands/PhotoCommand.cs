using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTelegramBot.Models.Commands
{
    public class PhotoCommand : ICommand
    {
        public string Name => string.Empty;

        public bool Contains(Message message)
        {
            if (message.Type == MessageType.Photo)
                return true;

            return false;
        }

        public async Task Execute(Message message, TelegramBotClient botClient, SecretSantaContext secretSantaContext)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "У меня тоже есть классная фотка", ParseMode.Markdown);

            using (var stream = System.IO.File.OpenRead("secret-santa-cartoon-vector-18094246.jpg"))
            {
                await botClient.SendPhotoAsync(message.Chat.Id, photo: stream);
            }
        }
    }
}
