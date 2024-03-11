using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;

namespace JobNet.CoreApi.Models.Response;

public class CommentSimpleResponse
{
    public int CommentId { get; set; }
    
    public string Content { get; set; }
    
    public DateTime CommentedAt { get; set; }
    
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public UserPostSimpleResponse User { get; set; }
    
}