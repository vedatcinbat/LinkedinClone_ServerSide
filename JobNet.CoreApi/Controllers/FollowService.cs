using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Services.FollowService;
using Microsoft.AspNetCore.Mvc;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FollowService : ControllerBase
{
    private IFollowService _followService;

    public FollowService(IFollowService followService)
    {
        this._followService = followService;
    }

    [HttpGet("allFollows")]
    public async Task<IActionResult> GetAllFollows()
    {
        List<FollowUserSimpleResponse> allFollows = await _followService.GetAllFollows();

        return Ok(allFollows);
    }

    [HttpPost("{followerId:int}/follow/{userId:int}")]
    public async Task<IActionResult> FollowUser(int followerId, int userId)
    {
        var result = await _followService.FollowUser(followerId, userId);

        return Ok(result);
    }
    [HttpPatch("{unFollowerId:int}/unfollow/{userId:int}")]
    public async Task<IActionResult> UnFollowUser(int unFollowerId, int userId)
    {
        var result = await _followService.UnFollowUser(unFollowerId, userId);

        return Ok(result);
    }
}