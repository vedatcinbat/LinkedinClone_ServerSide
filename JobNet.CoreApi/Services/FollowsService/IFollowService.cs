using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Response;

namespace JobNet.CoreApi.Services.FollowService;

public interface IFollowService
{
    Task<List<FollowUserSimpleResponse>> GetAllFollows();
    
    Task<string> FollowUser(int followerId, int userId);
    
    Task<string> UnFollowUser(int unFollowerId, int userId);
}