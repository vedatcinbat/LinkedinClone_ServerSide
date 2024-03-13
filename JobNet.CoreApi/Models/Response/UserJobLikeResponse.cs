using System.ComponentModel.DataAnnotations.Schema;

namespace JobNet.CoreApi.Models.Response;

public class UserJobLikeResponse
{
    public int UserId { get; set; }
    
    public string? Firstname { get; set; }
    
    public string? Lastname { get; set; }

    public string? Title { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
    
    public bool IsDeleted { get; set; }
    
    [ForeignKey("CompanyId")]
    public int? CompanyId { get; set; }
    
    public UserCompanySimpleResponse? Company { get; set; }
}