using Telegram.Bot;

namespace SecretSantaTelegramBot.Services
{
    public interface ITelegramBotService
    {
        TelegramBotClient TelegramBotClient { get; }
    }
}