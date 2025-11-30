using System;
using Xunit;
using Assignment;
using static Assignment.Program.Computer;

namespace Unit_Tests
{
    public class TrinaryConverterTest
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("2", 2)]
        [InlineData("10", 3)]
        [InlineData("112", 14)]
        [InlineData("1122000120", 32091)]
        // [InlineData("7", 7)] -> the input string can not contain any character other than '0', '1' and '2'.
        public void TestTheory(string input, double output)
        {
            // Arrange
            TrinaryConverter trinaryConverterProgram = new Assignment.Program.Computer.TrinaryConverter();

            // Act
            double expectedResult = output;
            double actualResult = trinaryConverterProgram.CalculateConversion(input, out _);

            // Assert
            Assert.Matches("^[0-2]*$", input); // Using RegEx to assert that the input is valid.
            Assert.Equal(expectedResult, actualResult);
        }

    }
}