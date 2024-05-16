using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace JobNet.CoreApi.Services.UserService;

public interface IUserService
{
    Task<List<User?>> GetAllUsers();
    
    Task<List<User?>> GetAllUsersActive();

    Task<User?> GetOneUserProfileDetails(int userId);

    Task<User?> GetOneUserPostsWithUserId(int userId);

    Task<User?> GetOneUserSimpleDetails(int userId);
    
    Task<User> CreateUser(CreateUserApiRequest createUserApiRequest);

    Task<List<User>> GetFollowers(int userId);
    
    Task<List<User>> GetFollowings(int userId);
    
    Task<User?> UpdateUserCompanyWithId(int userId, int companyId);
    
    Task<User> AddSchoolToUser(User user, School school, UserEducationApiRequest userEducationApiRequest);
    
    Task<User?> GetUserSkill(int userId);
    
    Task<User> Authenticate(string email, string password);

    Task<User?> SaveAccount(string email, string password);

    Task<List<Post>> GetUserConnectionsPosts(int userId);

}