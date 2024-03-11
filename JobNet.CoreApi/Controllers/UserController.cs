using JobNet.CoreApi.Data;
using JobNet.CoreApi.Models.Request;
using Microsoft.AspNetCore.Mvc;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Models.Response.Problem;
using JobNet.CoreApi.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/users")]
public class UserController : ControllerBase
{
    private IUserService _userService;
    private JobNetDbContext _dbContext;

    public UserController(IUserService userService, JobNetDbContext dbContext)
    {
        this._userService = userService;
        _dbContext = dbContext;
    }
    
    [HttpGet("allUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();

        List<UserSimpleApiResponse> userSimpleApiResponses = users.Select(user => new UserSimpleApiResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            HashedPassword = user.HashedPassword,
            Title = user.Title,
            Email = user.Email,
            Age = user.Age,
            Country = user.Country,
            CurrentLanguage = user.CurrentLanguage,
            ProfilePictureUrl = user.ProfilePictureUrl,
            AboutMe = user.AboutMe,
            IsDeleted = user.IsDeleted,
            CompanyId = user.CompanyId,
            Company = user.Company != null ? new UserCompanySimpleResponse
            {
                CompanyId = user.Company.CompanyId,
                CompanyName = user.Company.CompanyName,
                Industry = user.Company.Industry,
                Description = user.Company.Description,
                EmployeeCount = user.Company.EmployeeCount,
                WebsiteUrl = user.Company.WebsiteUrl,
                LogoUrl = user.Company.LogoUrl,
                FoundedAt = user.Company.FoundedAt
            } : null,
        }).ToList();
        
        return Ok(userSimpleApiResponses); 
    }
    [HttpGet("allActiveUsers")]
    public async Task<IActionResult> GetAllUsersActive()
    {
        var users = await _userService.GetAllUsersActive();
        
        List<UserSimpleApiResponse> userSimpleApiResponses = users.Select(user => new UserSimpleApiResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            HashedPassword = user.HashedPassword,
            Title = user.Title,
            Email = user.Email,
            Age = user.Age,
            Country = user.Country,
            CurrentLanguage = user.CurrentLanguage,
            ProfilePictureUrl = user.ProfilePictureUrl,
            AboutMe = user.AboutMe,
            IsDeleted = user.IsDeleted,
            CompanyId = user.CompanyId,
            Company = user.Company != null ? new UserCompanySimpleResponse
            {
                CompanyId = user.Company.CompanyId,
                CompanyName = user.Company.CompanyName,
                Industry = user.Company.Industry,
                Description = user.Company.Description,
                EmployeeCount = user.Company.EmployeeCount,
                WebsiteUrl = user.Company.WebsiteUrl,
                LogoUrl = user.Company.LogoUrl,
                FoundedAt = user.Company.FoundedAt
            } : null,
        }).ToList();
        
