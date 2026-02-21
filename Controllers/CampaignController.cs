using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// diğer using'ler...

[HttpGet]
public async Task<IActionResult> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? sort = "id_desc",
    [FromQuery] string? city = null
)
{
    // Guard rails
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 10;
    if (pageSize > 100) pageSize = 100;

    IQueryable<Campaign> query = _context.Campaigns.AsNoTracking();

    // Optional city filter
    if (!string.IsNullOrWhiteSpace(city))
    {
        query = query.Where(c => c.City == city);
    }

    // Sorting
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