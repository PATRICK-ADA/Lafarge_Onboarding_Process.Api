namespace Lafarge_Onboarding.application.Abstraction;

public interface ILocalHireInfoRepository
{
    Task AddAsync(LocalHireInfo localHireInfo);
    Task<LocalHireInfo?> GetLatestAsync();
    Task UpdateAsync(LocalHireInfo localHireInfo);
    Task DeleteLatestAsync();
}