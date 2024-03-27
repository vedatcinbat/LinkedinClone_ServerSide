using JobNet.CoreApi.Data;
using JobNet.CoreApi.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EducationController(JobNetDbContext dbContext) : ControllerBase
{
    [HttpGet("allEducations")]
    public async Task<IActionResult> GetAllEducations()
    {
        var allEducations = await dbContext.Educations.Include(education => education.User)
            .ThenInclude(user => user.Company).Include(education => education.School).ToListAsync();

        var allEducationsResponse = allEducations.Select(education => new GetEducationsSimpleResponse
        {
            Degree = education.Degree,
            FieldOfStudy = education.FieldOfStudy,
            StartDate = education.StartDate,
            EndDate = education.EndDate,
            UserId = education.UserId,
            User = new UserSimpleApiResponseWithSimpleCompanyResponse
            {
                UserId = education.User.UserId,
                Firstname = education.User.Firstname,
                Lastname = education.User.Lastname,
                Title = education.User.Title,
                ProfilePictureUrl = education.User.ProfilePictureUrl,
                AboutMe = education.User.AboutMe,
                IsDeleted = education.User.IsDeleted,
                CompanyId = education.User.CompanyId,
                Company = education.User.Company != null ? new CompanyWithCompanyNameResponse
                {
                    CompanyName = education.User.Company.CompanyName
                } : null,
            },
            SchoolId = education.SchoolId,
            School = new GetAllSchoolsResponse
            {
                SchoolId = education.School.SchoolId,
                SchoolName = education.School.SchoolName,
                Location = education.School.Location,
                EstablishedAt = education.School.EstablishedAt,
                GraduatesCount = education.School.Graduates.Count
            }
        });


        return Ok(allEducationsResponse);
    }
}