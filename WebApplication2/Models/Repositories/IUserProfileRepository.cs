using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public interface IUserProfileRepository
    {
        UserProfile? GetByUserId(string userId);
        UserProfile? GetById(int id);
        void Add(UserProfile profile);
        UserProfile? Update(UserProfile profile);
        void Delete(int id);
    }
}


