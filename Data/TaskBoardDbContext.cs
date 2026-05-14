using Microsoft.EntityFrameworkCore;
using TestBoard.API.Models;

namespace TestBoard.API.Data;

public sealed class TaskBoardDbContext(DbContextOptions<TaskBoardDbContext> options) : DbContext(options)
{
    public DbSet<TaskItemEntity> Tasks => Set<TaskItemEntity>();
}
