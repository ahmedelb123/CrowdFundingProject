using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("api/donation")]
[ApiController]
public class DonationController : ControllerBase
{
    private readonly DonationHandler _donationService;

    public DonationController(DonationHandler donationService)
    {
        _donationService = donationService;
    }

    // Create a Donation
    [HttpPost("create")]
    public async Task<IActionResult> CreateDonation([FromBody] CreateDonationDto request)
    {
        try
        {
            if (request.Amount <= 0)
            {
                return BadRequest(new { message = "Donation amount must be greater than zero." });
            }

              
            
            var result = await _donationService.CreateDonation(request);

            if (!result.Status)
                return BadRequest(result);

            return Ok(new
            {
                message = result.Message,
                donationDetails = result.DonationDetails
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the donation.", error = ex.Message });
        }
    }

    // Get All Donations for a Post
    [HttpGet("{postId}/all")]
    public async Task<IActionResult> GetDonationsByPostId(int postId)
    {
        try
        {
            var result = await _donationService.GetDonationsByPostId(postId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving donations.", error = ex.Message });
        }
    }
}
