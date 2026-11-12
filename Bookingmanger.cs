using MySqlConnector;
 
namespace HolidayMaker;
 
// Hantera en kunds befintliga bokningar: visa, ändra antal personer, avboka.
internal static class BookingManager
{
    // Returnerar true om kunden har minst en bokning
    public static bool Show(Customer customer)
    {
        using var conn = Db.Open();
        using var cmd = new MySqlCommand(
            "SELECT b.BookingID, a.Name, r.RoomNumber, b.CheckInDate, b.CheckOutDate, b.NumberOfGuests, " +
            "       DATEDIFF(b.CheckOutDate, b.CheckInDate) * r.PricePerNight AS Total " +
            "FROM Bookings b " +
            "JOIN Rooms r ON r.RoomID = b.RoomID " +
            "JOIN Accommodations a ON a.AccommodationID = r.AccommodationID " +
            "WHERE b.CustomerID = @customer " +
            "ORDER BY b.CheckInDate", conn);
        cmd.Parameters.AddWithValue("@customer", customer.Id);
 
        using var r = cmd.ExecuteReader();
        var count = 0;
        Console.WriteLine();
        Console.WriteLine($"{"Nr",-5}{"Boende",-22}{"Rum",-6}{"Incheckning",-13}{"Utcheckning",-13}{"Pers",-6}{"Totalpris"}");
        while (r.Read())
        {
            count++;
            Console.WriteLine($"{r.GetInt32("BookingID"),-5}{r.GetString("Name"),-22}{r.GetString("RoomNumber"),-6}" +
                              $"{r.GetDateTime("CheckInDate"):yyyy-MM-dd}   {r.GetDateTime("CheckOutDate"):yyyy-MM-dd}   " +
                              $"{r.GetInt32("NumberOfGuests"),-6}{r.GetDecimal("Total")} kr");
        }
        if (count == 0) Console.WriteLine("Kunden har inga bokningar.");
        return count > 0;
    }
 
    public static void ChangeGuests(Customer customer)
    {
        if (!Show(customer)) return;
 
        var bookingId = Input.AskOptionalInt("Ange bokningsnummer (Enter = avbryt): ");
        if (bookingId is null) return;
        var guests = Input.AskInt("Nytt antal personer: ", 1, 20);
 
        using var conn = Db.Open();
        // Uppdaterar bara om bokningen tillhör vald kund OCH det nya antalet ryms i rummet
        using var cmd = new MySqlCommand(
            "UPDATE Bookings b JOIN Rooms r ON r.RoomID = b.RoomID " +
            "SET b.NumberOfGuests = @guests " +
            "WHERE b.BookingID = @booking AND b.CustomerID = @customer AND r.MaxGuests >= @guests", conn);
        cmd.Parameters.AddWithValue("@guests", guests);
        cmd.Parameters.AddWithValue("@booking", bookingId.Value);
        cmd.Parameters.AddWithValue("@customer", customer.Id);
 
        Console.WriteLine(cmd.ExecuteNonQuery() == 1
            ? "Antal personer uppdaterat."
            : "Kunde inte ändra: bokningen finns inte för den här kunden, eller rummet rymmer inte så många.");
    }
 
    public static void Cancel(Customer customer)
    {
        if (!Show(customer)) return;
 
        var bookingId = Input.AskOptionalInt("Ange bokningsnummer att avboka (Enter = avbryt): ");
        if (bookingId is null) return;
        if (!Input.AskYesNo($"Är du säker på att du vill avboka #{bookingId}?")) return;
 
        using var conn = Db.Open();
        using var cmd = new MySqlCommand(
            "DELETE FROM Bookings WHERE BookingID = @booking AND CustomerID = @customer", conn);
        cmd.Parameters.AddWithValue("@booking", bookingId.Value);
        cmd.Parameters.AddWithValue("@customer", customer.Id);
 
        Console.WriteLine(cmd.ExecuteNonQuery() == 1
            ? "Bokningen är avbokad."
            : "Ingen sådan bokning för den här kunden.");
    }
}
 