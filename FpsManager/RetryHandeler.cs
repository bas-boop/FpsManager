using System;
using System.Diagnostics;
using System.IO;

namespace FpsManager
{
    public static class RetryHandler
    {
        public static void Retry()
        {
            Console.WriteLine("Want to set up the settings? This requires restarting the program.\nType 'y' for restart.");
            string userInput = Console.ReadLine();
        
            if (File.Exists(Saving.CONFIG_FILE_PATH)) 
                File.Delete(Saving.CONFIG_FILE_PATH);
        
            if (userInput?.Trim().ToLower() != "yes")
                return;
        
            RestartApplication();
        }

        private static void RestartApplication()
        {
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = Environment.GetCommandLineArgs()[0],
                    UseShellExecute = true
                };
            
                Process.Start(startInfo);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restarting the application: {ex.Message}");
            }
        }
    }   
}