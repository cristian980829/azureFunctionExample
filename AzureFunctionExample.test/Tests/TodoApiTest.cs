using AzureFunctionExample.common.Models;
using AzureFunctionExample.Functions.Functions;
using AzureFunctionExample.test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace AzureFunctionExample.test.Tests
{
    public class TodoApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateTodo_Should_Return_200()
        {
            //Arrenge --preparate unitary test --We need request, table and http
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo todoRequest = TestFactory.GetTodoRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoRequest);

            //Act --Execute unitary test
            IActionResult response = await TodoApi.CreateTodo(request, mockTodos, logger);

            //Assert --verification if the unitary test is correct
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateTodo_Should_Return_200()
        {
            //Arrenge --preparate unitary test --We need request, table and http
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo todoRequest = TestFactory.GetTodoRequest();
            Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, todoRequest);

            //Act --Execute unitary test
            IActionResult response = await TodoApi.UpdateTodo(request, mockTodos, todoId.ToString(), logger);

            //Assert --verification if the unitary test is correct
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
