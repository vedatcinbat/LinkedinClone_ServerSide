using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Data.Entities;

public class Job
{
    public int JobId { get; set; }
    
    public string JobTitle { get; set; }
    
    public JobType JobType { get; set; }
    
    public JobEmployeeLevel JobEmployeeLevel { get; set; }
    public string Description { get; set; }
    
    public string Location { get; set; }
    
    public DateTime PostedAt { get; set; }
    
    public DateTime Deadline { get; set; }
    
    [ForeignKey("CompanyId")]
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }

    public ICollection<UserJobLike>? UserJobLikes { get; set; } = new List<UserJobLike>();
}