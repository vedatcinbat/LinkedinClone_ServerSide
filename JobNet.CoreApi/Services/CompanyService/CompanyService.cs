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

    public async Task<List<Company>> GetAllCompanies(string? companyName)
    {
        IQueryable<Company> query = _dbContext.Companies
            .Include(c => c.CurrentAvailableJobs)
            .Include(c => c.TalentManagers);
        
        
        if (!string.IsNullOrEmpty(companyName))
        {
            query = query.Where(c => c.CompanyName.Contains(companyName));
        }

        var companies = await query.ToListAsync();

        return companies;
    }

    public async Task<Company> CreateCompany(CreateCompanyApiRequest createCompanyApiRequest)
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

        return newCompany;
    }
}