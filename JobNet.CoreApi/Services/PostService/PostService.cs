using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Models.Response.Problem;
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

    public async Task<List<Post>> GetAllPosts()
    {
        List<Post> posts = await _dbContext.Posts.Where(post => post.IsDeleted == false)
            .Include(p => p.User)
            .ThenInclude(u => u.Company)
            .Include(p => p.Comments.Where(c => c.IsDeleted == false && c.User.IsDeleted == false))
            .ThenInclude(comment => comment.User)
            .Include(p => p.Likes.Where(like => like.IsDeleted == false && like.User.IsDeleted == false))
            .ThenInclude(l => l.User)
            .ThenInclude(u => u.Company)
            .ToListAsync();


        return posts;
    }

    public async Task<Post> GetOnePost(int postId)
    {
        var post = await _dbContext.Posts.Where(p => p.IsDeleted == false)
            .Include(p => p.User)
            .ThenInclude(u => u.Company)
            .Include(p => p.Comments.Where(c => c.IsDeleted == false && c.User.IsDeleted == false))
            .ThenInclude(comment => comment.User)
            .Include(p => p.Likes.Where(l => l.IsDeleted == false && l.User.IsDeleted == false))
            .ThenInclude(l => l.User)
            .ThenInclude(u => u.Company)
            .FirstOrDefaultAsync(p => p.PostId == postId);
        return post;
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
                Company = user.Company != null ? new UserPostCompanySimpleResponse
                {
                    CompanyId = user.Company.CompanyId,
                    CompanyName = user.Company.CompanyName,
                    Industry = user.Company.Industry,
                    LogoUrl = user.Company.LogoUrl
                } : null
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