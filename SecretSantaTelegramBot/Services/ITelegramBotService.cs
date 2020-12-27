using SecretSantaTelegramBot.Models.Commands;
using System.Collections.Generic;
using Telegram.Bot;

namespace SecretSantaTelegramBot.Services
{
    public interface ITelegramBotService
    {
        TelegramBotClient TelegramBotClient { get; }
        IReadOnlyList<ICommand> Commands { get; }
    }
}