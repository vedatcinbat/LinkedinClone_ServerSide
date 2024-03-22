namespace JobNet.CoreApi.Models.Response;

public class GetCompanyJobsApiResponse
{
    public int JobId { get; set; }
    
    public string JobTitle { get; set; }
    
    public string Description { get; set; }
    
    public string Location { get; set; }
    
    public DateTime PostedAt { get; set; }
    
    public DateTime Deadline { get; set; }
    
    public int PublisherId { get; set; }
    
    public UserTalentManagerResponse PublisherUser { get; set; }
    
    public int AppliedUserCount { get; set; }
}