using Chronicle.Data;
using Chronicle.Models.Domain;
using Chronicle.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chonicle.Repositories;

public class CareerRequestRepository : ICareerRequestRepository
{
    private readonly ChronicleDbContext _chronicleDbContext;
    public CareerRequestRepository(ChronicleDbContext _chronicleDbContext)
    {
        this._chronicleDbContext = _chronicleDbContext;
    }
    public async Task<CareerRequest> AddAsync(CareerRequest careerRequest)
    {
        await _chronicleDbContext.CareerRequests.AddAsync(careerRequest);
        await _chronicleDbContext.SaveChangesAsync();
        return careerRequest;
    }

    public async Task<IEnumerable<CareerRequest>> FindByIsCheckedAsync(bool isChecked)
    {
        return await _chronicleDbContext.CareerRequests
                .Where(c => c.IsChecked == isChecked)
                .Include(c => c.User)
                .Include(c => c.Role)
                .ToListAsync();
    }

    public async Task<IEnumerable<CareerRequest>> GetAllAsync()
    {
        return await _chronicleDbContext.CareerRequests
            .Include(c => c.User)
            .Include(c => c.Role)
            .ToListAsync();
    }

    public async Task<CareerRequest?> GetAsync(long id)
    {
        return await _chronicleDbContext.CareerRequests
            .Include(c => c.User)
            .Include(c => c.Role)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<CareerRequest>> GetByUserIdAsync(string userId)
    {
        return await _chronicleDbContext.CareerRequests
                .Where(c => c.UserId == userId)
                .ToListAsync();
    }

    public async Task<CareerRequest?> UpdateAsync(CareerRequest careerRequest)
    {
        var existingCareer = await _chronicleDbContext.CareerRequests.FindAsync(careerRequest.Id);
        if(existingCareer != null)
        {
            existingCareer.IsChecked = careerRequest.IsChecked;
            existingCareer.IsApproved = careerRequest.IsApproved;
            await _chronicleDbContext.SaveChangesAsync();
            return existingCareer;
        }
        return null;
    }
}