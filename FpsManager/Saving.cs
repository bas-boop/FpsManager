using System;

namespace FpsManager
{
    /// <summary>
    /// Class that saves and give data that user has selected.
    /// </summary>
    public static class Saving
    {
        public const string CONFIG_FILE_PATH = "refreshRateConfig.txt";
        
        public static HzData GetRefreshRates()
        {
            if (!System.IO.File.Exists(CONFIG_FILE_PATH))
                return GetUserRefreshRates();
            
            try
            {
                string[] configContent = System.IO.File.ReadAllLines(CONFIG_FILE_PATH);
                
                if (configContent.Length == 2
                    && int.TryParse(configContent[0], out int standaloneHz)
                    && int.TryParse(configContent[1], out int lowestHz))
                {
                    return Screen.AreCurrentSettingsUsable(lowestHz, standaloneHz)
                        ? GetUserRefreshRates() 
                        : new (standaloneHz, lowestHz);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading config file: {ex.Message}");
            }

            return GetUserRefreshRates();
        }

        private static HzData GetUserRefreshRates()
        {
            Console.WriteLine("Enter the desired refresh rate for the standalone screen (e.g., 120, 240): ");
            string userInput = Console.ReadLine();
            
            if (!int.TryParse(userInput, out int userHz))
            {
                userHz = 120;
                Console.WriteLine($"Invalid input given. Desired refresh rate is set to {userHz}.");
            }
            
            Console.WriteLine("Enter the lowest possible refresh rate for your screen (e.g., 50, 60): ");
            string lowestInput = Console.ReadLine();
            
            if (!int.TryParse(lowestInput, out int lowestHz))
            {
                lowestHz = 60;
                Console.WriteLine($"Invalid input given. Lowest possible refresh rate is set to {lowestHz}.");
            }
            
            System.IO.File.WriteAllLines(CONFIG_FILE_PATH, [userHz.ToString(), lowestHz.ToString()]);
            return new (userHz, lowestHz);
        }
    }
}