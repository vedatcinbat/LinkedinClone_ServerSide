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
    
    [HttpGet("get-school-by-id/{schoolId}")]
    public async Task<IActionResult> GetSchoolById(int schoolId)
    {
        School? school = await dbContext.Schools.Include(school => school.Graduates).FirstOrDefaultAsync(school => school.SchoolId == schoolId);
        
        if (school == null)
        {
            return NotFound($"School with id {schoolId} not found.");
        }

        int graduatesCount = await dbContext.Educations.Where(e => e.SchoolId == schoolId).Distinct().CountAsync();
        
        GetAllSchoolsResponse schoolResponse = new GetAllSchoolsResponse
        {
            SchoolId = school.SchoolId,
            SchoolName = school.SchoolName,
            Location = school.Location,
            EstablishedAt = school.EstablishedAt,
            GraduatesCount = graduatesCount
        };
        
        return Ok(schoolResponse);
    }
    
    [HttpGet("get-school-graduates/{schoolId:int}")]
    public async Task<IActionResult> GetSchoolGraduates(int schoolId)
    {
        var school = await dbContext.Schools.Include(school => school.Graduates).ThenInclude(user => user.Company).FirstOrDefaultAsync(school => school.SchoolId == schoolId);
        
        if (school == null)
        {
            return NotFound($"School with id {schoolId} not found.");
        }
        
        List<Education> educations = await dbContext.Educations
            .Include(e => e.User).ThenInclude(user => user.Company)
            .Include(e => e.School)
            .Where(e => e.SchoolId == schoolId)
            .ToListAsync();

        var differentUsers = educations.Select(e => e.User).Distinct().ToList();
        

        List<UserSimpleWithSimpleCompanyResponse> users = differentUsers.Select(user => new UserSimpleWithSimpleCompanyResponse
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Title = user.Title,
            ProfilePictureUrl = user.ProfilePictureUrl,
            IsDeleted = user.IsDeleted,
            CompanyId = user.Company?.CompanyId,
            Company = user.Company != null ? new UserLikeCompanySimpleResponse
            {
                CompanyName = user.Company.CompanyName
            } : null,
        }).ToList();;
        
        return Ok(users);
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