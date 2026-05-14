namespace TestBoard.API.Models;

public sealed class TaskItemDto
{
    public int Id { get; init; }

    public required string Description { get; init; }

    public required string Priority { get; init; }

    public required string Status { get; init; }

    public DateTime DueDate { get; init; }

    public required string AssignedTo { get; init; }
}
