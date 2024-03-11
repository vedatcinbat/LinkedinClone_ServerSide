using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Services.JobService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class JobController : ControllerBase
{
    private IJobService _jobSerivce;

    public JobController(IJobService jobSerivce)
    {
        this._jobSerivce = jobSerivce;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobs()
    {
        List<GetJobsApiResponse> getJobsApiResponses = await _jobSerivce.GetAllJobs();

        return Ok(getJobsApiResponses);

    }
    
    [HttpPost]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobApiRequest createJobApiRequest)
    {
        CreateJobApiResponse createJobApiResponse = await _jobSerivce.CreateJob(createJobApiRequest);

        return Ok(createJobApiResponse);

    }
}