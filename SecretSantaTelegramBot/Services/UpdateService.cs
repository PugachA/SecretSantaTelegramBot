using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTelegramBot.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly ITelegramBotService _botService;
        private readonly SecretSantaContext _secretSantaContext;
        private readonly ILogger<UpdateService> _logger;
        private readonly CancellationToken _cancellationToken;
        private readonly NotificationService _notificationService;
        private readonly DrawService _drawService;

        public UpdateService(ITelegramBotService botService, NotificationService notificationService, DrawService drawService, ILogger<UpdateService> logger, SecretSantaContext secretSantaContext)
        {
            _botService = botService;
            _logger = logger;
            _secretSantaContext = secretSantaContext;

            _notificationService = notificationService;
            _drawService = drawService;

            _cancellationToken = new CancellationToken();
            _notificationService.StartAsync(_cancellationToken);
            _drawService.StartAsync(_cancellationToken);
        }

        public async Task GenerateAnswerAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
                return;

            var message = update.Message;

            _logger.LogInformation($"Received Type '{message.Type}' Message '{message.Text}' from {message.From.Id}:{message.From.FirstName} {message.From.LastName}");

            var command = _botService.Commands.SingleOrDefault(c => c.Contains(message));

            if (command is null)
            {
                await _botService.TelegramBotClient.SendTextMessageAsync(message.Chat.Id, "Хо хо хо. Затрудняюсь ответить 🎅", ParseMode.Markdown);
                return;
            }

            await command.Execute(message, _botService.TelegramBotClient, _secretSantaContext);
        }

        public void Dispose()
        {
            _notificationService?.StopAsync(_cancellationToken);
            _drawService?.StopAsync(_cancellationToken);

            _notificationService?.Dispose();
            _drawService?.Dispose();
        }
    }
}
