using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.DTOs.Response;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Models.Response.Problem;
using JobNet.CoreApi.Services;
using JobNet.CoreApi.Services.UserService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CompanyController : ControllerBase
{
    private ICompanyService _companyService;
    private readonly JobNetDbContext _dbContext;
    
    public CompanyController(ICompanyService companyService, JobNetDbContext dbContext)
    {
        this._companyService = companyService;
        this._dbContext = dbContext;
    }   

    [HttpGet]
    public async Task<IActionResult> GetAllCompanies([FromQuery] string? companyName)
    {
        var companies = await _companyService.GetAllCompanies(companyName);
        
        List<GetCompanyApiResponse> getCompanyApiResponses = companies.Select(company => new GetCompanyApiResponse
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            Industry = company.Industry,
            Description = company.Description,
            EmployeeCount = company.EmployeeCount,
            WebsiteUrl = company.WebsiteUrl,
            LogoUrl = company.LogoUrl,
            FoundedAt = company.FoundedAt,
            CurrentAvailableJobs = company.CurrentAvailableJobs != null ? company.CurrentAvailableJobs.Select(job => new JobDto
            {
                JobId = job.JobId,
                JobTitle = job.JobTitle,
                JobType = job.JobType,
                JobEmployeeLevel = job.JobEmployeeLevel,
                Description = job.Description,
                Location = job.Location,
                PostedAt = job.PostedAt,
                Deadline = job.Deadline
            }) : null,
            TalentManagers = company.TalentManagers?.Select(user => new UserTalentManagerResponse
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Title = user.Title,
                Email = user.Email,
                Age = user.Age,
                Country = user.Country,
                CurrentLanguage = user.CurrentLanguage,
                ProfilePictureUrl = user.ProfilePictureUrl
            }),
        }).ToList();
        
        return Ok(getCompanyApiResponses);
    }

    [HttpGet("{companyId:int}")]
    public async Task<IActionResult> GetOneCompany([FromRoute] int companyId)
    {
        var company = await _dbContext.Companies
            .Include(c => c.TalentManagers)
            .Include(c => c.CurrentAvailableJobs)
            .FirstOrDefaultAsync(c => c.CompanyId == companyId);
            
        if (company == null)
        {
            var problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Company Not Found",
                ProblemDescription = $"Company Not Found with CompanyId: ({companyId})"
            };

            return BadRequest(problemDetailResponse);
        }

        GetCompanyApiResponse getCompanyApiResponse = new GetCompanyApiResponse
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            Industry = company.Industry,
            Description = company.Description,
            EmployeeCount = company.EmployeeCount,
            WebsiteUrl = company.WebsiteUrl,
            LogoUrl = company.LogoUrl,
            FoundedAt = company.FoundedAt,
            CurrentAvailableJobs = company.CurrentAvailableJobs.Select(job => new JobDto
                {
                    JobId = job.JobId,
                    JobTitle = job.JobTitle,
                    JobType = job.JobType,
                    JobEmployeeLevel = job.JobEmployeeLevel,
                    Description = job.Description,
                    Location = job.Location,
                    PostedAt = job.PostedAt,
                    Deadline = job.Deadline
                }),
            TalentManagers = company.TalentManagers.Select(user => new UserTalentManagerResponse
                {
                    UserId = user.UserId,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Title = user.Title,
                    Email = user.Email,
                    Age = user.Age,
                    Country = user.Country,
                    CurrentLanguage = user.CurrentLanguage,
                    ProfilePictureUrl = user.ProfilePictureUrl
                }),
        };
        return Ok(getCompanyApiResponse);
    }

    [HttpGet("{companyId:int}/talentmanagers")]
    public async Task<IActionResult> GetOneCompanyTalentManagers([FromRoute] int companyId)
    {
        var company = await _dbContext.Companies.Include("TalentManagers").FirstOrDefaultAsync(company => company.CompanyId == companyId);

        if (company == null)
        {
            ProblemDetailResponse problemDetailResponse = new ProblemDetailResponse
            {
                ProblemTitle = "Company not found",
                ProblemDescription = $"Company not found with id : {companyId}",
            };
            return Ok(problemDetailResponse);
        }
        
        var talentManagers = company.TalentManagers.ToList();
        
        List<UserTalentManagerResponse> userTalentManagerResponses = talentManagers.Select(user =>
            new UserTalentManagerResponse
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Title = user.Title,
                Email = user.Email,
                Age = user.Age,
                Country = user.Country,
                CurrentLanguage = user.CurrentLanguage,
                ProfilePictureUrl = user.ProfilePictureUrl
            }).ToList();

        return Ok(userTalentManagerResponses);


    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyApiRequest createCompanyApiRequest)
    {
        Company newCompany = await _companyService.CreateCompany(createCompanyApiRequest);
        
        CreateCompanyApiResponse createCompanyApiResponse = new CreateCompanyApiResponse
        {
            CompanyId = newCompany.CompanyId,
            CompanyName = newCompany.CompanyName,
            Industry = newCompany.Industry,
            Description = newCompany.Description,
            EmployeeCount = createCompanyApiRequest.EmployeeCount,
            WebsiteUrl = newCompany.WebsiteUrl,
            LogoUrl = newCompany.LogoUrl,
            FoundedAt = newCompany.FoundedAt,
        };
        
        return Ok(createCompanyApiResponse);
    }
}