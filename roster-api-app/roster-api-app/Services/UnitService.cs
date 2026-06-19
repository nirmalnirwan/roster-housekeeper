using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class UnitService : IUnitService
{
    private readonly IAreaRepository _repository;
    private readonly IFloorRepository _floorRepository;

    public UnitService(IAreaRepository repository, IFloorRepository floorRepository)
    {
        _repository = repository;
        _floorRepository = floorRepository;
    }

    public async Task<IEnumerable<UnitDto>> GetAllAsync()
    {
        var units = await _repository.GetUnitsAsync();
        return units.Select(ToDto);
    }

    public async Task<IEnumerable<UnitDto>> GetByFloorAsync(int floorId)
    {
        var units = await _repository.GetUnitsByFloorAsync(floorId);
        return units.Select(ToDto);
    }

    public async Task<UnitDto?> GetByIdAsync(int id)
    {
        var unit = await _repository.GetUnitByIdAsync(id);
        return unit == null ? null : ToDto(unit);
    }

    public async Task<UnitDto> CreateAsync(UnitRequestDto dto)
    {
        Normalize(dto);
        await ValidateAsync(dto);

        var unit = new Unit
        {
            Name = dto.Name,
            UnitNumber = dto.UnitNumber,
            Notes = dto.Notes,
            FloorId = dto.FloorId
        };

        await _repository.AddUnitAsync(unit);
        return ToDto((await _repository.GetUnitByIdAsync(unit.Id))!);
    }

    public async Task UpdateAsync(int id, UnitRequestDto dto)
    {
        var unit = await _repository.GetUnitByIdAsync(id);
        if (unit == null) throw new KeyNotFoundException("Unit not found");

        Normalize(dto);
        await ValidateAsync(dto, id);

        unit.Name = dto.Name;
        unit.UnitNumber = dto.UnitNumber;
        unit.Notes = dto.Notes;
        unit.FloorId = dto.FloorId;
        await _repository.UpdateUnitAsync(unit);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteUnitAsync(id);
    }

    public async Task<IEnumerable<AssignableAreaDto>> GetAssignableAreasAsync()
    {
        var units = await _repository.GetUnitsAsync();
        return units.Select(ToAssignableDto);
    }

    internal static UnitDto ToDto(Unit unit)
    {
        return new UnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            UnitNumber = unit.UnitNumber,
            Notes = unit.Notes,
            FloorId = unit.FloorId,
            FloorName = unit.Floor?.Name ?? string.Empty,
            BuildingBlockId = unit.Floor?.BuildingBlockId ?? 0,
            BuildingBlockName = unit.Floor?.BuildingBlock?.Name ?? string.Empty,
            LocationTypeId = unit.Floor?.BuildingBlock?.LocationTypeId ?? 0,
            LocationTypeName = unit.Floor?.BuildingBlock?.LocationType?.Name ?? string.Empty
        };
    }

    internal static AssignableAreaDto ToAssignableDto(Unit unit)
    {
        return new AssignableAreaDto
        {
            Id = unit.Id,
            Name = unit.Name,
            Number = unit.UnitNumber,
            AreaType = "Unit",
            FloorId = unit.FloorId,
            FloorName = unit.Floor?.Name ?? string.Empty,
            BuildingBlockId = unit.Floor?.BuildingBlockId ?? 0,
            BuildingBlockName = unit.Floor?.BuildingBlock?.Name ?? string.Empty,
            LocationTypeId = unit.Floor?.BuildingBlock?.LocationTypeId ?? 0,
            LocationTypeName = unit.Floor?.BuildingBlock?.LocationType?.Name ?? string.Empty
        };
    }

    private async Task ValidateAsync(UnitRequestDto dto, int? excludeId = null)
    {
        if (await _floorRepository.GetByIdAsync(dto.FloorId) == null)
            throw new InvalidOperationException("Unit must belong to an existing floor.");

        if (await _repository.UnitNumberExistsAsync(dto.FloorId, dto.UnitNumber, excludeId))
            throw new InvalidOperationException("A unit with this number already exists on this floor.");
    }

    private static void Normalize(UnitRequestDto dto)
    {
        dto.Name = dto.Name.Trim();
        dto.UnitNumber = dto.UnitNumber.Trim();
        dto.Notes = dto.Notes.Trim();
    }
}
