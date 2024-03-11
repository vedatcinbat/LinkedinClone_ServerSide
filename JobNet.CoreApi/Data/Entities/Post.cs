using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Data.Entities;

public class Post
{
    public int PostId { get; set; }
    
    public bool IsDeleted { get; set; }
    
    [ForeignKey("UserId")]
    public int? UserId { get; set; }
    
    public User User { get; set; }
    
    public DateTime PublishTime { get; set; }
    
    public string Caption { get; set; }

    public PostType PostType;
    
    public string? TextContent { get; set; }
    
    public string? ImageContent { get; set; }
    
    public string? ImagesContent { get; set; }

    [NotMapped] 
    public int CommentCount => Comments?.Count ?? 0;

    [NotMapped]
    public int LikeCount => Likes?.Count ?? 0;
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<Like> Likes { get; set; } = new List<Like>();
    
}