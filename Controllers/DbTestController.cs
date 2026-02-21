using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Enes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DbTestController : ControllerBase
{
    private readonly IConfiguration _config;

    public DbTestController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        var cs = _config.GetConnectionString("Default");

        await using var conn = new NpgsqlConnection(cs);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT 1", conn);
        var result = await cmd.ExecuteScalarAsync();

        return Ok(new { ok = true, result });
    }
}