using System;
using System.Collections.Generic;
using AzureFunctionExample.Functions.Functions;
using AzureFunctionExample.test.Helpers;
using System.Text;
using Xunit;

namespace AzureFunctionExample.test.Tests
{
    public class ScheduledFunctionTest
    {

        [Fact]
        public void ScheduledFunction_Should_Log_Message()
        {
            //Arrenge --preparate unitary test --We need request, table and http
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            //Act --Execute unitary test
            ScheduledFunction.Run(null, mockTodos, logger);
            string message = logger.Logs[0];


            //Assert --verification if the unitary test is correct
            Assert.Contains("Deleting completed", message);

        }
    }
}
