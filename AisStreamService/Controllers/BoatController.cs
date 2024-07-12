using AisStreamService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AisStreamService.Controllers;

[ApiController]
[Route("[controller]")]
public class BoatController : Controller
{
    AisDbContext _dbContext;
    
    public BoatController(AisDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllBoats()
    {
        var boats = await _dbContext.Vessels.ToListAsync(); 
        return Ok(boats);
    }
}