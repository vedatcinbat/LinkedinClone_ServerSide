namespace JobNet.CoreApi.Models.Request;

public class CreateExperienceApiRequest
{
    public string Title { get; set; }
    
    public string Location { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public string Description { get; set; }
    
}