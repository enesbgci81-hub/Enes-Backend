using Microsoft.AspNetCore.Mvc;
using Enes.Models;

namespace Enes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private static List<Campaign> _campaigns = new List<Campaign>();

    [HttpGet]
    public IActionResult GetAll(string? city)
    {
        if (!string.IsNullOrEmpty(city))
        {
            var filtered = _campaigns
                .Where(c => c.City.ToLower() == city.ToLower())
                .ToList();

            return Ok(filtered);
        }

        return Ok(_campaigns);
    }

    [HttpPost]
    public IActionResult Create(Campaign campaign)
    {
        campaign.Id = _campaigns.Count + 1;
        _campaigns.Add(campaign);
        return Ok(campaign);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var campaign = _campaigns.FirstOrDefault(c => c.Id == id);

        if (campaign == null)
            return NotFound("Campaign not found");

        _campaigns.Remove(campaign);

        return Ok("Deleted successfully");
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Campaign updatedCampaign)
    {
        var campaign = _campaigns.FirstOrDefault(c => c.Id == id);

        if (campaign == null)
            return NotFound("Campaign not found");

        campaign.Title = updatedCampaign.Title;
        campaign.City = updatedCampaign.City;
        campaign.Description = updatedCampaign.Description;

        return Ok(campaign);
    }
}