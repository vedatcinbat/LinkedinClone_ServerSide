using System.ComponentModel.DataAnnotations.Schema;

namespace JobNet.CoreApi.Models.Response;

public class UserEducationResponseWithoutUserResponse
{
    public string Degree { get; set; }
    
    public string FieldOfStudy { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    [ForeignKey("SchoolId")]
    public int SchoolId { get; set; }
    
    public GetAllSchoolsResponse School { get; set; }
}