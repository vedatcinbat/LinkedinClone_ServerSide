using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.DTOs.Response;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Services.JobService;

public class JobService : IJobService
{
    private readonly JobNetDbContext _dbContext;

    public JobService(JobNetDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<List<GetJobsApiResponse>> GetAllJobs()
    {
        List<Job> jobs = await _dbContext.Jobs.Include("Company").Include("UserJobLikes").ToListAsync();
        List<GetJobsApiResponse> getJobsApiResponses = jobs.Select(job => new GetJobsApiResponse
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
            UserJobLikes = job.UserJobLikes
        }).ToList();
        return getJobsApiResponses;
    }

    [HttpPost]
    public async Task<CreateJobApiResponse> CreateJob(CreateJobApiRequest createJobApiRequest)
    {
        Job newJob = new Job
        {
            JobId = createJobApiRequest.JobId,
            JobTitle = createJobApiRequest.JobTitle,
            Description = createJobApiRequest.Description,
            JobType = createJobApiRequest.JobType,
            JobEmployeeLevel = createJobApiRequest.JobEmployeeLevel,
            Location = createJobApiRequest.Location,
            PostedAt = createJobApiRequest.PostedAt,
            Deadline = createJobApiRequest.Deadline,
            CompanyId = createJobApiRequest.CompanyId,
        };
        
        await _dbContext.Jobs.AddAsync(newJob);
        await _dbContext.SaveChangesAsync();
        
        CreateJobApiResponse createJobApiResponse = new CreateJobApiResponse
        {
            JobId = createJobApiRequest.JobId,
            JobTitle = createJobApiRequest.JobTitle,
            Description = createJobApiRequest.Description,
            JobType = createJobApiRequest.JobType,
            JobEmployeeLevel = createJobApiRequest.JobEmployeeLevel,
            Location = createJobApiRequest.Location,
            PostedAt = createJobApiRequest.PostedAt,
            Deadline = createJobApiRequest.Deadline,
            CompanyId = createJobApiRequest.CompanyId,
        };
        return createJobApiResponse;
    }
}