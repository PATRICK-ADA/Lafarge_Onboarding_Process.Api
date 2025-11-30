namespace Lafarge_Onboarding.application.Abstraction;

public interface IContactRepository
{
    Task AddRangeAsync(IEnumerable<Contact> contacts);
    Task<List<ContactDto>> GetAllAsync();
    Task DeleteAllAsync();
}