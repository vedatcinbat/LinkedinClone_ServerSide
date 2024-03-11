using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Models.Response;

public class CreateCompanyApiResponse
{
    public int CompanyId { get; set; }
    
    public string CompanyName { get; set; }
        
    public CompanyIndustry Industry { get; set; }
        
    public string? Description { get; set; }
    
    public string? EmployeeCount { get; set; }
    
    public string WebsiteUrl { get; set; }

    public string LogoUrl { get; set; }
    
    public DateTime FoundedAt { get; set; }
}