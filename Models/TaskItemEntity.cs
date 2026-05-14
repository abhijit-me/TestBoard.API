namespace TestBoard.API.Models;

public sealed class TaskItemEntity
{
    public int Id { get; set; }

    public required string Description { get; set; }

    public required string Priority { get; set; }

    public required string Status { get; set; }

    public DateTime DueDate { get; set; }

    public required string AssignedTo { get; set; }
}
