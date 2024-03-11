using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;

namespace JobNet.CoreApi.Models.Response;

public class UserFollowSimple
{
    public int UserId { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? HashedPassword { get; set; }
    
    public string? Email { get; set; }
    
    public string? Age { get; set; }

    public string? Country { get; set; }

    public string? CurrentLanguage { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public int? CompanyId { get; set; }
        
    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }
    

        
}