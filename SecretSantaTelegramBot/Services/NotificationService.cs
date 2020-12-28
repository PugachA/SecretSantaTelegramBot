using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SecretSantaTelegramBot.Services
{
    public class NotificationService : IHostedService, IDisposable
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly SecretSantaContext _secretSantaContext;
        private readonly ITelegramBotService _telegramBotService;
        private Timer timer;

        public NotificationService(SecretSantaContext secretSantaContext, ITelegramBotService telegramBotService, ILogger<NotificationService> logger)
        {
            _logger = logger;
            _secretSantaContext = secretSantaContext;
            _telegramBotService = telegramBotService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Notification service running.");

                timer = new Timer(
                    DoWork,
                    null,
                    TimeSpan.Zero,
                    TimeSpan.FromMilliseconds(10000));
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
                var notifications = _secretSantaContext.Notifications
                    .Include(n => n.Game)
                    .Include(n => n.Game.Participants)
                    .Where(n => n.NotificationDate <= DateTime.Now && n.IsNotified == false && DateTime.Now < n.Game.EndDate);

                foreach (var notification in notifications)
                {
                    var users = await _secretSantaContext.Participants
                        .Include(p => p.User)
                        .Where(p => p.GameId == notification.GameId)
                        .Select(p => p.User)
                        .ToListAsync();

                    foreach (var user in users)
                    {
                        var remainingTime = notification.Game.EndDate - DateTime.Now;
                        await _telegramBotService.TelegramBotClient.SendTextMessageAsync(user.Id, $"Хо хо хо, пишу напомнить," +
                            $" что до тайной жеребьевки осталось {remainingTime.Days}д. {remainingTime.Hours}ч. {remainingTime.Minutes}мин 🎅🏻 " +
                            $"Надеюсь ты успел подготовить подарок🎁");
                    }

                    notification.IsNotified = true;
                }

                await _secretSantaContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении команд на обработку");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notification service is stopping.");

            timer?.Change(Timeout.Infinite, 0);
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
