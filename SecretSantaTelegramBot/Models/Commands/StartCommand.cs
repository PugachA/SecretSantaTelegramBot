using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.EntityFrameworkCore;

namespace SecretSantaTelegramBot.Models.Commands
{
    public class StartCommand : ICommand
    {
        public string Name => @"/start";

        public bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.ToLower().Contains(this.Name);
        }

        public async Task Execute(Message message, TelegramBotClient botClient, SecretSantaContext secretSantaContext)
        {
            await secretSantaContext.CreateOrUpdateUser(message.From);

            var chatId = message.Chat.Id;
            await botClient.SendTextMessageAsync(chatId, "Привет! 🥳\r\n" +
"Pugach Secret Santa - это бот для организации рождественской церемонии тайного обмена подарками🤫\r\n" +
"Каждый год перед Рождеством люди во всем мире обмениваются подарками🎁 " +
"Чтобы добавить интриги, ты можешь принять участие в церемонии Тайный Санта🎅🏻 " +
"После секретной жеребьёвки ты узнаешь, кому тебе нужно дарить подарок✨\r\n" +
"Просто напиши \"`Играть в Тайного Санту`\"🎄", parseMode: ParseMode.Markdown);
        }
    }
}
