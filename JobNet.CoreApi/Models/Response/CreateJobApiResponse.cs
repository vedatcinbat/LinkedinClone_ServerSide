using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.Migrations;

namespace JobNet.CoreApi.Models.Response;

public class CreateJobApiResponse
{
    public int JobId { get; set; }
    
    public required string JobTitle { get; set; }
    
    public required string JobType { get; set; }
    
    public required string JobEmployeeLevel { get; set; }
    
    public required string Description { get; set; }
    
    public required string Location { get; set; }
    
    public DateTime PostedAt { get; set; }
    
    public DateTime Deadline { get; set; }
    
    [ForeignKey("UserId")]
    public int PublisherId { get; set; }
    
    public required UserTalentManagerResponse PublisherUser { get; set; }
    
    [ForeignKey("CompanyId")]
    public int CompanyId { get; set; }
    public UserCompanySimpleResponse? Company { get; set; }
    
    public required List<UserSimpleWithSimpleCompanyResponse> UserJobLikeResponses { get; set; }
    
}