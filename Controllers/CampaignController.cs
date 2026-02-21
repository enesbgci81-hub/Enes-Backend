using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Enes.Data;
using Enes.Models;

namespace Enes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly AppDbContext _context;

    public CampaignController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ Pagination + sorting + optional city filter
    // GET /api/Campaign?page=1&pageSize=10&sort=id_desc&city=Istanbul
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sort = "id_desc",
        [FromQuery] string? city = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        IQueryable<Campaign> query = _context.Campaigns.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(c => c.City == city);

        query = sort?.ToLower() switch
        {
            "id_asc" => query.OrderBy(c => c.Id),
            "id_desc" => query.OrderByDescending(c => c.Id),
            "title_asc" => query.OrderBy(c => c.Title),
            "title_desc" => query.OrderByDescending(c => c.Title),
            "city_asc" => query.OrderBy(c => c.City),
            "city_desc" => query.OrderByDescending(c => c.City),
            _ => query.OrderByDescending(c => c.Id)
        };

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling(total / (double)pageSize),
            sort,
            city,
            items
        });
    }

    // ✅ GET BY ID
    // GET /api/Campaign/3
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var campaign = await _context.Campaigns.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (campaign == null) return NotFound();
        return Ok(campaign);
    }

    // ✅ CREATE
    // POST /api/Campaign
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Campaign campaign)
    {
        // Güvenli olsun diye ID'yi sıfırla (DB üretsin)
        campaign.Id = 0;

        _context.Campaigns.Add(campaign);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = campaign.Id }, campaign);
    }

    // ✅ UPDATE
    // PUT /api/Campaign/3
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Campaign updated)
    {
        var existing = await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == id);
        if (existing == null) return NotFound();

        existing.Title = updated.Title;
        existing.City = updated.City;
        existing.Description = updated.Description;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    // ✅ DELETE
    // DELETE /api/Campaign/3
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var existing = await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == id);
        if (existing == null) return NotFound();

        _context.Campaigns.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}