namespace JobNet.CoreApi.Models.Response;

public class GetUserWithEducationResponse
{
    public int UserId { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }
        
    public string? Title { get; set; }

    public string? ProfilePictureUrl { get; set; }
        
    public string? AboutMe { get; set; }
        
    public bool IsDeleted { get; set; }
    
    public GetCompanyForUserSchoolResponse? Company { get; set; }
    
    public ICollection<UserEducationResponseWithoutUserResponse> Educations { get; set; } = new List<UserEducationResponseWithoutUserResponse>();
    
}