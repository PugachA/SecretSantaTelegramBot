using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecretSantaTelegramBot.Data;
using SecretSantaTelegramBot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SecretSantaTelegramBot.Services
{
    public class DrawService : IHostedService, IDisposable
    {
        private readonly ILogger<DrawService> _logger;
        private readonly SecretSantaContext _secretSantaContext;
        private readonly ITelegramBotService _telegramBotService;
        private Timer timer;

        public DrawService(SecretSantaContext secretSantaContext, ITelegramBotService telegramBotService, ILogger<DrawService> logger)
        {
            _logger = logger;
            _secretSantaContext = secretSantaContext;
            _telegramBotService = telegramBotService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Draw service running.");

                timer = new Timer(
                    DoWork,
                    null,
                    TimeSpan.Zero,
                    TimeSpan.FromMilliseconds(1000000));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Notification service");
            }
        }

        private async void DoWork(object state)
        {
            try
            {
                var games = _secretSantaContext.Games
                    .Where(g => g.IsEnded == false && DateTime.Now >= g.EndDate);

                if (games is null)
                    return;

                foreach (var game in games)
                {
                    var userIds = await _secretSantaContext.Participants
                        .Include(p => p.User)
                        .Where(p => p.GameId == game.Id)
                        .Select(p => p.User.Id)
                        .ToListAsync();

                    if (userIds is null)
                    {
                        game.IsEnded = true;
                        await _secretSantaContext.SaveChangesAsync();
                    }

                    var randomUserIds = userIds.RandomSort();
                    var drawDictionary = new Dictionary<int, int?>();

                    //Проводим жеребьевку
                    foreach (var userId in randomUserIds)
                    {
                        int? drawUserId = userIds.Any(u => u != userId) ? userIds.First(u => u != userId) : null;
                        drawDictionary.Add(userId, drawUserId);

                        if (drawUserId is int Id)
                            userIds.Remove(Id);
                    }

                    //Делаем нотификации и записываем в БД
                    foreach (var drawResult in drawDictionary)
                    {
                        _secretSantaContext.Draws.Add(new Data.Draw { GameId = game.Id, GivingUserId = drawResult.Key, RecievingUserId = drawResult.Value });
                        await _secretSantaContext.SaveChangesAsync();

                        var givingUser = await _secretSantaContext.Users.SingleOrDefaultAsync(u => u.Id == drawResult.Key);

                        if (drawResult.Value is not null)
                        {
                            var receivingUser = await _secretSantaContext.Users.SingleOrDefaultAsync(u => u.Id == drawResult.Value);

                            await _telegramBotService.TelegramBotClient.SendTextMessageAsync(givingUser.Id, $"Ура🎉🎉🎉 Новогоднее чудо свершилось, тайная жеребьёвка проведена🎅🏻 " +
                                $"Твой тайный получатель - '{receivingUser.FirstName} {receivingUser.LastName}'🎁 Скорее обрадуй его🎄");
                        }
                        else
                        {
                            await _telegramBotService.TelegramBotClient.SendTextMessageAsync(givingUser.Id, $"Числа сложились так, что тебе не определен тайный получатель🎅🏻 " +
                                $"Но тебе обязательно кто-то подарит подарок🎁 Жди новогоднего чуда🎄");
                        }

                        await _telegramBotService.TelegramBotClient.SendTextMessageAsync(givingUser.Id, $"Держи красивую елочку");
                        await _telegramBotService.TelegramBotClient.SendStickerAsync(givingUser.Id, "CAACAgIAAxkBAAPjX-j5xWNkZxZS0weMA9eItKCQNncAArUBAAIw1J0RU4pzFgFgbjEeBA");
                    }

                    game.IsEnded = true;
                    await _secretSantaContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing draw");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Draw service is stopping.");

            timer?.Change(Timeout.Infinite, 0);
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }

}
