using CarColorFrequencyApi.DataLayer;
using CarColorFrequencyApi.Models;

using Microsoft.AspNetCore.Mvc;

namespace CarColorFrequencyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarColorFrequencyController : ControllerBase
    {
        [HttpGet("FetchColorData")]
        public IEnumerable<ColorData> FetchColorData()
        {
            using (var db = new DBAccess())
            {
                var result = db.GetColors();
                return result.ToArray();
            }
        }

        [HttpPost("Commit")]
        public IActionResult Commit([FromBody] List<ColorData> data)
        {
            if (data == null)
            {
                return BadRequest("Invalid JSON payload.");
            }

            using (var db = new DBAccess())
            {
                foreach (var colorData in data)
                {
                    if (colorData.ColorDictId <= 0)
                    {
                        return BadRequest("Each ColorData item must have a valid ColorDictId.");
                    }
                    db.UpdateColorCounts(colorData);
                }
                var message = $"Color data was updated successfully.";
                return Ok(new { Message = message });
            }
        }
    }
}
