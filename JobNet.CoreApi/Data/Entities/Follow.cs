using System.Text.Json.Serialization;

namespace JobNet.CoreApi.Data.Entities;

public class Follow
{
    public int Id { get; set; }
    public int FollowerId { get; set; }
    public User FollowerUser { get; set; }
    public int FollowingId { get; set; }
    public User FollowingUser { get; set; }
    
    public bool IsDeleted { get; set; }
}