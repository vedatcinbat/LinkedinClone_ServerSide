using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.DTOs.Response;

public class JobDto
{
    public int JobId { get; set; }
    
    public string JobTitle { get; set; }
    
    public string Description { get; set; }
    
    public JobType JobType { get; set; }
    
    public JobEmployeeLevel JobEmployeeLevel { get; set; }
    
    public string Location { get; set; }
    
    public DateTime PostedAt { get; set; }
    
    public DateTime Deadline { get; set; }
    
}