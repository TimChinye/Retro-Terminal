using RetroTerminal.Logic.Interfaces;

namespace RetroTerminal.Logic.Services;

public class IsbnVerifier : IIsbnVerifier
{
    public bool IsValid(string input, out bool[] details)
    {
        input = input.Replace("-", "");
        
        bool len = input.Length == 10;
        bool first9 = false; 
        if (len) first9 = input.Take(9).All(char.IsDigit);
        
        bool lastChar = false;
        if (len) lastChar = input.TakeLast(1).Any(c => c == 'X' || char.IsDigit(c));

        bool checkSum = false;
        if (len && first9 && lastChar)
        {
            int sum = input.Select((digit, index) => {
                if (digit == 'X') return 10;
                return (digit - '0') * (10 - index);
            }).Sum();
            checkSum = (sum % 11 == 0);
        }

        details = new bool[] { len, first9, lastChar, checkSum };
        return details.All(x => x);
    }
}