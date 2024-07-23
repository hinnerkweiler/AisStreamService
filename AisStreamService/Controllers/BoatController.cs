using System.Net;
using AisStreamService.Data;
using AisStreamService.Models;
using AisStreamService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AisStreamService.Controllers;

[ApiController]
[Route("[controller]")]
public class BoatController : Controller
{
    AisDbContext _dbContext;
    private readonly AisBackgroundService _aisBackgroundService;
    
    public BoatController(AisDbContext dbContext, AisBackgroundService aisBackgroundService)
    {
        _dbContext = dbContext;
        _aisBackgroundService = aisBackgroundService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllBoats()
    {
        var boats = await _dbContext.Vessels.ToListAsync(); 
        return Ok(boats);
    }

    [HttpPost("addmany")]
    public async Task<IActionResult> AddManyBoats([FromBody] List<Vessel> vessels)
    {
        var apiKey = Request.Headers["x-api-key"];
        if (apiKey != Environment.GetEnvironmentVariable("WEB_API_KEY"))
        {
            return Unauthorized();
        }
        
        if (vessels == null || vessels.Count == 0)
        {
            return BadRequest("No Vessel data provided");
        }
        
        if (vessels.Any(v => v.Mmsi.ToString().Length != 9))
        {
            return BadRequest("Bad MMSI");
        }

        foreach (var vessel in vessels)
        {
            await AddBoatToDB(vessel);
        }
        _= _aisBackgroundService.RestartService();
        
        return Ok();
    }

    
    [HttpPost("add")]
    public async Task<IActionResult> AddBoat([FromBody] Vessel vessel)
    {
        //Get the API key from the request header and compare it to the one in the environment variables
        var apiKey = Request.Headers["x-api-key"];
        if (apiKey != Environment.GetEnvironmentVariable("WEB_API_KEY"))
        {
            return Unauthorized();
        }
        
        if (vessel == null)
        {
            return BadRequest("No Vessel data provided");
        }
        
        if (vessel.Mmsi.ToString().Length != 9)
        {
            return BadRequest("Bad MMSI");
        }
        
        if (vessel.Mmsi == 0)
        {
            return BadRequest("No MMSI provided");
        }

        await AddBoatToDB(vessel);
        _= _aisBackgroundService.RestartService();
        
        return Ok();
    }
    
    private async Task AddBoatToDB(Vessel vessel)
    {
        vessel.ShipName = WebUtility.HtmlEncode(vessel.ShipName);
        vessel.ShipUrl = vessel.ShipUrl != null ? WebUtility.HtmlEncode(vessel.ShipUrl) : null;
        vessel.ImageUrl = vessel.ImageUrl != null ? WebUtility.HtmlEncode(vessel.ImageUrl) : null;
        vessel.Country = vessel.Country != null ? WebUtility.HtmlEncode(vessel.Country) : null;
        vessel.ShipType = vessel.ShipType != null ? WebUtility.HtmlEncode(vessel.ShipType) : null;
        
        await _dbContext.Vessels.AddAsync(vessel);
        await _dbContext.SaveChangesAsync();
    }
    
}