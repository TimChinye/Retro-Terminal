namespace RetroTerminal.App.Services;

public interface IDisplayService
{
    int DisplayWidth { get; }
    int DisplayHeight { get; }
    
    void Initialize(bool animate);
    void ConfigureDisplaySize();
    
    // Core rendering methods matching original functionality
    void Write(string text, int x = 0, int y = 0, bool clearLine = true);
    void ClearContent();
    
    // Special method to support the exact scrolling logic of SchoolRoster
    void WriteScrollable(string text, int marginBottom);
    
    ConsoleKeyInfo ReadKey();
    void SetIndicator(char indicator);
}