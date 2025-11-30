namespace RetroTerminal.Logic.Interfaces;

public interface ITrinaryConverter
{
    double Convert(string input, out List<string> equationParts);
}