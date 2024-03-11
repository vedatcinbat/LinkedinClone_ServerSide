namespace JobNet.CoreApi.Models.Response;

public class PostCommentSimpleApiResponse
{
    public int CommentId { get; set; }
    
    public string Content { get; set; }
    
    public DateTime CommentedAt { get; set; }
    
    public UserCommentSimpleResponse UserCommentSimpleResponse { get; set; }
}