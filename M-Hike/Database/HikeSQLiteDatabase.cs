using SQLite;
using M_Hike.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Hike.Database
{
    public class HikeSQLiteDatabase
    {
        // Change the declarations of dbConnection and currentState to nullable types to resolve CS8618
        SQLiteConnection dbConnection;
        public const string dbFileName = "Hikes.db3";
        public const SQLiteOpenFlags flags =
            SQLiteOpenFlags.ReadWrite | // Open the database in read/write mode
            SQLiteOpenFlags.Create | // Create the database if it doesn't exist
            SQLiteOpenFlags.SharedCache; // Enable multi-threaded database access

        public static string dbPath = "";
        public string? currentState;

        public string TABLE_NAME = "hikes";
        public string ID_COLUMN = "id";
        public string NAME_COLUMN = "name";
        public string LOCATION_COLUMN = "location";
        public string DATE_COLUMN = "date";
        public string PARKING_COLUMN = "parkingAvailability";
        public string LENGTH_COLUMN = "length";
        public string DIFFICULTY_COLUMN = "difficulty";
        public string DESCRIPTION_COLUMN = "description";
        public string HIKE_MEMBERS_COLUMN = "members";
        public string HIKE_GEAR_COLUMN = "gear";

        public HikeSQLiteDatabase()
        {
            Init();
        }

        // Initialize the database connection and create the Hike table if it doesn't exist
        private void Init()
        {
            try
            {
                if (dbConnection != null)
                {
                    currentState = "Database connection already exists.";
                }

                dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, dbFileName);

                dbConnection = new SQLiteConnection(dbPath);
                dbConnection.CreateTable<Hike>();
                //ResetDatabase();
                InsertTestData();

                currentState = "Database created successfully.";
            }
            catch (SQLiteException ex)
            {
                currentState = $"SQLiteException: {ex.Message}";
            }
        }

        // Reset the database
        public void ResetDatabase()
        {
            try
            {
                if (dbConnection == null)
                {
                    currentState = "Database connection does not exist.";
                    return;
                }
                dbConnection.DropTable<Hike>();
                dbConnection.CreateTable<Hike>();
                currentState = "Database reset successfully.";
            }
            catch (SQLiteException ex)
            {
                currentState = $"SQLiteException: {ex.Message}";
            }
        }

        // Insert test data 
        public void InsertTestData()
        {
            if (dbConnection.Table<Hike>().Count() == 0)
            {
                var hikes = new List<Hike>
                {
                    new() {
                        Name = "Mountain Adventure",
                        Location = "Swiss Alps",
                        Date = new DateTime(2024, 8, 15),
                        ParkingAvailability = true,
                        Length = 15.8,
                        Difficulty = 6,
                        Description = "A challenging hike with breathtaking views.",
                        Members = "John Doe,Jane Smith",
                        Gear = "Hiking Boots,Water Bottle,Backpack"
                    },
                    new() {
                        Name = "Forest Trail",
                        Location = "Black Forest",
                        Date = new DateTime(2024, 9, 5),
                        ParkingAvailability = false,
                        Length = 8,
                        Difficulty = 3,
                        Description = "A beautiful walk through a dense forest.",
                        Members = "Peter Jones",
                        Gear = "Comfortable Shoes,Snacks"
                    },
                    new() {
                        Name = "Coastal Walk",
                        Location = "Cliffs of Moher",
                        Date = new DateTime(2024, 7, 20),
                        ParkingAvailability = true,
                        Length = 12.6,
                        Difficulty = 9,
                        Description = "A scenic walk along the stunning coastline.",
                        Members = "Mary Williams,David Brown",
                        Gear = "Windbreaker,Camera"
                    }
                };
                for (int i = 0; i < hikes.Count; i++)
                {
                    InsertHike(hikes[i]);
                }
            }
        }

        // CRUD Operations
        public void InsertHike(Hike hike)
        {
            try
            {
                dbConnection.Insert(hike);
                currentState = "Hike inserted successfully.";
            }
            catch (SQLiteException ex)
            {
                currentState = $"SQLiteException: {ex.Message}";
            }
        }

        public void UpdateHike(Hike hike)
        {
            try
            {
                dbConnection.Update(hike);
                currentState = "Hike updated successfully.";
            }
            catch (SQLiteException ex)
            {
                currentState = $"SQLiteException: {ex.Message}";
            }
        }

        public bool DeleteHikeById (int id)
        {
            try
            {
                var hikeToDelete = dbConnection.Find<Hike>(id);
                if (hikeToDelete != null)
                {
                    dbConnection.Delete(hikeToDelete);
                    currentState = "Hike deleted successfully.";
                    return true;
                }
                else
                {
                    currentState = "Hike not found.";
                    return false;
                }

            }
            catch (SQLiteException ex)
            {
                currentState = $"SQLiteException: {ex.Message}";
                return false;
            }
        }

        public List<Hike> GetAllHikes()
        {
            try
            {
                return dbConnection.Table<Hike>().ToList();
            }
            catch (SQLiteException ex)
            {
                currentState = $"SQLiteException: {ex.Message}";
                return new List<Hike>();
            }
        }

        public Hike GetHikeById(int id)
        {
            try
            {
                var hike = dbConnection.Find<Hike>(id);
                if (hike != null)
                {
                    return hike;
                }
                else
                {
                    currentState = "Hike not found.";
                    return new Hike();
                }
            }
            catch (SQLiteException ex)
            {
                currentState = $"SQLiteException: {ex.Message}";
                return new Hike();
            }
        }
    }
}
