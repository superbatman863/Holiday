using MySqlConnector;

namespace HolidayMaker;

// Sökning efter lediga rum (med filter och sortering). SQL sköter allt urval och all sortering.
internal static class RoomSearch
{
    private const string SearchSql =
        "SELECT r.RoomID, a.Name, a.Location, r.RoomNumber, r.MaxGuests, r.PricePerNight, a.Rating " +
        "FROM Rooms r JOIN Accommodations a ON a.AccommodationID = r.AccommodationID " +
        "WHERE a.Location = @location " +
        "AND r.MaxGuests >= @guests " +
        "AND " + BookingService.RoomIsFree + " " +
        "AND (@pool = 0 OR a.HasPool = 1) " +
        "AND (@restaurant = 0 OR a.HasRestaurant = 1) " +
        "AND (@maxBeach IS NULL OR a.BeachDistance <= @maxBeach) " +
        "AND (@maxCenter IS NULL OR a.CenterDistance <= @maxCenter) ";

    // ORDER BY kan inte parameteriseras. Därför väljs en av två fasta strängar.
    private const string OrderByPrice = "ORDER BY r.PricePerNight ASC, a.Name";
    private const string OrderByRating = "ORDER BY a.Rating DESC, r.PricePerNight ASC";

    public static void SearchAndBook(Customer customer)
    {
        var location = ChooseLocation();
        if (location is null) return;

        var (checkIn, checkOut) = Input.AskStay();
        var guests = Input.AskInt("Antal personer: ", 1, 20);
        var pool = Input.AskYesNo("Måste ha pool?");
        var restaurant = Input.AskYesNo("Måste ha restaurang?");
        var maxBeach = Input.AskOptionalInt("Max avstånd till strand i meter (Enter = valfritt): ");
        var maxCenter = Input.AskOptionalInt("Max avstånd till centrum i meter (Enter = valfritt): ");
        var sort = Input.AskInt("Sortera: 1 = lägst pris först, 2 = högst omdöme först: ", 1, 2);

        var sql = SearchSql + (sort == 1 ? OrderByPrice : OrderByRating);

        var found = 0;

        using (var conn = Db.Open())
        using (var cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@location", location);
            cmd.Parameters.AddWithValue("@guests", guests);
            cmd.Parameters.AddWithValue("@checkIn", checkIn);
            cmd.Parameters.AddWithValue("@checkOut", checkOut);
            cmd.Parameters.AddWithValue("@pool", pool ? 1 : 0);
            cmd.Parameters.AddWithValue("@restaurant", restaurant ? 1 : 0);
            cmd.Parameters.AddWithValue("@maxBeach", maxBeach.HasValue ? (object)maxBeach.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@maxCenter", maxCenter.HasValue ? (object)maxCenter.Value : DBNull.Value);

            using var r = cmd.ExecuteReader();

            Console.WriteLine();
            Console.WriteLine($"{"RoomID",-7}{"Boende",-22}{"Ort",-12}{"Rum",-6}{"Platser",-8}{"Pris/natt",-11}{"Omdöme"}");

            while (r.Read())
            {
                found++;

                Console.WriteLine($"{r.GetInt32("RoomID"),-7}{r.GetString("Name"),-22}{r.GetString("Location"),-12}" +
                                  $"{r.GetString("RoomNumber"),-6}{r.GetInt32("MaxGuests"),-8}" +
                                  $"{r.GetDecimal("PricePerNight") + " kr",-11}{r.GetDecimal("Rating")}");
            }
        }

        if (found == 0)
        {
            Console.WriteLine("Inga lediga rum hittades för sökningen.");
            return;
        }

        var roomId = Input.AskOptionalInt("Ange RoomID för att boka (Enter = avbryt): ");

        if (roomId is null)
            return;

        BookingService.Create(customer, roomId.Value, checkIn, checkOut, guests);
    }

    // Hämtar orterna från databasen och låter operatören välja med siffra.
    private static string? ChooseLocation()
    {
        var locations = new List<string>();

        using (var conn = Db.Open())
        using (var cmd = new MySqlCommand(
            "SELECT DISTINCT Location FROM Accommodations ORDER BY Location", conn))
        using (var r = cmd.ExecuteReader())
        {
            while (r.Read())
                locations.Add(r.GetString("Location"));
        }

        if (locations.Count == 0)
        {
            Console.WriteLine("Det finns inga orter i databasen.");
            return null;
        }

        Console.WriteLine("Välj ort:");

        for (var i = 0; i < locations.Count; i++)
            Console.WriteLine($"{i + 1}. {locations[i]}");

        return locations[Input.AskInt("Val: ", 1, locations.Count) - 1];
    }
}