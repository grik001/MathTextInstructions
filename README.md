# MathTextInstructions

The MathTextInstructions Azure Function parses an instructions file and processes the mathematical instructions. 

### ProcessInstruction Request

The below instructions are to be included in a text file and sent to the Function via a POST.

**URL**: {functionUrl}/api/ProcessInstruction  
**HTTP Request Type**: POST  
**Body Type**: form-data  
**Key**: instructionsfile  
**Value**: {fileName}.txt  
**Sample Content**:  

`Add 1`  
`subtract 2`  
`Multiply 10.00`  
`divide 02`  
`apply 24`  

## Unit Tests

All Unit tests can be found inside the MathInstructionProcessor.Unit.Tests project. Below is a list of all the tests being executed.

- Basic file with all instructions
- A huge file with over a million instructions
- Commands with Uppercase instructions such as "aDD"
- Commands with all Lowercase instructions such as "add"
- Commands with Extra spaces such as "Add       5"
- Commands with values bigger than the max value for Integer
- Instructions file with missing Apply
- Instructions file with an invalid command such as "addddddd"
- Instructions file with no numeric value supplied after the command
- Instructions file with no command supplied
- Command line with an extra separator
- Invalid value instead of the expected numeric value
- Divide by Zero check
- An empty line between instructions

The tests specified in the requirements  file are included with the following tests:

- instructions_Valid_Lowercase
- instructions_Valid_Uppercase

## Integration Tests

The postman tests can be found in the **MathInstructionProcessor.Unit.Tests** project under the folder "PostmanTests".

