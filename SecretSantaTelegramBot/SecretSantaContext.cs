using Microsoft.EntityFrameworkCore;
using SecretSantaTelegramBot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretSantaTelegramBot
{
    public class SecretSantaContext : DbContext
    {
        public DbSet<SecretSantaUser> Users { get; set; }
        public DbSet<SecretSantaGame> Games { get; set; }
        public DbSet<Draw> Draws { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public SecretSantaContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }

        public async Task<SecretSantaUser> CreateOrUpdateUser(Telegram.Bot.Types.User telegramUser)
        {
            var secretSantaUser = Users.SingleOrDefault(u => u.Id == telegramUser.Id);

            if (telegramUser is not null)
            {
                secretSantaUser.FirstName = telegramUser.FirstName;
                secretSantaUser.LastName = telegramUser.LastName;
                secretSantaUser.Username = telegramUser.Username;
                secretSantaUser.ModifiedDate = DateTime.Now;
            }
            else
            {
                secretSantaUser = new SecretSantaUser
                {
                    Id = telegramUser.Id,
                    FirstName = telegramUser.FirstName,
                    LastName = telegramUser.LastName,
                    Username = telegramUser.Username,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                await Users.AddAsync(secretSantaUser);
            }

            await Database.OpenConnectionAsync();
            try
            {
                await Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Users ON");
                await SaveChangesAsync();
                await Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Users OFF");
            }
            finally
            {
                await Database.CloseConnectionAsync();
            }

            return secretSantaUser;
        }
    }
}
