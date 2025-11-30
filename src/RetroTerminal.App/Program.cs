using RetroTerminal.App.Applications;
using RetroTerminal.App.Core;
using RetroTerminal.App.Services;
using RetroTerminal.Logic.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var display = new DisplayService();
        var roster = new SchoolRoster();
        // Seed default data if needed or leave empty
        
        var apps = new List<IApplication> {
            new TrinaryConverterApp(display, new TrinaryConverter()),
            new SchoolRosterApp(display, roster),
            new IsbnVerifierApp(display, new IsbnVerifier()),
            new PowerOffApp(display)
        };

        await new AppHost(display, apps).Start();
        Console.Clear();
    }
}