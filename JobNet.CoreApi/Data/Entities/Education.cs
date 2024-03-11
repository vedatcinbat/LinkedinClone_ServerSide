using System.ComponentModel.DataAnnotations.Schema;

namespace JobNet.CoreApi.Data.Entities;

public class Education
{
    public int EducationId { get; set; }
    
    public string SchoolName { get; set; }
    
    public string Degree { get; set; }
    
    public string FieldOfStudy { get; set; }
    
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("SchoolId")]
    public int SchoolId { get; set; }
    public School School { get; set; }
}