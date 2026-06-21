using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Entities.Enums;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class ApartmentService : IApartmentService
{
    private const string VillageUnitName = "Village Unit";
    private readonly IAreaRepository _repository;
    private readonly IFloorRepository _floorRepository;
    private readonly ICleaningTaskRepository _cleaningTaskRepository;

    public ApartmentService(
        IAreaRepository repository,
        IFloorRepository floorRepository,
        ICleaningTaskRepository cleaningTaskRepository)
    {
        _repository = repository;
        _floorRepository = floorRepository;
        _cleaningTaskRepository = cleaningTaskRepository;
    }

    public async Task<IEnumerable<ApartmentDto>> GetAllAsync()
    {
        var apartments = await _repository.GetApartmentsAsync();
        return apartments.Select(ToDto);
    }

    public async Task<IEnumerable<ApartmentDto>> GetByFloorAsync(int floorId)
    {
        var apartments = await _repository.GetApartmentsByFloorAsync(floorId);
        return apartments.Select(ToDto);
    }

    public async Task<ApartmentDto?> GetByIdAsync(int id)
    {
        var apartment = await _repository.GetApartmentByIdAsync(id);
        return apartment == null ? null : ToDto(apartment);
    }

    public async Task<ApartmentDto> CreateAsync(ApartmentRequestDto dto)
    {
        Normalize(dto);
        await ValidateAsync(dto);

        var apartment = new Apartment
        {
            Name = dto.Name,
            ApartmentNumber = dto.ApartmentNumber,
            Notes = dto.Notes,
            FloorId = dto.FloorId,
            CleaningTaskId = dto.CleaningTaskId
        };

        await _repository.AddApartmentAsync(apartment);
        return ToDto((await _repository.GetApartmentByIdAsync(apartment.Id))!);
    }

    public async Task UpdateAsync(int id, ApartmentRequestDto dto)
    {
        var apartment = await _repository.GetApartmentByIdAsync(id);
        if (apartment == null) throw new KeyNotFoundException("Apartment not found");

        Normalize(dto);
        await ValidateAsync(dto, id);

        apartment.Name = dto.Name;
        apartment.ApartmentNumber = dto.ApartmentNumber;
        apartment.Notes = dto.Notes;
        apartment.FloorId = dto.FloorId;
        apartment.CleaningTaskId = dto.CleaningTaskId;
        await _repository.UpdateApartmentAsync(apartment);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteApartmentAsync(id);
    }

    internal static ApartmentDto ToDto(Apartment apartment)
    {
        var resident = apartment.Residents.OrderBy(resident => resident.Id).FirstOrDefault();
        return new ApartmentDto
        {
            Id = apartment.Id,
            Name = apartment.Name,
            ApartmentNumber = apartment.ApartmentNumber,
            Notes = apartment.Notes,
            FloorId = apartment.FloorId,
            FloorName = apartment.Floor?.Name ?? string.Empty,
            BuildingBlockId = apartment.Floor?.BuildingBlockId ?? 0,
            BuildingBlockName = apartment.Floor?.BuildingBlock?.Name ?? string.Empty,
            LocationTypeId = apartment.Floor?.BuildingBlock?.LocationTypeId ?? 0,
            LocationTypeName = apartment.Floor?.BuildingBlock?.LocationType?.Name ?? string.Empty,
            CleaningTaskId = apartment.CleaningTaskId,
            CleaningTaskName = apartment.CleaningTask?.Name,
            ResidentId = resident?.Id,
            ResidentName = resident?.Name
        };
    }

    internal static AssignableAreaDto ToAssignableDto(Apartment apartment)
    {
        return new AssignableAreaDto
        {
            Id = apartment.Id,
            Name = apartment.Name,
            Number = apartment.ApartmentNumber,
            AreaType = "Apartment",
            FloorId = apartment.FloorId,
            FloorName = apartment.Floor?.Name ?? string.Empty,
            BuildingBlockId = apartment.Floor?.BuildingBlockId ?? 0,
            BuildingBlockName = apartment.Floor?.BuildingBlock?.Name ?? string.Empty,
            LocationTypeId = apartment.Floor?.BuildingBlock?.LocationTypeId ?? 0,
            LocationTypeName = apartment.Floor?.BuildingBlock?.LocationType?.Name ?? string.Empty
        };
    }

    private async Task ValidateAsync(ApartmentRequestDto dto, int? excludeId = null)
    {
        var floor = await _floorRepository.GetByIdAsync(dto.FloorId);
        if (floor == null)
            throw new InvalidOperationException("Apartment must belong to an existing floor.");

        if (!string.Equals(floor.BuildingBlock.LocationType.Name, VillageUnitName, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Apartments can only be created under Village Unit floors.");

        if (await _repository.ApartmentNumberExistsAsync(dto.FloorId, dto.ApartmentNumber, excludeId))
            throw new InvalidOperationException("An apartment with this number already exists on this floor.");

        var task = dto.CleaningTaskId.HasValue
            ? await _cleaningTaskRepository.GetByIdAsync(dto.CleaningTaskId.Value)
            : null;
        if (task == null)
            throw new InvalidOperationException("Apartment must have an existing cleaning task.");

        if (task.TaskCategory != CleaningTaskCategory.Apartment && task.TaskCategory != CleaningTaskCategory.SpecialTask)
            throw new InvalidOperationException("Apartments can only use Apartment or Special Task cleaning tasks.");
    }

    private static void Normalize(ApartmentRequestDto dto)
    {
        dto.Name = dto.Name.Trim();
        dto.ApartmentNumber = dto.ApartmentNumber.Trim();
        dto.Notes = dto.Notes.Trim();
    }
}
