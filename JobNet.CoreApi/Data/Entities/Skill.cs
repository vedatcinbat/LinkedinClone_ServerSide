using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Data.Entities;

public class Skill
{
    public int SkillId { get; set; }
    
    public string SkillName { get; set; }
    
    public SkillIndustry SkillIndustry { get; set; }
}