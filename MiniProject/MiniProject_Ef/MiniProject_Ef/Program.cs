using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MiniProject_Ef
{
    class Program
    {
        static AssignmentDbEntities db = new AssignmentDbEntities();
        static void Main(string[] args)
        {

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\t\t\t-------------------------Welcome to Indian Railways------------------------");
                Console.WriteLine("1. View all trains");
                Console.WriteLine("2. Book a ticket");
                Console.WriteLine("3. Cancel a booking");
                Console.WriteLine("4. Admin functions");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewAllTrains();
                        break;
                    case "2":
                        BookTicket();
                        break;
                    case "3":
                        CancelBooking();
                        break;
                    case "4":
                        AdminFunctions();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.WriteLine("\t\t\t-----------------------------Thank you for visiting our website------------------------------");
                Console.Read();
            }
        }

        static void ViewAllTrains()
        {
            var trains = db.trains.ToList();
            foreach (var train in trains)
            {
                Console.WriteLine($"Train ID: {train.train_No}, Name: {train.train_name}, Departure: {train.departure_time}, Arrival: {train.arrival_time},");
            }
        }
        static void BookTicket()
        {
            try
            {
                Console.WriteLine("Available Trains:");
                ViewAllTrains();
                Console.Write("Enter your name: ");
                string customerName = Console.ReadLine();

                Console.Write("Enter the Train No of the train you want to book: ");
                int trainNo = int.Parse(Console.ReadLine());

                Console.Write("Enter the class : ");
                string ticketClass = Console.ReadLine();

                Console.Write("Enter the date of travel : ");
                DateTime dateOfTravel;
                if (!DateTime.TryParse(Console.ReadLine(), out dateOfTravel))
                {
                    Console.WriteLine("Invalid date format. Please enter date in yyyy-MM-dd format.");
                    return;
                }

                Console.Write("Enter the total number of seats: ");
                int totalSeats;
                if (!int.TryParse(Console.ReadLine(), out totalSeats) || totalSeats <= 0)
                {
                    Console.WriteLine("Invalid number of seats. Please enter a valid positive integer.");
                    return;
                }

                var totalAmountParam = new SqlParameter("@Totalamount", SqlDbType.Float);
                totalAmountParam.Direction = ParameterDirection.Output;

                var customerNameParam = new SqlParameter("@CustomerName", customerName);
                var trainNoParam = new SqlParameter("@TrainNo", trainNo);
                var classParam = new SqlParameter("@Class", ticketClass);
                var dateOfTravelParam = new SqlParameter("@DateOfTravel", dateOfTravel);
                var totalSeatsParam = new SqlParameter("@TotalSeats", totalSeats);

                int rowsAffected = db.Database.ExecuteSqlCommand(
                    "EXEC InsertBookingAndUpdateTrainWithDates @CustomerName, @TrainNo, @Class, @DateOfTravel, @TotalSeats",
                    customerNameParam, trainNoParam, classParam, dateOfTravelParam, totalSeatsParam);

                if (rowsAffected > 0)
                {
                    var newBookingId = db.bookings
                        .Where(b => b.customer_name == customerName && b.train_No == trainNo && b.Class == ticketClass && b.data_of_travelling == dateOfTravel)
                        .Select(b => b.booking_id)
                        .FirstOrDefault();

                    if (newBookingId != 0)
                    {
                        Console.WriteLine($"Ticket booked successfully! Your booking ID is: {newBookingId}");
                    }
                }
                else
                {
                    Console.WriteLine("Booking failed. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void CancelBooking()
        {
            try
            {
                Console.Write("Enter booking ID to cancel: ");
                int bookingId = int.Parse(Console.ReadLine());

                var booking = db.bookings.FirstOrDefault(b => b.booking_id == bookingId);
                if (booking == null)
                {
                    Console.WriteLine("Booking not found.");
                    return;
                }

                // Implement cancellation logic here

                Console.WriteLine("Cancellation successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling booking: {ex.Message}");
            }
        }
        static void AdminFunctions()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            var admin = db.admin_details.FirstOrDefault(a => a.adminName == username && a.password == password);

            if (admin == null)
            {
                Console.WriteLine("Invalid username or password.");

            }
            else
            {

                Console.WriteLine("Admin login successful.");
            }
            AdminMenu();
        }

        private static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Add Train");
                Console.WriteLine("2. Modify Train");
                Console.WriteLine("3. Delete Train");
                Console.WriteLine("4. Exit");

                Console.Write("Enter your choice: ");
                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        AddTrain();
                        break;
                    case 2:
                        ModifyTrain();
                        break;
                    case 3:
                        DeleteTrain();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please select a valid option.");
                        break;
                }
            }
        }

        private static void AddTrain()
        {

            Console.WriteLine("Enter train details:");

            Console.Write("Train No: ");
            int trainNo = Convert.ToInt32(Console.ReadLine());

            Console.Write("Train Name: ");
            string trainName = Console.ReadLine();

            Console.Write("Departure Station: ");
            string departureStation = Console.ReadLine();

            Console.Write("Arrival Station: ");
            string arrivalStation = Console.ReadLine();

            Console.Write("Departure Time: ");
            TimeSpan departureTime;
            if (!TimeSpan.TryParse(Console.ReadLine(), out departureTime))
            {
                Console.WriteLine("Invalid departure time format.");

            }

            Console.Write("Arrival Time: ");
            TimeSpan arrivalTime;
            if (!TimeSpan.TryParse(Console.ReadLine(), out arrivalTime))
            {
                Console.WriteLine("Invalid arrival time format.");

            }

            // Assuming the train is active by default
            string status = "active";

            // Create a new train object
            var train = new train
            {
                train_No = trainNo,
                train_name = trainName,
                departure_station = departureStation,
                arrival_station = arrivalStation,
                departure_time = departureTime,
                arrival_time = arrivalTime,
                status = status
            };

            // Add the train to the trains table
            db.trains.Add(train);

            try
            {
                // Save changes to the database
                db.SaveChanges();
                Console.WriteLine("Train added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding train: {ex.Message}");
            }
        }
        private static void ModifyTrain()
        {
            Console.Write("Enter the Train No of the train you want to modify: ");
            int trainNo = Convert.ToInt32(Console.ReadLine());

            // Find the train with the provided train number
            var train = db.trains.FirstOrDefault(t => t.train_No == trainNo);
            if (train == null)
            {
                Console.WriteLine("Train not found.");

            }

            // Display current details of the train
            Console.WriteLine("Current details of the train:");
            Console.WriteLine($"Train No: {train.train_No}");
            Console.WriteLine($"Train Name: {train.train_name}");
            Console.WriteLine($"Source: {train.arrival_station}");
            Console.WriteLine($"Destination: {train.departure_station}");
            Console.WriteLine($"Departure Time: {train.departure_time}");
            Console.WriteLine($"Arrival Time: {train.arrival_time}");
            Console.WriteLine($"status: {(train.status == "yes" ? "active" : "inactive")}");
            // Prompt user to enter new details
            Console.WriteLine("Enter new details:");

            Console.Write("Train Name: ");
            string newTrainName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newTrainName))
            {
                train.train_name = newTrainName;
            }

            Console.Write("Source: ");
            string newSource = Console.ReadLine();
            if (!string.IsNullOrEmpty(newSource))
            {
                train.arrival_station = newSource;
            }

            Console.Write("Destination: ");
            string newDestination = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDestination))
            {
                train.departure_station = newDestination;
            }
            db.SaveChanges();

            Console.WriteLine("Train modified successfully!");
        }

        public static void DeleteTrain()
        {
            Console.Write("Enter Train No of the train you want to deactivate: ");
            int trainNo = Convert.ToInt32(Console.ReadLine());

            // Find the train in the database
            var train = db.trains.FirstOrDefault(t => t.train_No == trainNo);
            if (train == null)
            {
                Console.WriteLine("Train not found.");

            }
            else
            {
                train.status = "deactive";
            }

            try
            {
                // Save changes to the database
                db.SaveChanges();
                Console.WriteLine("Train deactivated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating train: {ex.Message}");
            }
        }
    }
}
