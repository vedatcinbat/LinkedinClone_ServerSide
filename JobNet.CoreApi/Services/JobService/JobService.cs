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
        _dbContext = dbContext;
    }

    public async Task<List<Job>> GetAllJobs()
    {
        List<Job> jobs = await _dbContext.Jobs
            .Include(job => job.PublisherUser)
            .Include(job => job.Company)
            .ThenInclude(c => c.TalentManagers)
            .Include(job => job.AppliedUsers)
            .ThenInclude(user => user.Company)
            .ToListAsync();

        return jobs;
    }

    public async Task<Job?> ApplyJobWithUserIdAndJobId(int userId, int jobId)
    {
        var job = await _dbContext.Jobs
            .Include(job => job.PublisherUser)
            .Include(job => job.Company)
            .ThenInclude(c => c.TalentManagers)
            .Include(job => job.AppliedUsers)
            .ThenInclude(user => user.Company)
            .FirstOrDefaultAsync(job => job.JobId == jobId);

        var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserId == userId);

        if (job == null)
        {
            throw new Exception($"Job with id {jobId} not found");
        }

        if(user == null) {
            throw new Exception($"User with id {userId} not found");
        }

        bool isAlreadyApplied = job.AppliedUsers.Contains(user);

        if(isAlreadyApplied) {
            throw new Exception($"User with id {userId} already applied to job with id {jobId}");
        }

        job.AppliedUsers.Add(user);
        await _dbContext.SaveChangesAsync();

        return job;
    }

    public async Task<Job?> GetJobById(int jobId)
    {
        var job = await _dbContext.Jobs
            .Include(job => job.Company)
            .ThenInclude(company => company.CurrentAvailableJobs).Include(job => job.AppliedUsers)
            .ThenInclude(user => user.Company).Include(job => job.Company)
            .ThenInclude(company => company.TalentManagers).FirstOrDefaultAsync(j => j.JobId == jobId);

        return job;
            
    }
}