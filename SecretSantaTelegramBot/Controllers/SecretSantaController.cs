using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretSantaTelegramBot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretSantaTelegramBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecretSantaController : Controller
    {
        private readonly SecretSantaContext _secretSantaContext;
        private readonly ILogger<SecretSantaController> _logger;

        public SecretSantaController(SecretSantaContext secretSantaContext, ILogger<SecretSantaController> logger)
        {
            _secretSantaContext = secretSantaContext;
            _logger = logger;
        }

        // POST api/update
        [HttpPost("game")]
        public async Task<IActionResult> PostGame([FromBody] SecretSantaGame game)
        {
            try
            {
                if (game is null)
                {
                    _logger.LogInformation($"Validation failed. Parametr {nameof(game)} can not ne null");
                    return BadRequest($"Validation failed. Parametr {nameof(game)} can not ne null");
                }

                await _secretSantaContext.AddAsync(game);
                await _secretSantaContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error while processing request");
                return StatusCode(500, "Internal server error while processing request");
            }
        }
    }
}
