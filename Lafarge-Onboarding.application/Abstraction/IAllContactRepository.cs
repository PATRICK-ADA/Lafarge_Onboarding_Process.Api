namespace Lafarge_Onboarding.application.Abstraction;

public interface IAllContactRepository
{
    Task AddRangeAsync(IEnumerable<AllContact> contacts);
    Task<List<AllContact>> GetAllAsync();
    Task DeleteAllAsync();
}