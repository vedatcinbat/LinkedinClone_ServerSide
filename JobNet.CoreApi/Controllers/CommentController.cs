using System.Security.Claims;
using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response.Problem;
using JobNet.CoreApi.Services.CommentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly JobNetDbContext _dbContext;

    public CommentController(ICommentService commentService, JobNetDbContext dbContext)
    {
        _commentService = commentService;
        _dbContext = dbContext;
    }


    [HttpGet("allComments")]
    public async Task<IActionResult> GetAllComments()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var comments = await _commentService.GetAllComments();
        
            return Ok(comments);
        }

        ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "User not authorized",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse);
    }
    
    [HttpGet("allCommentsActive")]
    public async Task<IActionResult> GetAllCommentsActive()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var comments = await _commentService.GetAllCommentsNotDeleted();
        
            return Ok(comments);
        }

        ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "User not authorized",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse);
    }
    
    
    [HttpPost("{userId:int}/comment/{postId:int}")]
    public async Task<IActionResult> AddComment(int userId, int postId, CreateCommmentApiRequest createCommentApiRequest)
    {
        ProblemDetailResponse problemDetailResponse;
        
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var commentedUserId = Convert.ToInt32(userIdClaim.Value);

            if (commentedUserId != userId)
            {
                problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "User Has Not The Permission",
                    ProblemDescription = $"User({commentedUserId}) cannot comment as User({userId})"
                };
                return Ok(problemDetailResponse);
            }

            var user = await _dbContext.Users.Where(u => u.IsDeleted == false)
                .FirstOrDefaultAsync(u => u.UserId == userId);
            var post = await _dbContext.Posts.Where(post => post.IsDeleted == false)
                .FirstOrDefaultAsync(p => p.PostId == postId);
            
            if (user == null)
            {
                problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "User Not Found",
                    ProblemDescription = $"User not found({commentedUserId})"
                };
                return Ok(problemDetailResponse);
            }

            if (post == null)
            {
                problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "Post Not Found",
                    ProblemDescription = $"Post not found({postId})"
                };
                return Ok(problemDetailResponse);
            }
            
            var commentId = await _dbContext.Comments.CountAsync() + 1;

            Comment comment = new Comment
            {
                CommentId = commentId,
                Content = createCommentApiRequest.Content,
                CommentedAt = DateTime.UtcNow,
                IsDeleted = false,
                UserId = userId,
                User = user,
                PostId = postId,
                Post = post
            };
            
            await _dbContext.Comments.AddAsync(comment);
            post.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            var res = $"User ({userId}) commented Post ({postId}) with content ({createCommentApiRequest.Content})";

            return Ok(res);
        }
        
        problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse);
    }
    
    [HttpPatch("{userId:int}/deleteComment/{postId:int}/{commentId:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int userId, [FromRoute] int postId, [FromRoute] int commentId)
    {
        ProblemDetailResponse problemDetailResponse;
        
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var commentedUserId = Convert.ToInt32(userIdClaim.Value);

            if (commentedUserId != userId)
            {
                problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "User Has Not The Permission",
                    ProblemDescription = $"User({commentedUserId}) cannot delete comment as User({userId})"
                };
                return Ok(problemDetailResponse);
            }

            var user = await _dbContext.Users.Where(u => u.IsDeleted == false)
                .FirstOrDefaultAsync(u => u.UserId == userId);
            var post = await _dbContext.Posts.Where(post => post.IsDeleted == false)
                .FirstOrDefaultAsync(p => p.PostId == postId);
            var comment = await _dbContext.Comments.Where(c => c.IsDeleted == false)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);

            if (user == null)
            {
                problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "User Not Found",
                    ProblemDescription = $"User not found({commentedUserId})"
                };
                return Ok(problemDetailResponse);
            }

            if (post == null)
            {
                problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "Post Not Found",
                    ProblemDescription = $"Post not found({postId})"
                };
                return Ok(problemDetailResponse);
            }

            if (comment == null)
            {
                problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "Comment Not Found",
                    ProblemDescription = $"Comment not found({commentId})"
                };
                return Ok(problemDetailResponse);
            }

            comment.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            
            var res = $"Comment({commentId}) successfully deleted from post({postId}) by user({userId})";

            return Ok(res);

        }
        problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "User not authenticated!",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse);
    }
}