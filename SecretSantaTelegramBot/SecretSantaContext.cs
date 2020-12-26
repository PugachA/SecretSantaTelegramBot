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
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Draw> Draws { get; set; }
        public DbSet<Participant> Participants { get; set; }

        public SecretSantaContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }
    }
}
