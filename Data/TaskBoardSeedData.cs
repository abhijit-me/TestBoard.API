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

        var descriptions = new[]
        {
            "Set up project repository and configure CI/CD pipeline",
            "Design wireframes and UI mockups for the main dashboard",
            "Implement user authentication with JWT and refresh tokens",
            "Create REST API endpoints for user profile management",
            "Build responsive navigation bar and sidebar components",
            "Integrate third-party payment gateway for subscription plans",
            "Write unit tests for authentication and authorisation logic",
            "Optimise database queries and add missing indexes",
            "Implement file upload functionality with cloud storage integration",
            "Build notification system with real-time WebSocket support",
            "Add role-based access control for admin and standard users",
            "Create end-to-end tests for the checkout and payment flow",
            "Implement search and filtering on the products listing page",
            "Set up error monitoring and structured logging with Serilog",
            "Build email template system and integrate SMTP provider",
            "Migrate legacy API endpoints to the new versioned route structure",
            "Implement pagination and infinite scroll on the activity feed",
            "Conduct accessibility audit and fix WCAG 2.1 AA violations",
            "Configure production environment variables and secrets management",
            "Write API documentation and update the OpenAPI specification"
        };

        var sampleTasks = Enumerable.Range(1, 20)
            .Select(index => new TaskItemEntity
            {
                Description = descriptions[index - 1],
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
