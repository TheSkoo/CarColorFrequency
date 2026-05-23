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
    }
}
