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
                Console.WriteLine("\nHave a good day!");
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
                Console.Clear();
                Console.WriteLine("Wrong input! Please type a number between 0 and 4.");
                break;
        }
    }
}

static void Insert()
{
    DateTime date = GetDateInput("Please insert the date: (Format: dd-mm-yyyy) or Type 0 to return to the main menu.");

    if (date == DateTime.MinValue)
    {
        Console.Clear();
        Console.WriteLine("Incorrect format!");
        GetUserInput();
    }

    if (date == DateTime.MaxValue)
    {
        Console.Clear();
        Console.WriteLine("You can't input a future date!");
        GetUserInput();
    }

    if (!CheckDuplicate(date))
    {
        Console.Clear();
        Console.WriteLine($"A record with the date {date:dd-MM-yyyy} already exists!");
        GetUserInput();
    }

    int quantity = GetNumberInput("Please insert the number of glasses or Type 0 to return to the main menu.");

    if (quantity == -1)
    {
        Console.Clear();
        Console.WriteLine("Incorrect format!");
        GetUserInput();
    }


    using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
    {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $"INSERT INTO drinking_water (Date, Quantity) VALUES (\"{date:dd-MM-yyyy}\", {quantity})";

        command.ExecuteNonQuery();

        connection.Close();
    }

    Console.Clear();
    Console.WriteLine("The record has been inserted!");
}

static void GetRecords()
{
    Console.Clear();

    using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
    {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $"SELECT Date, Quantity FROM drinking_water ORDER BY Date ASC";

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

        if (tableData.Count > 0)
            Console.WriteLine("\nThe records are:");

        foreach (var data in tableData)
            Console.WriteLine($"{data.Date:dd-MM-yyyy} - {data.Quantity} glasses.");
    }
}

static void Delete()
{
    if (GetNumberOfRecords() == 0)
    {
        Console.Clear();
        Console.WriteLine("\nThere are no records yet!");
        GetUserInput();
    }

    Console.WriteLine("\nType 1 if you wish to delete only one record.");
    Console.WriteLine("Type 2 if you wish to delete all of the records.");
    Console.WriteLine("\nType 0 if you wish to return to the main menu.");

    string deleteOption = Console.ReadLine();


    if (deleteOption == "1")
    {
        GetRecords();
        DateTime date = GetDateInput("\nWhich day would you like to delete? Type using the Format: (dd-mm-yyyy)");

        if (date == DateTime.MinValue)
        {
            Console.WriteLine("Incorrect format!");
            GetUserInput();
        }

        using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM drinking_water WHERE Date = \"{date:dd-MM-yyyy}\"";

            int rowCount = command.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.Clear();
                Console.WriteLine($"\nThe record with Date: {date:dd-MM-yyyy} doesn't exist.");
                GetUserInput();
            }
            else
            {
                Console.Clear();
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
                Console.Clear();
                Console.WriteLine("\nThere are no records yet");
                GetUserInput();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("\nAll of the records have been successfully deleted!");
            }

            connection.Close();
        }
    }
    else
    {
        Console.Clear();
        GetUserInput();
    }
}

