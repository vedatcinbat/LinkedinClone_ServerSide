using System.Security.Claims;
using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.DTOs.Response;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Models.Response.Problem;
using JobNet.CoreApi.Services.JobService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class JobController : ControllerBase
{
    private IJobService _jobService;
    private JobNetDbContext _dbContext;

    public JobController(IJobService jobService, JobNetDbContext dbContext)
    {
        this._jobService = jobService;
        this._dbContext = dbContext;
    }

    [HttpGet("getAllJobs")]
    public async Task<IActionResult> GetAllJobs()
    {
        List<Job> jobs = await _jobService.GetAllJobs();

        List<GetAllJobApiResponse> getJobsApiResponses = jobs.Select(job => new GetAllJobApiResponse
        {
            JobId = job.JobId,
            JobTitle = job.JobTitle,
            Description = job.Description,
            Location = job.Location,
            PostedAt = job.PostedAt,
            Deadline = job.Deadline,
            PublisherId = job.PublisherId,
            PublisherUser = new UserTalentManagerResponse
            {
                UserId = job.PublisherId,
                Firstname = job.PublisherUser.Firstname,
                Lastname = job.PublisherUser.Lastname,
                Title = job.PublisherUser.Title,
                Email = job.PublisherUser.Email,
                Age = job.PublisherUser.Age,
                Country = job.PublisherUser.Country,
                CurrentLanguage = job.PublisherUser.CurrentLanguage,
                ProfilePictureUrl = job.PublisherUser.ProfilePictureUrl
            },
            CompanyId = job.CompanyId,
            Company = new GetCompanyWithoutJobsAndTalentManagersApiResponse()
            {
                CompanyId = job.Company.CompanyId,
                CompanyName = job.Company.CompanyName,
                Industry = job.Company.Industry,
                Description = job.Company.Description,
                EmployeeCount = job.Company.EmployeeCount,
                WebsiteUrl = job.Company.WebsiteUrl,
                LogoUrl = job.Company.LogoUrl,
                FoundedAt = job.Company.FoundedAt,
            },
            AplliedUserCount = job.AppliedUsers.Count(),
        }).ToList();

        return Ok(getJobsApiResponses);
    }

    [HttpGet("getAllJobs/{companyId:int}")]
    public async Task<IActionResult> GetAllJobsFromOneCompany([FromRoute] int companyId)
    {
        List<Job> jobs = await _dbContext.Jobs.Where(job => job.CompanyId == companyId)
            .Include(job => job.Company)
            .ThenInclude(company => company.CurrentAvailableJobs).Include(job => job.AppliedUsers)
            .ThenInclude(user => user.Company).Include(job => job.Company)
            .ThenInclude(company => company.TalentManagers).Include(job => job.PublisherUser).ToListAsync();

        var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == companyId);
        if (company == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Company not found",
                ProblemDescription = $"Company not found companyId({companyId})"
            };
            return Ok(problemDetailResponse);
        }

        List<GetCompanyJobsApiResponse> getJobsApiResponses = jobs.Select(job => new GetCompanyJobsApiResponse
        {
            JobId = job.JobId,
            JobTitle = job.JobTitle,
            Description = job.Description,
            Location = job.Location,
            PostedAt = job.PostedAt,
            Deadline = job.Deadline,
            PublisherId = job.PublisherId,
            PublisherUser = new UserTalentManagerResponse
            {
                UserId = job.PublisherId,
                Firstname = job.PublisherUser.Firstname,
                Lastname = job.PublisherUser.Lastname,
                Title = job.PublisherUser.Title,
                Email = job.PublisherUser.Email,
                Age = job.PublisherUser.Age,
                Country = job.PublisherUser.Country,
                CurrentLanguage = job.PublisherUser.CurrentLanguage,
                ProfilePictureUrl = job.PublisherUser.ProfilePictureUrl
            },
            AppliedUserCount = job.AppliedUsers.Count()
        }).ToList();

        return Ok(getJobsApiResponses);
    }

    [HttpGet("jobs/{jobId:int}")]
    public async Task<IActionResult> GetOneJob([FromRoute] int jobId)
    {
        var job = await _jobService.GetJobById(jobId);

        if (job == null)
        {
            ProblemDetailResponse problemDetailResponseJob = new ProblemDetailResponse
            {
                ProblemTitle = "Job not found",
                ProblemDescription = $"Job not found with id({jobId})"
            };
            return Ok(problemDetailResponseJob);
        }
        
        GetJobsApiResponse getJobApiResponse = new GetJobsApiResponse {
            JobId = job.JobId,
            JobTitle = job.JobTitle,
            Description = job.Description,
            Location = job.Location,
            PostedAt = job.PostedAt,
            Deadline = job.Deadline,
            PublisherId = job.PublisherId,
            PublisherUser = new UserTalentManagerResponse
            {
                UserId = job.PublisherId,
                Firstname = job.PublisherUser.Firstname,
                Lastname = job.PublisherUser.Lastname,
                Title = job.PublisherUser.Title,
                Email = job.PublisherUser.Email,
                Age = job.PublisherUser.Age,
                Country = job.PublisherUser.Country,
                CurrentLanguage = job.PublisherUser.CurrentLanguage,
                ProfilePictureUrl = job.PublisherUser.ProfilePictureUrl
            },
            CompanyId = job.CompanyId,
            Company = job.Company != null ? new GetCompanyApiResponse
            {
                CompanyId = job.Company.CompanyId,
                CompanyName = job.Company.CompanyName,
                Industry = job.Company.Industry,
                Description = job.Company.Description,
                EmployeeCount = job.Company.EmployeeCount,
                WebsiteUrl = job.Company.WebsiteUrl,
                LogoUrl = job.Company.LogoUrl,
                FoundedAt = job.Company.FoundedAt,
                CurrentAvailableJobs = job.Company.CurrentAvailableJobs.Select(otherJob => new JobDto
                {
                    JobId = otherJob.JobId,
                    JobTitle = otherJob.JobTitle,
                    Description = otherJob.Description,
                    Location = otherJob.Location,
                    PostedAt = otherJob.PostedAt,
                    Deadline = otherJob.Deadline
                })
            } : null,
            AppliedUsers = job.AppliedUsers.Select(user => new UserSimpleWithSimpleCompanyResponse
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Title = user.Title,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsDeleted = user.IsDeleted,
                CompanyId = user.Company.CompanyId,
                Company = user.Company != null ? new UserLikeCompanySimpleResponse
                {
                    CompanyName = user.Company.CompanyName
                } : null,
            }).ToList()
        };

        return Ok(getJobApiResponse);
    }

    [HttpPost("addJob")]
    [Authorize]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobApiRequest createJobApiRequest)
    {
        // Who is requesting this ? Find the user
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var userId = Convert.ToInt32(userIdClaim.Value);

            var user = await _dbContext.Users.Where(user => user.IsDeleted == false)
                .FirstOrDefaultAsync(user => user.UserId == userId);

            var company =
                await _dbContext.Companies.FirstOrDefaultAsync(company =>
                    company.CompanyId == createJobApiRequest.CompanyId);

            if (user == null)
            {
                ProblemDetailResponse problemDetailResponse2 = new ProblemDetailResponse
                {
                    ProblemTitle = "User not found",
                    ProblemDescription = $"User not found with id({userId})"
                };
                return Ok(problemDetailResponse2);
            }

            if (company == null)
            {
                ProblemDetailResponse problemDetailResponse3 = new ProblemDetailResponse
                {
                    ProblemTitle = "Company not found",
                    ProblemDescription = $"Company not found with id({createJobApiRequest.CompanyId})"
                };
                return Ok(problemDetailResponse3);
            }

            var isUserTalentManagersOfThatCompany = company.TalentManagers.Contains(user);

            if (!isUserTalentManagersOfThatCompany)
            {
                ProblemDetailResponse problemDetailResponse4 = new ProblemDetailResponse
                {
                    ProblemTitle = "User is not allowed",
                    ProblemDescription = $"User({userId}) cannot add job for company ({company.CompanyId})-({company.CompanyName})"
                };
                return Ok(problemDetailResponse4);
            }

            int jobId = await _dbContext.Jobs.CountAsync() + 1;
            Job newJob = new Job
            {
                JobId = jobId,
                JobTitle = createJobApiRequest.JobTitle,
                Description = createJobApiRequest.Description,
                JobType = createJobApiRequest.JobType,
                JobEmployeeLevel = createJobApiRequest.JobEmployeeLevel,
                Location = createJobApiRequest.Location,
                PostedAt = createJobApiRequest.PostedAt,
                Deadline = createJobApiRequest.Deadline,
                CompanyId = createJobApiRequest.CompanyId,
                Company = company,
                PublisherId = userId,
                PublisherUser = user,
                AppliedUsers = new List<User>(),
            };

            await _dbContext.Jobs.AddAsync(newJob);
            company.CurrentAvailableJobs?.Add(newJob);
            await _dbContext.SaveChangesAsync();

            CreateJobApiResponse createJobApiResponse = new CreateJobApiResponse
            {
                JobId = newJob.JobId,
                JobTitle = newJob.JobTitle,
                Description = newJob.Description,
                JobType = newJob.JobType,
                JobEmployeeLevel = newJob.JobEmployeeLevel,
                Location = newJob.Location,
                PostedAt = newJob.PostedAt,
                Deadline = newJob.Deadline,
                PublisherId = newJob.PublisherId,
                PublisherUser = new UserTalentManagerResponse
                {
                    UserId = newJob.PublisherUser.UserId,
                    Firstname = newJob.PublisherUser.Firstname,
                    Lastname = newJob.PublisherUser.Lastname,
                    Title = newJob.PublisherUser.Title,
                    Email = newJob.PublisherUser.Email,
                    Age = newJob.PublisherUser.Age,
                    Country = newJob.PublisherUser.Country,
                    CurrentLanguage = newJob.PublisherUser.CurrentLanguage,
                    ProfilePictureUrl = newJob.PublisherUser.ProfilePictureUrl
                },
                CompanyId = newJob.Company.CompanyId,
                Company = new UserCompanySimpleResponse
                {
                    CompanyId = newJob.Company.CompanyId,
                    CompanyName = newJob.Company.CompanyName,
                    Industry = newJob.Company.Industry,
                    Description = newJob.Company.Description,
                    EmployeeCount = newJob.Company.EmployeeCount,
                    WebsiteUrl = newJob.Company.WebsiteUrl,
                    LogoUrl = newJob.Company.LogoUrl,
                    FoundedAt = newJob.Company.FoundedAt
                },
                UserJobLikeResponses = newJob.AppliedUsers.Select(appliedUser => new UserSimpleWithSimpleCompanyResponse
                {
                    UserId = appliedUser.UserId,
                    Firstname = appliedUser.Firstname,
                    Lastname = appliedUser.Lastname,
                    Title = appliedUser.Title,
                    ProfilePictureUrl = appliedUser.ProfilePictureUrl,
                    IsDeleted = appliedUser.IsDeleted,
                    CompanyId = appliedUser.Company?.CompanyId,
                    Company = new UserLikeCompanySimpleResponse
                    {
                        CompanyName = appliedUser.Company.CompanyName
                    }
                }).ToList()
            };

            return Ok(createJobApiResponse);

        }

        ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authenticate first !"
        };
        return Ok(problemDetailResponse);
    }

    [HttpPatch("{userId:int}/applyJob/{jobId:int}")]
    [Authorize]
    public async Task<IActionResult> ApplyJobWithUserId([FromRoute] int userId, [FromRoute] int jobId)
    {

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var currentUserId = Convert.ToInt32(userIdClaim.Value);

            if (currentUserId != userId)
            {
                ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
                {
                    ProblemTitle = "User has no permission",
                    ProblemDescription = $"You User({currentUserId}) cant apply job as User({userId})"
                };

                return Ok(problemDetailResponse);
            }
            
            var job = await _jobService.ApplyJobWithUserIdAndJobId(userId, jobId);

            if (job != null)
            {
                GetJobsApiResponse getJobsApiResponse = new GetJobsApiResponse()
                {
                    JobId = job.JobId,
                    JobTitle = job.JobTitle,
                    Description = job.Description,
                    Location = job.Location,
                    PostedAt = job.PostedAt,
                    Deadline = job.Deadline,
                    CompanyId = job.CompanyId,
                    Company = new GetCompanyApiResponse
                    {
                        CompanyId = job.Company.CompanyId,
                        CompanyName = job.Company.CompanyName,
                        Industry = job.Company.Industry,
                        Description = job.Company.Description,
                        EmployeeCount = job.Company.EmployeeCount,
                        WebsiteUrl = job.Company.WebsiteUrl,
                        LogoUrl = job.Company.LogoUrl,
                        FoundedAt = job.Company.FoundedAt,
                        CurrentAvailableJobs = job.Company.CurrentAvailableJobs.Select(job => new JobDto
                        {
                            JobId = job.JobId,
                            JobTitle = job.JobTitle,
                            Description = job.Description,
                            Location = job.Location,
                            PostedAt = job.PostedAt,
                            Deadline = job.Deadline
                        })
                    },
                    AppliedUsers = job.AppliedUsers.Select(user => new UserSimpleWithSimpleCompanyResponse
                    {
                        UserId = user.UserId,
                        Firstname = user.Firstname,
                        Lastname = user.Lastname,
                        Title = user.Title,
                        ProfilePictureUrl = user.ProfilePictureUrl,
                        IsDeleted = user.IsDeleted,
                        CompanyId = user.Company.CompanyId,
                        Company = user.Company != null ? new UserLikeCompanySimpleResponse
                        {
                            CompanyName = user.Company.CompanyName
                        } : null,
                    }).ToList()
                };
                return Ok(getJobsApiResponse);
            }
            
            ProblemDetailResponse problemDetailResponseNotFoundJob = new ProblemDetailResponse
            {
                ProblemTitle = "Job not found",
                ProblemDescription = $"Job({jobId}) Not Found!"
            };

            return Ok(problemDetailResponseNotFoundJob);
        }

        ProblemDetailResponse problemDetailResponseNotFound = new ProblemDetailResponse
        {
            ProblemTitle = "User not found",
            ProblemDescription = $"You have to authorize first!"
        };

        return Ok(problemDetailResponseNotFound);
    }

}

