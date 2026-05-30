using CarColorFrequencyApi.DataLayer;
using CarColorFrequencyApi.Models;

using Microsoft.AspNetCore.Mvc;

namespace CarColorFrequencyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarColorFrequencyController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<ColorData> Get()
        {
            using (var db = new DBAccess())
            {
                var result = db.GetColors();
                return result.ToArray();
            }
        }

        [HttpPost]
        public IActionResult Get([FromBody] ColorData data)
        {
            if (data == null)
            {
                return BadRequest("Invalid JSON payload.");
            }

            using (var db = new DBAccess())
            {
                db.UpdateColorCounts(data);
                var message = $"Color data was updated successfully.";
                return Ok(new { Message = message });
            }
        }
    }
}
