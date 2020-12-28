using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SecretSantaTelegramBot.Services
{
    public interface IUpdateService : IDisposable
    {
        Task GenerateAnswerAsync(Update update);
    }
}
