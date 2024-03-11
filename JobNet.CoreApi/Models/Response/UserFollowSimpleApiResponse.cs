using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;

namespace JobNet.CoreApi.Models.Response;

public class UserFollowSimpleApiResponse
{
    [Key]
    public int UserId { get; set; }
    
    public string? Firstname { get; set; }
    
    public string? Lastname { get; set; }
    
    public string? Email { get; set; }
    
    public string? Age { get; set; }
    
    public string? Country { get; set; }
    
    public string? CurrentLanguage { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
    
    [ForeignKey("CompanyId")]
    public int? CompanyId { get; set; }
    
    public UserCompanySimpleResponse? Company { get; set; }
    
}