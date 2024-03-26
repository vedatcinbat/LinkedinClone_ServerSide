namespace JobNet.CoreApi.Models.Request;

public class CreateSchoolApiRequest
{
    public string SchoolName { get; set; }
    
    public string Location { get; set; }
    
    public DateTime EstablishedAt { get; set; }
}