using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.DTOs.Response;
using JobNet.CoreApi.DTOs.Response;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Services;

public class CompanyService : ICompanyService
{
    private readonly JobNetDbContext _dbContext;

    public CompanyService(JobNetDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<List<GetCompanyApiResponse>> GetAllCompanies()
    {
        List<Company> companies = await _dbContext.Companies.Include("CurrentAvailableJobs").Include("TalentManagers").ToListAsync();
        
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
            CurrentAvailableJobs = company.CurrentAvailableJobs.Select(job => new JobDto
            {
                JobId = job.JobId,
                JobTitle = job.JobTitle,
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
                HashedPassword = user.HashedPassword,
                Email = user.Email,
                Age = user.Age,
                Country = user.Country,
                CurrentLanguage = user.CurrentLanguage,
                ProfilePictureUrl = user.ProfilePictureUrl
            }),
        }).ToList();
        
        return getCompanyApiResponses;
    }

    public async Task<CreateCompanyApiResponse> CreateCompany(CreateCompanyApiRequest createCompanyApiRequest)
    {
        int companyId = await _dbContext.Companies.CountAsync() + 1;
        
        Company newCompany = new Company
        {
            CompanyId = companyId,
            CompanyName = createCompanyApiRequest.CompanyName,
            Industry = createCompanyApiRequest.Industry,
            Description = createCompanyApiRequest.Description,
            EmployeeCount = createCompanyApiRequest.EmployeeCount,
            WebsiteUrl = createCompanyApiRequest.WebsiteUrl,
            LogoUrl = createCompanyApiRequest.LogoUrl,
            FoundedAt = createCompanyApiRequest.FoundedAt,
            CurrentAvailableJobs = null,
            TalentManagers = null,
        };
        
        await _dbContext.Companies.AddAsync(newCompany);
        await _dbContext.SaveChangesAsync();

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

        return createCompanyApiResponse;
    }

    public async Task<List<UserTalentManagerResponse>> GetCompanyTalentManagers(int companyId)
    {
        Company company = await _dbContext.Companies.Include("TalentManagers").FirstOrDefaultAsync(company => company.CompanyId == companyId);

        if (company != null)
        {
            var users = company.TalentManagers.ToList();

            List<UserTalentManagerResponse> userTalentManagerResponses = users.Select(user =>
                new UserTalentManagerResponse
                {
                    UserId = user.UserId,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    HashedPassword = user.HashedPassword,
                    Email = user.Email,
                    Age = user.Age,
                    Country = user.Country,
                    CurrentLanguage = user.CurrentLanguage,
                    ProfilePictureUrl = user.ProfilePictureUrl
                }).ToList();

            return userTalentManagerResponses;
        }

        return null;


    }
}