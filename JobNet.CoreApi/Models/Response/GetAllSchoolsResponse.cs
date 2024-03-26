namespace JobNet.CoreApi.Models.Response;

public class GetAllSchoolsResponse
{
    public int SchoolId { get; set; }
    
    public string SchoolName { get; set; }
    
    public string Location { get; set; }
    
    public DateTime EstablishedAt { get; set; }
    
    public int? GraduatesCount { get; set; }
}