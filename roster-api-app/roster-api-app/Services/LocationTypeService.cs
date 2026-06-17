using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class LocationTypeService : ILocationTypeService
{
    private readonly ILocationTypeRepository _repository;

    public LocationTypeService(ILocationTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LocationTypeDto>> GetAllAsync()
    {
        var locationTypes = await _repository.GetAllAsync();
        return locationTypes.Select(ToDto);
    }

    public async Task<LocationTypeDto?> GetByIdAsync(int id)
    {
        var locationType = await _repository.GetByIdAsync(id);
        return locationType == null ? null : ToDto(locationType);
    }

    public async Task<LocationTypeDto> CreateAsync(LocationTypeRequestDto dto)
    {
        Normalize(dto);

        if (await _repository.NameExistsAsync(dto.Name))
            throw new InvalidOperationException("A location type with this name already exists.");

        var locationType = new LocationType { Name = dto.Name };
        await _repository.AddAsync(locationType);
        return ToDto(locationType);
    }

    public async Task UpdateAsync(int id, LocationTypeRequestDto dto)
    {
        var locationType = await _repository.GetByIdAsync(id);
        if (locationType == null) throw new KeyNotFoundException("Location type not found");

        Normalize(dto);

        if (await _repository.NameExistsAsync(dto.Name, id))
            throw new InvalidOperationException("A location type with this name already exists.");

        locationType.Name = dto.Name;
        await _repository.UpdateAsync(locationType);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    internal static LocationTypeDto ToDto(LocationType locationType)
    {
        return new LocationTypeDto
        {
            Id = locationType.Id,
            Name = locationType.Name,
            BuildingBlocks = locationType.BuildingBlocks
                .OrderBy(bb => bb.Name)
                .Select(BuildingBlockService.ToDto)
                .ToList()
        };
    }

    private static void Normalize(LocationTypeRequestDto dto)
    {
        dto.Name = dto.Name.Trim();
    }
}
