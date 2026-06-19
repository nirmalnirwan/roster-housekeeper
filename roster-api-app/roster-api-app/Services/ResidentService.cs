using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class ResidentService : IResidentService
{
    private readonly IResidentRepository _repository;
    private readonly IAreaRepository _areaRepository;

    public ResidentService(IResidentRepository repository, IAreaRepository areaRepository)
    {
        _repository = repository;
        _areaRepository = areaRepository;
    }

    public async Task<IEnumerable<ResidentDto>> GetAllAsync()
    {
        var residents = await _repository.GetAllAsync();
        return residents.Select(ToDto);
    }

    public async Task<ResidentDto?> GetByIdAsync(int id)
    {
        var resident = await _repository.GetByIdAsync(id);
        return resident == null ? null : ToDto(resident);
    }

    public async Task<ResidentDto> CreateAsync(ResidentRequestDto dto)
    {
        Normalize(dto);
        await ValidateAssignmentAsync(dto);

        var resident = new Resident
        {
            Name = dto.Name,
            RoomNumber = dto.RoomNumber,
            Building = dto.Building,
            CleaningFrequency = dto.CleaningFrequency,
            Notes = dto.Notes,
            UnitId = dto.UnitId,
            ApartmentId = dto.ApartmentId
        };

        await _repository.AddAsync(resident);
        return ToDto((await _repository.GetByIdAsync(resident.Id))!);
    }

    public async Task UpdateAsync(int id, ResidentRequestDto dto)
    {
        var resident = await _repository.GetByIdAsync(id);
        if (resident == null) throw new KeyNotFoundException("Resident not found");

        Normalize(dto);
        await ValidateAssignmentAsync(dto);

        resident.Name = dto.Name;
        resident.RoomNumber = dto.RoomNumber;
        resident.Building = dto.Building;
        resident.CleaningFrequency = dto.CleaningFrequency;
        resident.Notes = dto.Notes;
        resident.UnitId = dto.UnitId;
        resident.ApartmentId = dto.ApartmentId;
        await _repository.UpdateAsync(resident);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<AssignableAreaDto>> GetAssignableAreasAsync()
    {
        var units = (await _areaRepository.GetUnitsAsync()).Select(UnitService.ToAssignableDto);
        var apartments = (await _areaRepository.GetApartmentsAsync()).Select(ApartmentService.ToAssignableDto);
        return units.Concat(apartments)
            .OrderBy(area => area.LocationTypeName)
            .ThenBy(area => area.BuildingBlockName)
            .ThenBy(area => area.FloorName)
            .ThenBy(area => area.AreaType)
            .ThenBy(area => area.Number);
    }

    internal static ResidentDto ToDto(Resident resident)
    {
        var isUnit = resident.UnitId.HasValue;
        var isApartment = resident.ApartmentId.HasValue;
        return new ResidentDto
        {
            Id = resident.Id,
            Name = resident.Name,
            RoomNumber = resident.RoomNumber,
            Building = resident.Building,
            CleaningFrequency = resident.CleaningFrequency,
            Notes = resident.Notes,
            UnitId = resident.UnitId,
            UnitName = resident.Unit?.Name,
            ApartmentId = resident.ApartmentId,
            ApartmentName = resident.Apartment?.Name,
            AssignmentType = isUnit ? "Unit" : isApartment ? "Apartment" : "Unassigned",
            AssignmentName = isUnit
                ? $"{resident.Unit?.Name} ({resident.Unit?.UnitNumber})"
                : isApartment
                    ? $"{resident.Apartment?.Name} ({resident.Apartment?.ApartmentNumber})"
                    : "Unassigned"
        };
    }

    private async Task ValidateAssignmentAsync(ResidentRequestDto dto)
    {
        var hasUnit = dto.UnitId.HasValue;
        var hasApartment = dto.ApartmentId.HasValue;

        if (hasUnit == hasApartment)
            throw new InvalidOperationException("Resident must be assigned to either a unit or an apartment.");

        if (hasUnit && await _areaRepository.GetUnitByIdAsync(dto.UnitId!.Value) == null)
            throw new InvalidOperationException("Selected unit does not exist.");

        if (hasApartment && await _areaRepository.GetApartmentByIdAsync(dto.ApartmentId!.Value) == null)
            throw new InvalidOperationException("Selected apartment does not exist.");
    }

    private static void Normalize(ResidentRequestDto dto)
    {
        dto.Name = dto.Name.Trim();
        dto.RoomNumber = dto.RoomNumber.Trim();
        dto.Building = dto.Building.Trim();
        dto.CleaningFrequency = dto.CleaningFrequency.Trim();
        dto.Notes = dto.Notes.Trim();
    }
}
