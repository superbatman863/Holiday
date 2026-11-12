using MySqlConnector;
 
namespace HolidayMaker;
 
// Skapar bokningar.
internal static class BookingService
{
    // Ett rum är upptaget om en befintlig bokning överlappar: start < vår utcheckning OCH slut > vår incheckning.
    // Eftersom jämförelserna är strikta (< och >) upptar utcheckningsdagen inte rummet.
    // Används både vid sökning (RoomSearch) och när bokningen sparas.
    internal const string RoomIsFree =
        "NOT EXISTS (SELECT 1 FROM Bookings b WHERE b.RoomID = r.RoomID " +
        "AND b.CheckInDate < @checkOut AND b.CheckOutDate > @checkIn)";
 
    // Kontrollerar tillgänglighet och kapacitet IGEN i samma sats som sparar bokningen.
    private const string InsertBookingSql =
        "INSERT INTO Bookings (CustomerID, RoomID, CheckInDate, CheckOutDate, NumberOfGuests) " +
        "SELECT @customer, r.RoomID, @checkIn, @checkOut, @guests " +
        "FROM Rooms r " +
        "WHERE r.RoomID = @room AND r.MaxGuests >= @guests AND " + RoomIsFree;
 
    public static void Create(Customer customer, int roomId, DateTime checkIn, DateTime checkOut, int guests)
    {
        using var conn = Db.Open();
 
        using var insert = new MySqlCommand(InsertBookingSql, conn);
        insert.Parameters.AddWithValue("@customer", customer.Id);
        insert.Parameters.AddWithValue("@room", roomId);
        insert.Parameters.AddWithValue("@checkIn", checkIn);
        insert.Parameters.AddWithValue("@checkOut", checkOut);
        insert.Parameters.AddWithValue("@guests", guests);
 
        // 0 rader = rummet fanns inte, var för litet eller blev upptaget
        if (insert.ExecuteNonQuery() == 0)
        {
            Console.WriteLine("Bokningen kunde inte genomföras (rummet finns inte, är för litet eller är upptaget).");
            return;
        }
        var bookingId = insert.LastInsertedId;
 
        using var price = new MySqlCommand(
            "SELECT DATEDIFF(@checkOut, @checkIn) * PricePerNight FROM Rooms WHERE RoomID = @room", conn);
        price.Parameters.AddWithValue("@checkIn", checkIn);
        price.Parameters.AddWithValue("@checkOut", checkOut);
        price.Parameters.AddWithValue("@room", roomId);
        var total = Convert.ToDecimal(price.ExecuteScalar());
 
        Console.WriteLine();
        Console.WriteLine("Bokning bekräftad!");
        Console.WriteLine($"Bokningsnummer: {bookingId}");
        Console.WriteLine($"Totalpris: {total} kr ({(checkOut - checkIn).Days} nätter)");
    }
}
 