static void Update()
{
    if (GetNumberOfRecords() == 0)
    {
        Console.Clear();
        Console.WriteLine("\nThere are no records yet!");
        GetUserInput();
    }

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
        if (!CheckDate(oldDate))
        {
            Console.Clear();
            Console.WriteLine($"\nThe record with Date: {oldDate:dd-MM-yyyy} doesn't exist.");
            GetUserInput();
        }

        DateTime newDate = GetDateInput("Please insert the new date: (Format: dd-mm-yyyy) or Type 0 to return to the main menu.");

        if (newDate.CompareTo(DateTime.Now) > 0 || newDate == DateTime.MinValue)
        {
            Console.Clear();
            Console.WriteLine("\nYou can't input a future date!");
            GetUserInput();
        }

        if (newDate == DateTime.MinValue)
        {
            Console.Clear();
            Console.WriteLine("\nIncorrect format");
            GetUserInput();
        }

        using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"UPDATE drinking_water SET Date = \"{newDate:dd-MM-yyyy}\" where Date = \"{oldDate:dd-MM-yyyy}\"";

            int rowCount = command.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.Clear();
                Console.WriteLine($"\nA record with the date {oldDate:dd-MM-yyyy} doesn't exist.");
                GetUserInput();
            }
            else
            {
                Console.Clear();
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

        if (quantity == -1)
        {
            Console.Clear();
            Console.WriteLine("\nYou can't input a negative number");
            GetUserInput();
        }

        using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"UPDATE drinking_water SET Quantity = {quantity} where Date = \"{date:dd-MM-yyyy}\"";

            int rowCount = command.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.Clear();
                Console.WriteLine($"\nA record with the date {date:dd-MM-yyyy} doesn't exist.");
                GetUserInput();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("\nThe record has been successfully updated!");
            }

            connection.Close();
        }
    }
    else if (updateChoice == "3")
    {
        GetRecords();
        DateTime oldDate = GetDateInput("\nWhat date would you like to update? Type it in the Format: (dd-MM-yyyy)");
        if (!CheckDate(oldDate))
        {
            Console.Clear();
            Console.WriteLine($"\nThe record with Date: {oldDate:dd-MM-yyyy} doesn't exist.");
            GetUserInput();
        }

        DateTime newDate = GetDateInput("Please insert the new date: (Format: dd-mm-yyyy) or Type 0 to return to the main menu.");
        if (newDate.CompareTo(DateTime.Now) > 0)
        {
            Console.Clear();
            Console.WriteLine("\nYou can't input a future date!");
            GetUserInput();
        }

        if (newDate == DateTime.MinValue)
        {
            Console.Clear();
            Console.WriteLine("\nIncorrect format");
            GetUserInput();
        }

        int quantity = GetNumberInput("Please insert the new number of glasses or Type 0 to return to the main menu.");

        if (quantity == -1)
        {
            Console.Clear();
            Console.WriteLine("\nYou can't input a negative number");
            GetUserInput();
        }

        using (var connection = new SqliteConnection("Data Source=habit-logger.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"UPDATE drinking_water SET Date = \"{newDate:dd-MM-yyyy}\", Quantity = {quantity} where Date = \"{oldDate:dd-MM-yyyy}\"";

            int rowCount = command.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.Clear();
                Console.WriteLine($"\nA record with the date {oldDate:dd-MM-yyyy} doesn't exist.");
                GetUserInput();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("\nThe record has been successfully updated!");
            }

            connection.Close();
        }
    }
    else
    {
        Console.Clear();
        GetUserInput();
    }
}

static DateTime GetDateInput(string message)
{
    Console.WriteLine($"\n{message}");

    string dateInput = Console.ReadLine();

    if (dateInput == "0")
    {
        Console.Clear();
        GetUserInput();
    }

    if (!DateTime.TryParseExact(dateInput, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date))
        return DateTime.MinValue;

    if (date.CompareTo(DateTime.Now) > 0)
        return DateTime.MaxValue;

    return date;
}

static int GetNumberInput(string message)
{
    Console.WriteLine($"\n{message}");
    
    string numberInput = Console.ReadLine();

    if (numberInput == "0")
    {
        Console.Clear();
        GetUserInput();
    }

    if (!int.TryParse(numberInput, out int number) || number < 0)
        return -1;

    return number;
}

static bool CheckDuplicate(DateTime date)
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

        connection.Close();

        foreach (var data in tableData)
            if (data.Date.ToString("dd-MM-yyyy") == date.ToString("dd-MM-yyyy"))
                return false;
    
        return true;
    }
}

static int GetNumberOfRecords()
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

        connection.Close();

        return tableData.Count;
    }
}

static bool CheckDate(DateTime date)
{
    bool dateFound = false;

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

        connection.Close();

        foreach (var data in tableData)
            if (data.Date.ToString("dd-MM-yyyy") == date.ToString("dd-MM-yyyy"))
                dateFound = true;

        return dateFound;
    }
}

public class DrinkingWater
{
    public DateTime Date { get; set; }
    
    public int Quantity { get; set; }
}