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
            if (request.ApiKey != Environment.GetEnvironmentVariable("WEB_API_KEY"))
                return Unauthorized("Invalid Key provided.");
            
            var geoJsonFeatures = new List<GeoJsonFeature>();
            Vessel vessel;

            //Find vessel by NAME
            if (request.ShipName != null)
            {
                vessel = await _dbContext.Vessels.FirstOrDefaultAsync(v => v.ShipName == request.ShipName);
                if (vessel != null)
                {
                    geoJsonFeatures.Add(new GeoJsonFeature
                    {
                        Properties = new Dictionary<string, object>
                        {
                            { "mmsi", vessel.Mmsi },
                            { "shipName", vessel.ShipName },
                            { "latitude", vessel.Latitude },
                            { "longitude", vessel.Longitude },
                            { "shipUrl", vessel.ShipUrl ?? "" },
                            { "imageUrl", vessel.ImageUrl ?? "" },
                            { "country", vessel.Country ?? "" },
                            { "type", vessel.Type ?? "" },
                            { "group", vessel.Group },
                        },
                        Geometry = new Geometry
                        {
                            Coordinates = new List<double> { vessel.Longitude, vessel.Latitude }
                        }
                    });
                }
                else return NotFound("No vessel found with the provided name.");
            }
            
            
            // Find vessel by GROUP
            if (request.Group != null) {
                var vessels = await _dbContext.Vessels.Where(v => v.Group == request.Group).ToListAsync();
                
                if (vessels.Count == 0)
                    return NotFound("No vessels found in the provided group.");
                
                foreach (var boat in vessels)
                {
                    geoJsonFeatures.Add(new GeoJsonFeature
                    {
                        Properties = new Dictionary<string, object>
                        {
                            { "mmsi", boat.Mmsi },
                            { "shipName", boat.ShipName },
                            { "latitude", boat.Latitude },
                            { "longitude", boat.Longitude },
                            { "shipUrl", boat.ShipUrl ?? ""},
                            { "imageUrl", boat.ImageUrl ?? ""},
                            { "country", boat.Country ?? ""},
                            { "type", boat.Type ?? ""},
                            { "group", boat.Group },
                        },
                        Geometry = new Geometry
                        {
                            Coordinates = new List<double> { boat.Longitude, boat.Latitude }
                        }
                    });
                }
            }

            // Find vessel by MMSI
            if (request.MmsiNumbers != null) {
                foreach (var mmsi in request.MmsiNumbers)
                {
                    vessel = await _dbContext.Vessels.FirstOrDefaultAsync(v => v.Mmsi == mmsi);
                    if (vessel != null)
                    {
                        geoJsonFeatures.Add(new GeoJsonFeature
                        {
                            Properties = new Dictionary<string, object>
                            {
                                { "mmsi", vessel.Mmsi },
                                { "shipName", vessel.ShipName },
                                { "latitude", vessel.Latitude },
                                { "longitude", vessel.Longitude },
                                { "shipUrl", vessel.ShipUrl ?? ""},
                                { "imageUrl", vessel.ImageUrl ?? ""},
                                { "country", vessel.Country ?? ""},
                                { "type", vessel.Type ?? ""},
                                { "group", vessel.Group },
                            },
                            Geometry = new Geometry
                            {
                                Coordinates = new List<double> { vessel.Longitude, vessel.Latitude }
                            }
                        });
                    }
                }
            }
            
            if(geoJsonFeatures.Count == 0)
                return NotFound("No vessels found with the provided parameters.");

            var geoJson = new
            {
                type = "FeatureCollection",
                features = geoJsonFeatures
            };

            return Ok(geoJson);
        }
    }
}