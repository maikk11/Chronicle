using Chronicle.Models.Domain;
namespace Chronicle.Repositories;

public interface ICareerRequestRepository
{
    Task<CareerRequest> AddAsync(CareerRequest careerRequest);
    Task<IEnumerable<CareerRequest>> GetAllAsync();
    Task<IEnumerable<CareerRequest>> FindByIsCheckedAsync(bool isChecked);
    Task<CareerRequest?> GetAsync(long id);
    Task<CareerRequest?> UpdateAsync(CareerRequest careerRequest);
    Task<IEnumerable<CareerRequest>> GetByUserIdAsync(string userId);
}