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
        private Timer timer;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Notification service running.");

                timer = new Timer(
                    DoWork,
                    null,
                    TimeSpan.Zero,
                    TimeSpan.FromMilliseconds(900));
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
