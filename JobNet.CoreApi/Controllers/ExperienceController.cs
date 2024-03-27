using JobNet.CoreApi.Data;
using JobNet.CoreApi.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExperienceController(JobNetDbContext dbContext) : ControllerBase
{
    [HttpGet("getAllExperiences")]
    public async Task<IActionResult> GetAllExperiences()
    {
        var experiences = await dbContext.Experiences.Include(experience => experience.User)
            .ThenInclude(user => user.Company).Include(experience => experience.Company).ToListAsync();

        var experiencesResponse = experiences.Select(experience => new ExperienceSimpleApiResponse
        {
            ExperienceId = experience.ExperienceId,
            Title = experience.Title,
            CompanyName = experience.CompanyName,
            Location = experience.Location,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            Description = experience.Description,
            UserId = experience.User.UserId,
            User = new UserSimpleApiResponseWithSimpleCompanyResponse
            {
                UserId = experience.User.UserId,
                Firstname = experience.User.Firstname,
                Lastname = experience.User.Lastname,
                Title = experience.User.Title,
                ProfilePictureUrl = experience.User.ProfilePictureUrl,
                AboutMe = experience.User.AboutMe,
                IsDeleted = experience.User.IsDeleted,
                CompanyId = experience.User.Company?.CompanyId,
                Company = experience.User.Company != null ? new CompanyWithCompanyNameResponse
                {
                    CompanyName = experience.User.Company.CompanyName
                } : null
            },
            CompanyId = experience.CompanyId,
            Company = new CompanyWithCompanyNameResponse
            {
                CompanyName = experience.Company.CompanyName
            }
        }).ToList();
        
        
        return Ok(experiencesResponse);
    }
}