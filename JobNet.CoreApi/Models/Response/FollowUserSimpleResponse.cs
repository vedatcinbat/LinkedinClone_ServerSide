namespace JobNet.CoreApi.Models.Response;

public class FollowUserSimpleResponse
{
    public int Id { get; set; }
    
    public int FollowerId { get; set; }
    
    public UserFollowSimpleApiResponse FollowerUser { get; set; }
    
    public int FollowingId { get; set; }
    
    public UserFollowSimpleApiResponse FollowingUser { get; set; }
    
    public bool IsDeleted { get; set; }
}