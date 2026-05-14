# TestBoard.API

Azure Functions (.NET 8) backend for a JIRA-style task board application using an in-memory database.

## Available operations

- `POST /api/tasks`
- `PUT /api/tasks/{id}`
- `PATCH /api/tasks/{id}/status`
- `GET /api/tasks`
- `GET /api/tasks/assignee/{assignedTo}`
- `DELETE /api/tasks/{id}`

## Local run

```bash
dotnet restore
dotnet build
func start
```

Swagger UI is exposed by the Azure Functions OpenAPI extension when the app is running.
