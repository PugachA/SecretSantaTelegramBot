using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace SecretSantaTelegramBot.Models.Commands
{
    public class StickerCommand : ICommand
    {
        public string Name => string.Empty;

        public bool Contains(Message message)
        {
            if (message.Type == MessageType.Sticker)
                return true;

            return false;
        }

        public async Task Execute(Message message, TelegramBotClient botClient, SecretSantaContext secretSantaContext)
        {
            await botClient.SendStickerAsync(message.Chat.Id, "CAACAgIAAxkBAAPjX-j5xWNkZxZS0weMA9eItKCQNncAArUBAAIw1J0RU4pzFgFgbjEeBA");
        }
    }
}
