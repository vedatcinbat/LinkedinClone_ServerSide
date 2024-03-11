namespace JobNet.CoreApi.Data.Entities;

public class School
{
    public int SchoolId { get; set; }
    
    public string SchoolName { get; set; }
    
    public string Location { get; set; }
    
    public DateTime EstablishedAt { get; set; }
    
    public ICollection<Education> Educations { get; set; } = new List<Education>();
}