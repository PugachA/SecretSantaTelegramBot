using Microsoft.AspNetCore.Mvc;
using SecretSantaTelegramBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SecretSantaTelegramBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly IUpdateService _updateService;

        public UpdateController(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            await _updateService.GenerateAnswerAsync(update);
            return Ok();
        }

        // GET api/update
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("Hello");
        }
    }
}
