using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Enes.Data;
using Enes.Models;

namespace Enes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly AppDbContext _db;

    public CampaignController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /api/campaign?city=Istanbul
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? city)
    {
        IQueryable<Campaign> q = _db.Campaigns;

        if (!string.IsNullOrWhiteSpace(city))
        {
            var cityNorm = city.Trim().ToLower();
            q = q.Where(c => c.City.ToLower() == cityNorm);
        }

        var result = await q.AsNoTracking().ToListAsync();
        return Ok(result);
    }

    // GET: /api/campaign/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var campaign = await _db.Campaigns.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return campaign is null ? NotFound("Campaign not found") : Ok(campaign);
    }

    // POST: /api/campaign
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Campaign campaign)
    {
        // Id'yi client göndermesin diye sıfırlayalım
        campaign.Id = 0;

        _db.Campaigns.Add(campaign);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = campaign.Id }, campaign);
    }

    // PUT: /api/campaign/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Campaign updated)
    {
        var campaign = await _db.Campaigns.FirstOrDefaultAsync(c => c.Id == id);
        if (campaign is null) return NotFound("Campaign not found");

        campaign.Title = updated.Title;
        campaign.City = updated.City;
        campaign.Description = updated.Description;

        await _db.SaveChangesAsync();
        return Ok(campaign);
    }

    // DELETE: /api/campaign/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var campaign = await _db.Campaigns.FirstOrDefaultAsync(c => c.Id == id);
        if (campaign is null) return NotFound("Campaign not found");

        _db.Campaigns.Remove(campaign);
        await _db.SaveChangesAsync();

        return Ok("Deleted successfully");
    }
}