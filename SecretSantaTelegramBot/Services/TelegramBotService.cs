using Microsoft.Extensions.Options;
using MihaZupan;
using Ngrok.AspNetCore;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace SecretSantaTelegramBot.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly BotConfiguration _config;
        public TelegramBotClient TelegramBotClient { get; }

        public TelegramBotService(INgrokHostedService ngrokHostedService, IOptions<BotConfiguration> config)
        {
            _config = config.Value;

            TelegramBotClient = new TelegramBotClient(_config.BotToken);

            if (!string.IsNullOrEmpty(_config.Socks5Host))
                TelegramBotClient = new TelegramBotClient(_config.BotToken, new HttpToSocks5Proxy(_config.Socks5Host, _config.Socks5Port));

            var httpsTunnel = ngrokHostedService.GetTunnelsAsync().Result.Single(t => t.Proto == "https");
            TelegramBotClient.SetWebhookAsync($"{httpsTunnel.PublicURL}/api/update").Wait();
        }
    }
}
