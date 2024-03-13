using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.Migrations;

namespace JobNet.CoreApi.Models.Response;

public class CreateJobApiResponse
{
    public int JobId { get; set; }
    
    public string JobTitle { get; set; }
    
    public string JobType { get; set; }
    
    public string JobEmployeeLevel { get; set; }
    
    public string Description { get; set; }
    
    public string Location { get; set; }
    
    public DateTime PostedAt { get; set; }
    
    public DateTime Deadline { get; set; }
    
    [ForeignKey("UserId")]
    public int PublisherId { get; set; }
    
    public UserTalentManagerResponse PublisherUser { get; set; }
    
    [ForeignKey("CompanyId")]
    public int CompanyId { get; set; }
    public UserCompanySimpleResponse? Company { get; set; }
    
    public List<UserSimpleWithSimpleCompanyResponse> UserJobLikeResponses { get; set; }
    
}