using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JobNet.CoreApi.Data.Enums;

namespace JobNet.CoreApi.Data.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }
        
        public string? Title { get; set; }
        public string? HashedPassword { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public string? Age { get; set; }

        public string? Country { get; set; }

        public string? CurrentLanguage { get; set; }

        public string? ProfilePictureUrl { get; set; }
        
        public string? AboutMe { get; set; }
        
        public bool IsDeleted { get; set; }
        public int? CompanyId { get; set; }
        
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        
        // Collections
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();

        public ICollection<Education> Educations { get; set; } = new List<Education>();

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();

        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
    }
}