using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GameContentManager.API.Models;
using GameContentManager.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameContentManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GameCharacterController : ControllerBase
    {
        private readonly IGameCharacterService _characterService;

        public GameCharacterController(IGameCharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameCharacter>>> GetAll()
        {
            var characters = await _characterService.GetAllCharactersAsync();
            return Ok(characters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameCharacter>> GetById(string id)
        {
            var character = await _characterService.GetCharacterByIdAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            return Ok(character);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<GameCharacter>>> Search([FromQuery] string term)
        {
            var characters = await _characterService.SearchCharacters(term);
            return Ok(characters);
        }

        [HttpPost]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<ActionResult<GameCharacter>> Create([FromBody] GameCharacter character)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var createdCharacter = await _characterService.CreateCharacterAsync(character, userId);
            return CreatedAtAction(nameof(GetById), new { id = createdCharacter.Id }, createdCharacter);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] GameCharacter character)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            await _characterService.UpdateCharacterAsync(id, character,userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _characterService.DeleteCharacterAsync(id, userId);
            return NoContent();
        }
    }
} 