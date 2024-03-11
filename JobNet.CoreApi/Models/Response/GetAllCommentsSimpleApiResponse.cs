using System.ComponentModel.DataAnnotations.Schema;

namespace JobNet.CoreApi.Models.Response;

public class GetAllCommentsSimpleApiResponse
{
    public int CommentId { get; set; }
    
    public string Content { get; set; }
    
    public DateTime CommentedAt { get; set; }
    
    public bool IsDeleted { get; set; }
    
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public UserCommentSimpleResponse User { get; set; }
    
    [ForeignKey("PostId")]
    public int PostId { get; set; }
    public PostCommentSimpleResponse Post { get; set; }
}