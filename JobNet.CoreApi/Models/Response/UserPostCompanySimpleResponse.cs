using System.ComponentModel.DataAnnotations;
using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Models.Response;

public class UserPostCompanySimpleResponse
{
    [Key]
    public int CompanyId { get; set; }

    public string CompanyName { get; set; }

    public CompanyIndustry Industry { get; set; }

    public string LogoUrl { get; set; }
    
}