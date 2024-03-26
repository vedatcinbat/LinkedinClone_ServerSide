namespace JobNet.CoreApi.Models.Response;

public class GetUserEducationsResponse
{
    public int UserId { get; set; }
    
    public string? Firstname { get; set; }
    
    public string? Lastname { get; set; }
    
    public string? Title { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
    
    public string? AboutMe { get; set; }
    
    public List<UserEducationResponseWithoutUserResponse>? Education { get; set; }
    
}