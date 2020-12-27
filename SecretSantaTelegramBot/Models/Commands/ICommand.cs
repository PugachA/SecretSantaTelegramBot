using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaTelegramBot.Models.Commands
{
    public interface ICommand
    {
        public string Name { get; }
        public Task Execute(Message message, TelegramBotClient botClient, SecretSantaContext secretSantaContext);
        public bool Contains(Message message);
    }
}
