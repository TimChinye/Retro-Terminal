using RetroTerminal.App.Services;
using RetroTerminal.Logic.Interfaces;

namespace RetroTerminal.App.Applications;

public class TrinaryConverterApp : IApplication
{
    private readonly IDisplayService _display;
    private readonly ITrinaryConverter _converter;
    public string Name => "Trinary Converter";

    public TrinaryConverterApp(IDisplayService display, ITrinaryConverter converter)
    {
        _display = display; _converter = converter;
    }

    public Task Run()
    {
        int index = 0;
        string input = "";
        string output = ""; // State variable for error messages
        
        while (true)
        {
            _display.ClearContent();
            _display.Write("Input a ternary value:", 0, 0);
            
            _display.Write(((index == 0) ? "> " : "") + input, 0, 1);
            
            // Render the output/error message state
            _display.Write(output, 0, 3);
            
            _display.Write(((index == 1) ? "> " : "") + "Back to Menu", 0, _display.DisplayHeight - 3);

            var key = _display.ReadKey();

            // Clear output on navigation or valid entry start
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow) 
            {
                output = "";
            }

            if (key.Key == ConsoleKey.DownArrow && index < 1) index++;
            else if (key.Key == ConsoleKey.UpArrow && index > 0) index--;
            else if (key.Key == ConsoleKey.Enter)
            {
                output = ""; // Clear errors on enter
                if (index == 1) return Task.CompletedTask;
                if (!string.IsNullOrEmpty(input))
                {
                    double val = _converter.Convert(input, out var parts);
                    
                    // Animation
                    string equation = "Calculating: ";
                    if (parts.Count > 1) {
                        foreach(var p in parts) {
                            equation += p;
                            _display.Write(equation, 0, 3);
                            Thread.Sleep(1000);
                        }
                    }
                    
                    _display.ClearContent();
                    _display.Write($"The decimal value is: {val}\n\nPress any key to go again.", 0, 3);
                    _display.ReadKey();
                    input = "";
                    output = "";
                }
            }
            else if (index == 0)
            {
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    output = "";
                }
                else if (char.IsDigit(key.KeyChar))
                {
                    if (key.KeyChar >= '0' && key.KeyChar <= '2') 
                    {
                        input += key.KeyChar;
                        output = "";
                    }
                    else 
                    {
                        // Set the error state, which will be rendered in the next loop iteration
                        output = $"Error: {key.KeyChar} is not used in base 3!";
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                     output = "Invalid character!";
                }
            }
        }
    }
}