using roster_api_app.DTOs;
using roster_api_app.Entities;
using roster_api_app.Repositories;

namespace roster_api_app.Services;

public class CleaningTaskService : ICleaningTaskService
{
    private readonly ICleaningTaskRepository _repository;

    public CleaningTaskService(ICleaningTaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CleaningTaskDto>> GetAllAsync()
    {
        var tasks = await _repository.GetAllAsync();
        return tasks.Select(ToDto);
    }

    public async Task<CleaningTaskDto?> GetByIdAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);
        return task == null ? null : ToDto(task);
    }

    public async Task<CleaningTaskDto> CreateAsync(CleaningTaskRequestDto dto)
    {
        Normalize(dto);

        if (await _repository.NameExistsAsync(dto.Name))
        {
            throw new InvalidOperationException("A cleaning task with this name already exists.");
        }

        var task = new CleaningTask
        {
            Name = dto.Name,
            Description = dto.Description,
            TaskCategory = dto.TaskCategory!.Value,
            EstimatedDuration = dto.EstimatedDuration,
            Frequency = dto.Frequency
        };

        await _repository.AddAsync(task);
        return ToDto(task);
    }

    public async Task UpdateAsync(int id, CleaningTaskRequestDto dto)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task == null) throw new KeyNotFoundException("Cleaning task not found");

        Normalize(dto);

        if (await _repository.NameExistsAsync(dto.Name, id))
        {
            throw new InvalidOperationException("A cleaning task with this name already exists.");
        }

        task.Name = dto.Name;
        task.Description = dto.Description;
        task.TaskCategory = dto.TaskCategory!.Value;
        task.EstimatedDuration = dto.EstimatedDuration;
        task.Frequency = dto.Frequency;

        await _repository.UpdateAsync(task);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private static CleaningTaskDto ToDto(CleaningTask task)
    {
        return new CleaningTaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            TaskCategory = task.TaskCategory,
            EstimatedDuration = task.EstimatedDuration,
            Frequency = task.Frequency
        };
    }

    private static void Normalize(CleaningTaskRequestDto dto)
    {
        dto.Name = dto.Name.Trim();
        dto.Description = dto.Description.Trim();
        dto.Frequency = dto.Frequency.Trim();
    }
}
