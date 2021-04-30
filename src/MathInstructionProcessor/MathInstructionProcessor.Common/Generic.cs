using System;
using System.IO;
using System.Reflection;

namespace MathInstructionProcessor.Common
{
    public static class Generic
    {
        public static string InstructionSeperator = " ";

        public enum OperationEnum
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Apply
        }

        public static Stream LoadEmbeddedResourceAsStream(Assembly assembly, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            Stream stream = assembly.GetManifestResourceStream(name);

            if (stream == null)
                return null;

            return stream;
        }
    }
}
