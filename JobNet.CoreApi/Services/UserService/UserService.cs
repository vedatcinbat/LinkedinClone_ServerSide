using System.Security.Cryptography;
using System.Text;
using JobNet.CoreApi.Data;
using JobNet.CoreApi.Data.Entities;
using JobNet.CoreApi.Models.Request;
using System.Security.Cryptography;

using Microsoft.EntityFrameworkCore;

namespace JobNet.CoreApi.Services.UserService;

public class UserService(JobNetDbContext dbContext) : IUserService
{
    public async Task<List<User?>> GetAllUsers()
    {
        var users = await dbContext.Users.Include("Company").ToListAsync();

        return users;
    }
    
    public async Task<List<User?>> GetAllUsersActive()
    {
        var users = await dbContext.Users.Where(user => user.IsDeleted == false).Include("Company").ToListAsync();

        return users;
    }

    public async Task<User?> GetOneUserProfileDetails(int userId)
    {
        User? user = await
            dbContext.Users
                .Where(user => user.IsDeleted == false)
                .Include(u => u.Company)
                .Include(u => u.Posts.Where(p => p.IsDeleted == false))
                    .ThenInclude(p => p.Likes.Where(like => like.IsDeleted == false))
                .Include(u => u.Posts.Where(p => p.IsDeleted == false))
                    .ThenInclude(p => p.Comments.Where(comment => comment.IsDeleted == false))
                .Include(u => u.Experiences)
                    .ThenInclude(e => e.Company)
                .Include(u => u.Educations)
                .ThenInclude(e => e.School)
                .Include(u => u.Skills)
                .FirstOrDefaultAsync(u => u.UserId == userId);

        return user ?? null;
    }

    public async Task<User?> GetOneUserPostsWithUserId(int userId)
    {
        var user = await dbContext.Users
            .Where(u => u.UserId == userId && u.IsDeleted == false)
            .Include(u => u.Company)
            .Include(u => u.Posts)
                .ThenInclude(p => p.Comments.Where(c => c.IsDeleted == false))
                    .ThenInclude(c => c.User)
                        .ThenInclude(u => u.Company)
            .Include(u => u.Posts)
                .ThenInclude(p => p.Likes.Where(l => l.IsDeleted == false))
                    .ThenInclude(l => l.User)
                        .ThenInclude(u => u.Company)
            .Include(u => u.Experiences)
            .Include(u => u.Educations)
            .Include(u => u.Skills)
            .FirstOrDefaultAsync();
        
        return user ?? null;
    }

    public async Task<User?> GetOneUserSimpleDetails(int userId)
    {
        var user = await dbContext.Users.Where(user => user.IsDeleted == false).Include("Company").FirstOrDefaultAsync(u => u.UserId == userId);

        return user ?? null;
    }

