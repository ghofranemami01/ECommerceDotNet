using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public interface IAddressRepository
    {
        Address? GetById(int id);
        IList<Address> GetByUserProfileId(int userProfileId);
        Address? GetDefaultAddress(int userProfileId);
        void Add(Address address);
        Address? Update(Address address);
        void Delete(int id);
        void SetAsDefault(int addressId, int userProfileId);
    }
}


