using MySqlConnector;
 
namespace HolidayMaker;
 
// Bara menyn: läser operatörens val och anropar rätt tjänst.
internal static class Program
{
    private static Customer? _customer;
 
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
 
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Holiday Maker ===");
            Console.WriteLine(_customer is null ? "Vald kund: (ingen)" : $"Vald kund: {_customer.Name} (#{_customer.Id})");
            Console.WriteLine("1. Registrera ny kund");
            Console.WriteLine("2. Välj befintlig kund");
            Console.WriteLine("3. Sök lediga rum och boka");
            Console.WriteLine("4. Visa kundens bokningar");
            Console.WriteLine("5. Ändra antal personer i en bokning");
            Console.WriteLine("6. Avboka");
            Console.WriteLine("0. Avsluta");
 
            var choice = Input.Ask("Val: ");
            try
            {
                switch (choice)
                {
                    case "1": _customer = CustomerService.Register() ?? _customer; break;
                    case "2": _customer = CustomerService.Choose() ?? _customer; break;
                    case "3": if (RequireCustomer()) RoomSearch.SearchAndBook(_customer!); break;
                    case "4": if (RequireCustomer()) BookingManager.Show(_customer!); break;
                    case "5": if (RequireCustomer()) BookingManager.ChangeGuests(_customer!); break;
                    case "6": if (RequireCustomer()) BookingManager.Cancel(_customer!); break;
                    case "0": return;
                    default: Console.WriteLine("Ogiltigt val."); break;
                }
            }
            catch (MySqlException ex)
            {
                // Databasfel (t.ex. databasen nere) ska inte krascha programmet
                Console.WriteLine($"Databasfel: {ex.Message}");
            }
        }
    }
 
    private static bool RequireCustomer()
    {
        if (_customer is not null) return true;
        Console.WriteLine("Välj eller registrera en kund först.");
        return false;
    }
}