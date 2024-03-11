using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;

namespace JobNet.CoreApi.Models.Response;

public class LikeSimpleResponse
{
    public int LikeId { get; set; }
    
    public bool IsDeleted {get; set; }
    
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    
    public UserPostSimpleResponse User { get; set; }
}