using System.Text;

namespace RetroTerminal.App.Services;

public class DisplayService : IDisplayService
{
    private readonly string[] _splashTexts = {
        "I may have tried too hard...", "Tim Chinye's PC", "/|\\ ATARI SM124",
        "I'm only going to write 5 of these.", "Nothing to see here, just a comSHUter.",
        "Designed & Developed by Tim", "Made with <3... kidding, I just want a good grade.",
        "I'm having too much fun with this.", "This going to take so long...",
    };
    private string _splashText;
    
    // These match 'screen' and 'currentScreen' from original
    private string? _screenTemplate;
    private string? _currentScreen;
    
    private int _displayWidth;
    private int _displayHeight;
    private const int WidthPadding = 5;
    private const int HeightPadding = 1;
    private char _indicator = 'O';

    public int DisplayWidth => _displayWidth;
    public int DisplayHeight => _displayHeight;

    public DisplayService()
    {
        _splashText = _splashTexts[new Random().Next(0, _splashTexts.Length)];
        Build(68, 21);
    }

    private void Build(int width, int height)
    {
        int minWidth = WidthPadding * 2; 
        int minHeight = HeightPadding * 2; 

        if (width <= minWidth) width = minWidth + 1;
        if (height <= minHeight) height = minHeight + 1;
        
        if (width <= minWidth || height <= minHeight) 
        {
            Build(width, height);
            return;
        }

        _displayWidth = width;
        _displayHeight = height;

        string hBorder = new string('_', _displayWidth);
        string hSpace = new string(' ', _displayWidth);
        
        string tempSplash = _splashText.PadRight(_displayWidth);
        string dimDisplay = $"{_displayWidth} x {_displayHeight}".PadRight(_displayWidth + 10);
        string ctrlCMessage = "Press CTRL+C to Force Stop".PadSides(_displayWidth + 14);

        var sb = new StringBuilder();
        sb.AppendLine(dimDisplay);
        sb.AppendLine($"  _____{hBorder}_____  ");
        sb.AppendLine($" /     {hSpace}     \\ ");
        sb.AppendLine($"|      {hSpace}      |");
        sb.AppendLine($"|      {hBorder}      |");
        sb.AppendLine($"|     /{hSpace}\\     |");
        sb.AppendLine("Display:"); // Token
        sb.AppendLine($"|     \\{hBorder}/     |");
        sb.AppendLine($"|      {hSpace}      |");
        sb.AppendLine($"|     {tempSplash.Substring(0, _displayWidth)} {_indicator}     |");
        sb.AppendLine($" \\_____{hBorder}_____/ ");
        sb.AppendLine($"   !___{hBorder}___!   ");
        sb.AppendLine();
        sb.Append(ctrlCMessage);

        _screenTemplate = sb.ToString();

        // Initialize currentScreen with empty content rows
        string rowTemplate = $"|     |{hSpace}|     |";
        string displayRows = string.Join("\n", Enumerable.Repeat(rowTemplate, _displayHeight));
        _currentScreen = _screenTemplate.Replace("\nDisplay:", $"\n{displayRows}");
    }

    public void Initialize(bool animate)
    {
        if (Console.IsOutputRedirected) { Build(68, 21); return; }

        // Auto-size logic
        int maxWidth = Console.WindowWidth - 14;
        int maxHeight = Console.WindowHeight - 14;
        int pWidth = (int)((double)maxHeight / 41 * 136);
        int pHeight = (int)((double)maxWidth / 136 * 41);

        int w = 68, h = 21;
        if (pWidth <= maxWidth) { w = pWidth; h = maxHeight; }
        else if (pHeight <= maxHeight) { w = maxWidth; h = pHeight; }

        Build(w, h);

        if (animate)
        {
            string gradient = """ '|V#""";
            foreach (char c in gradient)
            {
                string fill = string.Concat(Enumerable.Repeat(c, (_displayWidth - (WidthPadding * 2)) * (_displayHeight - (HeightPadding * 2))));
                Write(fill);
                Thread.Sleep(1000 / gradient.Length * 3);
            }
        }
        ClearContent();
    }

    public void ConfigureDisplaySize()
    {
        if (Console.IsOutputRedirected) return;
        ConsoleKeyInfo k;
        do
        {
            Build(_displayWidth, _displayHeight);
            Write("Choose your screen size\n(Use the Arrow Keys then press 'Enter')", 0, 0);
            
            k = Console.ReadKey(true);

            if (k.Key == ConsoleKey.UpArrow) _displayHeight--;
            if (k.Key == ConsoleKey.DownArrow) _displayHeight++;
            if (k.Key == ConsoleKey.LeftArrow) _displayWidth--;
            if (k.Key == ConsoleKey.RightArrow) _displayWidth++;

            _indicator = k.Key switch {
                ConsoleKey.UpArrow => '^', ConsoleKey.DownArrow => 'v',
                ConsoleKey.LeftArrow => '<', ConsoleKey.RightArrow => '>', _ => _indicator
            };

        } while (k.Key != ConsoleKey.Enter);
        
        _indicator = 'O';
        Build(_displayWidth, _displayHeight);
        ClearContent();
    }

