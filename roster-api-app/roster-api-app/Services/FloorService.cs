using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class FloorService : IFloorService
{
    private readonly IFloorRepository _repository;
    private readonly IBuildingBlockRepository _buildingBlockRepository;

    public FloorService(IFloorRepository repository, IBuildingBlockRepository buildingBlockRepository)
    {
        _repository = repository;
        _buildingBlockRepository = buildingBlockRepository;
    }

    public async Task<IEnumerable<FloorDto>> GetAllAsync()
    {
        var floors = await _repository.GetAllAsync();
        return floors.Select(ToDto);
    }

    public async Task<FloorDto?> GetByIdAsync(int id)
    {
        var floor = await _repository.GetByIdAsync(id);
        return floor == null ? null : ToDto(floor);
    }

    public async Task<IEnumerable<FloorDto>> GetByBuildingBlockAsync(int buildingBlockId)
    {
        var floors = await _repository.GetByBuildingBlockAsync(buildingBlockId);
        return floors.Select(ToDto);
    }

    public async Task<FloorDto> CreateAsync(FloorRequestDto dto)
    {
        Normalize(dto);
        await ValidateAsync(dto);

        var floor = new Floor
        {
            Name = dto.Name,
            FloorNumber = dto.FloorNumber,
            BuildingBlockId = dto.BuildingBlockId
        };

        await _repository.AddAsync(floor);
        return ToDto((await _repository.GetByIdAsync(floor.Id))!);
    }

    public async Task UpdateAsync(int id, FloorRequestDto dto)
    {
        var floor = await _repository.GetByIdAsync(id);
        if (floor == null) throw new KeyNotFoundException("Floor not found");

        Normalize(dto);
        await ValidateAsync(dto, id);

        floor.Name = dto.Name;
        floor.FloorNumber = dto.FloorNumber;
        floor.BuildingBlockId = dto.BuildingBlockId;
        await _repository.UpdateAsync(floor);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    internal static FloorDto ToDto(Floor floor)
    {
        return new FloorDto
        {
            Id = floor.Id,
            Name = floor.Name,
            FloorNumber = floor.FloorNumber,
            BuildingBlockId = floor.BuildingBlockId,
            BuildingBlockName = floor.BuildingBlock?.Name ?? string.Empty
        };
    }

    private async Task ValidateAsync(FloorRequestDto dto, int? excludeId = null)
    {
        if (!await _buildingBlockRepository.ExistsAsync(dto.BuildingBlockId))
            throw new InvalidOperationException("Floor must belong to an existing building block.");

        if (await _repository.FloorNumberExistsAsync(dto.BuildingBlockId, dto.FloorNumber, excludeId))
            throw new InvalidOperationException("A floor with this floor number already exists for this building block.");
    }

    private static void Normalize(FloorRequestDto dto)
    {
        dto.Name = dto.Name.Trim();
    }
}
