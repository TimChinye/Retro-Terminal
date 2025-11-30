using RetroTerminal.Logic.Interfaces;

namespace RetroTerminal.Logic.Services;

public class TrinaryConverter : ITrinaryConverter
{
    public double Convert(string input, out List<string> equationParts)
    {
        var parts = new List<string>();
        double result = input.Select((digit, i) => {
            double part = ((int)digit - '0') * Math.Pow(3, input.Length - (i + 1));
            parts.Add(i == 0 ? "" + part : $" + {part}");
            return part;
        }).Sum();

        equationParts = parts;
        return result;
    }
}