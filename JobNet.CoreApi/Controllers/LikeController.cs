using JobNet.CoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var res = await _likeService.LikePostWithUserIdAndPostId(userId, postId);

        return Ok(res);
    }
    
    [HttpPatch("{userId:int}/unlike/{postId:int}")]
    public async Task<IActionResult> UnLikePost([FromRoute] int userId, [FromRoute] int postId)
    {
        var res = await _likeService.UnlikePostWithUserIdAndPostId(userId, postId);

        return Ok(res);
    }
}