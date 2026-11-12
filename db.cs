using MySqlConnector;

namespace HolidayMaker;

// Ansvarar bara för att öppna en anslutning till databasen.
internal static class Db
{
    private static readonly string ConnStr =
        "Server=localhost;Port=3306;Database=holiday_maker;User ID=root;Password=Realmadrid2006;";

    public static MySqlConnection Open()
    {
        var conn = new MySqlConnection(ConnStr);
        conn.Open();
        return conn;
    }
}