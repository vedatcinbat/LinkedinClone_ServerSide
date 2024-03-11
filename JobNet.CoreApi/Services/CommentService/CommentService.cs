using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Services.CommentService;

public class CommentService : ICommentService
{
    private readonly JobNetDbContext _dbContext;

    public CommentService(JobNetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GetAllCommentsSimpleApiResponse>> GetAllComments()
    {
        List<Comment> comments = await _dbContext.Comments
            .Include(comment => comment.Post)
            .Include(comment => comment.User).ThenInclude(user => user.Company)
            .Include(comment => comment.Post)
            .ThenInclude(post => post.User)
            .ThenInclude(user => user.Company)
            .ToListAsync();

        List<GetAllCommentsSimpleApiResponse> commentSimpleResponses = comments.Select(comment => new GetAllCommentsSimpleApiResponse
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            CommentedAt = comment.CommentedAt,
            IsDeleted = comment.IsDeleted,
            UserId = comment.UserId,
            User = new UserCommentSimpleResponse
            {
                UserId = comment.User.UserId,
                Firstname = comment.User.Firstname,
                Lastname = comment.User.Lastname,
                Title = comment.User.Title,
                ProfilePictureUrl = comment.User.ProfilePictureUrl,
                IsDeleted = comment.User.IsDeleted,
            },
            PostId = comment.PostId,
            Post = new PostCommentSimpleResponse()
            {
                PostId = comment.Post.PostId,
                IsDeleted = comment.Post.IsDeleted,
                UserId = comment.User.UserId,
                User = new UserPostSimpleResponse
                {
                    UserId = comment.Post.User.UserId,
                    Firstname = comment.Post.User.Firstname,
                    Lastname = comment.Post.User.Lastname,
                    Title = comment.Post.User.Title,
                    ProfilePictureUrl = comment.Post.User.ProfilePictureUrl,
                    IsDeleted = comment.Post.User.IsDeleted,
                    CompanyId = comment.Post.User.CompanyId,
                    Company = comment.Post.User.Company != null ? new UserPostCompanySimpleResponse
                    {
                        CompanyId = comment.Post.User.Company.CompanyId,
                        CompanyName = comment.Post.User.Company.CompanyName,
                        Industry = comment.Post.User.Company.Industry,
                        LogoUrl = comment.Post.User.Company.LogoUrl
                    } : null,
                },
                PublishTime = comment.Post.PublishTime,
                Caption = comment.Post.Caption,
                PostType = comment.Post.PostType,
                TextContent = comment.Post.TextContent,
                ImageContent = comment.Post.ImageContent,
                ImagesContent = comment.Post.ImagesContent,
                CommentCount = comment.Post.CommentCount,
                LikeCount = comment.Post.LikeCount,
            },
        }).ToList();

        return commentSimpleResponses;

    }
    
    public async Task<List<GetAllCommentsSimpleApiResponse>> GetAllCommentsNotDeleted()
    {
        List<Comment> comments = await _dbContext.Comments
            .Where(c => c.IsDeleted == false)
            .Include(comment => comment.Post)
            .Include(comment => comment.User).ThenInclude(user => user.Company)
            .Include(comment => comment.Post)
            .ThenInclude(post => post.User)
            .ThenInclude(user => user.Company)
            .ToListAsync();

        List<GetAllCommentsSimpleApiResponse> commentSimpleResponses = comments.Select(comment => new GetAllCommentsSimpleApiResponse
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            CommentedAt = comment.CommentedAt,
            IsDeleted = comment.IsDeleted,
            UserId = comment.UserId,
            User = new UserCommentSimpleResponse
            {
                UserId = comment.User.UserId,
                Firstname = comment.User.Firstname,
                Lastname = comment.User.Lastname,
                Title = comment.User.Title,
                ProfilePictureUrl = comment.User.ProfilePictureUrl,
                IsDeleted = comment.User.IsDeleted,
            },
            PostId = comment.PostId,
            Post = new PostCommentSimpleResponse()
            {
                PostId = comment.Post.PostId,
                IsDeleted = comment.Post.IsDeleted,
                UserId = comment.User.UserId,
                User = new UserPostSimpleResponse
                {
                    UserId = comment.Post.User.UserId,
                    Firstname = comment.Post.User.Firstname,
                    Lastname = comment.Post.User.Lastname,
                    Title = comment.Post.User.Title,
                    ProfilePictureUrl = comment.Post.User.ProfilePictureUrl,
                    IsDeleted = comment.Post.User.IsDeleted,
                    CompanyId = comment.Post.User.CompanyId,
                    Company = comment.Post.User.Company != null ? new UserPostCompanySimpleResponse
                    {
                        CompanyId = comment.Post.User.Company.CompanyId,
                        CompanyName = comment.Post.User.Company.CompanyName,
                        Industry = comment.Post.User.Company.Industry,
                        LogoUrl = comment.Post.User.Company.LogoUrl
                    } : null,
                },
                PublishTime = comment.Post.PublishTime,
                Caption = comment.Post.Caption,
                PostType = comment.Post.PostType,
                TextContent = comment.Post.TextContent,
                ImageContent = comment.Post.ImageContent,
                ImagesContent = comment.Post.ImagesContent,
                CommentCount = comment.Post.CommentCount,
                LikeCount = comment.Post.LikeCount,
            },
        }).ToList();

        return commentSimpleResponses;

    }

    public async Task<string> AddComment(int userId, int postId, CreateCommmentApiRequest createCommentApiRequest)
    {
        var res = "";

        var user = await _dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            res = $"User not found id({userId})";
            return res;
        }

        var post = await _dbContext.Posts.Where(p => p.IsDeleted == false).FirstOrDefaultAsync(p => p.PostId == postId);

        if (post == null)
        {
            res = $"Post not found id({postId})";
            return res;
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

        res = $"User ({userId}) commented Post ({postId}) with content ({createCommentApiRequest.Content})";
        return res;
    }

    public async Task<string> DeleteComment(int userId, int postId, int commentId)
    {
        var res = "";

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IsDeleted == false && u.UserId == userId);

        if (user == null)
        {
            res = $"Uesr not found with id ({userId})";
            return res;
        }

        var post = await _dbContext.Posts.Where(p => p.IsDeleted == false && p.PostId == postId).ToListAsync();

        if (post == null)
        {
            res = $"Post not found with id({postId})";
            return res;
        }

        var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.IsDeleted == false && c.CommentId == commentId);
        
        if (comment == null)
        {
            res = $"comment not found with id({commentId})";
            return res;
        }

        var isUserAllowedToDelete = user.UserId == comment.UserId;

        if (!isUserAllowedToDelete)
        {
            res = $"User with id({userId}) is not allowed to delete this comment";
            return res;
        }

        comment.IsDeleted = true;
        await _dbContext.SaveChangesAsync();

        res = $"Comment({commentId}) successfully deleted from post({postId}) by user({userId})";

        return res;
    }
}