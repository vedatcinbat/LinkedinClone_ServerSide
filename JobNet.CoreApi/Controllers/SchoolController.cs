using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchoolController(JobNetDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllSchools()
    {
        List<School> schools = await dbContext.Schools.Include(school => school.Graduates).ToListAsync();
        
        List<GetAllSchoolsResponse> schoolsResponse = schools.Select(school => new GetAllSchoolsResponse
        {
            SchoolId = school.SchoolId,
            SchoolName = school.SchoolName,
            Location = school.Location,
            EstablishedAt = school.EstablishedAt,
            GraduatesCount = school.Graduates?.Count
        }).ToList();
        
        return Ok(schoolsResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchool(CreateSchoolApiRequest createSchoolApiRequest)
    {
        
        int schoolId = await dbContext.Schools.CountAsync() + 1;

        School newSchool = new School
        {
            SchoolId = schoolId,
            SchoolName = createSchoolApiRequest.SchoolName,
            Location = createSchoolApiRequest.Location,
            EstablishedAt = createSchoolApiRequest.EstablishedAt,
            Graduates = new List<User>()
        };

        await dbContext.Schools.AddAsync(newSchool);
        await dbContext.SaveChangesAsync();

        GetAllSchoolsResponse getSchool = new GetAllSchoolsResponse
        {
            SchoolId = newSchool.SchoolId,
            SchoolName = newSchool.SchoolName,
            Location = newSchool.Location,
            EstablishedAt = newSchool.EstablishedAt,
            GraduatesCount = newSchool.Graduates?.Count
        };

        return Ok(getSchool);
    }
}