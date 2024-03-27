using System.ComponentModel.DataAnnotations.Schema;

namespace JobNet.CoreApi.Models.Response;

public class ExperienceSimpleApiResponse
{
    public int ExperienceId { get; set; }
    
    public string Title { get; set; }
    
    public string CompanyName { get; set; }
    
    public string Location { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public string Description { get; set; }
    
    [ForeignKey("UserId")]
    public int UserId { get; set; }

    public UserSimpleApiResponseWithSimpleCompanyResponse User { get; set; }

    [ForeignKey("CompanyId")] 
    public int CompanyId { get; set; }
    
    public CompanyWithCompanyNameResponse Company { get; set; }
}