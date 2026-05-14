using Microsoft.Extensions.DependencyInjection;
using TestBoard.API.Models;

namespace TestBoard.API.Data;

public static class TaskBoardSeedData
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();

        if (dbContext.Tasks.Any())
        {
            return;
        }

        var startDate = DateTime.UtcNow.Date;
        var assignees = new[]
        {
            "Alex", "Jordan", "Priya", "Sam", "Taylor",
            "Morgan", "Casey", "Riley", "Jamie", "Avery"
        };
        var priorities = new[] { "Low", "Medium", "High", "Critical" };
        var statuses = new[] { "To Do", "In Progress", "Done", "Blocked" };

        var sampleTasks = Enumerable.Range(1, 20)
            .Select(index => new TaskItemEntity
            {
                Description = $"Sample task {index}: Complete backlog item #{index}",
                Priority = priorities[(index - 1) % priorities.Length],
                Status = statuses[(index - 1) % statuses.Length],
                DueDate = startDate.AddDays(index),
                AssignedTo = assignees[(index - 1) % assignees.Length]
            })
            .ToList();

        await dbContext.Tasks.AddRangeAsync(sampleTasks);
        await dbContext.SaveChangesAsync();
    }
}
