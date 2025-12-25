using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        readonly AppDbContext context;

        public AddressRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Address? GetById(int id)
        {
            return context.Addresses
                .Include(a => a.UserProfile)
                .FirstOrDefault(a => a.AddressId == id);
        }

        public IList<Address> GetByUserProfileId(int userProfileId)
        {
            return context.Addresses
                .Where(a => a.UserProfileId == userProfileId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.AddressId)
                .ToList();
        }

        public Address? GetDefaultAddress(int userProfileId)
        {
            return context.Addresses
                .FirstOrDefault(a => a.UserProfileId == userProfileId && a.IsDefault);
        }

        public void Add(Address address)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            
            // Si c'est la première adresse ou si elle est marquée comme défaut, la définir comme défaut
            var existingAddresses = GetByUserProfileId(address.UserProfileId);
            if (!existingAddresses.Any() || address.IsDefault)
            {
                // Retirer le défaut des autres adresses
                foreach (var addr in existingAddresses)
                {
                    addr.IsDefault = false;
                }
                address.IsDefault = true;
            }
            
            context.Addresses.Add(address);
            context.SaveChanges();
        }

        public Address? Update(Address address)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            var existing = context.Addresses.Find(address.AddressId);
            if (existing != null)
            {
                existing.StreetAddress = address.StreetAddress;
                existing.City = address.City;
                existing.PostalCode = address.PostalCode;
                existing.Country = address.Country;
                existing.AddressType = address.AddressType;
                
                if (address.IsDefault && !existing.IsDefault)
                {
                    SetAsDefault(address.AddressId, existing.UserProfileId);
                }
                else
                {
                    existing.IsDefault = address.IsDefault;
                }
                
                context.SaveChanges();
            }
            return existing;
        }

        public void Delete(int id)
        {
            var address = context.Addresses.Find(id);
            if (address != null)
            {
                context.Addresses.Remove(address);
                context.SaveChanges();
            }
        }

        public void SetAsDefault(int addressId, int userProfileId)
        {
            // Retirer le défaut de toutes les adresses de l'utilisateur
            var addresses = context.Addresses.Where(a => a.UserProfileId == userProfileId);
            foreach (var addr in addresses)
            {
                addr.IsDefault = false;
            }
            
            // Définir la nouvelle adresse par défaut
            var address = context.Addresses.Find(addressId);
            if (address != null)
            {
                address.IsDefault = true;
            }
            
            context.SaveChanges();
        }
    }
}



