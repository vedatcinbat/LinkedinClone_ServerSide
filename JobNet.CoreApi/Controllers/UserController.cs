using System.Security.Claims;
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
public class UserController(IUserService userService, JobNetDbContext dbContext) : ControllerBase
{
    [HttpGet("allUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllUsers();

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
        var users = await userService.GetAllUsersActive();
        
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
        var user = await userService.GetOneUserProfileDetails(userId);
        
        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }

        
        int followerCount = await dbContext.Follows.Where(f => f.IsDeleted == false && f.FollowingId == userId).CountAsync();

        int followingCount = await dbContext.Follows.Where(f => f.IsDeleted == false && f.FollowerId == userId).CountAsync();
        
        
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
            Experiences = user.Experiences.Select(experience => new ExperienceSimpleWithoutUserApiResponse
            {
                ExperienceId = experience.ExperienceId,
                Title = experience.Title,
                CompanyName = experience.CompanyName,
                Location = experience.Location,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                Description = experience.Description,
                CompanyId = experience.CompanyId,
                Company = new UserCompanySimpleResponse
                {
                    CompanyId = experience.Company.CompanyId,
                    CompanyName = experience.Company.CompanyName,
                    Industry = experience.Company.Industry,
                    Description = experience.Company.Description,
                    EmployeeCount = experience.Company.EmployeeCount,
                    WebsiteUrl = experience.Company.WebsiteUrl,
                    LogoUrl = experience.Company.LogoUrl,
                    FoundedAt = experience.Company.FoundedAt
                }
            }).ToList(),
            Educations = user.Educations.Select(education => new UserEducationResponseWithoutUserResponse
            {
                Degree = education.Degree,
                FieldOfStudy = education.FieldOfStudy,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                SchoolId = education.SchoolId,
                School = new GetAllSchoolsResponse
                {
                    SchoolId = education.School.SchoolId,
                    SchoolName = education.School.SchoolName,
                    Location = education.School.Location,
                    EstablishedAt = education.School.EstablishedAt,
                    GraduatesCount = education.School.Graduates.Count()
                }
            }).ToList(),
            Skills = user.Skills,

        };
        
