using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SecretSantaTelegramBot.Data
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsEnded { get; set; }

        [JsonIgnore]
        public List<Participant> Participants { get; set; }
        [JsonIgnore]
        public List<Draw> Draws { get; set; }
    }
}
