using Microsoft.EntityFrameworkCore;
using SecretSantaTelegramBot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

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

        public async Task<SecretSantaUser> CreateOrUpdateUser(Message message)
        {
            var secretSantaUser = Users.SingleOrDefault(u => u.Id == message.From.Id);

            if (secretSantaUser is not null)
            {
                secretSantaUser.ChartId = message.Chat.Id;
                secretSantaUser.FirstName = message.From.FirstName;
                secretSantaUser.LastName = message.From.LastName;
                secretSantaUser.Username = message.From.Username;
                secretSantaUser.ModifiedDate = DateTime.Now;
            }
            else
            {
                secretSantaUser = new SecretSantaUser
                {
                    Id = message.From.Id,
                    ChartId = message.Chat.Id,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    Username = message.From.Username,
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
