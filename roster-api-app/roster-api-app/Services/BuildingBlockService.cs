using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class BuildingBlockService : IBuildingBlockService
{
    private readonly IBuildingBlockRepository _repository;
    private readonly ILocationTypeRepository _locationTypeRepository;

    public BuildingBlockService(
        IBuildingBlockRepository repository,
        ILocationTypeRepository locationTypeRepository)
    {
        _repository = repository;
        _locationTypeRepository = locationTypeRepository;
    }

    public async Task<IEnumerable<BuildingBlockDto>> GetAllAsync()
    {
        var buildingBlocks = await _repository.GetAllAsync();
        return buildingBlocks.Select(ToDto);
    }

    public async Task<BuildingBlockDto?> GetByIdAsync(int id)
    {
        var buildingBlock = await _repository.GetByIdAsync(id);
        return buildingBlock == null ? null : ToDto(buildingBlock);
    }

    public async Task<BuildingBlockDto> CreateAsync(BuildingBlockRequestDto dto)
    {
        Normalize(dto);
        await ValidateAsync(dto);

        var buildingBlock = new BuildingBlock
        {
            Name = dto.Name,
            LocationTypeId = dto.LocationTypeId
        };

        await _repository.AddAsync(buildingBlock);
        return ToDto((await _repository.GetByIdAsync(buildingBlock.Id))!);
    }

    public async Task UpdateAsync(int id, BuildingBlockRequestDto dto)
    {
        var buildingBlock = await _repository.GetByIdAsync(id);
        if (buildingBlock == null) throw new KeyNotFoundException("Building block not found");

        Normalize(dto);
        await ValidateAsync(dto, id);

        buildingBlock.Name = dto.Name;
        buildingBlock.LocationTypeId = dto.LocationTypeId;
        await _repository.UpdateAsync(buildingBlock);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    internal static BuildingBlockDto ToDto(BuildingBlock buildingBlock)
    {
        return new BuildingBlockDto
        {
            Id = buildingBlock.Id,
            Name = buildingBlock.Name,
            LocationTypeId = buildingBlock.LocationTypeId,
            LocationTypeName = buildingBlock.LocationType?.Name ?? string.Empty,
            Floors = buildingBlock.Floors
                .OrderBy(f => f.FloorNumber)
                .ThenBy(f => f.Name)
                .Select(FloorService.ToDto)
                .ToList()
        };
    }

    private async Task ValidateAsync(BuildingBlockRequestDto dto, int? excludeId = null)
    {
        if (await _locationTypeRepository.GetByIdAsync(dto.LocationTypeId) == null)
            throw new InvalidOperationException("Building block must belong to an existing location type.");

        if (await _repository.NameExistsAsync(dto.LocationTypeId, dto.Name, excludeId))
            throw new InvalidOperationException("A building block with this name already exists for this location type.");
    }

    private static void Normalize(BuildingBlockRequestDto dto)
    {
        dto.Name = dto.Name.Trim();
    }
}
