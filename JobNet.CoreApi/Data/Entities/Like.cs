using System.ComponentModel.DataAnnotations.Schema;

namespace JobNet.CoreApi.Data.Entities;

public class Like
{
    public int LikeId { get; set; }
    
    public bool IsDeleted { get; set; }
    
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("PostId")]
    public int PostId { get; set; }
    public Post Post { get; set; }
}