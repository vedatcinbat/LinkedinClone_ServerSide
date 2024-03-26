using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SkillController(JobNetDbContext dbContext) : ControllerBase
{

    [HttpGet("allSkills")]
    public async Task<IActionResult> GetAllSkills()
    {
        List<Skill> skills = await dbContext.Skills.ToListAsync();
        
        return Ok(skills);
    }

    [HttpPost("addSkill")]
    public async Task<IActionResult> CreateSkill([FromQuery] AddSkillApiRequest addSkillApiRequest)
    {
        var skillId = await dbContext.Skills.CountAsync() + 1;

        Skill skill = new Skill()
        {
            SkillId = skillId,
            SkillName = addSkillApiRequest.SkillName,
            SkillIndustry = addSkillApiRequest.SkillIndustry
        };

        await dbContext.Skills.AddAsync(skill);
        await dbContext.SaveChangesAsync();

        return Ok($"Skill added successfully : {skill.SkillId} {skill.SkillName}");


    }
    
    
    
}