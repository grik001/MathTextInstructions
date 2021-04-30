using System;
using System.IO;
using System.Threading.Tasks;
using MathInstructionProcessor.Common;
using MathInstructionProcessor.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static MathInstructionProcessor.Common.Generic;

namespace MathInstructionProcessor
{
    public static class ProcessInstruction
    {
        [FunctionName("ProcessInstruction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string[] instructions = new string[] { "add 5", "ADD 3", "multiply 9", " divide 02 ", "apply 1", "apply 1" };

            double result = 0;

            Instruction applyInstruction = ParseInstruction(instructions[instructions.Length - 1]);

            if(applyInstruction.Operation == OperationEnum.Apply)
            {
                result = applyInstruction.Value;
            }
            else
            {
                //throw
            }


            for (int i = 0; i < instructions.Length-1; i++)
            {
                string currentTextInstruction = instructions[i];
                Instruction currentInstruction = ParseInstruction(currentTextInstruction);
                result = Calculate(result, currentInstruction);
            }

            return new OkObjectResult(result);
        }


        private static double Calculate(double currentValue, Instruction currentInstruction)
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
                    throw new Exception("Duplicate apply detected");
                default:
                    //log and throw
                    break;
            }

            return currentValue;
        }

        private static Instruction ParseInstruction(string textInstruction)
        {
            if(string.IsNullOrWhiteSpace(textInstruction))
            {
                //log and throw
            }

            string[] instructions = textInstruction.Trim().Split(Generic.InstructionSeperator);

            if(instructions.Length > 2)
            {
                //log and throw
            }

            double value;
            OperationEnum instruction;

            if (!double.TryParse(instructions[1], out value))
            {
                //log and throw
            }

            if (!Enum.TryParse(instructions[0], true, out instruction))
            {
                //log and throw
            }

            if(instruction == OperationEnum.Divide && value == 0)
            {
                //log and throw
            }

            return new Instruction(value, instruction);
        }
    }
}

