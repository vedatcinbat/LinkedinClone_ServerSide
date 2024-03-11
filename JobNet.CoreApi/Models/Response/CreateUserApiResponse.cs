using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;

namespace JobNet.CoreApi.Models.Response;

public class CreateUserApiResponse
{
    public int UserId { get; set; }
    
    public string? Firstname { get; set; }
    
    public string? Lastname { get; set; }
    
    public string? Title { get; set; }
    
    public string? Email { get; set; }
    
    public string? Age { get; set; }
    
    public string? Country { get; set; }
    
    public string? AboutMe { get; set; }
    
    public bool IsDeleted { get; set; }
    
    [ForeignKey("CompanyId")]
    public int? CompanyId { get; set; }
    
    public ICollection<Post>? Posts { get; set; }

    public ICollection<Experience>? Experiences { get; set; }

    public ICollection<Education>? Educations { get; set; }

    public ICollection<Skill>? Skills { get; set; }
}