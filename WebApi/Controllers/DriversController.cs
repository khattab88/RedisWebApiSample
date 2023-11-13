using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ApplicationDbContext _db;

        public DriversController(ICacheService cacheService, ApplicationDbContext db)
        {
            _cacheService = cacheService;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Driver driver)
        {
            if (!ModelState.IsValid) { return BadRequest(); }

            var addedObj = _db.Drivers.Add(driver);

            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<Driver>($"driver-{driver.Id}", addedObj.Entity, expirationTime);

            await _db.SaveChangesAsync();

            return Created("", driver);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cachedDrivers = _cacheService.GetData<IEnumerable<Driver>>(CacheKeys.Drivers);

            if (cachedDrivers != null && cachedDrivers.Count() > 0)
            {
                return Ok(cachedDrivers);
            }

            var drivers = await _db.Drivers.ToListAsync();

            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<IEnumerable<Driver>>(CacheKeys.Drivers, drivers, expirationTime);

            return Ok(drivers);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var driver = await _db.Drivers.FirstOrDefaultAsync(x => x.Id == id);

            if (driver != null)
            {
                _db.Remove(driver);
                _cacheService.RemoveData($"driver-{id}");

                await _db.SaveChangesAsync();

                return NoContent();
            }

            return NotFound();
        }
    }
}
