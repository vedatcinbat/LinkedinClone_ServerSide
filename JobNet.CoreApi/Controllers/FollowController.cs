using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Models.Response.Problem;
using JobNet.CoreApi.Services.FollowService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FollowController : ControllerBase
{
    private IFollowService _followService;
    private readonly JobNetDbContext _dbContext;

    public FollowController(IFollowService followService, JobNetDbContext dbContext)
    {
        this._followService = followService;
        this._dbContext = dbContext;
    }

    [HttpGet("allFollows")]
    public async Task<IActionResult> GetAllFollows()
    {
        List<Follow> allFollows = await _followService.GetAllFollows();

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

        return Ok(followUserSimpleResponses);
    }

    [HttpPost("{followerId:int}/follow/{userId:int}")]
    public async Task<IActionResult> FollowUser(int followerId, int userId)
    {
        if(followerId == userId)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Follower and User are the same",
                ProblemDescription = $"Follower and User are the same with id : {followerId}",
            };
            return Ok(problemDetailResponse);
        }
        
        var followerUser = await _dbContext.Users.Where(user => user.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == followerId);
        if(followerUser == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Follower not found",
                ProblemDescription = $"Follower not found with id : {followerId}",
            };
            return Ok(problemDetailResponse);
        }
        
        var user = await _dbContext.Users.Where(user => user.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);
        if(user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "User not found",
                ProblemDescription = $"User not found with id : {userId}",
            };
            return Ok(problemDetailResponse);
        }
        
        var result = await _followService.FollowUser(followerId, userId, followerUser, user);
        
        return Ok(result);
    }
    [HttpPatch("{unFollowerId:int}/unfollow/{userId:int}")]
    public async Task<IActionResult> UnFollowUser(int unFollowerId, int userId)
    {
        if (unFollowerId == userId)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Unfollower and User are the same",
                ProblemDescription = $"Unfollower and User are the same with id : {unFollowerId}",
            };
            return Ok(problemDetailResponse);
        }
        
        var unFollowerUser = await _dbContext.Users.Where(user => user.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == unFollowerId);
        if(unFollowerUser == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "UnFollower not found",
                ProblemDescription = $"UnFollower not found with id : {unFollowerId}",
            };
            return Ok(problemDetailResponse);
        }
        
        var user = await _dbContext.Users.Where(user => user.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);
        if(user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "User not found",
                ProblemDescription = $"User not found with id : {userId}",
            };
            return Ok(problemDetailResponse);
        }
        
        var result = await _followService.UnFollowUser(unFollowerId, userId);


        return Ok(result);
    }
}