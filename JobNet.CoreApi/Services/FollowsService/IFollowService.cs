using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Response;

namespace JobNet.CoreApi.Services.FollowService;

public interface IFollowService
{
    Task<List<Follow>> GetAllFollows();
    
    Task<string> FollowUser(int followerId, int userId, User followerUser, User user);
    
    Task<string> UnFollowUser(int unFollowerId, int userId);
}