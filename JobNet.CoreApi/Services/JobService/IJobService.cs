using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;

namespace JobNet.CoreApi.Services.JobService;

public interface IJobService
{
    Task<List<Job>> GetAllJobs();
}