using System.Globalization;
using System.Text.RegularExpressions;

namespace HolidayMaker;
// Hjälpmetoder för inmatning. Frågar om tills värdet är giltigt, så felinmatning inte kraschar programmet.
internal static class Input
{
    // Alla rum kan bokas under december 2026 (utcheckning får vara 1 januari 2027)
    private static readonly DateTime MinDate = new(2026, 12, 1);
    private static readonly DateTime MaxDate = new(2027, 1, 1);
 
    public static string Ask(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? "").Trim();
    }
 
    public static int AskInt(string prompt, int min, int max)
    {
        while (true)
        {
            if (int.TryParse(Ask(prompt), out var v) && v >= min && v <= max) return v;
            Console.WriteLine($"Ange ett heltal mellan {min} och {max}.");
        }
    }
 
    public static int? AskOptionalInt(string prompt)
    {
        while (true)
        {
            var s = Ask(prompt);
            if (s == "") return null;
            if (int.TryParse(s, out var v) && v >= 0) return v;
            Console.WriteLine("Ange ett positivt heltal, eller tryck Enter.");
        }
    }

    public static bool AskYesNo(string prompt)
    { 
        while (true)
        {
        var s = Ask(prompt + " (j/n): ").ToLowerInvariant();
        if (s is "j" or "ja") return true;
        if (s is "n" or "nej") return false;
        Console.WriteLine("Svara j eller n.");
        }
    }
 
   
    public static DateTime AskDate(string prompt)
    {
        while (true)
        {
            if (DateTime.TryParseExact(Ask(prompt), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var d))
                return d;
            Console.WriteLine("Skriv datum som ÅÅÅÅ-MM-DD, till exempel 2026-12-10.");
        }
    }

    public static string AskPhone(string prompt)
    {
        while (true)
        {
        var s = Ask(prompt);
        if (Regex.IsMatch(s, @"^\+?[0-9][0-9 \-]{5,18}$")) return s;
        Console.WriteLine("Ange ett giltigt telefonnummer (siffror, mellanslag och bindestreck), till exempel 070-1234567.");
        }
    }
 
    // Frågar efter in- och utcheckning och kontrollerar datumreglerna
    public static (DateTime checkIn, DateTime checkOut) AskStay()
    {
        while (true)
        {
            var checkIn = AskDate("Incheckning (ÅÅÅÅ-MM-DD): ");
            var checkOut = AskDate("Utcheckning (ÅÅÅÅ-MM-DD): ");
 
            if (checkOut <= checkIn)
                Console.WriteLine("Utcheckning måste vara efter incheckning.");
            else if (checkIn < MinDate || checkOut > MaxDate)
                Console.WriteLine("Bokning kan bara göras under december 2026.");
            else
                return (checkIn, checkOut);
        }
    }
}
 