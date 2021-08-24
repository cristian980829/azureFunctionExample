using AzureFunctionExample.common.Models;
using AzureFunctionExample.common.Responses;
using AzureFunctionExample.Functions.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureFunctionExample.Functions.Functions
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new todo.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (string.IsNullOrEmpty(todo?.taskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucces = false,
                    Message = "The request must have a TaskDescription."
                });
            }

            TodoEntity todoEntity = new TodoEntity
            {
                CreatedTime = DateTime.UtcNow,
                ETag = "*",
                isCompleted = false,
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                taskDescription = todo.taskDescription
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await todoTable.ExecuteAsync(addOperation);

            string message = "New todo stored in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucces = true,
                Message = message,
                Result = todoEntity
            });
        }

        [FunctionName(nameof(UpdateTodo))]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for todo: {id}. received.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            // Validate todo id
            TableOperation findOperation = TableOperation.Retrieve<TodoEntity>("TODO", id);
            TableResult findResult = await todoTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucces = false,
                    Message = "Todo not found."
                });
            }

            //Update todo
            TodoEntity todoEntity = (TodoEntity)findResult.Result;
            todoEntity.isCompleted = todo.isCompleted;
            if (!string.IsNullOrEmpty(todo.taskDescription))
            {
                todoEntity.taskDescription = todo.taskDescription;
            }

            TableOperation addOperation = TableOperation.Replace(todoEntity);
            await todoTable.ExecuteAsync(addOperation);

            string message = $"Todo: {id}, updated in table. ";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucces = true,
                Message = message,
                Result = todoEntity
            });
        }

        [FunctionName(nameof(getAllTodos))]
        public static async Task<IActionResult> getAllTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Get all todos received.");

            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>();
            TableQuerySegment<TodoEntity> todos = await todoTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Rtrieved all todos.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucces = true,
                Message = message,
                Result = todos
            });
        }

        [FunctionName(nameof(deleteTodo))]
        public static async Task<IActionResult> deleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoEntity todoEntity,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete todo: {id} received.");

            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucces = false,
                    Message = "Todo not found."
                });
            }

            await todoTable.ExecuteAsync(TableOperation.Delete(todoEntity));
            //string message = $"Todo: {id}, received.";
            string message = $"Todo: {todoEntity.RowKey}, deleted.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucces = true,
                Message = message,
                Result = todoEntity
            });
        }

        [FunctionName(nameof(getTodoById))]
        public static IActionResult getTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoEntity todoEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"Get todo by id: {id} received.");

            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucces = false,
                    Message = "Todo not found."
                });
            }

            //string message = $"Todo: {id}, received.";
            string message = $"Todo: {todoEntity.RowKey}, received.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucces = true,
                Message = message,
                Result = todoEntity
            });
        }

    }
}
