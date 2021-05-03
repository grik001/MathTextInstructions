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
    public class ProcessInstruction_Tests
    {
        [Theory]
        [InlineData(115.0, "instructions_Valid.txt")]
        [InlineData(814883.5, "instructions_Valid_Big.txt")]
        [InlineData(15.0, "instructions_Valid_Uppercase.txt")]
        [InlineData(45.0, "instructions_Valid_Lowercase.txt")]
        [InlineData(115.0, "instructions_Valid_ExtraSpaces.txt")]
        [InlineData(107374182465.0, "instructions_Valid_BiggerInt.txt")]
        public async Task ProcessInstruction_ValidData(double expectedValue, string fileName)
        {
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            //Setup Request
            Mock<HttpRequest> mockRequest = new Mock<HttpRequest>();

            ProcessInstructionFunction function = new ProcessInstructionFunction();
            var setup = mockRequest.SetupSequence(x => x.Form.Files["instructionsfile"].OpenReadStream());

            setup.Returns(() => RetrieveStreamForInstructionsFile(fileName));
            setup.Returns(() => RetrieveStreamForInstructionsFile(fileName));

            //Run
            IActionResult result = await function.Run(mockRequest.Object, mockLogger.Object);

            //Assert Result
            var response = result as OkObjectResult;
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(expectedValue, response.Value);
        }

        [Theory]
        [InlineData("Last operation in the file has to always be of type \"apply\"", "instructions_Invalid_MissingApply.txt")]
        [InlineData("The operation supplied is invalid at line position: 0", "instructions_Invalid_InvalidCommand.txt")]
        [InlineData("Invalid input detected at line position: 2", "instructions_Invalid_MissingNumber.txt")]
        [InlineData("Invalid input detected at line position: 3", "instructions_Invalid_MissingOperation.txt")]
        [InlineData("Invalid input detected at line position: 0", "instructions_Invalid_ExtraSeperator.txt")]
        [InlineData("The numeric value has to be of type double at line position: 3", "instructions_Invalid_LetterInsteadOfNumber.txt")]
        [InlineData("Divide by 0 detected in file at line position: 3", "instructions_Invalid_DivideByZero.txt")]
        [InlineData("Empty line detected at line position: 2", "instructions_Invalid_EmptyLine.txt")]
        public async Task ProcessInstruction_InvalidData(string expectedErrorValue, string fileName)
        {
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            //Setup Request
            Mock<HttpRequest> mockRequest = new Mock<HttpRequest>();

            ProcessInstructionFunction function = new ProcessInstructionFunction();
            var setup = mockRequest.SetupSequence(x => x.Form.Files["instructionsfile"].OpenReadStream());

            setup.Returns(() => RetrieveStreamForInstructionsFile(fileName));
            setup.Returns(() => RetrieveStreamForInstructionsFile(fileName));

            //Run
            IActionResult result = await function.Run(mockRequest.Object, mockLogger.Object);

            //Assert Result
            var response = result as BadRequestObjectResult;
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(expectedErrorValue, response.Value);
        }

        private static Stream RetrieveStreamForInstructionsFile(string fileName)
        {
            return Generic.LoadEmbeddedResourceAsStream(Assembly.GetExecutingAssembly(), $"MathInstructionProcessor.Unit.Tests.Data.{fileName}");
        }
    }
}
