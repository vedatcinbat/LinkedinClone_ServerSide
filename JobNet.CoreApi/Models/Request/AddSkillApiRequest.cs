using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Models.Request;

public class AddSkillApiRequest
{
    public string SkillName { get; set; }
    
    public SkillIndustry SkillIndustry { get; set; }
}