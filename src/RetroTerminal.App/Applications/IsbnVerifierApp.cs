using RetroTerminal.App.Services;
using RetroTerminal.Logic.Interfaces;

namespace RetroTerminal.App.Applications;

public class IsbnVerifierApp : IApplication
{
    private readonly IDisplayService _display;
    private readonly IIsbnVerifier _verifier;
    public string Name => "ISBN Verifier";

    public IsbnVerifierApp(IDisplayService display, IIsbnVerifier verifier)
    {
        _display = display; _verifier = verifier;
    }

    public Task Run()
    {
        int index = 0;
        string input = "";
        string output = ""; // State variable for error messages

        while (true)
        {
            _display.ClearContent();
            _display.Write("Input your ISBN-10:", 0, 0);
            
            _display.Write(((index == 0) ? "> " : "") + input, 0, 1);
            
            // Render the output/error message state
            _display.Write(output, 0, 3);
            
            _display.Write(((index == 1) ? "> " : "") + "Back to Menu", 0, _display.DisplayHeight - 3);

            var key = _display.ReadKey();

            // Clear output on navigation
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow)
            {
                output = "";
            }

            if (key.Key == ConsoleKey.DownArrow && index < 1) index++;
            else if (key.Key == ConsoleKey.UpArrow && index > 0) index--;
            else if (key.Key == ConsoleKey.Enter)
            {
                output = "";
                if (index == 1) return Task.CompletedTask;
                if (!string.IsNullOrEmpty(input))
                {
                    bool valid = _verifier.IsValid(input, out var details);
                    string eq = "Validating...\n";
                    string[] names = { "ISBN Length", "First 9 characters", "Last character", "Final check" };
                    for(int i=0; i<4; i++) {
                        eq += $"\n{names[i]} is {(details[i] ? "Valid" : "Invalid")}";
                        _display.Write(eq, 0, 3);
                        Thread.Sleep(2500);
                    }
                    _display.ClearContent();
                    _display.Write($"The ISBN-10 is: {(valid ? "Valid" : "Invalid")}\n\nPress any key to go again.", 0, 3);
                    _display.ReadKey();
                    input = "";
                }
            }
            else if (index == 0)
            {
                List<ConsoleKey> acceptedKeys = new List<ConsoleKey> { ConsoleKey.X, ConsoleKey.OemMinus, ConsoleKey.Backspace, ConsoleKey.Enter, ConsoleKey.UpArrow };
                
                if (char.IsDigit(key.KeyChar) || acceptedKeys.Contains(key.Key))
                {
                    output = "";
                    if (key.Key == ConsoleKey.Backspace && input.Length > 0) 
                    {
                        input = input[..^1];
                    }
                    else if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.UpArrow)
                    {
                        input += char.ToUpper(key.KeyChar);
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    output = $"You can't put '{char.ToUpper(key.KeyChar)}' here.";
                }
                else 
                {
                    output = "Invalid character!";
                }
            }
        }
    }
}