using System.Linq;
using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class HousekeeperService : IHousekeeperService
{
    private readonly IHousekeeperRepository _repository;

    public HousekeeperService(IHousekeeperRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HousekeeperDto>> GetAllAsync()
    {
        var housekeepers = await _repository.GetAllAsync();
        return housekeepers.Select(h => new HousekeeperDto
        {
            Id = h.Id,
            Name = h.Name,
            Phone = h.Phone,
            Email = h.Email,
            Status = h.Status,
            EmploymentType = h.EmploymentType
        });
    }

    public async Task<HousekeeperDto?> GetByIdAsync(int id)
    {
        var housekeeper = await _repository.GetByIdAsync(id);
        if (housekeeper == null) return null;
        return new HousekeeperDto
        {
            Id = housekeeper.Id,
            Name = housekeeper.Name,
            Phone = housekeeper.Phone,
            Email = housekeeper.Email,
            Status = housekeeper.Status,
            EmploymentType = housekeeper.EmploymentType
        };
    }

    public async Task<HousekeeperDto> CreateAsync(HousekeeperDto dto)
    {
        var housekeeper = new Housekeeper
        {
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            Status = dto.Status,
            EmploymentType = dto.EmploymentType
        };
        await _repository.AddAsync(housekeeper);
        dto.Id = housekeeper.Id;
        return dto;
    }

    public async Task UpdateAsync(int id, HousekeeperDto dto)
    {
        var housekeeper = await _repository.GetByIdAsync(id);
        if (housekeeper == null) throw new KeyNotFoundException("Housekeeper not found");
        housekeeper.Name = dto.Name;
        housekeeper.Phone = dto.Phone;
        housekeeper.Email = dto.Email;
        housekeeper.Status = dto.Status;
        housekeeper.EmploymentType = dto.EmploymentType;
        await _repository.UpdateAsync(housekeeper);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}