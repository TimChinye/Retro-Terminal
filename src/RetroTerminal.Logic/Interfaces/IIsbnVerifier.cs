namespace RetroTerminal.Logic.Interfaces;

public interface IIsbnVerifier
{
    bool IsValid(string input, out bool[] details);
}