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

    public async Task<List<FollowUserSimpleResponse>> GetAllFollows()
    {
        var allFollows = await _dbContext.Follows
            .Include(f => f.FollowerUser)
            .ThenInclude(f => f.Company)
            .Include(f => f.FollowingUser)
            .ThenInclude(f => f.Company)
            .ToListAsync();

        var followUserSimpleResponses = allFollows.Select(follow => new FollowUserSimpleResponse
        {
            Id = follow.Id,
            FollowerId = follow.FollowerId,
            FollowerUser = new UserFollowSimpleApiResponse()
            {
                UserId = follow.FollowerUser.UserId,
                Firstname = follow.FollowerUser.Firstname,
                Lastname = follow.FollowerUser.Lastname,
                Email = follow.FollowerUser.Email,
                Age = follow.FollowerUser.Age,
                Country = follow.FollowerUser.Country,
                CurrentLanguage = follow.FollowerUser.CurrentLanguage,
                ProfilePictureUrl = follow.FollowerUser.ProfilePictureUrl,
                CompanyId = follow.FollowerUser.CompanyId,
                Company = follow.FollowerUser.Company != null
                    ? new UserCompanySimpleResponse
                    {
                        CompanyId = follow.FollowerUser.Company.CompanyId,
                        CompanyName = follow.FollowerUser.Company.CompanyName,
                        Industry = follow.FollowerUser.Company.Industry,
                        Description = follow.FollowerUser.Company.Description,
                        EmployeeCount = follow.FollowerUser.Company.EmployeeCount,
                        WebsiteUrl = follow.FollowerUser.Company.WebsiteUrl,
                        LogoUrl = follow.FollowerUser.Company.LogoUrl,
                        FoundedAt = follow.FollowerUser.Company.FoundedAt
                    }
                    : null,
            },
            FollowingId = follow.FollowingId,
            FollowingUser = new UserFollowSimpleApiResponse
            {
                UserId = follow.FollowingUser.UserId,
                Firstname = follow.FollowingUser.Firstname,
                Lastname = follow.FollowingUser.Lastname,
                Email = follow.FollowingUser.Email,
                Age = follow.FollowingUser.Age,
                Country = follow.FollowingUser.Country,
                CurrentLanguage = follow.FollowingUser.CurrentLanguage,
                ProfilePictureUrl = follow.FollowingUser.ProfilePictureUrl,
                CompanyId = follow.FollowingUser.CompanyId,
                Company = follow.FollowingUser.Company != null
                    ? new UserCompanySimpleResponse
                    {
                        CompanyId = follow.FollowingUser.Company.CompanyId,
                        CompanyName = follow.FollowingUser.Company.CompanyName,
                        Industry = follow.FollowingUser.Company.Industry,
                        Description = follow.FollowingUser.Company.Description,
                        EmployeeCount = follow.FollowingUser.Company.EmployeeCount,
                        WebsiteUrl = follow.FollowingUser.Company.WebsiteUrl,
                        LogoUrl = follow.FollowingUser.Company.LogoUrl,
                        FoundedAt = follow.FollowingUser.Company.FoundedAt
                    }
                    : null,
            },
            IsDeleted = follow.IsDeleted,
        }).ToList();

        return followUserSimpleResponses;
    }

    public async Task<string> FollowUser(int followerId, int userId)
    {
        var followerUser = await _dbContext.Users.Where(user => user.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == followerId);
        if (followerUser == null)
        {
            return $"Follower with ID {followerId} not found.";
        }

        var user = await _dbContext.Users.Where(user => user.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            return $"User with ID {userId} not found.";
        }

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
        var unFollowerUser = await _dbContext.Users.Where(user => user.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == unFollowerId);
        if (unFollowerUser == null)
        {
            return $"Unfollower with ID {unFollowerId} not found.";
        }

        var user = await _dbContext.Users.Where(user => user.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            return $"User with ID {userId} not found.";
        }

        var follow = await _dbContext.Follows.Where(f => f.IsDeleted == false).FirstOrDefaultAsync(f => f.FollowerId == unFollowerId && f.FollowingId == userId);
        if (follow != null)
        {
            follow.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return $"Unfollower with ID {unFollowerId} unfollowed user with ID {userId}.";
        }

        return $"No follow record found for unfollower ID {unFollowerId} following user ID {userId}.";
    }
}