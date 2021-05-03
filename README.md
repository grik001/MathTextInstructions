# MathTextInstructions

The Azure Function will parse an instructions file and process the mathematical instructions. The file is expected to be passed within the body with the key "instructionsfile".

## Unit Tests

All Unit tests can be found inside the MathInstructionProcessor.Unit.Tests project. Below is a list of all the tests being carried out.

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
- Command line with an extra seperator
- Invalid value instead of the expected numeric value
- Divide by Zero check
- An empty line between instructions

## Integration Tests

The postman tests can be found in the **MathInstructionProcessor.Unit.Tests** project under the folder "PostmanTests".

