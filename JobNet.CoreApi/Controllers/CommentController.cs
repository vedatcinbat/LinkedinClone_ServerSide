using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Services.CommentService;
using Microsoft.AspNetCore.Mvc;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }


    [HttpGet("allComments")]
    public async Task<IActionResult> GetAllComments()
    {
        var comments = await _commentService.GetAllComments();
        
        return Ok(comments);
    }
    
    [HttpGet("allCommentsActive")]
    public async Task<IActionResult> GetAllCommentsActive()
    {
        var comments = await _commentService.GetAllCommentsNotDeleted();
        
        return Ok(comments);
    }
    
    
    [HttpPost("{userId:int}/commented/{postId:int}")]
    public async Task<IActionResult> AddComment(int userId, int postId, CreateCommmentApiRequest createCommentApiRequest)
    {
        var result = await _commentService.AddComment(userId, postId, createCommentApiRequest);
        
        return Ok(result);
    }
    
    [HttpPatch("{userId:int}/deleteComment/{postId:int}/{commentId:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int userId, [FromRoute] int postId, [FromRoute] int commentId)
    {
        var result = await _commentService.DeleteComment(userId, postId, commentId);
        
        return Ok(result);
    }
}