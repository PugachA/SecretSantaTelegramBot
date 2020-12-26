using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SecretSantaTelegramBot.Services
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}
