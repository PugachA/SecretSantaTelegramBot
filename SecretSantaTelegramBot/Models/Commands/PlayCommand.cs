using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTelegramBot.Models.Commands
{
    public class PlayCommand : ICommand
    {
        public string Name => "Играть в Тайного Санту".ToLower();

        public bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.ToLower().Contains(this.Name);
        }

        public async Task Execute(Message message, TelegramBotClient botClient, SecretSantaContext secretSantaContext)
        {
            var secretSantaUser = await secretSantaContext.CreateOrUpdateUser(message);

            var secretSantaGame = secretSantaContext.Games.FirstOrDefault(g => g.IsEnded == false && g.StartDate <= DateTime.Now && g.EndDate > DateTime.Now);

            if (secretSantaGame is null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Хо хо хо к сожалению сейчас нет начатых игр🎄 " +
"Ну ничего, они обязательно скоро начнутся. " +
"Попробуй позже 🎅🏻", ParseMode.Markdown);
                return;
            }

            if(secretSantaContext.Participants.Any(p => p.GameId == secretSantaGame.Id && p.UserId == secretSantaUser.Id))
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Ты уже участвуешь в Тайном Санте🎅🏻 Жеребьёвка начнется {secretSantaGame.EndDate:HH:mm dd.MM.yyyy}. Не забудь приготовить подарок🎁", ParseMode.Markdown);
                return;
            }

            secretSantaContext.Participants.Add(new Data.Participant { GameId = secretSantaGame.Id, UserId = secretSantaUser.Id });
            await secretSantaContext.SaveChangesAsync();

            await botClient.SendTextMessageAsync(message.Chat.Id, "Хо хо хо ты зарегистрировался в церемонии Тайный Санта🎅🏻 " +
"После тайной жеребьёвки тебе выпадет человек, которому ты должен отдать подарок. " +
$"Жеребьёвка начнется в {secretSantaGame.EndDate:HH:mm dd.MM.yyyy}. Не забудь приготовить подарок🎁", ParseMode.Markdown);
        }
    }
}
