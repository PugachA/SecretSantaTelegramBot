using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SecretSantaTelegramBot.Data
{
    public class SecretSantaGame
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
        public DateTime StartDate { get; set; }

        [Required]
        [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsEnded { get; set; }

        [JsonIgnore]
        public List<Participant> Participants { get; set; }
        [JsonIgnore]
        public List<Draw> Draws { get; set; }
        [JsonIgnore]
        public List<Notification> Notifications { get; set; }
    }
}
