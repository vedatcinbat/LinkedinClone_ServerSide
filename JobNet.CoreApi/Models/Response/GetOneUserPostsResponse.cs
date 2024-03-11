using System.ComponentModel.DataAnnotations;

namespace JobNet.CoreApi.Models.Response;

public class GetOneUserPostsResponse
{
    [Key]
    public int UserId { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public ICollection<PostSimpleApiResponseWithoutUser> Posts { get; set; } = new List<PostSimpleApiResponseWithoutUser>();
    
}