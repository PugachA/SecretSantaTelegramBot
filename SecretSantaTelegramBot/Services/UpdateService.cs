using System.IO;
using System.Linq;
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

        public UpdateService(ITelegramBotService botService, ILogger<UpdateService> logger, SecretSantaContext secretSantaContext)
        {
            _botService = botService;
            _logger = logger;
            _secretSantaContext = secretSantaContext;
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

            //switch (message.Type)
            //{
            //    case MessageType.Photo:
            //        // Download Photo
            //        var fileId = message.Photo.LastOrDefault()?.FileId;
            //        var file = await _botService.TelegramBotClient.GetFileAsync(fileId);

            //        var filename = file.FileId + "." + file.FilePath.Split('.').Last();
            //        using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
            //        {
            //            await _botService.TelegramBotClient.DownloadFileAsync(file.FilePath, saveImageStream);
            //        }

            //        await _botService.TelegramBotClient.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            //        break;
            //}
        }
    }
}
