using Microsoft.EntityFrameworkCore;
using TestBoard.API.Data;
using TestBoard.API.Models;

namespace TestBoard.API.Services;

public sealed class TaskService(TaskBoardDbContext dbContext) : ITaskService
{
    private static readonly string[] AllowedPriorities = ["Low", "Medium", "High", "Critical"];
    private static readonly string[] AllowedStatuses = ["To Do", "In Progress", "Done", "Blocked"];

    public async Task<TaskItemDto> CreateAsync(CreateTaskRequest request)
    {
        var entity = new TaskItemEntity
        {
            Description = ValidateRequired(request.Description, nameof(request.Description)),
            Priority = NormalizeValue(request.Priority, AllowedPriorities, nameof(request.Priority)),
            Status = NormalizeValue(request.Status, AllowedStatuses, nameof(request.Status)),
            DueDate = ValidateDueDate(request.DueDate),
            AssignedTo = ValidateRequired(request.AssignedTo, nameof(request.AssignedTo))
        };

        dbContext.Tasks.Add(entity);
        await dbContext.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskRequest request)
    {
        var entity = await dbContext.Tasks.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        entity.Description = ValidateRequired(request.Description, nameof(request.Description));
        entity.Priority = NormalizeValue(request.Priority, AllowedPriorities, nameof(request.Priority));
        entity.Status = NormalizeValue(request.Status, AllowedStatuses, nameof(request.Status));
        entity.DueDate = ValidateDueDate(request.DueDate);
        entity.AssignedTo = ValidateRequired(request.AssignedTo, nameof(request.AssignedTo));

        await dbContext.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<TaskItemDto?> UpdateStatusAsync(int id, UpdateTaskStatusRequest request)
    {
        var entity = await dbContext.Tasks.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        entity.Status = NormalizeValue(request.Status, AllowedStatuses, nameof(request.Status));
        await dbContext.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<IReadOnlyList<TaskItemDto>> GetAllAsync()
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .OrderBy(task => task.Id)
            .Select(task => Map(task))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TaskItemDto>> GetByAssigneeAsync(string assignedTo)
    {
        var normalizedAssignee = ValidateRequired(assignedTo, nameof(assignedTo));

        return await dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.AssignedTo.ToLower() == normalizedAssignee.ToLower())
            .OrderBy(task => task.DueDate)
            .ThenBy(task => task.Id)
            .Select(task => Map(task))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TaskItemDto>> SearchByDescriptionAsync(string description)
    {
        var searchTerm = ValidateRequired(description, nameof(description));

        return await dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.Description.ToLower().Contains(searchTerm.ToLower()))
            .OrderBy(task => task.Id)
            .Select(task => Map(task))
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await dbContext.Tasks.FindAsync(id);
        if (entity is null)
        {
            return false;
        }

        dbContext.Tasks.Remove(entity);
        await dbContext.SaveChangesAsync();
        return true;
    }

    private static TaskItemDto Map(TaskItemEntity entity) => new()
    {
        Id = entity.Id,
        Description = entity.Description,
        Priority = entity.Priority,
        Status = entity.Status,
        DueDate = entity.DueDate,
        AssignedTo = entity.AssignedTo
    };

    private static string ValidateRequired(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.");
        }

        return value.Trim();
    }

    private static DateTime ValidateDueDate(DateTime? dueDate)
    {
        if (!dueDate.HasValue)
        {
            throw new ArgumentException("DueDate is required.");
        }

        return dueDate.Value;
    }

    private static string NormalizeValue(string? value, IEnumerable<string> allowedValues, string fieldName)
    {
        var candidate = ValidateRequired(value, fieldName);
        var normalized = allowedValues.FirstOrDefault(option => option.Equals(candidate, StringComparison.OrdinalIgnoreCase));

        if (normalized is null)
        {
            throw new ArgumentException($"{fieldName} must be one of: {string.Join(", ", allowedValues)}.");
        }

        return normalized;
    }
}
