using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface IHousekeeperService
{
    Task<IEnumerable<HousekeeperDto>> GetAllAsync();
    Task<HousekeeperDto?> GetByIdAsync(int id);
    Task<HousekeeperDto> CreateAsync(HousekeeperDto dto);
    Task UpdateAsync(int id, HousekeeperDto dto);
    Task DeleteAsync(int id);
}