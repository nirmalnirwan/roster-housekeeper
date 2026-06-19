using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface ICommonAreaService
{
    Task<IEnumerable<CommonAreaDto>> GetAllAsync();
    Task<IEnumerable<CommonAreaDto>> GetByFloorAsync(int floorId);
    Task<CommonAreaDto?> GetByIdAsync(int id);
    Task<CommonAreaDto> CreateAsync(CommonAreaRequestDto dto);
    Task UpdateAsync(int id, CommonAreaRequestDto dto);
    Task DeleteAsync(int id);
}
