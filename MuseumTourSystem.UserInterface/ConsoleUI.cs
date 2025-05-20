using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using MuseumTourSystem.BusinessLogic.Services;
using MuseumTourSystem.Models.Models;

namespace MuseumTourSystem.UserInterface
{
    public class ConsoleUI
    {

        private readonly MuseumTourService _service;
        private bool _running = true;

        public ConsoleUI(MuseumTourService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void Run()
        {
            DisplayWelcomeMessage();

            while (_running)
            {
                try
                {
                    DisplayMainMenu();
                    var choice = GetMenuChoice(0, 8);

                    ProcessMainMenuChoice(choice);
                }
                catch (ApplicationException ex)
                {
                    DisplayError(ex.Message);
                }
                catch (Exception ex)
                {
                    DisplayError($"An unexpected error occurred: {ex.Message}");
                }
            }
        }


        #region Display Methods

        private void DisplayWelcomeMessage()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("         MUSEUM TOUR ADMINISTRATION SYSTEM         ");
            Console.WriteLine("===================================================");
            Console.WriteLine();
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===================================================");
            Console.WriteLine("                     MAIN MENU                     ");
            Console.WriteLine("===================================================");
            Console.WriteLine("1. Manage Tours");
            Console.WriteLine("2. Manage Cities");
            Console.WriteLine("3. Manage Museum Visits");
            Console.WriteLine("4. Manage Members");
            Console.WriteLine("5. Manage City-Tour Relationships");
            Console.WriteLine("6. Manage Member-Tour Relationships");
            Console.WriteLine("7. Manage Member-Museum Visit Relation-ships");
            Console.WriteLine("8. View Reports");
            Console.WriteLine("0. Exit");
            Console.WriteLine("===================================================");
            Console.Write("Enter your choice: ");
        }

