using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Entities;

namespace JobNet.CoreApi.Models.Response;

public class UserSkillSimpleApiResponse
{
    [Key]
    public int UserId { get; set; }
    
    public string? Firstname { get; set; }
    
    public string? Lastname { get; set; }
    
    public string? Title { get; set; }
    public ICollection<Skill> Skills { get; set; }
}