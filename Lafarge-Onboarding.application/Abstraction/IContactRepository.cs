namespace Lafarge_Onboarding.application.Abstraction;

using Lafarge_Onboarding.domain.Entities;

public interface IContactRepository
{
    Task AddRangeAsync(IEnumerable<Contact> contacts);
    Task<List<Contact>> GetAllAsync();
}