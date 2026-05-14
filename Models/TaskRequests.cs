namespace TestBoard.API.Models;

public sealed class CreateTaskRequest
{
    public string? Description { get; init; }

    public string? Priority { get; init; }

    public string? Status { get; init; }

    public DateTime? DueDate { get; init; }

    public string? AssignedTo { get; init; }
}

public sealed class UpdateTaskRequest
{
    public string? Description { get; init; }

    public string? Priority { get; init; }

    public string? Status { get; init; }

    public DateTime? DueDate { get; init; }

    public string? AssignedTo { get; init; }
}

public sealed class UpdateTaskStatusRequest
{
    public string? Status { get; init; }
}
