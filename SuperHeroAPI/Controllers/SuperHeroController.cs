using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperHeroAPI.Data;
using SuperHeroAPI.Entities;

namespace SuperHeroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<SuperHeroController> _logger;

        // Best practice is to autowire Service, Repository, and etc.
        // And autowire the DataContext inside the Repository
        public SuperHeroController(DataContext dataContext, ILogger<SuperHeroController> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        // Best practice is to use service or repo instead of putting all the logic in the controller
        [HttpGet, Authorize]
        public async Task<ActionResult<List<SuperHero>>> GetAllHeroes()
        {
            var heroes = await _dataContext.SuperHeroes.ToListAsync();
            return Ok(heroes);
        }

        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<SuperHero>> GetHero(int id)
        {
            var hero = await _dataContext.SuperHeroes.FindAsync(id);
            if (hero == null)
            {
                string msg = $"Hero ID '{id}' not found.";
                _logger.LogError(msg);
                return NotFound(msg);
            }

            return Ok(hero);
        }

        // Best practice is to pass a DTO instead of the entity
        [HttpPost, Authorize]
        public async Task<ActionResult<string>> CreateHero([FromBody] SuperHero hero)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(ModelState.ValidationState.ToString());
                return BadRequest(ModelState);
            }
            
            _dataContext.SuperHeroes.Add(hero);
            await _dataContext.SaveChangesAsync();
            return Ok("Create hero success!");
        }

        [HttpPut, Authorize]
        public async Task<ActionResult<SuperHero>> UpdateHero([FromBody] SuperHero hero)
        {
            // May not be necessary
            /*if (hero == null)
            {
                string msg = "The request body is invalid.";
                _logger.LogError(msg);
                return BadRequest(msg);
            }*/

            var heroRecord = await _dataContext.SuperHeroes.FindAsync(hero.Id);
            if (heroRecord == null)
            {
                string msg = $"Hero ID '{hero.Id}' not found.";
                _logger.LogError(msg);
                return NotFound(msg);
            }

            // Create a mapper for this
            heroRecord.Name = hero.Name;
            heroRecord.FirstName = hero.FirstName;
            heroRecord.LastName = hero.LastName;
            heroRecord.Place = hero.Place;
            await _dataContext.SaveChangesAsync();

            return Ok(heroRecord);
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult<string>> DeleteHero(int id)
        {
            var hero = await _dataContext.SuperHeroes.FindAsync(id);
            if (hero == null)
            {
                string msg = $"Hero ID '{id}' not found.";
                _logger.LogError(msg);
                return NotFound(msg);
            }

            _dataContext.SuperHeroes.Remove(hero);
            await _dataContext.SaveChangesAsync();

            return Ok("Delete hero success.");
        }
    }
}
