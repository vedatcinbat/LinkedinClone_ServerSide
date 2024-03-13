using System.Security.Claims;

namespace JobNet.CoreApi.Auth;

public class Token
{
    public string AccessToken { get; set; }
    
    public string RefreshToken { get; set; }
    
    public DateTime Expiration { get; set; }
    public int UserId { get; set; }
    
    public string Email { get; set; }
    
    public string Firstname { get; set; }
    
    public string Lastname { get; set; }
    public List<Claim> Claims { get; set; }
}