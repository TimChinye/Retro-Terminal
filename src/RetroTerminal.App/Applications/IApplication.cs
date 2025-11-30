namespace RetroTerminal.App.Applications;

public interface IApplication
{
    string Name { get; }
    Task Run();
}