        return Ok(userSimpleApiResponses);
        
    }

    [HttpGet("{userId:int}/profile")]
    public async Task<IActionResult> GetOneUserProfileInDetails([FromRoute] int userId)
    {
        var user = await _userService.GetOneUserProfileDetails(userId);
        
        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }

        
        int followerCount = await _dbContext.Follows.Where(f => f.IsDeleted == false && f.FollowingId == userId).CountAsync();

        int followingCount = await _dbContext.Follows.Where(f => f.IsDeleted == false && f.FollowerId == userId).CountAsync();
        
        
        UserProfileApiResponse userProfileApiResponse = new UserProfileApiResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Title = user.Title,
            Email = user.Email,
            Age = user.Age,
            Country = user.Country,
            AboutMe = user.AboutMe,
            FollowerCount = followerCount,
            FollowingCount = followingCount,
            CompanyId = user.CompanyId,
            Company = user.Company != null
                ? new UserCompanySimpleResponse
                {
                    CompanyId = user.Company.CompanyId,
                    CompanyName = user.Company.CompanyName,
                    Industry = user.Company.Industry,
                    Description = user.Company.Description,
                    EmployeeCount = user.Company.EmployeeCount,
                    WebsiteUrl = user.Company.WebsiteUrl,
                    LogoUrl = user.Company.LogoUrl,
                    FoundedAt = user.Company.FoundedAt
                }
                : null,
            Posts = user.Posts.Select(post => new PostApiResponseWithoutUser
            {
                PostId = post.PostId,
                IsDeleted = post.IsDeleted,
                UserId = post.UserId,
                PublishTime = post.PublishTime,
                Caption = post.Caption,
                PostType = post.PostType,
                TextContent = post.TextContent,
                ImageContent = post.ImageContent,
                ImagesContent = post.ImagesContent,
                CommentCount = post.CommentCount,
                LikeCount = post.LikeCount,
            }).ToList(),
            Experiences = user.Experiences,
            Educations = user.Educations,
            Skills = user.Skills,

        };
        
        return Ok(userProfileApiResponse);
    }
    
    [HttpGet("{userId:int}/simple")]
    public async Task<IActionResult> GetOneUserSimpleDetails([FromRoute] int userId)
    {
        var user = await _userService.GetOneUserSimpleDetails(userId);

        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        UserSimpleApiResponse userSimpleApiResponse = new UserSimpleApiResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Title = user.Title,
            HashedPassword = user.HashedPassword,
            Email = user.Email,
            Age = user.Age,
            Country = user.Country,
            CurrentLanguage = user.CurrentLanguage,
            ProfilePictureUrl = user.ProfilePictureUrl,
            AboutMe = user.AboutMe,
            CompanyId = user.CompanyId,
            Company = user.Company != null ? new UserCompanySimpleResponse
            {
                CompanyId = user.Company.CompanyId,
                CompanyName = user.Company.CompanyName,
                Industry = user.Company.Industry,
                Description = user.Company.Description,
                EmployeeCount = user.Company.EmployeeCount,
                WebsiteUrl = user.Company.WebsiteUrl,
                LogoUrl = user.Company.LogoUrl,
                FoundedAt = user.Company.FoundedAt
            } : null,
        };
        
        return Ok(userSimpleApiResponse);
    }

    [HttpGet("{userId:int}/getPosts")]
    public async Task<IActionResult> GetOneUserPostsWithUserId([FromRoute] int userId)
    {
        var user = await _userService.GetOneUserPostsWithUserId(userId);

        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        var getOneUserPostsResponse = new GetOneUserPostsResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Posts = user.Posts.Select(post => new PostSimpleApiResponseWithoutUser()
            {
                PostId = post.PostId,
                IsDeleted = post.IsDeleted,
                PublishTime = post.PublishTime,
                Caption = post.Caption,
                PostType = post.PostType,
                TextContent = post.TextContent,
                ImageContent = post.ImageContent,
                ImagesContent = post.ImagesContent,
                CommentCount = post.CommentCount,
                LikeCount = post.LikeCount,
                Comments = post.Comments.Select(comment => new PostCommentSimpleApiResponse
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
                        IsDeleted = comment.User.IsDeleted,
                    },

                }).ToList(),
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
                }).ToList()
            }).ToList()
        };

        return Ok(getOneUserPostsResponse);
    }

    [HttpGet("{userId:int}/followers")]
    public async Task<IActionResult> GetOneUserFollowers([FromRoute] int userId)
    {
        var user = await _dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);
        
        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        
        var followers = await _userService.GetFollowers(userId);
        
        var followersSimple = followers.Select(user => new UserFollowerSimpleApiResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Title = user.Title,
            ProfilePictureUrl = user.ProfilePictureUrl,
            IsDeleted = user.IsDeleted,
            CompanyId = user.CompanyId,
            Company = user.Company != null
                ? new UserCompanySimpleResponse
                {
                    CompanyId = user.Company.CompanyId,
                    CompanyName = user.Company.CompanyName,
                    Industry = user.Company.Industry,
                    Description = user.Company.Description,
                    EmployeeCount = user.Company.EmployeeCount,
                    WebsiteUrl = user.Company.WebsiteUrl,
                    LogoUrl = user.Company.LogoUrl,
                    FoundedAt = user.Company.FoundedAt
                }
                : null,
        }).ToList(); 

        return Ok(followersSimple);
        
    }
    
    [HttpGet("{userId:int}/followings")]
    public async Task<IActionResult> GetOneUserFollowings([FromRoute] int userId)
    {
        var user = await _dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        var followings = await _userService.GetFollowings(userId);
        
        var followingsSimple = followings.Select(user => new UserFollowerSimpleApiResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Title = user.Title,
            ProfilePictureUrl = user.ProfilePictureUrl,
            IsDeleted = user.IsDeleted,
            CompanyId = user.CompanyId,
            Company = user.Company != null
                ? new UserCompanySimpleResponse
                {
                    CompanyId = user.Company.CompanyId,
                    CompanyName = user.Company.CompanyName,
                    Industry = user.Company.Industry,
                    Description = user.Company.Description,
                    EmployeeCount = user.Company.EmployeeCount,
                    WebsiteUrl = user.Company.WebsiteUrl,
                    LogoUrl = user.Company.LogoUrl,
                    FoundedAt = user.Company.FoundedAt
                }
                : null,
        }).ToList();
        
        return Ok(followingsSimple);
    }
    
    [HttpGet("{userId:int}/getSkills")]
    public async Task<IActionResult> GetOneUsersSkills([FromRoute] int userId)
    {
        var user = await _dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        var userResponse = await _userService.GetUserSkill(userId);
        
        UserSkillSimpleApiResponse userSkillSimpleApiResponse = new UserSkillSimpleApiResponse
        {
            UserId = userResponse.UserId,
            Firstname = userResponse.Firstname,
            Lastname = userResponse.Lastname,
            Title = userResponse.Title,
            Skills = userResponse.Skills
        };
        
        return Ok(userSkillSimpleApiResponse);
    }

    [HttpPost("createUser")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateUserApiResponse))]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserApiRequest createUserApiRequest)
    {
        var user = await _userService.CreateUser(createUserApiRequest);
        
        CreateUserApiResponse createUserApiResponse = new CreateUserApiResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Title = user.Title,
            Email = user.Email,
            Age = user.Age,
            Country = user.Country,
            AboutMe = user.AboutMe,
            IsDeleted = user.IsDeleted,
            CompanyId = user.CompanyId,
            Posts = user.Posts,
            Experiences = user.Experiences,
            Educations = user.Educations,
            Skills = user.Skills,
        };
        
        return Ok(createUserApiResponse);
    }

    [HttpPatch("{userId:int}/work/{companyId:int}")]
    public async Task<IActionResult> UpdateCompany([FromRoute] int userId, [FromRoute] int companyId)
    {
        var user = await _dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);
        var company = await _dbContext.Companies.FirstOrDefaultAsync(u => u.CompanyId == companyId);
        
        if (user == null && company != null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        if (company == null && user != null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"Company not found with id({companyId})"
            };
            return Ok(problemDetailResponse);
        }

        if (user == null && company == null)
        {
            List<ProblemDetailResponse> problemDetailResponses = new List<ProblemDetailResponse>();
            
            ProblemDetailResponse problemDetailResponseUser = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            
            ProblemDetailResponse problemDetailResponseCompany = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"Company not found with id({companyId})"
            };
            
            problemDetailResponses.Add(problemDetailResponseUser);
            problemDetailResponses.Add(problemDetailResponseCompany);

            return Ok(problemDetailResponses);
        }
        
        
        var userResponse = await _userService.UpdateUserCompanyWithId(userId, companyId);
        

        UserSimpleWithCompanyApiResponse userSimpleWithCompanyApiResponse = new UserSimpleWithCompanyApiResponse
        {
            UserId = userResponse.UserId,
            Firstname = userResponse.Firstname,
            Lastname = userResponse.Lastname,
            Title = userResponse.Title,
            ProfilePictureUrl = userResponse.ProfilePictureUrl,
            AboutMe = userResponse.AboutMe,
            IsDeleted = userResponse.IsDeleted,
            CompanyId = userResponse.CompanyId,
            Company =  new UserCompanySimpleResponse
                {
                    CompanyId = company.CompanyId,
                    CompanyName = company.CompanyName,
                    Industry = company.Industry,
                    Description = company.Description,
                    EmployeeCount = company.EmployeeCount,
                    WebsiteUrl = company.WebsiteUrl,
                    LogoUrl = company.LogoUrl,
                    FoundedAt = company.FoundedAt
                }
        };

        return Ok(userSimpleWithCompanyApiResponse);
    }

    [HttpPatch("{userId:int}/addSkill/{skillId:int}")]
    public async Task<IActionResult> AddSkillToUser([FromRoute] int userId, [FromRoute] int skillId)
    {
        ProblemDetailResponse problemDetailResponse;
        
        var user = await _dbContext.Users.Include(user => user!.Skills).FirstOrDefaultAsync(u => u.UserId == userId);
        
        var skill = await _dbContext.Skills.FirstOrDefaultAsync(s => s.SkillId == skillId);
        
        if (user == null && skill != null)
        {
            problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id ({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        if (skill == null && user != null)
        {
            problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"Skill not found with id ({skillId})"
            };
            return Ok(problemDetailResponse);
        }

        if (user == null && skill == null)
        {
            List<ProblemDetailResponse> problemDetailResponses = new List<ProblemDetailResponse>();

            ProblemDetailResponse problemDetailResponseUser = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id ({userId})"
            };
            ProblemDetailResponse problemDetailResponseSkill = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"Skill not found with id ({skillId})"
            };

            problemDetailResponses.Add(problemDetailResponseUser);
            problemDetailResponses.Add(problemDetailResponseSkill);
            
            return Ok(problemDetailResponses);
        }

        var isSkillAlreadyAdded = user.Skills.Contains(skill);
        
        if (!isSkillAlreadyAdded)
        {
            user.Skills.Add(skill);
            await _dbContext.SaveChangesAsync();
        
            var userSkillAddedApiResponse = new UserSkillAddedApiResponse
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Title = user.Title,
                HashedPassword = user.HashedPassword,
                Email = user.Email,
                Age = user.Age,
                Country = user.Country,
                CurrentLanguage = user.CurrentLanguage,
                ProfilePictureUrl = user.ProfilePictureUrl,
                AboutMe = user.AboutMe,
                Skills = user.Skills
            };
            
            return Ok(userSkillAddedApiResponse);
        }
        
        problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "Already Skill Added",
            ProblemDescription = $"Skill already added to user({userId}) with skillName({skill.SkillName})"
        };

        return Ok(problemDetailResponse);


    }
    [HttpPatch("{userId:int}/removeSkill/{skillId:int}")]
    public async Task<IActionResult> RemoveSkillFromUser([FromRoute] int userId, [FromRoute] int skillId)
    {
        ProblemDetailResponse problemDetailResponse;
        
        var user = await _dbContext.Users.Include(user => user!.Skills).FirstOrDefaultAsync(u => u.UserId == userId);
        
        var skill = await _dbContext.Skills.FirstOrDefaultAsync(s => s.SkillId == skillId);
        
        if (user == null && skill != null)
        {
            problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id ({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        if (skill == null && user != null)
        {
            problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"Skill not found with id ({skillId})"
            };
            return Ok(problemDetailResponse);
        }

        if (user == null && skill == null)
        {
            List<ProblemDetailResponse> problemDetailResponses = new List<ProblemDetailResponse>();

            ProblemDetailResponse problemDetailResponseUser = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id ({userId})"
            };
            ProblemDetailResponse problemDetailResponseSkill = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"Skill not found with id ({skillId})"
            };

            problemDetailResponses.Add(problemDetailResponseUser);
            problemDetailResponses.Add(problemDetailResponseSkill);
            
            return Ok(problemDetailResponses);
        }
        
        var isThisUserHasSkill = user.Skills.Contains(skill);
        
        if (isThisUserHasSkill)
        {
            user.Skills.Remove(skill);
            await _dbContext.SaveChangesAsync();
        
            var userSkillRemovedApiResponse = new UserSkillRemovedApiResponse()
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Title = user.Title,
                HashedPassword = user.HashedPassword,
                Email = user.Email,
                Age = user.Age,
                Country = user.Country,
                CurrentLanguage = user.CurrentLanguage,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Skills = user.Skills
            };

            return Ok(userSkillRemovedApiResponse);
        }

        problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "User Does Not Have That Skill",
            ProblemDescription = $"User({user.Firstname} {user.Lastname}) doesnt have skill({skill.SkillName}))"
        };

        return Ok(problemDetailResponse);

    }

    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int userId)
    {
        var user = await _dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id ({userId})"
            };
            return Ok(problemDetailResponse);
        }

        user.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        
        return Ok(user);

    }
    
}