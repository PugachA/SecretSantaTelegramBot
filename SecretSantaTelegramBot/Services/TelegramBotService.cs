using Cryptography.Wrappers;
using Cryptography.Wrappers.Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MihaZupan;
using Ngrok.AspNetCore;
using SecretSantaTelegramBot.Helpers;
using SecretSantaTelegramBot.Models.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace SecretSantaTelegramBot.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly BotConfiguration _botConfiguration;
        public TelegramBotClient TelegramBotClient { get; }
        public IReadOnlyList<ICommand> Commands { get; }

        public TelegramBotService(INgrokHostedService ngrokHostedService, IEncryptor encryptor, IOptions<BotConfiguration> botConfiguration, IWebHostEnvironment enviroment)
        {
            _botConfiguration = botConfiguration.Value;
            var token = encryptor.DecryptStringFromBase64String(_botConfiguration.BotToken, Encoding.UTF8);

            TelegramBotClient = new TelegramBotClient(token);

            if (!string.IsNullOrEmpty(_botConfiguration.Socks5Host))
                TelegramBotClient = new TelegramBotClient(token, new HttpToSocks5Proxy(_botConfiguration.Socks5Host, _botConfiguration.Socks5Port));

            var httpsTunnel = ngrokHostedService.GetTunnelsAsync().Result.Single(t => t.Proto == "https");

            if (enviroment.IsDevelopment())
                TelegramBotClient.SetWebhookAsync($"{httpsTunnel.PublicURL}/api/update").Wait();

            if (enviroment.IsProduction())
                TelegramBotClient.SetWebhookAsync($"{httpsTunnel.PublicURL}/SecretSantaTelegramBot/api/update").Wait();

            Commands = GetCommands().AsReadOnly();
        }

        private List<ICommand> GetCommands() => new List<ICommand>
        {
            new StartCommand(),
            new PlayCommand(),
            new StickerCommand(),
            new PhotoCommand()
        };
    }
}
