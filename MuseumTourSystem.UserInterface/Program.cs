using System;
using System.IO;
using MuseumTourSystem.BusinessLogic.Services;
using MuseumTourSystem.DataStorage.Repositories;

namespace MuseumTourSystem.UserInterface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Set up data directory
                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MuseumTourSystem");
                Directory.CreateDirectory(appDataPath);

                string dataFilePath = Path.Combine(appDataPath, "Muse-umTourData.xml");
                string schemaFilePath = Path.Combine(appDataPath, "Muse-umTourSchema.xsd");

                // Create repository and service
                var repository = new XMLMuseumTourRepository(dataFilePath, schemaFilePath);
                var service = new MuseumTourService(repository);

                // Create and run UI
                var ui = new ConsoleUI(service);
                ui.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }
    }
}


