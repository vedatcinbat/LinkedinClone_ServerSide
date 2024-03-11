using System.ComponentModel.DataAnnotations.Schema;

namespace JobNet.CoreApi.Data.Entities;

public class UserJobLike
{
    public int UserJobLikeId { get; set; }
    
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("JobId")]
    public int JobId { get; set; }
    public Job Job { get; set; }
}