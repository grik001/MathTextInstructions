using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MathInstructionProcessor.Common;
using MathInstructionProcessor.Common.CustomExceptions;
using MathInstructionProcessor.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using static MathInstructionProcessor.Common.Generic;

namespace MathInstructionProcessor
{
    public class ProcessInstructionFunction
    {
        [FunctionName("ProcessInstruction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log?.LogTrace($"ProcessInstruction received request");

            try
            {
                double result = ProcessInstruction(req, log);
                return new OkObjectResult(result);
            }
            catch (InvalidInstructionException ex)
            {
                log.LogWarning(ex, "Invalid operation sent to ProcessInstruction");
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unhandled exception triggered");
                throw;
            }
        }

        private double ProcessInstruction(HttpRequest req, ILogger log)
        {
            double result = 0;

            log?.LogTrace("Loading file from request");
            var file = LoadFileFromForm(req);

            if(file == null)
            {
                log?.LogWarning($"ProcessInstruction was not executed due to file not being part of the request");
                throw new InvalidInstructionException("Request expects an instructions file");

            }

            //get last line from file and set the value as the starting value
            var applyLine = GetLastLineFromFile(file, log);
            log?.LogTrace($"Apply line loaded: {applyLine}");

            Instruction applyInstruction = ParseInstruction(applyLine.line, applyLine.position);
            log?.LogTrace($"Apply line parsed - Operation: {applyInstruction?.Operation} - Value: {applyInstruction?.Value}");

            if (applyInstruction.Operation == OperationEnum.Apply)
            {
                result = applyInstruction.Value;
            }
            else
            {
                log?.LogWarning($"Apply line invalid - Operation: {applyInstruction?.Operation} - Value: {applyInstruction?.Value}");
                throw new InvalidInstructionException("Last operation in the file has to always be of type \"apply\"");
            }

            //process remaining commands in the file - line by line
            log?.LogTrace($"Lines processing started");
            using (Stream readStream = file.OpenReadStream())
            {
                using (StreamReader reader = new StreamReader(readStream))
                {
                    long linePosition = 0;

                    do
                    {
                        string line = reader.ReadLine();
                        log?.LogTrace($"Starting process for line at {linePosition}: {line}");

                        if (line == null || applyLine.position == linePosition)
                            break;

                        Instruction currentInstruction = ParseInstruction(line, linePosition);

                        log?.LogTrace($"Parsed line at position {linePosition} - Operation: {currentInstruction?.Operation} - Value: {currentInstruction?.Value} - CurrentResult: {result}");
                        result = Calculate(result, currentInstruction, linePosition);
                        log?.LogTrace($"Calculated line at position {linePosition} - Operation: {currentInstruction?.Operation} - Value: {currentInstruction?.Value} - CurrentResult: {result}");
                        linePosition++;
                    } while (true);
                }
            }

            log?.LogTrace($"Lines processing complete");
            return result;
        }

        private (long position, string line) GetLastLineFromFile(IFormFile file, ILogger log)
        {
            try
            {
                //This would have been more efficient using the System IO File object
                string line = "";
                long linePosition = 0;

                using (Stream readStream = file.OpenReadStream())
                {
                    using (StreamReader reader = new StreamReader(readStream))
                    {
                        do
                        {
                            string currentline = reader.ReadLine();

                            if (currentline == null)
                                break;

                            line = currentline;
                            linePosition++;
                        } while (true);
                    }
                }

                return (linePosition-1, line);
            }
            catch (Exception ex)
            {
                log?.LogError(ex, "GetLastLineFromFile threw an error");
                throw;
            }
        }

        private IFormFile LoadFileFromForm(HttpRequest req)
        {
            var file = req.Form.Files["instructionsfile"];
            return file;
        }

        private double Calculate(double currentValue, Instruction currentInstruction, long linePosition)
        {
            switch (currentInstruction.Operation)
            {
                case OperationEnum.Add:
                    currentValue += currentInstruction.Value;
                    break;
                case OperationEnum.Subtract:
                    currentValue -= currentInstruction.Value;
                    break;
                case OperationEnum.Multiply:
                    currentValue *= currentInstruction.Value;
                    break;
                case OperationEnum.Divide:
                    currentValue /= currentInstruction.Value;
                    break;
                case OperationEnum.Apply:
                    throw new InvalidInstructionException($"Duplicate apply detected at line position: {linePosition}");
                default:
                    throw new InvalidInstructionException($"The operation supplied is invalid at line position: {linePosition}");
                    break;
            }

            return currentValue;
        }

        private Instruction ParseInstruction(string textInstruction, long linePosition)
        {
            if (string.IsNullOrWhiteSpace(textInstruction))
            {
                throw new InvalidInstructionException($"Empty line detected at line position: {linePosition}");
            }

            string[] instructions = textInstruction.Trim().Split(Generic.InstructionSeperator);

            if (instructions.Length != 2)
            {
                throw new InvalidInstructionException($"Invalid input detected at line position: {linePosition}");
            }

            double value;
            OperationEnum instruction;

            if (!double.TryParse(instructions[1], out value))
            {
                throw new InvalidInstructionException($"The numeric value has to be of type double at line position: {linePosition}");
            }

            if (!Enum.TryParse(instructions[0], true, out instruction))
            {
                throw new InvalidInstructionException($"The operation supplied is invalid at line position: {linePosition}");
            }

            if (instruction == OperationEnum.Divide && value == 0)
            {
                throw new InvalidInstructionException($"Divide by 0 detected in file at line position: {linePosition}");
            }

            return new Instruction(value, instruction);
        }
    }
}

