using roster_api_app.Entities;

namespace roster_api_app.Repositories;

public interface IAreaRepository
{
    Task<IEnumerable<CommonArea>> GetCommonAreasAsync();
    Task<IEnumerable<CommonArea>> GetCommonAreasByFloorAsync(int floorId);
    Task<CommonArea?> GetCommonAreaByIdAsync(int id);
    Task<bool> CommonAreaNameExistsAsync(int floorId, string name, int? excludeId = null);
    Task AddCommonAreaAsync(CommonArea commonArea);
    Task UpdateCommonAreaAsync(CommonArea commonArea);
    Task DeleteCommonAreaAsync(int id);

    Task<IEnumerable<Unit>> GetUnitsAsync();
    Task<IEnumerable<Unit>> GetUnitsByFloorAsync(int floorId);
    Task<Unit?> GetUnitByIdAsync(int id);
    Task<bool> UnitNumberExistsAsync(int floorId, string unitNumber, int? excludeId = null);
    Task AddUnitAsync(Unit unit);
    Task UpdateUnitAsync(Unit unit);
    Task DeleteUnitAsync(int id);

    Task<IEnumerable<Apartment>> GetApartmentsAsync();
    Task<IEnumerable<Apartment>> GetApartmentsByFloorAsync(int floorId);
    Task<Apartment?> GetApartmentByIdAsync(int id);
    Task<bool> ApartmentNumberExistsAsync(int floorId, string apartmentNumber, int? excludeId = null);
    Task AddApartmentAsync(Apartment apartment);
    Task UpdateApartmentAsync(Apartment apartment);
    Task DeleteApartmentAsync(int id);
}
