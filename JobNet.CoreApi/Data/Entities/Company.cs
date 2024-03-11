using System.ComponentModel.DataAnnotations;
using JobNet.CoreApi.Data.Enums;
using JobNet.CoreApi.Models.Response;

namespace JobNet.CoreApi.Data.Entities
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; }
        
        public CompanyIndustry Industry { get; set; }
        
        public string? Description { get; set; }
        
        public string? EmployeeCount { get; set; }

        [Url(ErrorMessage = "Invalid URL")]
        public string WebsiteUrl { get; set; }

        public string LogoUrl { get; set; }

        [Required(ErrorMessage = "Founded date is required")]
        public DateTime FoundedAt { get; set; }

        public ICollection<Job>? CurrentAvailableJobs { get; set; } = new List<Job>();

        public ICollection<User>? TalentManagers { get; set; } = new List<User>();

    }
}