using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Services;
using JobNet.CoreApi.Services.UserService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JobNet.CoreApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CompanyController : ControllerBase
{
    private ICompanyService _companyService;
    
    public CompanyController(ICompanyService companyService)
    {
        this._companyService = companyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        var getCompaniesApiResponses = await _companyService.GetAllCompanies();
        
        return Ok(getCompaniesApiResponses);
    }

    [HttpGet("/{companyId:int}/talentmanagers")]
    public async Task<IActionResult> GetOneCompanyTalentManagers([FromRoute] int companyId)
    {
        var talentManagers = await _companyService.GetCompanyTalentManagers(companyId);
        
        if (talentManagers != null)
        {
            return Ok(talentManagers);     
        }

        return BadRequest($"Company not found with id : {companyId}");


    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyApiRequest createCompanyApiRequest)
    {
        CreateCompanyApiResponse createCompanyApiResponse = await _companyService.CreateCompany(createCompanyApiRequest);
        
        return Ok(createCompanyApiResponse);
    }
}