        private void DisplayError(string message)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {message}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private void DisplaySuccess(string message)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"SUCCESS: {message}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private bool GetConfirmation(string message)
        {
            Console.WriteLine();
            Console.Write($"{message} (y/n): ");

            while (true)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Y)
                {
                    Console.WriteLine("Yes");
                    return true;
                }
                else if (key == ConsoleKey.N)
                {
                    Console.WriteLine("No");
                    return false;
                }
            }
        }

        #endregion


        #region Input Methods


        /// <summary>
        /// Gets a menu choice within the specified range
        /// </summary>
        /// <param name="min">Minimum allowed choice</param>
        /// <param name="max">Maximum allowed choice</param>
        /// <returns>The selected menu choice</returns>
        private int GetMenuChoice(int min, int max)
        {
            int choice;

            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out choice) && choice >= min && choice <= max)
                {
                    return choice;
                }

                Console.Write($"Invalid choice. Please enter a number be-tween {min} and {max}: ");
            }
        }

        private string GetNonEmptyString(string prompt)
        {
            string input;

            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();
            }
            while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        private DateTime GetDate(string prompt)
        {
            DateTime date;

            do
            {
                Console.Write(prompt);

                if (DateTime.TryParse(Console.ReadLine(), out date))
                {
                    return date;
                }

                Console.WriteLine("Invalid date format. Please try again.");
            }
            while (true);
        }

        private decimal GetDecimal(string prompt)
        {
            decimal value;

            do
            {
                Console.Write(prompt);

                if (decimal.TryParse(Console.ReadLine(), out value) && value >= 0)
                {
                    return value;
                }

                Console.WriteLine("Invalid value. Please enter a non-negative decimal number.");
            }
            while (true);
        }

        #endregion

        #region Menu Processing Methods

        private void ProcessMainMenuChoice(int choice)
        {
            switch (choice)
            {
                case 0:
                    _running = false;
                    Console.WriteLine("Thank you for using the Museum Tour Administration System. Goodbye!");
                    break;
                case 1:
                    ManageTours();
                    break;
                case 2:
                    ManageCities();
                    break;
                case 3:
                    ManageMuseumVisits();
                    break;
                case 4:
                    ManageMembers();
                    break;
                case 5:
                    ManageCityTourRelationships();
                    break;
                case 6:
                    ManageMemberTourRelationships();
                    break;
                case 7:
                    ManageMemberMuseumVisitRelationships();
                    break;
                case 8:
                    ViewReports();
                    break;
            }
        }


        private void ManageTours()
        {
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("===================================================");
                Console.WriteLine("                   MANAGE TOURS                    ");
                Console.WriteLine("===================================================");
                Console.WriteLine("1. Add a new tour");
                Console.WriteLine("2. Remove a tour");
                Console.WriteLine("3. View all tours");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("===================================================");
                Console.Write("Enter your choice: ");

                var choice = GetMenuChoice(0, 3);

                switch (choice)
                {
                    case 0:
                        managing = false;
                        break;
                    case 1:
                        AddTour();
                        break;
                    case 2:
                        RemoveTour();
                        break;
                    case 3:
                        ViewAllTours();
                        break;
                }
            }
        }

        private void AddTour()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                    ADD A TOUR                     ");
            Console.WriteLine("===================================================");

            var id = GetNonEmptyString("Enter tour ID: ");
            var name = GetNonEmptyString("Enter tour name: ");

            try
            {
                var tour = _service.AddTour(id, name);
                DisplaySuccess($"Tour '{tour.Name}' (ID: {tour.Id}) added successfully.");
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }


        private void RemoveTour()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                   REMOVE A TOUR                   ");
            Console.WriteLine("===================================================");

            var tours = _service.GetAllTours().ToList();

            if (tours.Count == 0)
            {
                DisplayError("No tours available to remove.");
                return;
            }

            for (int i = 0; i < tours.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tours[i].Name} (ID: {tours[i].Id})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a tour to remove: ");

            var choice = GetMenuChoice(0, tours.Count);

            if (choice == 0)
            {
                return;
            }

            var selectedTour = tours[choice - 1];

            if (GetConfirmation($"Are you sure you want to remove tour '{selectedTour.Name}' (ID: {selectedTour.Id})?"))
            {
                try
                {
                    if (_service.RemoveTour(selectedTour.Id))
                    {
                        DisplaySuccess($"Tour '{selectedTour.Name}' (ID: {selectedTour.Id}) removed successfully.");
                    }
                    else
                    {
                        DisplayError($"Failed to remove tour '{selectedTour.Name}' (ID: {selectedTour.Id}).");
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }

        private void ViewAllTours()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                    ALL TOURS                      ");
            Console.WriteLine("===================================================");

            var tours = _service.GetAllTours().ToList();

            if (tours.Count == 0)
            {
                Console.WriteLine("No tours available.");
            }
            else
            {
                foreach (var tour in tours)
                {
                    Console.WriteLine($"Tour: {tour.Name} (ID: {tour.Id})");
                    Console.WriteLine($"Cities: {tour.Cities.Count}");
                    Console.WriteLine($"Members: {tour.Members.Count}");
                    Console.WriteLine("---------------------------------------------------");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }


        private void ManageCities()
        {
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("===================================================");
                Console.WriteLine("                  MANAGE CITIES                    ");
                Console.WriteLine("===================================================");
                Console.WriteLine("1. Add a new city");
                Console.WriteLine("2. Remove a city");
                Console.WriteLine("3. View all cities");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("===================================================");
                Console.Write("Enter your choice: ");

                var choice = GetMenuChoice(0, 3);

                switch (choice)
                {
                    case 0:
                        managing = false;
                        break;
                    case 1:
                        AddCity();
                        break;
                    case 2:
                        RemoveCity();
                        break;
                    case 3:
                        ViewAllCities();
                        break;
                }
            }
        }

        private void AddCity()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                    ADD A CITY                     ");
            Console.WriteLine("===================================================");

            var id = GetNonEmptyString("Enter city ID: ");
            var name = GetNonEmptyString("Enter city name: ");

            try
            {
                var city = _service.AddCity(id, name);
                DisplaySuccess($"City '{city.Name}' (ID: {city.Id}) added successfully.");
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void RemoveCity()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                   REMOVE A CITY                   ");
            Console.WriteLine("===================================================");

            var cities = _service.GetAllCities().ToList();

            if (cities.Count == 0)
            {
                DisplayError("No cities available to remove.");
                return;
            }

            for (int i = 0; i < cities.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cities[i].Name} (ID: {cities[i].Id})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a city to remove: ");

            var choice = GetMenuChoice(0, cities.Count);

            if (choice == 0)
            {
                return;
            }

            var selectedCity = cities[choice - 1];

            if (GetConfirmation($"Are you sure you want to remove city '{selectedCity.Name}' (ID: {selectedCity.Id})?"))
            {
                try
                {
                    if (_service.RemoveCity(selectedCity.Id))
                    {
                        DisplaySuccess($"City '{selectedCity.Name}' (ID: {selectedCity.Id}) removed successfully.");
                    }
                    else
                    {
                        DisplayError($"Failed to remove city '{selectedCity.Name}' (ID: {selectedCity.Id}).");
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }


        private void ViewAllCities()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                    ALL CITIES                     ");
            Console.WriteLine("===================================================");

            var cities = _service.GetAllCities().ToList();

            if (cities.Count == 0)
            {
                Console.WriteLine("No cities available.");
            }
            else
            {
                foreach (var city in cities)
                {
                    Console.WriteLine($"City: {city.Name} (ID: {city.Id})");
                    Console.WriteLine($"Museum Visits: {city.MuseumVisits.Count}");
                    Console.WriteLine("---------------------------------------------------");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }


        private void ManageMuseumVisits()
        {
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("===================================================");
                Console.WriteLine("               MANAGE MUSEUM VISITS                ");
                Console.WriteLine("===================================================");
                Console.WriteLine("1. Add a new museum visit");
                Console.WriteLine("2. Remove a museum visit");
                Console.WriteLine("3. View all museum visits");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("===================================================");
                Console.Write("Enter your choice: ");

                var choice = GetMenuChoice(0, 3);

                switch (choice)
                {
                    case 0:
                        managing = false;
                        break;
                    case 1:
                        AddMuseumVisit();
                        break;
                    case 2:
                        RemoveMuseumVisit();
                        break;
                    case 3:
                        ViewAllMuseumVisits();
                        break;
                }
            }
        }


        private void AddMuseumVisit()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                ADD A MUSEUM VISIT                 ");
            Console.WriteLine("===================================================");
            var id = GetNonEmptyString("Enter museum visit ID: ");
            var museumName = GetNonEmptyString("Enter museum name: ");
            var visitDate = GetDate("Enter visit date (yyyy-mm-dd): ");
            var cost = GetDecimal("Enter cost: ");
            var cities = _service.GetAllCities().ToList();
            if (cities.Count == 0)
            {
                DisplayError("No cities available to assign to the museum visit.");
                return;
            }
            for (int i = 0; i < cities.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cities[i].Name} (ID: {cities[i].Id})");
            }
            Console.WriteLine("0. Cancel");
            Console.Write("Select a city to assign to the museum visit: ");
            var choice = GetMenuChoice(0, cities.Count);
            if (choice == 0)
            {
                return;
            }
            var selectedCity = cities[choice - 1];
            try
            {
                var museumVisit = _service.AddMuseumVisit(id, selectedCity.Id, museumName, visitDate, cost);
                DisplaySuccess($"Museum Visit '{museumVisit.MuseumName}' (ID: {museumVisit.Id}) added successfully.");
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void RemoveMuseumVisit()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("               REMOVE A MUSEUM VISIT               ");
            Console.WriteLine("===================================================");

            var museumVisits = _service.GetAllMuseumVisits().ToList();

            if (museumVisits.Count == 0)
            {
                DisplayError("No museum visits available to remove.");
                return;
            }

            for (int i = 0; i < museumVisits.Count; i++)
            {
                var visit = museumVisits[i];
                Console.WriteLine($"{i + 1}. {visit.MuseumName} in {visit.City?.Name} on {visit.VisitDate.ToShortDateString()} (ID: {visit.Id})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a museum visit to remove: ");

            var choice = GetMenuChoice(0, museumVisits.Count);

            if (choice == 0)
            {
                return;
            }

            var selectedVisit = museumVisits[choice - 1];

            if (GetConfirmation($"Are you sure you want to remove the visit to {selectedVisit.MuseumName} in {selectedVisit.City?.Name} on {selectedVisit.VisitDate.ToShortDateString()}?"))
            {
                try
                {
                    if (_service.RemoveMuseumVisit(selectedVisit.Id))
                    {
                        DisplaySuccess($"Museum visit to {selectedVisit.MuseumName} in {selectedVisit.City?.Name} on {selectedVisit.VisitDate.ToShortDateString()} removed successfully.");
                    }
                    else
                    {
                        DisplayError($"Failed to remove museum visit (ID: {selectedVisit.Id}).");
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }


        private void ViewAllMuseumVisits()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                ALL MUSEUM VISITS                  ");
            Console.WriteLine("===================================================");

            var museumVisits = _service.GetAllMuseumVisits().ToList();

            if (museumVisits.Count == 0)
            {
                Console.WriteLine("No museum visits available.");
            }
            else
            {
                foreach (var visit in museumVisits)
                {
                    Console.WriteLine($"Museum: {visit.MuseumName}");
                    Console.WriteLine($"City: {visit.City?.Name}");
                    Console.WriteLine($"Date: {visit.VisitDate.ToShortDateString()}");
                    Console.WriteLine($"Cost: {visit.Cost:C}");
                    Console.WriteLine($"Registered Members: {visit.RegisteredMembers.Count}");
                    Console.WriteLine("---------------------------------------------------");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }


        private void ManageMembers()
        {
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("===================================================");
                Console.WriteLine("                  MANAGE MEM-BERS                   ");
                Console.WriteLine("===================================================");
                Console.WriteLine("1. Add a new member");
                Console.WriteLine("2. Remove a member");
                Console.WriteLine("3. View all members");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("===================================================");
                Console.Write("Enter your choice: ");

                var choice = GetMenuChoice(0, 3);

                switch (choice)
                {
                    case 0:
                        managing = false;
                        break;
                    case 1:
                        AddMember();
                        break;
                    case 2:
                        RemoveMember();
                        break;
                    case 3:
                        ViewAllMembers();
                        break;
                }
            }
        }

        private void AddMember()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                   ADD A MEMBER                    ");
            Console.WriteLine("===================================================");

            var tours = _service.GetAllTours().ToList();

            if (tours.Count == 0)
            {
                DisplayError("No tours available. Please add a tour first.");
                return;
            }

            Console.WriteLine("Available tours:");
            for (int i = 0; i < tours.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tours[i].Name} (ID: {tours[i].Id})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a tour: ");

            var tourChoice = GetMenuChoice(0, tours.Count);

            if (tourChoice == 0)
            {
                return;
            }

            var selectedTour = tours[tourChoice - 1];

            var id = GetNonEmptyString("Enter member ID: ");
            var name = GetNonEmptyString("Enter member name: ");
            var bookingNumber = GetNonEmptyString("Enter booking number: ");

            try
            {
                var member = _service.AddMember(id, selectedTour.Id, name, bookingNumber);
                DisplaySuccess($"Member '{member.Name}' (Booking: {member.BookingNumber}) added to tour '{selectedTour.Name}' successfully.");
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void RemoveMember()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                  REMOVE A MEMBER                  ");
            Console.WriteLine("===================================================");

            var members = _service.GetAllMembers().ToList();

            if (members.Count == 0)
            {
                DisplayError("No members available to remove.");
                return;
            }

            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                Console.WriteLine($"{i + 1}. {member.Name} (Booking: {member.BookingNumber}, Tour: {member.Touring?.Name})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a member to remove: ");

            var choice = GetMenuChoice(0, members.Count);

            if (choice == 0)
            {
                return;
            }

            var selectedMember = members[choice - 1];

            if (GetConfirmation($"Are you sure you want to remove member '{selectedMember.Name}' (Booking: {selectedMember.BookingNumber})?"))
            {
                try
                {
                    if (_service.RemoveMember(selectedMember.Id))
                    {
                        DisplaySuccess($"Member '{selectedMember.Name}' (Booking: {selectedMember.BookingNumber}) removed successfully.");
                    }
                    else
                    {
                        DisplayError($"Failed to remove member (ID: {selectedMember.Id}).");
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }

        private void ViewAllMembers()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                   ALL MEMBERS                     ");
            Console.WriteLine("===================================================");

            var members = _service.GetAllMembers().ToList();

            if (members.Count == 0)
            {
                Console.WriteLine("No members available.");
            }
            else
            {
                foreach (var member in members)
                {
                    Console.WriteLine($"Name: {member.Name}");
                    Console.WriteLine($"Booking Number: {member.BookingNumber}");
                    Console.WriteLine($"Tour: {member.Touring?.Name}");
                    Console.WriteLine($"Museum Visits: {member.RegisteredMuseumVisits.Count}");

                    decimal additionalCost = member.CalculateAdditionalCost();
                    if (additionalCost > 0)
                    {
                        Console.WriteLine($"Additional Cost: {additionalCost:C}");
                    }
                    else
                    {
                        Console.WriteLine("Additional Cost: None");
                    }

                    Console.WriteLine("---------------------------------------------------");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private void ManageCityTourRelationships()
        {
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("===================================================");
                Console.WriteLine("            MANAGE CITY TOUR RELATIONSHIPS         ");
                Console.WriteLine("===================================================");
                Console.WriteLine("1. Add a city to a tour");
                Console.WriteLine("2. Remove a city from a tour");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("===================================================");
                Console.Write("Enter your choice: ");

                var choice = GetMenuChoice(0, 2);

                switch (choice)
                {
                    case 0:
                        managing = false;
                        break;
                    case 1:
                        AddCityToTour();
                        break;
                    case 2:
                        RemoveCityFromTour();
                        break;
                }
            }
        }

        private void AddCityToTour()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                ADD A CITY TO A TOUR               ");
            Console.WriteLine("===================================================");

            var tours = _service.GetAllTours().ToList();

            if (tours.Count == 0)
            {
                DisplayError("No tours available. Please add a tour first.");
                return;
            }

            Console.WriteLine("Available tours:");
            for (int i = 0; i < tours.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tours[i].Name} (ID: {tours[i].Id})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a tour: ");

            var tourChoice = GetMenuChoice(0, tours.Count);

            if (tourChoice == 0)
            {
                return;
            }

            var selectedTour = tours[tourChoice - 1];

            var cities = _service.GetAllCities()
                .Where(c => !selectedTour.ContainsCity(c))
                .ToList();

            if (cities.Count == 0)
            {
                DisplayError("No cities available to add to this tour. Ei-ther there are no cities in the system, or all cities are already part of this tour.");
                return;
            }

            Console.WriteLine($"Available cities to add to tour '{selectedTour.Name}':");
            for (int i = 0; i < cities.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cities[i].Name} (ID: {cities[i].Id})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a city to add: ");

            var cityChoice = GetMenuChoice(0, cities.Count);

            if (cityChoice == 0)
            {
                return;
            }

            var selectedCity = cities[cityChoice - 1];

            try
            {
                if (_service.AddCityToTour(selectedTour.Id, selectedCity.Id))
                {
                    DisplaySuccess($"City '{selectedCity.Name}' added to tour '{selectedTour.Name}' successfully.");
                }
                else
                {
                    DisplayError($"Failed to add city '{selectedCity.Name}' to tour '{selectedTour.Name}'.");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void RemoveCityFromTour()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("              REMOVE A CITY FROM A TOUR            ");
            Console.WriteLine("===================================================");

            var tours = _service.GetAllTours().ToList();

            if (tours.Count == 0)
            {
                DisplayError("No tours available.");
                return;
            }

            Console.WriteLine("Available tours:");
            for (int i = 0; i < tours.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tours[i].Name} (ID: {tours[i].Id})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a tour: ");

            var tourChoice = GetMenuChoice(0, tours.Count);

            if (tourChoice == 0)
            {
                return;
            }

            var selectedTour = tours[tourChoice - 1];

            if (selectedTour.Cities.Count == 0)
            {
                DisplayError($"Tour '{selectedTour.Name}' does not have any cities to remove.");
                return;
            }

            Console.WriteLine($"Cities in tour '{selectedTour.Name}':");
            for (int i = 0; i < selectedTour.Cities.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {selectedTour.Cities[i].Name} (ID: {selectedTour.Cities[i].Id})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a city to remove: ");

            var cityChoice = GetMenuChoice(0, selectedTour.Cities.Count);

            if (cityChoice == 0)
            {
                return;
            }

            var selectedCity = selectedTour.Cities[cityChoice - 1];

            try
            {
                if (_service.RemoveCityFromTour(selectedTour.Id, selectedCity.Id))
                {
                    DisplaySuccess($"City '{selectedCity.Name}' removed from tour '{selectedTour.Name}' successfully.");
                }
                else
                {
                    DisplayError($"Failed to remove city '{selectedCity.Name}' from tour '{selectedTour.Name}'.");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void ManageMemberTourRelationships()
        {
            DisplayError("This functionality is already handled in the Man-age Members section. A member is always created with a tour assignment, and removing a member removes them from their tour.");
        }

        private void ManageMemberMuseumVisitRelationships()
        {
            bool managing = true;

            while (managing)
            {
                Console.Clear();
                Console.WriteLine("===================================================");
                Console.WriteLine("      MANAGE MEMBER-MUSEUM VISIT RELATIONSHIPS     ");
                Console.WriteLine("===================================================");
                Console.WriteLine("1. Add a member to a museum visit");
                Console.WriteLine("2. Remove a member from a museum vis-it");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("===================================================");
                Console.Write("Enter your choice: ");

                var choice = GetMenuChoice(0, 2);

                switch (choice)
                {
                    case 0:
                        managing = false;
                        break;
                    case 1:
                        AddMemberToMuseumVisit();
                        break;
                    case 2:
                        RemoveMemberFromMuseumVisit();
                        break;
                }
            }
        }

        /// <summary>
        /// Adds a member to a museum visit
        /// </summary>
        private void AddMemberToMuseumVisit()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("           ADD A MEMBER TO A MUSEUM VISIT          ");
            Console.WriteLine("===================================================");

            var members = _service.GetAllMembers().ToList();

            if (members.Count == 0)
            {
                DisplayError("No members available. Please add a member first.");
                return;
            }

            Console.WriteLine("Available members:");
            for (int i = 0; i < members.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {members[i].Name} (Booking: {members[i].BookingNumber}, Tour: {members[i].Touring?.Name})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a member: ");

            var memberChoice = GetMenuChoice(0, members.Count);

            if (memberChoice == 0)
            {
                return;
            }

            var selectedMember = members[memberChoice - 1];

            if (selectedMember.Touring == null)
            {
                DisplayError($"Member '{selectedMember.Name}' is not as-signed to any tour.");
                return;
            }

            // Get museum visits for cities in the selected member's tour
            var tourCities = selectedMember.Touring.Cities;
            var museumVisits = _service.GetAllMuseumVisits()
                .Where(v => tourCities.Contains(v.City) && !v.RegisteredMembers.Contains(selectedMember))
                .ToList();

            if (museumVisits.Count == 0)
            {
                DisplayError($"No available museum visits for member '{selectedMember.Name}' to join. Either there are no museum visits in the cit-ies of their tour, or they're already registered for all available vis-its.");
                return;
            }

            Console.WriteLine($"Available museum visits for member '{selectedMember.Name}':");
            for (int i = 0; i < museumVisits.Count; i++)
            {
                var visit = museumVisits[i];
                Console.WriteLine($"{i + 1}. {visit.MuseumName} in {visit.City?.Name} on {visit.VisitDate.ToShortDateString()} (Cost: {visit.Cost:C})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a museum visit: ");

            var visitChoice = GetMenuChoice(0, museumVisits.Count);

            if (visitChoice == 0)
            {
                return;
            }

            var selectedVisit = museumVisits[visitChoice - 1];

            try
            {
                if (_service.AddMemberToMuseumVisit(selectedMember.Id, selectedVisit.Id))
                {
                    DisplaySuccess($"Member '{selectedMember.Name}' added to museum visit to {selectedVisit.MuseumName} in {selectedVisit.City?.Name} successfully.");

                    // Check if there's additional cost
                    decimal additionalCost = selectedMember.CalculateAdditionalCost();
                    if (additionalCost > 0)
                    {
                        DisplaySuccess($"NOTE: Member '{selectedMember.Name}' now has additional costs of {additionalCost:C} due to exceeding the included visits limit.");
                    }
                }
                else
                {
                    DisplayError($"Failed to add member '{selectedMember.Name}' to museum visit.");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

     
        private void RemoveMemberFromMuseumVisit()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("        REMOVE A MEMBER FROM A MUSEUM VISIT        ");
            Console.WriteLine("===================================================");

            var members = _service.GetAllMembers().ToList();

            if (members.Count == 0)
            {
                DisplayError("No members available.");
                return;
            }

            Console.WriteLine("Available members:");
            for (int i = 0; i < members.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {members[i].Name} (Booking: {members[i].BookingNumber}, Tour: {members[i].Touring?.Name})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a member: ");

            var memberChoice = GetMenuChoice(0, members.Count);

            if (memberChoice == 0)
            {
                return;
            }

            var selectedMember = members[memberChoice - 1];

            if (selectedMember.RegisteredMuseumVisits.Count == 0)
            {
                DisplayError($"Member '{selectedMember.Name}' is not regis-tered for any museum visits.");
                return;
            }

            Console.WriteLine($"Museum visits for member '{selectedMember.Name}':"); for (int i = 0; i < selectedMember.RegisteredMuseumVisits.Count; i++)
            {
                var visit = selectedMember.RegisteredMuseumVisits[i];
                Console.WriteLine($"{i + 1}. {visit.MuseumName} in {visit.City?.Name} on {visit.VisitDate.ToShortDateString()} (Cost: {visit.Cost:C})");
            }

            Console.WriteLine("0. Cancel");
            Console.Write("Select a museum visit to remove the member from: ");

            var visitChoice = GetMenuChoice(0, selectedMember.RegisteredMuseumVisits.Count);

            if (visitChoice == 0)
            {
                return;
            }

            var selectedVisit = selectedMember.RegisteredMuseumVisits[visitChoice - 1];

            try
            {
                if (_service.RemoveMemberFromMuseumVisit(selectedMember.Id, selectedVisit.Id))
                {
                    DisplaySuccess($"Member '{selectedMember.Name}' removed from museum visit to {selectedVisit.MuseumName} in {selectedVisit.City?.Name} successfully.");

                    // Check if there's still additional cost
                    decimal additionalCost = selectedMember.CalculateAdditionalCost();
                    if (additionalCost > 0)
                    {
                        DisplaySuccess($"NOTE: Member '{selectedMember.Name}' still has additional costs of {additionalCost:C} due to exceed-ing the included visits limit.");
                    }
                    else
                    {
                        DisplaySuccess($"NOTE: Member '{selectedMember.Name}' no longer has additional costs.");
                    }
                }
                else
                {
                    DisplayError($"Failed to remove member '{selectedMember.Name}' from museum visit.");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Views reports about the museum tour system
        /// </summary>
        private void ViewReports()
        {
            bool viewing = true;

            while (viewing)
            {
                Console.Clear();
                Console.WriteLine("===================================================");
                Console.WriteLine("                   VIEW REPORTS                    ");
                Console.WriteLine("===================================================");
                Console.WriteLine("1. Tour summary report");
                Console.WriteLine("2. Member costs report");
                Console.WriteLine("3. Museum visit popularity report");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("===================================================");
                Console.Write("Enter your choice: ");

                var choice = GetMenuChoice(0, 3);

                switch (choice)
                {
                    case 0:
                        viewing = false;
                        break;
                    case 1:
                        ViewTourSummaryReport();
                        break;
                    case 2:
                        ViewMemberCostsReport();
                        break;
                    case 3:
                        ViewMuseumVisitPopularityReport();
                        break;
                }
            }
        }


        private void ViewTourSummaryReport()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("                 TOUR SUMMARY REPORT               ");
            Console.WriteLine("===================================================");

            var tours = _service.GetAllTours().ToList();

            if (tours.Count == 0)
            {
                Console.WriteLine("No tours available.");
            }
            else
            {
                foreach (var tour in tours)
                {
                    Console.WriteLine($"Tour: {tour.Name} (ID: {tour.Id})");
                    Console.WriteLine($"Members: {tour.Members.Count}");
                    Console.WriteLine("Cities:");

                    if (tour.Cities.Count == 0)
                    {
                        Console.WriteLine("  None");
                    }
                    else
                    {
                        foreach (var city in tour.Cities)
                        {
                            Console.WriteLine($"  - {city.Name} ({city.MuseumVisits.Count} museum visits)");
                        }
                    }

                    Console.WriteLine("---------------------------------------------------");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Displays a report of additional costs for members
        /// </summary>
        private void ViewMemberCostsReport()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("               MEMBER COSTS REPORT                 ");
            Console.WriteLine("===================================================");

            var members = _service.GetAllMembers().ToList();

            if (members.Count == 0)
            {
                Console.WriteLine("No members available.");
            }
            else
            {
                foreach (var member in members)
                {
                    Console.WriteLine($"Member: {member.Name} (Booking: {member.BookingNumber})");
                    Console.WriteLine($"Tour: {member.Touring?.Name}");
                    Console.WriteLine($"Total Museum Visits: {member.RegisteredMuseumVisits.Count}");
                    Console.WriteLine($"Included Visits: {member.IncludedVisits}");

                    decimal additionalCost = member.CalculateAdditionalCost();
                    Console.WriteLine($"Additional Cost: {additionalCost:C}"); if (member.RegisteredMuseumVisits.Count > 0)
                    {
                        Console.WriteLine("Museum Visits:");
                        foreach (var visit in member.RegisteredMuseumVisits)
                        {
                            Console.WriteLine($"  - {visit.MuseumName} in {visit.City?.Name} on {visit.VisitDate.ToShortDateString()} (Cost: {visit.Cost:C})");
                        }
                    }

                    Console.WriteLine("---------------------------------------------------");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

  
        private void ViewMuseumVisitPopularityReport()
        {
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("           MUSEUM VISIT POPULARITY REPORT          ");
            Console.WriteLine("===================================================");

            var museumVisits = _service.GetAllMuseumVisits().ToList();

            if (museumVisits.Count == 0)
            {
                Console.WriteLine("No museum visits available.");
            }
            else
            {
                // Sort by popularity (number of registered members)
                museumVisits = museumVisits.OrderByDescending(v => v.RegisteredMembers.Count).ToList();

                foreach (var visit in museumVisits)
                {
                    Console.WriteLine($"Museum: {visit.MuseumName}");
                    Console.WriteLine($"City: {visit.City?.Name}");
                    Console.WriteLine($"Date: {visit.VisitDate.ToShortDateString()}");
                    Console.WriteLine($"Cost: {visit.Cost:C}");
                    Console.WriteLine($"Registered Members: {visit.RegisteredMembers.Count}");

                    if (visit.RegisteredMembers.Count > 0)
                    {
                        Console.WriteLine("Members:");
                        foreach (var member in visit.RegisteredMembers)
                        {
                            Console.WriteLine($"  - {member.Name} (Booking: {member.BookingNumber})");
                        }
                    }

                    Console.WriteLine("---------------------------------------------------");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
        #endregion

    }
}
