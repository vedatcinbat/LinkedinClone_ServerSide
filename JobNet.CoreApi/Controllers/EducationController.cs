using System.Security.Claims;
using JobNet.CoreApi.Data;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Models.Response.Problem;
using JobNet.CoreApi.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EducationController(JobNetDbContext dbContext, IUserService userService) : ControllerBase
{
    [HttpGet("allEducations")]
    public async Task<IActionResult> GetAllEducations()
    {
        var allEducations = await dbContext.Educations.Include(education => education.User)
            .ThenInclude(user => user.Company).Include(education => education.School).ToListAsync();

        var allEducationsResponse = allEducations.Select(education => new GetEducationsSimpleResponse
        {
            Degree = education.Degree,
            FieldOfStudy = education.FieldOfStudy,
            StartDate = education.StartDate,
            EndDate = education.EndDate,
            UserId = education.UserId,
            User = new UserSimpleApiResponseWithSimpleCompanyResponse
            {
                UserId = education.User.UserId,
                Firstname = education.User.Firstname,
                Lastname = education.User.Lastname,
                Title = education.User.Title,
                ProfilePictureUrl = education.User.ProfilePictureUrl,
                AboutMe = education.User.AboutMe,
                IsDeleted = education.User.IsDeleted,
                CompanyId = education.User.CompanyId,
                Company = education.User.Company != null ? new CompanyWithCompanyNameResponse
                {
                    CompanyName = education.User.Company.CompanyName
                } : null,
            },
            SchoolId = education.SchoolId,
            School = new GetAllSchoolsResponse
            {
                SchoolId = education.School.SchoolId,
                SchoolName = education.School.SchoolName,
                Location = education.School.Location,
                EstablishedAt = education.School.EstablishedAt,
                GraduatesCount = education.School.Graduates.Count
            }
        });


        return Ok(allEducationsResponse);
    }
    
    [HttpGet("{userId:int}/getEducations")]
    public async Task<IActionResult> GetOneUserEducations([FromRoute] int userId)
    {
        var user = await dbContext.Users
            .Where(u => u.IsDeleted == false)
            .Include(user => user.Educations)
            .ThenInclude(education => education.School)
            .ThenInclude(school => school.Graduates)
            .FirstOrDefaultAsync(u => u.UserId == userId);
        
        if (user == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Not Found",
                ProblemDescription = $"User not found with id({userId})"
            };
            return Ok(problemDetailResponse);
        }

        var userWithEducationsResponse = new GetUserEducationsResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Title = user.Title,
            ProfilePictureUrl = user.ProfilePictureUrl,
            AboutMe = user.AboutMe,
            Education = user.Educations.Select(education => new UserEducationResponseWithoutUserResponse
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
                },
            }).ToList()
        };
        
        return Ok(userWithEducationsResponse);

    }
    
    [HttpPost("{userId:int}/education/{schoolId:int}")]
    public async Task<IActionResult> UpdateUserEducation([FromRoute] int userId,[FromRoute] int schoolId, [FromBody] UserEducationApiRequest userEducationApiRequest)
    {
        var currentUserIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (currentUserIdClaim != null)
        {
            var currentUserId = Convert.ToInt32(currentUserIdClaim.Value);

            var user = await dbContext.Users.Where(u => u.IsDeleted == false).Include(user => user.Educations)
                .ThenInclude(education => education.School)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (currentUserId != userId)
            {
                ProblemDetailResponse problemDetailResponseUserHasNoPermission = new ProblemDetailResponse
                {
                    ProblemTitle = "User has no permission",
                    ProblemDescription = $"User({currentUserId}) has no permission to update user({userId}) profile"
                };
                return Ok(problemDetailResponseUserHasNoPermission);
            }
            
            if (user == null)
            {
                ProblemDetailResponse problemDetailResponseUserNull = new ProblemDetailResponse
                {
                    ProblemTitle = "User not found",
                    ProblemDescription = $"User not found with id({userId})"
                };
                return Ok(problemDetailResponseUserNull);
            }

            var school = await dbContext.Schools.FirstOrDefaultAsync(s => s.SchoolId == schoolId);

            if (school == null)
            {
                ProblemDetailResponse problemDetailResponseSchoolNotFound = new ProblemDetailResponse
                {
                    ProblemTitle = "School not found",
                    ProblemDescription = $"School not found with id({school})"
                };
                return Ok(problemDetailResponseSchoolNotFound);
            }
            
            var userResponse = await userService.AddSchoolToUser(user, school, userEducationApiRequest);

            GetUserWithEducationResponse getUserWithEducationResponse = new GetUserWithEducationResponse
            {
                UserId = userResponse.UserId,
                Firstname = userResponse.Firstname,
                Lastname = userResponse.Lastname,
                Title = userResponse.Title,
                ProfilePictureUrl = userResponse.ProfilePictureUrl,
                AboutMe = userResponse.AboutMe,
                IsDeleted = userResponse.IsDeleted,
                Company = userResponse.Company != null ? new GetCompanyForUserSchoolResponse
                {
                    CompanyId = userResponse.Company.CompanyId,
                    CompanyName = userResponse.Company.CompanyName,
                    Industry = userResponse.Company.Industry,
                    Description = userResponse.Company.Description,
                    WebsiteUrl = userResponse.Company.WebsiteUrl,
                    LogoUrl = userResponse.Company.LogoUrl,
                    FoundedAt = userResponse.Company.FoundedAt
                } : null,
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
                }).ToList()
            };

            return Ok(getUserWithEducationResponse);


        }
        
        ProblemDetailResponse problemDetailResponseNotAuthenticated = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponseNotAuthenticated);
        
    }
}