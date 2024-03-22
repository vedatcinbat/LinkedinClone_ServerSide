using System.Security.Claims;
using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Models.Response.Problem;
using JobNet.CoreApi.Services.PostService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class PostController(IPostService postService, JobNetDbContext _dbContext) : ControllerBase
{
    private readonly IPostService _postService = postService;
    
    [HttpGet("allPosts")]
    public async Task<IActionResult> GetAllPosts()
    {
        List<Post> posts = await _postService.GetAllPosts();
        
        var postSimpleApiResponses = posts.Select(post => new GetAllPostSimpleResponse()
        {
            PostId = post.PostId,
            IsDeleted = post.IsDeleted,
            UserId = post.UserId,
            User = new UserPostSimpleResponse
            {
                UserId = post.User.UserId,
                Firstname = post.User.Firstname,
                Lastname = post.User.Lastname,
                Title = post.User.Title,
                ProfilePictureUrl = post.User.ProfilePictureUrl,
                IsDeleted = post.User.IsDeleted,
                CompanyId = post.User.CompanyId,
                Company = post.User.Company != null
                    ? new UserPostCompanySimpleResponse
                    {
                        CompanyId = post.User.Company.CompanyId,
                        CompanyName = post.User.Company.CompanyName,
                        Industry = post.User.Company.Industry,
                        LogoUrl = post.User.Company.LogoUrl
                    }
                    : null,
            },
            PublishTime = post.PublishTime,
            Caption = post.Caption,
            PostType = post.PostType,
            TextContent = post.TextContent,
            ImageContent = post.ImageContent,
            ImagesContent = post.ImagesContent,
            CommentCount = post.Comments.Count(),
            LikeCount = post.Likes.Count(),
        }).ToList();

        return Ok(postSimpleApiResponses);
    }

    [HttpGet("{postId:int}")]
    public async Task<IActionResult> GetOnePostWithPostId([FromRoute] int postId)
    {
        var p = await _dbContext.Posts.FirstOrDefaultAsync(p => p.IsDeleted == false && p.PostId == postId);
        
        if (p == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Post not found",
                ProblemDescription = $"Post not found with id ({postId})"
            };
            return Ok(problemDetailResponse);
        }
        
        var post = await _postService.GetOnePost(postId);
        
        PostSimpleApiResponse postSimpleApiResponse = new PostSimpleApiResponse
        {
            PostId = post.PostId,
            IsDeleted = post.IsDeleted,
            UserId = post.UserId,
            User = new UserPostSimpleResponse
            {
                UserId = post.User.UserId,
                Firstname = post.User.Firstname,
                Lastname = post.User.Lastname,
                Title = post.User.Title,
                ProfilePictureUrl = post.User.ProfilePictureUrl,
                IsDeleted = post.User.IsDeleted,
                CompanyId = post.User.CompanyId,
                Company = post.User.Company != null
                    ? new UserPostCompanySimpleResponse
                    {
                        CompanyId = post.User.Company.CompanyId,
                        CompanyName = post.User.Company.CompanyName,
                        Industry = post.User.Company.Industry,
                        LogoUrl = post.User.Company.LogoUrl
                    }
                    : null,
            },
            PublishTime = post.PublishTime,
            Caption = post.Caption,
            PostType = post.PostType,
            TextContent = post.TextContent,
            ImageContent = post.ImageContent,
            ImagesContent = post.ImagesContent,
            CommentCount = post.Comments.Count(),
            LikeCount = post.Likes.Count(),
            Comments = post.Comments != null
                ? post.Comments.Select(comment => new PostCommentSimpleApiResponse
                    {
                        CommentId = comment.CommentId,
                        Content = comment.Content,
                        CommentedAt = comment.CommentedAt,
                        UserCommentSimpleResponse = new UserCommentSimpleResponse
                        {
                            UserId = comment.User.UserId,
                            Firstname = comment.User.Firstname,
                            Lastname = comment.User.Lastname,
                            Title = comment.User.Title,
                            ProfilePictureUrl = comment.User.ProfilePictureUrl,
                            IsDeleted = comment.User.IsDeleted
                        }
                    })
                    .ToList()
                : null,
            Likes = post.Likes.Select(like => new LikeSimpleResponse
                {
                    LikeId = like.LikeId,
                    IsDeleted = like.IsDeleted,
                    UserId = like.UserId,
                    User = new UserPostSimpleResponse
                    {
                        UserId = like.User.UserId,
                        Firstname = like.User.Firstname,
                        Lastname = like.User.Lastname,
                        Title = like.User.Title,
                        ProfilePictureUrl = like.User.ProfilePictureUrl,
                        IsDeleted = like.User.IsDeleted,
                        CompanyId = like.User.CompanyId,
                        Company = like.User.Company != null ? new UserPostCompanySimpleResponse
                        {
                            CompanyId = like.User.Company.CompanyId,
                            CompanyName = like.User.Company.CompanyName,
                            Industry = like.User.Company.Industry,
                            LogoUrl = like.User.Company.LogoUrl
                        } : null
                    }
                })
                .ToList(),
        };
        
        return Ok(postSimpleApiResponse);
    }
    
    [HttpPost("/createPost")]
    [Authorize]
    public async Task<IActionResult> CreatePost(CreatePostApiRequest createPostApiRequest)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            
            var userId = Convert.ToInt32(userIdClaim.Value);
            
            var response = await _postService.CreatePost(userId, createPostApiRequest);

            return Ok(response);
        }
        
        ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse);
        
        
    }

    [HttpDelete("/deletePost/{postId:int}")]
    [Authorize]
    public async Task<IActionResult> DeletePost([FromRoute] int postId)
    {
        Post post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.IsDeleted == false && p.PostId == postId);
        if (post == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Post not found",
                ProblemDescription = $"Post not found with id ({postId})"
            };
            return Ok(problemDetailResponse);
        }
        
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var userId = userIdClaim.Value;
            
            var isUserAllowedToDeletePost = post.UserId == Convert.ToInt32(userId);

            if (!isUserAllowedToDeletePost)
            {
                ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "User not allowed to delete",
                    ProblemDescription = $"User({userId}) does not have the permission for deleting post({postId})"
                };
                return Ok(problemDetailResponse);
            }

            var isPostAlreadyDeleted = post.IsDeleted;
        
            if (isPostAlreadyDeleted)
            {
                ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "Post is already been deleted",
                    ProblemDescription = $"Post({postId}) has already been deleted"
                };
                return Ok(problemDetailResponse);
            }
        
            post.IsDeleted = true;
            await _dbContext.SaveChangesAsync();

            return Ok($"Post({postId}) has been deleted");
        }

        ProblemDetailResponse problemDetailResponse2 = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"User not found with id ({userIdClaim.Value})"
        };
        return Ok(problemDetailResponse2);


    }
    
    
    
    
}