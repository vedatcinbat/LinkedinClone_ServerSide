namespace JobNet.CoreApi.Models.Request;

public class UserEducationApiRequest
{
    public string Degree { get; set; }
    
    public string FieldOfStudy { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
}