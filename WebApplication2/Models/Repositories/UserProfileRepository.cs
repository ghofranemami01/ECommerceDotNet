using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        readonly AppDbContext context;

        public UserProfileRepository(AppDbContext context)
        {
            this.context = context;
        }

        public UserProfile? GetByUserId(string userId)
        {
            return context.UserProfiles
                .Include(p => p.Addresses)
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userId);
        }

        public UserProfile? GetById(int id)
        {
            return context.UserProfiles
                .Include(p => p.Addresses)
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserProfileId == id);
        }

        public void Add(UserProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            context.UserProfiles.Add(profile);
            context.SaveChanges();
        }

        public UserProfile? Update(UserProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            var existing = context.UserProfiles.Find(profile.UserProfileId);
            if (existing != null)
            {
                existing.FirstName = profile.FirstName;
                existing.LastName = profile.LastName;
                existing.PhoneNumber = profile.PhoneNumber;
                existing.DateOfBirth = profile.DateOfBirth;
                context.SaveChanges();
            }
            return existing;
        }

        public void Delete(int id)
        {
            var profile = context.UserProfiles.Find(id);
            if (profile != null)
            {
                context.UserProfiles.Remove(profile);
                context.SaveChanges();
            }
        }
    }
}



