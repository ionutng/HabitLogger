using Microsoft.Data.Sqlite;

using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
{
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = 
        @"
            CREATE TABLE IF NOT EXISTS drinking_water (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER)
        ";

    command.ExecuteNonQuery();

    connection.Close();
}