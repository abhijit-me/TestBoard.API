using TestBoard.API.Models;

namespace TestBoard.API.Services;

public interface ITaskService
{
    Task<TaskItemDto> CreateAsync(CreateTaskRequest request);

    Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskRequest request);

    Task<TaskItemDto?> UpdateStatusAsync(int id, UpdateTaskStatusRequest request);

    Task<IReadOnlyList<TaskItemDto>> GetAllAsync();

    Task<IReadOnlyList<TaskItemDto>> GetByAssigneeAsync(string assignedTo);

    Task<IReadOnlyList<TaskItemDto>> SearchByDescriptionAsync(string description);

    Task<bool> DeleteAsync(int id);
}
