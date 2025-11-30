using RetroTerminal.App.Applications;
using RetroTerminal.App.Services;

namespace RetroTerminal.App.Core;

public class AppHost
{
    private readonly IDisplayService _display;
    private readonly List<IApplication> _applications;
    private int _selectedIndex = 0;

    public AppHost(IDisplayService display, IEnumerable<IApplication> applications)
    {
        _display = display;
        _applications = applications.ToList();
    }

    public async Task Start()
    {
        _display.Initialize(true);
        _display.ConfigureDisplaySize();

        while (true)
        {
            DrawMainMenu();
            var k = _display.ReadKey();

            if (k.Key == ConsoleKey.UpArrow) _selectedIndex = (_selectedIndex - 1 + _applications.Count) % _applications.Count;
            else if (k.Key == ConsoleKey.DownArrow) _selectedIndex = (_selectedIndex + 1) % _applications.Count;
            else if (k.Key == ConsoleKey.Enter)
            {
                var app = _applications[_selectedIndex];
                await app.Run();
                if (app is PowerOffApp) return;
            }
        }
    }

    private void DrawMainMenu()
    {
        _display.ClearContent();
        for (int i = 0; i < _applications.Count; i++)
        {
            var app = _applications[i];
            string text = (i == _selectedIndex ? "> " : "") + app.Name;
            
            // Programs at top (index * 2), Power Off at bottom (DisplayHeight - padding - 1)
            // Padding is 1 (heightPadding) on top/bottom of content area.
            // In original: PowerOff was at "displayHeight - (heightPadding * 2) - 1"
            int pos = (app is PowerOffApp) ? _display.DisplayHeight - 3 : i * 2;
            
            _display.Write(text, 0, pos);
        }
    }
}