using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Models.Request;

namespace JobNet.CoreApi.Models.Response;

public class UserEducationSimpleResponse
{
    public string Degree { get; set; }
    
    public string FieldOfStudy { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public UserSimpleWithCompanyApiResponse user { get; set; }
    
    [ForeignKey("SchoolId")]
    public int SchoolId { get; set; }
    
    public GetAllSchoolsResponse School { get; set; }
}