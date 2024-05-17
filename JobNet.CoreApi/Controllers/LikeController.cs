using System.Security.Claims;
using JobNet.CoreApi.Models.Response.Problem;
using JobNet.CoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikeController : ControllerBase
{
    private readonly ILikeService _likeService;

    public LikeController(ILikeService likeService)
    {
        _likeService = likeService;
    }


    [HttpPatch("{userId:int}/like/{postId:int}")]
    public async Task<IActionResult> LikePost([FromRoute] int userId, [FromRoute] int postId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var currentUserId = Convert.ToInt32(userIdClaim.Value);

            if (currentUserId == userId)
            {
                var res = await _likeService.LikePostWithUserIdAndPostId(userId, postId);

                return Ok(res);
            }

            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "User has no permission",
                ProblemDescription = $"You User({currentUserId}) cant like post as User({userId})"
            };

            return Ok(problemDetailResponse);
        }
        
        ProblemDetailResponse problemDetailResponseNotFound = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authorize first!"
        };

        return Ok(problemDetailResponseNotFound);

        
    }
    
    [HttpPatch("{userId:int}/dislike/{postId:int}")]
    public async Task<IActionResult> UnLikePost([FromRoute] int userId, [FromRoute] int postId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var currentUserId = Convert.ToInt32(userIdClaim.Value);

            if (currentUserId == userId)
            {
                var res = await _likeService.UnlikePostWithUserIdAndPostId(userId, postId);

                return Ok(res);
            }

            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "User has no permission",
                ProblemDescription = $"You User({currentUserId}) cant like post as User({userId})"
            };

            return Ok(problemDetailResponse);
        }
        
        ProblemDetailResponse problemDetailResponseNotFound = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authorize first!"
        };

        return Ok(problemDetailResponseNotFound);
    }
}