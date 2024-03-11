using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Models.Response;

public class PostSimpleApiResponseWithoutUser
{
    public int PostId { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime PublishTime { get; set; }
    
    public string Caption { get; set; }

    public PostType PostType;
    
    public string? TextContent { get; set; }
    
    public string? ImageContent { get; set; }
    
    public string? ImagesContent { get; set; }
    
    public int CommentCount { get; set; }
    
    public int LikeCount { get; set; }
    
    public ICollection<PostCommentSimpleApiResponse> Comments { get; set; } = new List<PostCommentSimpleApiResponse>();

    public ICollection<LikeSimpleResponse> Likes { get; set; } = new List<LikeSimpleResponse>();
}