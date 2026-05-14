using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using TestBoard.API.Models;
using TestBoard.API.Services;

namespace TestBoard.API.Functions;

public sealed class TaskFunctions(ITaskService taskService)
{
    [Function(nameof(CreateTask))]
    [OpenApiOperation(operationId: "CreateTask", tags: new[] { "Tasks" }, Summary = "Create a new task", Description = "Creates a new task board item.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateTaskRequest), Required = true, Description = "Task payload to create")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(TaskItemDto), Summary = "Created task", Description = "The created task item")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid request", Description = "The task payload is invalid")]
    public async Task<HttpResponseData> CreateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks")] HttpRequestData request)
    {
        var payload = await request.ReadFromJsonAsync<CreateTaskRequest>();
        if (payload is null)
        {
            return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, "Request body is required.");
        }

        try
        {
            var createdTask = await taskService.CreateAsync(payload);
            var response = request.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(createdTask);
            return response;
        }
        catch (ArgumentException exception)
        {
            return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, exception.Message);
        }
    }

    [Function(nameof(UpdateTask))]
    [OpenApiOperation(operationId: "UpdateTask", tags: new[] { "Tasks" }, Summary = "Update an existing task", Description = "Updates all task details.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "Task identifier", Description = "The id of the task to update")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateTaskRequest), Required = true, Description = "Updated task payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TaskItemDto), Summary = "Updated task", Description = "The updated task item")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid request", Description = "The task payload is invalid")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Task not found", Description = "No task exists for the supplied id")]
    public async Task<HttpResponseData> UpdateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "tasks/{id:int}")] HttpRequestData request,
        int id)
    {
        var payload = await request.ReadFromJsonAsync<UpdateTaskRequest>();
        if (payload is null)
        {
            return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, "Request body is required.");
        }

        try
        {
            var updatedTask = await taskService.UpdateAsync(id, payload);
            if (updatedTask is null)
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }

            var response = request.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(updatedTask);
            return response;
        }
        catch (ArgumentException exception)
        {
            return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, exception.Message);
        }
    }

    [Function(nameof(UpdateTaskStatus))]
    [OpenApiOperation(operationId: "UpdateTaskStatus", tags: new[] { "Tasks" }, Summary = "Update task status", Description = "Updates only the task status.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "Task identifier", Description = "The id of the task to update")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateTaskStatusRequest), Required = true, Description = "Updated task status payload")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TaskItemDto), Summary = "Updated task", Description = "The task with its updated status")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid request", Description = "The status payload is invalid")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Task not found", Description = "No task exists for the supplied id")]
    public async Task<HttpResponseData> UpdateTaskStatus(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "tasks/{id:int}/status")] HttpRequestData request,
        int id)
    {
        var payload = await request.ReadFromJsonAsync<UpdateTaskStatusRequest>();
        if (payload is null)
        {
            return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, "Request body is required.");
        }

        try
        {
            var updatedTask = await taskService.UpdateStatusAsync(id, payload);
            if (updatedTask is null)
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }

            var response = request.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(updatedTask);
            return response;
        }
        catch (ArgumentException exception)
        {
            return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, exception.Message);
        }
    }

    [Function(nameof(GetAllTasks))]
    [OpenApiOperation(operationId: "GetAllTasks", tags: new[] { "Tasks" }, Summary = "Get all tasks", Description = "Returns all tasks in the task board.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<TaskItemDto>), Summary = "All tasks", Description = "The collection of all task board items")]
    public async Task<HttpResponseData> GetAllTasks(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks")] HttpRequestData request)
    {
        var tasks = await taskService.GetAllAsync();
        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(tasks);
        return response;
    }

    [Function(nameof(GetTaskByAssignee))]
    [OpenApiOperation(operationId: "GetTaskByAssignee", tags: new[] { "Tasks" }, Summary = "Get tasks by assignee", Description = "Returns tasks assigned to a specific user.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "assignedTo", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Assignee name", Description = "The assignee used to filter tasks")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<TaskItemDto>), Summary = "Matching tasks", Description = "The collection of tasks assigned to the user")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid assignee", Description = "The assignee value is required")]
    public async Task<HttpResponseData> GetTaskByAssignee(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks/assignee/{assignedTo}")] HttpRequestData request,
        string assignedTo)
    {
        try
        {
            var tasks = await taskService.GetByAssigneeAsync(assignedTo);
            var response = request.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(tasks);
            return response;
        }
        catch (ArgumentException exception)
        {
            return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, exception.Message);
        }
    }

    [Function(nameof(SearchTaskByDescription))]
    [OpenApiOperation(operationId: "SearchTaskByDescription", tags: new[] { "Tasks" }, Summary = "Search tasks by description", Description = "Returns tasks whose description contains the given search string.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "description", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Search string", Description = "The string to search for within task descriptions")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<TaskItemDto>), Summary = "Matching tasks", Description = "The collection of tasks whose description contains the search string")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid search string", Description = "The description search string is required")]
    public async Task<HttpResponseData> SearchTaskByDescription(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks/search/{description}")] HttpRequestData request,
        string description)
    {
        try
        {
            var tasks = await taskService.SearchByDescriptionAsync(description);
            var response = request.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(tasks);
            return response;
        }
        catch (ArgumentException exception)
        {
            return await CreateErrorResponseAsync(request, HttpStatusCode.BadRequest, exception.Message);
        }
    }

    [Function(nameof(DeleteTask))]
    [OpenApiOperation(operationId: "DeleteTask", tags: new[] { "Tasks" }, Summary = "Delete a task", Description = "Deletes an existing task.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "Task identifier", Description = "The id of the task to delete")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Summary = "Task deleted", Description = "The task was deleted successfully")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Task not found", Description = "No task exists for the supplied id")]
    public async Task<HttpResponseData> DeleteTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "tasks/{id:int}")] HttpRequestData request,
        int id)
    {
        var deleted = await taskService.DeleteAsync(id);
        return request.CreateResponse(deleted ? HttpStatusCode.NoContent : HttpStatusCode.NotFound);
    }

    private static async Task<HttpResponseData> CreateErrorResponseAsync(HttpRequestData request, HttpStatusCode statusCode, string message)
    {
        var response = request.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(new ErrorResponse(message));
        return response;
    }

    private sealed record ErrorResponse(string Error);
}
