using roster_api_app.DTOs;

namespace roster_api_app.Services;

public interface IRosterService
{
    Task<IEnumerable<RosterDto>> GetAllAsync();
    Task<RosterDto?> GetByIdAsync(int id);
    Task<RosterDto?> GetByHousekeeperAndWeekAsync(int housekeeperId, DateTime weekStartDate);
    Task<RosterDto> CreateAsync(RosterDto dto);
    Task UpdateAsync(int id, RosterDto dto);
    Task DeleteAsync(int id);
    Task<byte[]> ExportPdfAsync(int rosterId);
    Task<byte[]> ExportExcelAsync(int rosterId);
}
