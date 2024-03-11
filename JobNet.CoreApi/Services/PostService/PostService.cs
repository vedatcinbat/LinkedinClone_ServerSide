using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Services.PostService;

public class PostService : IPostService
{
    private readonly JobNetDbContext _dbContext;

    public PostService(JobNetDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<List<PostSimpleApiResponse>> GetAllPosts()
    {
        List<Post> posts = await _dbContext.Posts
            .Include(p => p.User)
            .ThenInclude(u => u.Company)
            .Include(p => p.Comments.Where(c => c.IsDeleted == false)).ThenInclude(comment => comment.User)
            .Include(p => p.Likes)
            .ThenInclude(l => l.User)
            .ThenInclude(u => u.Company)
            .ToListAsync();


        var postSimpleApiResponses = posts.Select(post => new PostSimpleApiResponse
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
                        UserCommentSimpleResponse = comment.User != null ? new UserCommentSimpleResponse
                        {
                            UserId = comment.User.UserId,
                            Firstname = comment.User.Firstname,
                            Lastname = comment.User.Lastname,
                            Title = comment.User.Title,
                            ProfilePictureUrl = comment.User.ProfilePictureUrl,
                            IsDeleted = comment.User.IsDeleted
                        } : null,
                    }).ToList()
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
                        Company = new UserPostCompanySimpleResponse
                        {
                            CompanyId = like.User.Company.CompanyId,
                            CompanyName = like.User.Company.CompanyName,
                            Industry = like.User.Company.Industry,
                            LogoUrl = like.User.Company.LogoUrl
                        }
                    }
                })
                .ToList(),
        }).ToList();
        
        
        return postSimpleApiResponses;
    }

    public async Task<PostSimpleApiResponse> GetOnePost(int postId)
    {
        var post = await _dbContext.Posts.Where(p => p.IsDeleted == false)
            .Include(p => p.User)
            .ThenInclude(u => u.Company)
            .Include(p => p.Comments.Where(c => c.IsDeleted == false && c.User.IsDeleted == false))
            .ThenInclude(comment => comment.User)
            .Include(p => p.Likes.Where(l => l.IsDeleted == false))
            .ThenInclude(l => l.User)
            .ThenInclude(u => u.Company)
            .FirstOrDefaultAsync(p => p.PostId == postId);

        if (post == null)
        {
            throw new Exception($"Post not found with ID: {postId}"); 
        }


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
                        Company = new UserPostCompanySimpleResponse
                        {
                            CompanyId = like.User.Company.CompanyId,
                            CompanyName = like.User.Company.CompanyName,
                            Industry = like.User.Company.Industry,
                            LogoUrl = like.User.Company.LogoUrl
                        }
                    }
                })
                .ToList(),
        };


        return postSimpleApiResponse;
    }

    public async Task<CreatePostApiResponse> CreatePost(int userId, CreatePostApiRequest createPostApiRequest)
    {
        if (createPostApiRequest == null)
        {
            throw new ArgumentNullException(nameof(createPostApiRequest));
        }
        
        var user = await _dbContext.Users.Where(u => u.IsDeleted == false).Include("Company").FirstOrDefaultAsync(u => u.UserId == userId);

        var postCount = await _dbContext.Posts.CountAsync() + 1;

        Post post = new Post
        {
            PostId = postCount,
            IsDeleted = false,
            UserId = userId,
            User = user,
            PublishTime = DateTime.UtcNow,
            Caption = createPostApiRequest.Caption,
            PostType = createPostApiRequest.PostType,
            TextContent = createPostApiRequest.TextContent,
            ImageContent = createPostApiRequest.ImageContent,
            ImagesContent = createPostApiRequest.ImagesContent,
            Comments = new List<Comment>(),
            Likes = new List<Like>(),

        };

        user.Posts.Add(post);
        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();
        
        CreatePostApiResponse createPostApiResponse = new CreatePostApiResponse
        {
            PostId = postCount,
            IsDeleted = false,
            UserId = userId,
            User = new UserPostSimpleResponse
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Title = user.Title,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsDeleted = user.IsDeleted,
                CompanyId = user.CompanyId,
                Company = new UserPostCompanySimpleResponse
                {
                    CompanyId = user.Company.CompanyId,
                    CompanyName = user.Company.CompanyName,
                    Industry = user.Company.Industry,
                    LogoUrl = user.Company.LogoUrl
                }
            },
            PublishTime = DateTime.UtcNow,
            Caption = createPostApiRequest.Caption,
            PostType = createPostApiRequest.PostType,
            TextContent = createPostApiRequest.TextContent,
            ImageContent = createPostApiRequest.ImageContent,
            ImagesContent = createPostApiRequest.ImagesContent,
            CommentCount = post.CommentCount,
            LikeCount = post.LikeCount,
            Comments = new List<CommentSimpleResponse>(),
            Likes = new List<LikeSimpleResponse>(),

        };
        
        

        return createPostApiResponse;

    }
}