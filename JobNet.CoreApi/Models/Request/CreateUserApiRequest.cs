using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;

namespace JobNet.CoreApi.Models.Request;

public class CreateUserApiRequest
{
    public string Firstname { get; set; }
    
    public string Lastname { get; set; }
    
    public string HashedPassword { get; set; }
    
    public string? Title { get; set; }
    
    public string Email { get; set; }
    
    public string Age { get; set; }
    
    public string Country { get; set; }
    
    public string CurrentLanguage { get; set; }
    
    public string ProfilePictureUrl { get; set; }
    
    public string? AboutMe { get; set; }
    
    [ForeignKey("CompanyId")]
    public int? CompanyId { get; set; }

}