    public async Task<User> CreateUser(CreateUserApiRequest createUserApiRequest)
    {
        var userId = await dbContext.Users.CountAsync() + 1;

        string hashedPassword = HashPassword(createUserApiRequest.HashedPassword);
        
        var newUser = new User
        {
            UserId = userId,
            Firstname = createUserApiRequest.Firstname,
            Lastname = createUserApiRequest.Lastname,
            Title = createUserApiRequest.Title,
            HashedPassword = hashedPassword,
            Email = createUserApiRequest.Email,
            Age = createUserApiRequest.Age,
            Country = createUserApiRequest.Country,
            CurrentLanguage = createUserApiRequest.CurrentLanguage,
            ProfilePictureUrl = createUserApiRequest.ProfilePictureUrl,
            AboutMe = createUserApiRequest.AboutMe,
            IsDeleted = false,
            CompanyId = createUserApiRequest.CompanyId,
            Posts = new List<Post>(),
            Experiences = new List<Experience>(),
            Educations = new List<Education>(),
            Skills = new List<Skill>(),

        };
        
        await dbContext.Users.AddAsync(newUser);
        await dbContext.SaveChangesAsync();

        return newUser;
    }
    
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }

    public async Task<List<User>> GetFollowers(int userId)
    {
        var followers = await dbContext.Follows.Where(follow => follow.IsDeleted == false)
            .Include(f => f.FollowerUser)
            .ThenInclude(u => u.Company)
            .Where(f => f.FollowingId == userId && f.FollowerUser.IsDeleted == false)
            .Select(f => f.FollowerUser)
            .ToListAsync();

        return followers;
    }

    public async Task<List<User>> GetFollowings(int userId)
    {
        var followings = await dbContext.Follows.Where(follow => follow.IsDeleted == false)
            .Include(f => f.FollowingUser) 
            .ThenInclude(u => u.Company)
            .Where(f => f.FollowerId == userId && f.FollowingUser.IsDeleted == false)  
            .Select(f => f.FollowingUser)  
            .ToListAsync();

        return followings;
    }

    public async Task<User?> UpdateUserCompanyWithId(int userId, int companyId)
    {
        var user = await dbContext.Users.Include("Company").Where(user => user.IsDeleted == false).FirstOrDefaultAsync(u => u.UserId == userId);
        
        var company = await dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == companyId);
        
        user.CompanyId = companyId;
        user.Company = company;

        await dbContext.SaveChangesAsync();
        
        return user;
    }

    public async Task<User> AddSchoolToUser(User user, School school, UserEducationApiRequest userEducationApiRequest)
    {
        int educationId = await dbContext.Educations.CountAsync() + 1;

        Education education = new Education
        {
            EducationId = educationId,
            Degree = userEducationApiRequest.Degree,
            FieldOfStudy = userEducationApiRequest.FieldOfStudy,
            StartDate = userEducationApiRequest.StartDate,
            EndDate = userEducationApiRequest.EndDate,
            UserId = user.UserId,
            User = user,
            SchoolId = school.SchoolId,
            School = school
        };
        
        await dbContext.Educations.AddAsync(education);
        school.Graduates.Add(user);
        await dbContext.SaveChangesAsync();
        
        return user;
    }

    public async Task<List<Post>> GetUserConnectionsPosts(int userId)
    {
        var followings = await dbContext.Follows.Where(f => f.FollowerId == userId && f.IsDeleted == false).ToListAsync();
        
        List<Post> posts = new List<Post>();
        
        foreach (var following in followings)
        {
            var followingPosts = await dbContext.Posts
                .Include(p => p.User).ThenInclude(u => u.Company)
                .Include(p => p.Likes.Where(l => l.IsDeleted == false))
                .ThenInclude(l => l.User)
                .Include(p => p.Comments.Where(c => c.IsDeleted == false))
                .Where(p => p.UserId == following.FollowingId && p.IsDeleted == false)
                .ToListAsync();
            
            posts.AddRange(followingPosts);
        }
        
        return posts;
    }


    public async Task<User?> GetUserSkill(int userId)
    {
        var user = await dbContext.Users.Where(user => user.IsDeleted == false).Include("Skills").FirstOrDefaultAsync(u => u.UserId == userId);

        return user ?? null;
    }
    public async Task<User> Authenticate(string email, string password)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return null;
        }
            
        
        if (!VerifyPasswordHash(password, user.HashedPassword))
            return null;

        return user;
    }

    private bool VerifyPasswordHash(string password, string storedHash)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hashedPassword == storedHash;
        }
    }
    public async Task<User?> SaveAccount(string email, string password)
    {
        // Hash the password
        string hashedPassword = HashPassword(password);

        var user = await dbContext.Users.FirstOrDefaultAsync(user =>
            user.IsDeleted == true && user.Email == email);
        if (user != null)
        {
            // Update the user's password with the new hashed password
            user.HashedPassword = hashedPassword;

            // Restore other user-related data as needed
            var userLikes = await dbContext.Likes.Where(l => l.User.UserId == user.UserId).ToListAsync();
            var userComments = await dbContext.Comments.Where(c => c.User.UserId == user.UserId).ToListAsync();
            var userFollows = await dbContext.Follows
                .Where(f => f.IsDeleted == true && (f.FollowerId == user.UserId || f.FollowingId == user.UserId))
                .ToListAsync();

            foreach (var comment in userComments)
            {
                comment.IsDeleted = false;
            }
            foreach (var like in userLikes)
            {
                like.IsDeleted = false;
            }
            foreach (var follow in userFollows)
            {
                follow.IsDeleted = false;
            }

            // Restore the user account
            user.IsDeleted = false;
        
            // Save changes to the database
            await dbContext.SaveChangesAsync();

            return user;
        }

        return null;
    }
    
}

