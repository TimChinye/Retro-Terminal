using RetroTerminal.App.Services;

namespace RetroTerminal.App.Applications;

public class PowerOffApp : IApplication
{
    private readonly IDisplayService _display;
    public string Name => "Power Off";

    public PowerOffApp(IDisplayService display) => _display = display;

    public Task Run()
    {
        _display.ClearContent();
        string text = "Turned Off";
        int pad = (_display.DisplayWidth - text.Length) / 2 + text.Length;
        _display.Write(text.PadLeft(pad), 0, (_display.DisplayHeight-2)/2);
        return Task.CompletedTask;
    }
}