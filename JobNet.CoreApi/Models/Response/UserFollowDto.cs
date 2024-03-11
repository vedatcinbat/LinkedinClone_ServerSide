namespace JobNet.CoreApi.Models.Response;

public class UserFollowDto
{
    public int Id { get; set; }
    
    public int FollowerId { get; set; }
    
    public UserFollowSimple FollowerUser { get; set; }
    
    public int FollowingId { get; set; }
    
    public UserFollowSimple FollowingUser { get; set; }
}