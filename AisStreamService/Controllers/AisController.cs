using Microsoft.AspNetCore.Mvc;
using AisStreamService.Data;
using AisStreamService.Models;
using Microsoft.EntityFrameworkCore;

namespace AisStreamService.Controllers
{
    [ApiController]
    [Route("/v1/ais")]
    public class AisController : Controller
    {
        private readonly AisDbContext _dbContext;

        public AisController(AisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("query")]
        public async Task<IActionResult> QueryAis([FromBody] AisRequestModel request)
        {
            var geoJsonFeatures = new List<GeoJsonFeature>();

            foreach (var mmsi in request.MmsiNumbers)
            {
                var vessel = await _dbContext.Vessels.FirstOrDefaultAsync(v => v.Mmsi == mmsi);
                if (vessel != null)
                {
                    geoJsonFeatures.Add(new GeoJsonFeature
                    {
                        Properties = new Dictionary<string, object>
                        {
                            { "mmsi", vessel.Mmsi },
                            { "shipName", vessel.ShipName }
                        },
                        Geometry = new Geometry
                        {
                            Coordinates = new List<double> { vessel.Longitude, vessel.Latitude }
                        }
                    });
                }
            }

            var geoJson = new
            {
                type = "FeatureCollection",
                features = geoJsonFeatures
            };

            return Ok(geoJson);
        }
    }
}