using Microsoft.Data.Sqlite;
using System.Globalization;

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

GetUserInput();

static void GetUserInput()
{
    while (true)
    {
        Console.WriteLine("\nWelcome to the Habit Logger app!");
        Console.WriteLine("What would you like to do?\n");
        Console.WriteLine("Type 0 to Close Application.");
        Console.WriteLine("Type 1 to View All Records.");
        Console.WriteLine("Type 2 to Insert Record.");
        Console.WriteLine("Type 3 to Delete Record.");
        Console.WriteLine("Type 4 to Update Record.");
        Console.WriteLine("------------------------------");

        string userInput = Console.ReadLine();

        switch (userInput)
        {
            case "0":
                Console.WriteLine("Have a good day!");
                Environment.Exit(0);
                break;
            case "1":
                GetRecords();
                break;
            case "2":
                Insert();
                break;
            case "3":
                Delete();
                break;
            case "4":
                Console.WriteLine("update");
                break;
            default:
                Console.WriteLine("Wrong input! Please type a number between 0 and 4.");
                break;
        }
    }
}

static void Insert()
{
    string date = GetDateInput();

    int quantity = GetNumberInput();

    using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
    {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $"INSERT INTO drinking_water (Date, Quantity) VALUES (\"{date}\", {quantity})";

        command.ExecuteNonQuery();

        connection.Close();
    }
}

static void GetRecords()
{
    using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
    {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $"SELECT Date, Quantity FROM drinking_water";

        List<DrinkingWater> tableData = new List<DrinkingWater>();

        SqliteDataReader reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                tableData.Add(new DrinkingWater
                {
                    Date = DateTime.ParseExact(reader.GetString(0), "dd-MM-yyyy", new CultureInfo("en-US")),
                    Quantity = reader.GetInt32(1)
                });
            }
        }
        else
        {
            Console.WriteLine("\nThere are no records yet!");
        }

        connection.Close();

        Console.WriteLine();
        foreach (var data in tableData)
            Console.WriteLine($"{data.Date:dd-MM-yyyy} - {data.Quantity} glasses.");
    }
}

static void Delete()
{
    Console.WriteLine("\nType 1 if you wish to delete only one record.");
    Console.WriteLine("Type 2 if you wish to delete all of the records.");
    Console.WriteLine("\nType 0 if you wish to return to the main menu.");

    string deleteOption = Console.ReadLine();

    if (deleteOption == "0")
        GetUserInput();
    else if (deleteOption == "1")
    {
        GetRecords();
        Console.WriteLine("\nWhich day would you like to delete? Type using the Format: (dd-mm-yyyy)");
        string dateString = Console.ReadLine();
        DateTime date;

        if (!DateTime.TryParseExact(dateString, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out date))
        {
            Console.WriteLine("Incorrect format!");
            Delete();
        }

        using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM drinking_water WHERE Date = \"{date:dd-MM-yyyy}\"";

            int rowCount = command.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"\nThe record with Date: {date:dd-MM-yyyy} doesn't exist.");
                Delete();
            }
            else
            {
                Console.WriteLine("\nThe record has been successfully deleted!");
            }

            connection.Close();
            
        }
    }
    else if (deleteOption == "2")
    {
        using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM drinking_water";

            int rowCount = command.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine("\nThere are no records yet");
                GetUserInput();
            }
            else
            {
                Console.WriteLine("\nAll of the records have been successfully deleted!");
            }

            connection.Close();
        }
    }
}

static string GetDateInput()
{
    Console.WriteLine("Please insert the date: (Format: dd-mm-yyyy) or Type 0 to return to the main menu.");

    string dateInput = Console.ReadLine();

    if (dateInput == "0") 
        GetUserInput();

    return dateInput;
}

static int GetNumberInput()
{
    Console.WriteLine("Please insert the number of glasses or Type 0 to return to the main menu.");
    
    string numberInput = Console.ReadLine();

    if (numberInput == "0")
        GetUserInput();

    return Convert.ToInt32(numberInput);
}

public class DrinkingWater
{
    public DateTime Date { get; set; }
    
    public int Quantity { get; set; }
}