        return Ok(userProfileApiResponse);
    }
    
    [HttpGet("{userId:int}/simple")]
    public async Task<IActionResult> GetOneUserSimpleDetails([FromRoute] int userId)
    {
        var user = await userService.GetOneUserSimpleDetails(userId);

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
        var user = await userService.GetOneUserPostsWithUserId(userId);

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
            Posts = user.Posts.Where(post => post.IsDeleted == false).Select(post => new PostSimpleApiResponseWithoutUser()
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
                Comments = post.Comments.Where(c => c.IsDeleted == false && c.User.IsDeleted == false).Select(comment => new PostCommentSimpleApiResponse
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
                Likes = post.Likes.Where(like => like.IsDeleted == false && like.User.IsDeleted == false).Select(like => new LikeSimpleResponse
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
                        Company = like.User.Company != null ? new UserPostCompanySimpleResponse
                        {
                            CompanyId = like.User.Company.CompanyId,
                            CompanyName = like.User.Company.CompanyName,
                            Industry = like.User.Company.Industry,
                            LogoUrl = like.User.Company.LogoUrl
                        } : null,
                    }
                }).ToList()
            }).ToList()
        };

        return Ok(getOneUserPostsResponse);
    }

    [HttpGet("{userId:int}/followers")]
    public async Task<IActionResult> GetOneUserFollowers([FromRoute] int userId)
    {
        var user = await dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);
        
        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        
        var followers = await userService.GetFollowers(userId);
        
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
        var user = await dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        var followings = await userService.GetFollowings(userId);
        
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
        var user = await dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }
        
        var userResponse = await userService.GetUserSkill(userId);
        
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

    [HttpGet("getConnectionPosts")]
    public async Task<IActionResult> GetCurrentUserConnectionsPosts()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        
        if (userIdClaim != null)
        {
            var userId = Convert.ToInt32(userIdClaim.Value);

            var posts = await userService.GetUserConnectionsPosts(userId);

            var postsResponse = posts.Select(post => new UserConnectionsPostsResponse()
            {
                PostId = post.PostId,
                IsDeleted = post.IsDeleted,
                UserId = post.User.UserId,
                User = new UserPostSimpleResponse
                {
                    UserId = post.User.UserId,
                    Firstname = post.User.Firstname,
                    Lastname = post.User.Lastname,
                    Title = post.User.Title,
                    ProfilePictureUrl = post.User.ProfilePictureUrl == null ? null : post.User.ProfilePictureUrl,
                    IsDeleted = post.User.IsDeleted,
                    CompanyId = post.User.CompanyId,
                    Company = post.User.Company != null ? new UserPostCompanySimpleResponse
                    {
                        CompanyId = post.User.Company.CompanyId,
                        CompanyName = post.User.Company.CompanyName,
                        Industry = post.User.Company.Industry,
                        LogoUrl = post.User.Company.LogoUrl
                    } : null
                },
                PublishTime = post.PublishTime,
                Caption = post.Caption,
                PostType = post.PostType,
                TextContent = post.TextContent,
                ImageContent = post.ImageContent,
                ImagesContent = post.ImagesContent,
                CommentCount = post.CommentCount,
                LikeCount = post.LikeCount,
            });

            return Ok(postsResponse);
        }

        ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "Authentication Error",
            ProblemDescription = "You have to authenticate first !"
        };
        
        return BadRequest(problemDetailResponse);
    }

    [HttpPost("createUser")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateUserApiResponse))]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserApiRequest createUserApiRequest)
    {
        var emails = await dbContext.Users.Select(u => u.Email).ToListAsync();

        if (emails.Contains(createUserApiRequest.Email))
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "You have already an account",
                ProblemDescription = "Email already exists in the system !"
            };
            return BadRequest(problemDetailResponse);
        }
        
        var user = await userService.CreateUser(createUserApiRequest);
        
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

    [HttpPatch("work/{companyId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateCompany([FromRoute] int companyId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var userId = Convert.ToInt32(userIdClaim.Value);
            
            var user = await dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);
            
        var company = await dbContext.Companies.FirstOrDefaultAsync(u => u.CompanyId == companyId);
        
        if (user == null && company != null)
        {
            ProblemDetailResponse problemDetailResponse1 = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse1);
        }
        
        if (company == null && user != null)
        {
            ProblemDetailResponse problemDetailResponse2 = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"Company not found with id({companyId})"
            };
            return Ok(problemDetailResponse2);
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
        
        
        var userResponse = await userService.UpdateUserCompanyWithId(userId, companyId);
        

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
            Company =  userResponse.Company != null ? new UserCompanySimpleResponse
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                Industry = company.Industry,
                Description = company.Description,
                EmployeeCount = company.EmployeeCount,
                WebsiteUrl = company.WebsiteUrl,
                LogoUrl = company.LogoUrl,
                FoundedAt = company.FoundedAt
            } : null,
        };

        return Ok(userSimpleWithCompanyApiResponse);
            
        }
        
        ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse);
        
    }

    [HttpPatch("addSkill/{skillId:int}")]
    public async Task<IActionResult> AddSkillToUser([FromRoute] int skillId)
    {
        
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            ProblemDetailResponse problemDetailResponse;
            
            var userId = Convert.ToInt32(userIdClaim.Value);
        
        var user = await dbContext.Users.Include(user => user!.Skills).FirstOrDefaultAsync(u => u.UserId == userId);
        
        var skill = await dbContext.Skills.FirstOrDefaultAsync(s => s.SkillId == skillId);
        
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
            await dbContext.SaveChangesAsync();
        
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
        
        ProblemDetailResponse problemDetailResponse2 = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse2);


    }

    [HttpPost("{userId:int}/experience/{companyId:int}")]
    public async Task<IActionResult> CreateExperience([FromRoute] int userId, [FromRoute] int companyId, CreateExperienceApiRequest createExperienceApiRequest)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var currentUserId = Convert.ToInt32(userIdClaim.Value);

            if (currentUserId != userId)
            {
                ProblemDetailResponse problemDetailResponsePermission = new ProblemDetailResponse
                {
                    ProblemTitle = "User has no permission",
                    ProblemDescription =
                        $"User({currentUserId}) has no permission to create experience for user({userId})"
                };
                return Ok(problemDetailResponsePermission);
            }

            var user = await dbContext.Users.Where(u => u.IsDeleted == false).Include(user => user.Company)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            var company = await dbContext.Companies.FirstOrDefaultAsync(company => company.CompanyId == companyId);

            if (user == null)
            {
                ProblemDetailResponse problemDetailResponseNoUser = new ProblemDetailResponse
                {
                    ProblemTitle = "User not found",
                    ProblemDescription = $"User not found with this id {userId}"
                };
                return Ok(problemDetailResponseNoUser);
            }


            if (company == null)
            {
                ProblemDetailResponse problemDetailResponseNoCompany = new ProblemDetailResponse
                {
                    ProblemTitle = "Company not found",
                    ProblemDescription = $"Company not found with this id {companyId}"
                };
                return Ok(problemDetailResponseNoCompany);
            }

            var experienceId = await dbContext.Experiences.CountAsync() + 1;

            Experience newExperience = new Experience
            {
                ExperienceId = experienceId,
                Title = createExperienceApiRequest.Title,
                CompanyName = company.CompanyName,
                Location = createExperienceApiRequest.Title,
                StartDate = createExperienceApiRequest.StartDate,
                EndDate = createExperienceApiRequest.EndDate,
                Description = createExperienceApiRequest.Description,
                UserId = user.UserId,
                User = user,
                CompanyId = company.CompanyId,
                Company = company
            };

            await dbContext.Experiences.AddAsync(newExperience);
            await dbContext.SaveChangesAsync();

            var experienceSimpleResponse = new ExperienceSimpleApiResponse
            {
                ExperienceId = newExperience.ExperienceId,
                Title = newExperience.Title,
                CompanyName = newExperience.CompanyName,
                Location = newExperience.Location,
                StartDate = newExperience.StartDate,
                EndDate = newExperience.EndDate,
                Description = newExperience.Description,
                UserId = newExperience.UserId,
                User = new UserSimpleApiResponseWithSimpleCompanyResponse
                {
                    UserId = newExperience.User.UserId,
                    Firstname = newExperience.User.Firstname,
                    Lastname = newExperience.User.Lastname,
                    Title = newExperience.User.Title,
                    ProfilePictureUrl = newExperience.User.ProfilePictureUrl,
                    AboutMe = newExperience.User.AboutMe,
                    IsDeleted = newExperience.User.IsDeleted,
                    CompanyId = newExperience.User.CompanyId,
                    Company = new CompanyWithCompanyNameResponse
                    {
                        CompanyName = newExperience.User.Company.CompanyName
                    }
                },
                CompanyId = newExperience.CompanyId,
                Company = new CompanyWithCompanyNameResponse
                {
                    CompanyName = newExperience.Company.CompanyName
                }
            };
            
            return Ok(experienceSimpleResponse);

        }


        ProblemDetailResponse problemDetailResponseAuth = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponseAuth);
    }

    [HttpPatch("{userId:int}/updateAboutMe")]
    [Authorize]
    public async Task<IActionResult> UpdateUserAboutMe([FromRoute] int userId, [FromBody] UpdateUserAboutMeApiRequest updateUserAboutMeApiRequest)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var currentUserId = Convert.ToInt32(userIdClaim.Value);

            if (currentUserId != userId)
            {
                ProblemDetailResponse problemDetailResponsePermission = new ProblemDetailResponse
                {
                    ProblemTitle = "User has no permission",
                    ProblemDescription =
                        $"User({currentUserId}) has no permission to update about me for user({userId})"
                };
                return Ok(problemDetailResponsePermission);
            }

            var user = await dbContext.Users.Where(u => u.IsDeleted == false).Include(u => u.Company).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "User not found",
                    ProblemDescription = $"User not found with id({userId})"
                };
                return Ok(problemDetailResponse);
            }

            user.AboutMe = updateUserAboutMeApiRequest.AboutMe;
            await dbContext.SaveChangesAsync();

            var userSimpleApiResponse = new UserSimpleApiResponse
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

        ProblemDetailResponse problemDetailResponse2 = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse2);
    }

    [HttpPatch("{userId:int}/updateTitle/{updateTitle}")]
    public async Task<IActionResult> UpdateUserTitle([FromRoute] int userId, [FromRoute] string updateTitle)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var currentUserId = Convert.ToInt32(userIdClaim.Value);

            if (currentUserId != userId)
            {
                ProblemDetailResponse problemDetailResponsePermission = new ProblemDetailResponse
                {
                    ProblemTitle = "User has no permission",
                    ProblemDescription =
                        $"User({currentUserId}) has no permission to update about me for user({userId})"
                };
                return Ok(problemDetailResponsePermission);
            }
            
            var user = await dbContext.Users.Where(u => u.IsDeleted == false).Include(u => u.Company).FirstOrDefaultAsync(u => u.UserId == userId);
            
            if (user == null)
            {
                ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "User not found",
                    ProblemDescription = $"User not found with id({userId})"
                };
                return Ok(problemDetailResponse);
            }
            
            user.Title = updateTitle;
            await dbContext.SaveChangesAsync();
            
            var userSimpleApiResponse = new UserSimpleApiResponse
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
        
        ProblemDetailResponse problemDetailResponse2 = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse2);
        
    }

    [HttpPatch("removeSkill/{skillId:int}")]
    public async Task<IActionResult> RemoveSkillFromUser([FromRoute] int skillId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var userId = Convert.ToInt32(userIdClaim.Value);
            
            ProblemDetailResponse problemDetailResponse;
        
        var user = await dbContext.Users.Include(user => user!.Skills).FirstOrDefaultAsync(u => u.UserId == userId);
        
        var skill = await dbContext.Skills.FirstOrDefaultAsync(s => s.SkillId == skillId);
        
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
            await dbContext.SaveChangesAsync();
        
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

        ProblemDetailResponse problemDetailResponse2 = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse2);

    }

    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int userId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var requestUserId = Convert.ToInt32(userIdClaim.Value);

            if (requestUserId == userId)
            {
                var user = await dbContext.Users.Where(u => u.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
                    {
                        ProblemTitle = "Not Found",
                        ProblemDescription = $"User not found with id ({userId})"
                    };
                    return Ok(problemDetailResponse);
                }

                
                List<Like> userLikeRecords = await dbContext.Likes.Where(l => l.IsDeleted == false && l.UserId == userId).ToListAsync();
                List<Comment> userComments = await dbContext.Comments
                    .Where(c => c.IsDeleted == false && c.UserId == userId).ToListAsync();
                List<Follow> followRecords = await dbContext.Follows.Where(f =>
                    f.IsDeleted == false && (f.FollowerId == userId || f.FollowingId == userId)).ToListAsync();
                
                foreach (var comment in userComments)
                {
                    comment.IsDeleted = true;
                }
                foreach (var like in userLikeRecords)
                {
                    like.IsDeleted = true;
                }
                foreach (var follow in followRecords)
                {
                    follow.IsDeleted = true;
                }
                
                user.IsDeleted = true;
                await dbContext.SaveChangesAsync();
        
                return Ok(user);
            }
            
            ProblemDetailResponse problemDetailResponse3 = new ProblemDetailResponse
            {
                ProblemTitle = "User has no permission",
                ProblemDescription = $"User({requestUserId}) can not delete user({userId}) "
            };
            return Ok(problemDetailResponse3);
        }

        ProblemDetailResponse problemDetailResponse2 = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse2);

    }

    [HttpPatch("saveAccount")]
    public async Task<IActionResult> ReCreateAccount(SaveAccountApiRequest saveAccountApiRequest)
    {
        var user = await userService.SaveAccount(saveAccountApiRequest.Email, saveAccountApiRequest.Password);

        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = $"User Record Could Not Fonud",
                ProblemDescription = $"User Record Could Not Found Email({saveAccountApiRequest.Email})"
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
    
}