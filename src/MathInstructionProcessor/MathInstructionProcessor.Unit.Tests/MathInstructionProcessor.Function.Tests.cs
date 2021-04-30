using MathInstructionProcessor.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Xunit;

namespace MathInstructionProcessor.Unit.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task ProcessInstruction_ValidData()
        {
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            //Setup Request
            Mock<HttpRequest> mockRequest = new Mock<HttpRequest>();

            ProcessInstructionFunction function = new ProcessInstructionFunction();
            var setup = mockRequest.SetupSequence(x => x.Form.Files["instructionsfile"].OpenReadStream());

            setup.Returns(() => RetrieveStreamForInstructionsFile("instructions1.txt"));
            setup.Returns(() => RetrieveStreamForInstructionsFile("instructions1.txt"));

            //Run
            IActionResult result = await function.Run(mockRequest.Object, mockLogger.Object);

            //Assert Result
            var response = result as OkObjectResult;
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(115.00, response.Value);
            //mockHttpHelper.Verify(x => x.PostAsync<StringContent, HttpResponseMessage>(It.Is<string>(x => x == "https://prod-117.westeurope.logic.azure.com/workflows/01fb5e472fd8476bb8f197b53de65a02/runs/08585856323751261468026327562CU160/actions/Wait_for_execution/run?api-version=2016-06-01&sp=%2Fruns%2F08585856323751261468026327562CU160%2Factions%2FWait_for_execution%2Frun%2C%2Fruns%2F08585856323751261468026327562CU160%2Factions%2FWait_for_execution%2Fread&sv=1.0&sig=DBTnyszO7MwAnk1O6njIcgejjPWndNoRb3QeuAwIrgg"), It.IsAny<HttpContent>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(1));
        }

        private static Stream RetrieveStreamForInstructionsFile(string fileName)
        {
            return Generic.LoadEmbeddedResourceAsStream(Assembly.GetExecutingAssembly(), $"MathInstructionProcessor.Unit.Tests.Data.{fileName}");
        }
    }
}