    // Direct port of DisplayText logic using string splicing on _currentScreen
    public void Write(string text, int x = 0, int y = 0, bool clearLine = true)
    {
        if (!Console.IsOutputRedirected) Console.Clear();
        if (_screenTemplate == null || _currentScreen == null) { Console.WriteLine(text); return; }

        int startXPos = x + 7 + WidthPadding;
        int startYPos = y + HeightPadding;

        // Extract the rows representing the screen content (skip header)
        List<string> displayRows = _currentScreen.Split('\n').ToList().GetRange(6, _displayHeight);

        foreach (var nowrapString in text.Split('\n'))
        {
            List<string> wrappedStrings = WrapOverflow(nowrapString, _displayWidth - (WidthPadding * 2));

            foreach (string wrappedString in wrappedStrings)
            {
                int padWidth = Math.Max(0, _displayWidth - (WidthPadding * 2) - x);
                string fullLine = clearLine ? wrappedString.PadRight(padWidth) : wrappedString;
                
                if (startYPos > displayRows.Count - HeightPadding) continue;

                string currentRow = displayRows[startYPos];
                
                // Splice string
                if (startXPos + fullLine.Length <= currentRow.Length)
                {
                    displayRows[startYPos] = currentRow.Substring(0, startXPos) + fullLine + currentRow.Substring(startXPos + fullLine.Length);
                }
                
                startYPos++;
            }
        }

        string displayContent = string.Join('\n', displayRows);
        _currentScreen = _screenTemplate.Replace("\nDisplay:", $"\n{displayContent}");
        
        Console.WriteLine(_currentScreen);
    }

    // Logic from SchoolRoster.SendText adapted to this class
    public void WriteScrollable(string text, int marginBottom)
    {
        if (!Console.IsOutputRedirected) Console.Clear();
        if (_screenTemplate == null || _currentScreen == null) { Console.WriteLine(text); return; }

        int startXPos = 7 + WidthPadding;
        int startYPos = HeightPadding; // 0 + HeightPadding

        List<string> displayRows = _currentScreen.Split('\n').ToList().GetRange(6, _displayHeight);
        
        // Preserve bottom menu area
        List<string> lastDisplayRows = displayRows.TakeLast(marginBottom).ToList();
        displayRows.RemoveRange(displayRows.Count - marginBottom, marginBottom);

        foreach (var nowrapString in text.Split('\n'))
        {
            List<string> wrappedStrings = WrapOverflow(nowrapString, _displayWidth - (WidthPadding * 2));

            foreach (string wrappedString in wrappedStrings)
            {
                string fullLine = wrappedString.PadRight(_displayWidth - (WidthPadding * 2));
                
                string ReplacementRow(int rowIndex) 
                {
                    string r = displayRows[rowIndex];
                    return r.Substring(0, startXPos) + fullLine + r.Substring(startXPos + fullLine.Length);
                }

                if (startYPos > displayRows.Count - 1)
                {
                    // Scroll: remove top, insert bottom
                    displayRows.Insert(displayRows.Count, ReplacementRow(startYPos - 1));
                    displayRows.RemoveAt(HeightPadding); // Remove from top of content area
                    continue;
                }

                displayRows[startYPos] = ReplacementRow(startYPos);
                startYPos++;
            }
        }

        displayRows.AddRange(lastDisplayRows);

        string display = string.Join('\n', displayRows);
        _currentScreen = _screenTemplate.Replace("\nDisplay:", $"\n{display}");
        Console.WriteLine(_currentScreen);
    }

    public void ClearContent()
    {
        string blank = string.Join("\n", Enumerable.Repeat(new string(' ', _displayWidth - (WidthPadding * 2)), _displayHeight - (HeightPadding * 2)));
        Write(blank);
    }

    public void SetIndicator(char indicator) 
    {
        _indicator = indicator;
        Build(_displayWidth, _displayHeight); 
    }

    public ConsoleKeyInfo ReadKey() => Console.IsOutputRedirected ? new ConsoleKeyInfo() : Console.ReadKey(true);

    // Helpers from original code
    private static List<string> WrapOverflow(string text, int maxCharactersPerLine)
    {
        if (maxCharactersPerLine <= 0) return new List<string> { "" };

        int quotient = Math.DivRem(text.Length, maxCharactersPerLine, out int remainder);
        List<string> lines = Enumerable.Repeat("", quotient + (remainder == 0 ? 0 : 1)).ToList();
        if (quotient == 0 && remainder == 0) lines.Add("");

        return lines.Select((item, i) => {
            return lines.Count == (i + 1) 
                ? text.Substring(i * maxCharactersPerLine, text.Length - (i * maxCharactersPerLine)) 
                : text.Substring(i * maxCharactersPerLine, maxCharactersPerLine);
        }).ToList();
    }
}

public static class StringExtensions
{
    public static string PadSides(this string str, int totalWidth)
    {
        int spaces = totalWidth - str.Length;
        int padLeft = spaces / 2 + str.Length;
        return str.PadLeft(padLeft).PadRight(totalWidth);
    }
}