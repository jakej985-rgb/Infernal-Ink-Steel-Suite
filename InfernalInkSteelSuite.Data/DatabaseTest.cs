using System;

namespace InfernalInkSteelSuite.Data
{
    public static class DatabaseTest
    {
        public static void TestConnection()
        {
            try
            {
                using (var connection = Database.CreateConnection())
                {
                    Console.WriteLine("Connection successful!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }
    }
}
