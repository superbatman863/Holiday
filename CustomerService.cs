using MySqlConnector;
 
namespace HolidayMaker;
 
internal record Customer(int Id, string Name);
 
// Registrera och välja kund
internal static class CustomerService
{
    // Returnerar den nya kunden, eller null om inmatningen var ogiltig
    public static Customer? Register()
    {
        var first = Input.Ask("Förnamn: ");
        var last = Input.Ask("Efternamn: ");
        if (first == "" || last == "")
        {
            Console.WriteLine("Förnamn och efternamn måste fyllas i.");
            return null;
        }
        var email = Input.Ask("E-post: ");
        if (email == "")
        {
        Console.WriteLine("E-post måste fyllas i.");
        return null;
        }
        var phone = Input.AskPhone("Telefon: ");
        
        using var conn = Db.Open();
        using var cmd = new MySqlCommand(
            "INSERT INTO Customers (First_name, Last_name, Email, Phone) VALUES (@first, @last, @email, @phone)", conn);
        cmd.Parameters.AddWithValue("@first", first);
        cmd.Parameters.AddWithValue("@last", last);
        cmd.Parameters.AddWithValue("@email", email == "" ? DBNull.Value : (object)email);
        cmd.Parameters.AddWithValue("@phone", phone == "" ? DBNull.Value : (object)phone);
        cmd.ExecuteNonQuery();
 
        var customer = new Customer((int)cmd.LastInsertedId, $"{first} {last}");
        Console.WriteLine($"Kund registrerad och vald: {customer.Name} (#{customer.Id})");
        return customer;
    }
 
    // Visar alla kunder och låter operatören välja ett ID. Returnerar null om inget valdes.
    public static Customer? Choose()
    {
        using var conn = Db.Open();
 
        using (var list = new MySqlCommand(
            "SELECT CustomerID, First_name, Last_name, Email, Phone FROM Customers ORDER BY CustomerID", conn))
        using (var r = list.ExecuteReader())
        {
            Console.WriteLine();
            while (r.Read())
                Console.WriteLine($"{r.GetInt32("CustomerID"),3}  {r.GetString("First_name")} {r.GetString("Last_name")}" +
                                  $"  {(r.IsDBNull(r.GetOrdinal("Email")) ? "" : r.GetString("Email"))}" +
                                  $"  {(r.IsDBNull(r.GetOrdinal("Phone")) ? "" : r.GetString("Phone"))}");
        }
 
        var id = Input.AskOptionalInt("Ange kund-ID (Enter = avbryt): ");
        if (id is null) return null;
 
        using var find = new MySqlCommand(
            "SELECT First_name, Last_name FROM Customers WHERE CustomerID = @id", conn);
        find.Parameters.AddWithValue("@id", id.Value);
        using var rd = find.ExecuteReader();
        if (!rd.Read())
        {
            Console.WriteLine("Ingen kund med det ID:t.");
            return null;
        }
 
        var customer = new Customer(id.Value, $"{rd.GetString("First_name")} {rd.GetString("Last_name")}");
        Console.WriteLine($"Vald kund: {customer.Name}");
        return customer;
    }
}
 