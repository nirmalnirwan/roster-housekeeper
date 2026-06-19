using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class CommonAreaService : ICommonAreaService
{
    private readonly IAreaRepository _repository;
    private readonly IFloorRepository _floorRepository;

    public CommonAreaService(IAreaRepository repository, IFloorRepository floorRepository)
    {
        _repository = repository;
        _floorRepository = floorRepository;
    }

    public async Task<IEnumerable<CommonAreaDto>> GetAllAsync()
    {
        var areas = await _repository.GetCommonAreasAsync();
        return areas.Select(ToDto);
    }

    public async Task<IEnumerable<CommonAreaDto>> GetByFloorAsync(int floorId)
    {
        var areas = await _repository.GetCommonAreasByFloorAsync(floorId);
        return areas.Select(ToDto);
    }

    public async Task<CommonAreaDto?> GetByIdAsync(int id)
    {
        var area = await _repository.GetCommonAreaByIdAsync(id);
        return area == null ? null : ToDto(area);
    }

    public async Task<CommonAreaDto> CreateAsync(CommonAreaRequestDto dto)
    {
        Normalize(dto);
        await ValidateAsync(dto);

        var area = new CommonArea
        {
            Name = dto.Name,
            Description = dto.Description,
            FloorId = dto.FloorId
        };

        await _repository.AddCommonAreaAsync(area);
        return ToDto((await _repository.GetCommonAreaByIdAsync(area.Id))!);
    }

    public async Task UpdateAsync(int id, CommonAreaRequestDto dto)
    {
        var area = await _repository.GetCommonAreaByIdAsync(id);
        if (area == null) throw new KeyNotFoundException("Common area not found");

        Normalize(dto);
        await ValidateAsync(dto, id);

        area.Name = dto.Name;
        area.Description = dto.Description;
        area.FloorId = dto.FloorId;
        await _repository.UpdateCommonAreaAsync(area);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteCommonAreaAsync(id);
    }

    internal static CommonAreaDto ToDto(CommonArea area)
    {
        return new CommonAreaDto
        {
            Id = area.Id,
            Name = area.Name,
            Description = area.Description,
            FloorId = area.FloorId,
            FloorName = area.Floor?.Name ?? string.Empty,
            BuildingBlockId = area.Floor?.BuildingBlockId ?? 0,
            BuildingBlockName = area.Floor?.BuildingBlock?.Name ?? string.Empty,
            LocationTypeId = area.Floor?.BuildingBlock?.LocationTypeId ?? 0,
            LocationTypeName = area.Floor?.BuildingBlock?.LocationType?.Name ?? string.Empty
        };
    }

    private async Task ValidateAsync(CommonAreaRequestDto dto, int? excludeId = null)
    {
        if (await _floorRepository.GetByIdAsync(dto.FloorId) == null)
            throw new InvalidOperationException("Common area must belong to an existing floor.");

        if (await _repository.CommonAreaNameExistsAsync(dto.FloorId, dto.Name, excludeId))
            throw new InvalidOperationException("A common area with this name already exists on this floor.");
    }

    private static void Normalize(CommonAreaRequestDto dto)
    {
        dto.Name = dto.Name.Trim();
        dto.Description = dto.Description.Trim();
    }
}
