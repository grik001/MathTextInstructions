using System;
using System.Collections.Generic;
using System.Text;
using static MathInstructionProcessor.Common.Generic;

namespace MathInstructionProcessor.Common.Models
{
    public class Instruction
    {
        public double Value { get; set; }
        public OperationEnum Operation { get; set; }

        public Instruction(double value, OperationEnum operation)
        {
            this.Value = value;
            this.Operation = operation;
        }
    }
}
