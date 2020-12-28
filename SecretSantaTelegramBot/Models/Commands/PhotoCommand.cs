using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTelegramBot.Models.Commands
{
    public class PhotoCommand : ICommand
    {
        private readonly string _imagePath;
        public string Name => string.Empty;

        public PhotoCommand(string imagePath)
        {
            _imagePath = imagePath;
        }

        public bool Contains(Message message)
        {
            if (message.Type == MessageType.Photo)
                return true;

            return false;
        }

        public async Task Execute(Message message, TelegramBotClient botClient, SecretSantaContext secretSantaContext)
        {
            var fileId = message.Photo.LastOrDefault()?.FileId;
            var file = await botClient.GetFileAsync(fileId);

            var filename = Path.Combine(_imagePath, $"{message.From.Id}\\{file.FileId}.{file.FilePath.Split('.').Last()}");

            if(!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
            {
                await botClient.DownloadFileAsync(file.FilePath, saveImageStream);
            }

            await botClient.SendTextMessageAsync(message.Chat.Id, "У меня тоже есть классная фотка", ParseMode.Markdown);

            using (var stream = System.IO.File.OpenRead("secret-santa-cartoon-vector-18094246.jpg"))
            {
                await botClient.SendPhotoAsync(message.Chat.Id, photo: stream);
            }
        }
    }
}
