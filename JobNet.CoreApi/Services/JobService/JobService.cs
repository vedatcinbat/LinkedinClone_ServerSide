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

    public async Task<List<Job>> GetAllJobs()
    {
        List<Job> jobs = await _dbContext.Jobs
            .Include(job => job.PublisherUser)
            .Include(job => job.Company)
            .ThenInclude(c => c.TalentManagers)
            .ToListAsync();
        
        return jobs;
    }
    
}