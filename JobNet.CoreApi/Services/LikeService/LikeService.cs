using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Services;

public class LikeService(JobNetDbContext dbContext) : ILikeService
{
    private readonly JobNetDbContext _dbContext = dbContext;

    public async Task<string> LikePostWithUserIdAndPostId(int userId, int postId)
    {
        var res = "";

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IsDeleted == false && u.UserId == userId);

        if (user == null)
        {
            res = $"User not fount with id({userId})";
            return res;
        }
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.IsDeleted == false && p.PostId == postId);
        
        if (post == null)
        {
            res = $"Post not fount with id({postId})";
            return res;
        }

        var isAlreadyLikedThatPost = _dbContext.Likes.Where(l => l.IsDeleted == false).Any(l => l.UserId == userId && l.PostId == postId);

        if (!isAlreadyLikedThatPost)
        {
            int likeCount = await _dbContext.Likes.CountAsync() + 1;

            Like like = new Like
            {
                LikeId = likeCount,
                IsDeleted = false,
                UserId = user.UserId,
                User = user,
                PostId = post.PostId,
                Post = post
            };

            await _dbContext.Likes.AddAsync(like);
            post.Likes.Add(like);
            await _dbContext.SaveChangesAsync();

            res = $"UserId({userId}) liked postId({postId})";

            return res;
        }
        
        res = $"UserId({userId}) has already been liked postId({postId})";

        return res;
    }

    public async Task<string> UnlikePostWithUserIdAndPostId(int userId, int postId)
    {
        var res = "";

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IsDeleted == false && u.UserId == userId);

        if (user == null)
        {
            res = $"User not fount with id({userId})";
            return res;
        }
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.IsDeleted == false && p.PostId == postId);
        
        if (post == null)
        {
            res = $"Post not fount with id({postId})";
            return res;
        }

        var like = await _dbContext.Likes.FirstOrDefaultAsync(l =>
            !l.IsDeleted && l.UserId == userId && l.PostId == postId);
                   
        if (like == null)
        {
            res = $"User({userId}) already didnt like post({postId})";
            return res;
        }

        like.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        
        res = $"User({userId}) has unliked post({postId})";
        return res;

    }
}