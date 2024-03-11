using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;

namespace JobNet.CoreApi.Services.JobService;

public interface IJobService
{
    Task<List<GetJobsApiResponse>> GetAllJobs();
    Task<CreateJobApiResponse> CreateJob(CreateJobApiRequest createJobApiRequest);
}