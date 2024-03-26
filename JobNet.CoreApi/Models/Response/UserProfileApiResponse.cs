using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace JobNet.CoreApi.Models.Response;

public class UserProfileApiResponse
{
    [Key]
    public int UserId { get; set; }
    
    public string? Firstname { get; set; }
    
    public string? Lastname { get; set; }
    
    public string? Title { get; set; }
    public string? Email { get; set; }
    
    public string? Age { get; set; }
    
    public string? Country { get; set; }
    
    public string? AboutMe { get; set; }
    public int FollowerCount { get; set; }
    
    public int FollowingCount { get; set; }
        
    [ForeignKey("CompanyId")]
    public int? CompanyId { get; set; }
    
    public UserCompanySimpleResponse? Company { get; set; }
    
    public ICollection<PostApiResponseWithoutUser>? Posts { get; set; }

    public ICollection<Experience>? Experiences { get; set; }

    public ICollection<UserEducationResponseWithoutUserResponse>? Educations { get; set; }

    public ICollection<Skill>? Skills { get; set; }
    
}