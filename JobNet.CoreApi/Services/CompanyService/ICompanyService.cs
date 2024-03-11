using JobNet.CoreApi.Models.Request;
using JobNet.CoreApi.Models.Response;
using JobNet.CoreApi.Data.Entities;
namespace JobNet.CoreApi.Services;

public interface ICompanyService
{
    Task<List<GetCompanyApiResponse>> GetAllCompanies();
    Task<CreateCompanyApiResponse> CreateCompany(CreateCompanyApiRequest createCompanyApiRequest);
    Task<List<UserTalentManagerResponse>> GetCompanyTalentManagers(int companyId);
}