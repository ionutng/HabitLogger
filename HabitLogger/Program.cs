using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
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
                Update();
                break;
            default:
                Console.WriteLine("Wrong input! Please type a number between 0 and 4.");
                break;
        }
    }
}

static void Insert()
{
    DateTime date = GetDateInput("Please insert the date: (Format: dd-mm-yyyy) or Type 0 to return to the main menu.");

    int quantity = GetNumberInput("Please insert the number of glasses or Type 0 to return to the main menu.");

    using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
    {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $"INSERT INTO drinking_water (Date, Quantity) VALUES (\"{date:dd-MM-yyyy}\", {quantity})";

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

        Console.WriteLine("\nThe records are:");
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
        DateTime date = GetDateInput("\nWhich day would you like to delete? Type using the Format: (dd-mm-yyyy)");

        if (date == DateTime.MinValue)
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

static void Update()
{
    Console.WriteLine("\nWhat would you like to update?");
    Console.WriteLine("Type 1 for date");
    Console.WriteLine("Type 2 for quantity");
    Console.WriteLine("Type 3 for both");
    Console.WriteLine("\nType 0 if you wish to return to the main menu.");

    string updateChoice = Console.ReadLine();

    if (updateChoice == "1")
    {
        GetRecords();
        DateTime oldDate = GetDateInput("\nWhat date would you like to update? Type it in the Format: (dd-MM-yyyy)");
        DateTime newDate = GetDateInput("Please insert the new date: (Format: dd-mm-yyyy) or Type 0 to return to the main menu.");

        using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"UPDATE drinking_water SET Date = \"{newDate:dd-MM-yyyy}\" where Date = \"{oldDate:dd-MM-yyyy}\"";

            int rowCount = command.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"\nA record with the date {oldDate:dd-MM-yyyy} doesn't exist.");
                GetUserInput();
            }
            else
            {
                Console.WriteLine("\nThe record has been successfully updated!");
            }

            connection.Close();
        }
    }
    else if (updateChoice == "2")
    {
        GetRecords();
        DateTime date = GetDateInput("\nWhat date would you like to update? Type it in the Format: (dd-MM-yyyy)");
        int quantity = GetNumberInput("Please insert the new number of glasses or Type 0 to return to the main menu.");

        using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"UPDATE drinking_water SET Quantity = {quantity} where Date = \"{date:dd-MM-yyyy}\"";

            int rowCount = command.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"\nA record with the date {date:dd-MM-yyyy} doesn't exist.");
                GetUserInput();
            }
            else
            {
                Console.WriteLine("\nThe record has been successfully updated!");
            }

            connection.Close();
        }
    }
    else if (updateChoice == "3")
    {
        GetRecords();
        DateTime oldDate = GetDateInput("\nWhat date would you like to update? Type it in the Format: (dd-MM-yyyy)");
        DateTime newDate = GetDateInput("Please insert the new date: (Format: dd-mm-yyyy) or Type 0 to return to the main menu.");
        int quantity = GetNumberInput("Please insert the new number of glasses or Type 0 to return to the main menu.");

        using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"UPDATE drinking_water SET Date = \"{newDate:dd-MM-yyyy}\", Quantity = {quantity} where Date = \"{oldDate:dd-MM-yyyy}\"";

            int rowCount = command.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"\nA record with the date {oldDate:dd-MM-yyyy} doesn't exist.");
                GetUserInput();
            }
            else
            {
                Console.WriteLine("\nThe record has been successfully updated!");
            }

            connection.Close();
        }
    }
}

static DateTime GetDateInput(string message)
{
    Console.WriteLine(message);

    string dateInput = Console.ReadLine();

    if (dateInput == "0") 
        GetUserInput();

    if (!DateTime.TryParseExact(dateInput, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date))
    {
        Console.WriteLine("Incorrect format!");
        return DateTime.MinValue;
    }

    return date;
}

static int GetNumberInput(string message)
{
    Console.WriteLine(message);
    
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