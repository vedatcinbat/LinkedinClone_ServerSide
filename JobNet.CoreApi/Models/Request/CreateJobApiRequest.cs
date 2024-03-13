using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Models.Request;

public class CreateJobApiRequest
{
    public string JobTitle { get; set; }
    
    public string JobType { get; set; }
    
    public string JobEmployeeLevel { get; set; }
    
    public string Description { get; set; }
    
    public string Location { get; set; }
    
    public DateTime PostedAt { get; set; }
    
    public DateTime Deadline { get; set; }
    
    [ForeignKey("CompanyId")]
    public int CompanyId { get; set; }
}