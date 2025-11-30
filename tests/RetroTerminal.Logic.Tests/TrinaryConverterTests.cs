using RetroTerminal.Logic.Services;
using System.Text.RegularExpressions;

namespace RetroTerminal.Logic.Tests;

public class TrinaryConverterTests
{
    private readonly TrinaryConverter _converter = new();

    [Theory]
    [InlineData("1", 1)]
    [InlineData("2", 2)]
    [InlineData("10", 3)]
    [InlineData("112", 14)]
    [InlineData("222", 26)] 
    [InlineData("1122000120", 32091)]
    public void Convert_ValidTrinary_ReturnsCorrectDecimal(string input, double expected)
    {
        // Updated to match your original logic: returns double, outputs list
        double result = _converter.Convert(input, out _);
        
        // Assert matches your original test logic
        Assert.Matches("^[0-2]*$", input);
        Assert.Equal(expected, result);
    }

    // Your original code handled invalid inputs in the UI, not the calculator.
    // The calculator would just process '3' as digit 3.
    // Therefore, we only test valid inputs here to match your original scope.
}