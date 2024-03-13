using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Services.FollowService;

public class FollowService : IFollowService
{
    private readonly JobNetDbContext _dbContext;

    public FollowService(JobNetDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<List<Follow>> GetAllFollows()
    {
        var allFollows = await _dbContext.Follows
            .Include(f => f.FollowerUser)
            .ThenInclude(f => f.Company)
            .Include(f => f.FollowingUser)
            .ThenInclude(f => f.Company)
            .ToListAsync();

        return allFollows;
    }

    public async Task<string> FollowUser(int followerId, int userId, User followerUser, User user)
    {
        var alreadyFollowed = await _dbContext.Follows.Where(f => f.IsDeleted == false)
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == userId);
        if (alreadyFollowed)
        {
            return $"Follower with ID {followerId} is already following user with ID {userId}.";
        }

        var isThereFollowRecord =
            await _dbContext.Follows.AnyAsync(f => f.FollowerId == followerId && f.FollowingId == userId);

        if (!isThereFollowRecord)
        {
            var newFollow = new Follow
            {
                FollowerId = followerId,
                FollowerUser = followerUser,
                FollowingId = userId,
                FollowingUser = user,
                IsDeleted = false
            };

            await _dbContext.Follows.AddAsync(newFollow);
            await _dbContext.SaveChangesAsync();

            return $"Follower with ID {followerId} started following user with ID {userId}.";
        }

        var followRec =
            await _dbContext.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == userId);

        if (followRec != null) followRec.IsDeleted = false;
        await _dbContext.SaveChangesAsync();
        
        return $"Follower with ID {followerId} started following again user with ID {userId}.";

    }

    public async Task<string> UnFollowUser(int unFollowerId, int userId)
    {
        var follow = await _dbContext.Follows.Where(f => f.IsDeleted == false).FirstOrDefaultAsync(f => f.FollowerId == unFollowerId && f.FollowingId == userId);
        
        if (follow != null)
        {
            follow.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return $"User with ID {unFollowerId} unfollowed user with ID {userId}.";
        }

        return $"No follow record found for unfollower ID {unFollowerId} following user ID {userId}.";
    }
}