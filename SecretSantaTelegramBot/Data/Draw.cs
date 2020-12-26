﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SecretSantaTelegramBot.Data
{
    public class Draw
    {
        public int Id { get; set; }

        [JsonIgnore]
        public Game Game { get; set; }

        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }

        [JsonIgnore]
        public User GivingUser { get; set; }

        [ForeignKey(nameof(GivingUser))]
        public int GivingUserId { get; set; }

        public int RecievingUserId { get; set; }
    }
}
