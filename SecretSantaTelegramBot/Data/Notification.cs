using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SecretSantaTelegramBot.Data
{
    public class Notification
    {
        public int Id { get; set; }

        [JsonIgnore]
        public SecretSantaGame Game { get; set; }

        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }

        [Required]
        public DateTime NotificationDate { get; set; }

        [Required]
        public bool IsNotified { get; set; }
    }
}
