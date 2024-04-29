﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Mini_Project
{
    class Program
    {
        static sampledbEntities db = new sampledbEntities();
        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("*                                                                  *");
            Console.WriteLine("|            Welcome To Indian Railway Reservation System          |");
            Console.WriteLine("*                                                                  *");
            Console.WriteLine("--------------------------------------------------------------------");
            Console.WriteLine("1. Admin -> Press 1");
            Console.WriteLine("2. User -> Press 2");
            Console.WriteLine("3. Exit -> Press 3");
            Console.Write("YOUR CHOICE: ");

            string choice = Console.ReadLine();


            switch (choice)
            {
                case "1":
                    Console.Clear();
                    Admin();

                    break;
                case "2":
                    Console.Clear();
                    UserMenu();

                    break;
                case "3":
                    Console.Clear();
                    Environment.Exit(0);

                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
            Console.Clear();
            Console.WriteLine("Thank you visit again!...");
        }
        static void UserMenu()
        {
            Console.WriteLine("-------------------------------------------------------------------");


            Console.WriteLine("|                         User  Menu Details                       |");
            Console.WriteLine("-------------------------------------------------------------------");

            Console.WriteLine("Enter Your User Details:");
            Console.WriteLine("1. Existing User -> Press 1");
            Console.WriteLine("2. New User -> Press 2");
            Console.Write("YOUR CHOICE: ");
            string userChoice = Console.ReadLine();

            switch (userChoice)
            {
                case "1":
                   
                    ExistingUserLogin();

                    break;
                case "2":
                   
                    RegisterNewUser();

                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }

        }

        static void ExistingUserLogin()
        {
            Console.WriteLine("-------------------------------------------------------------------");


            Console.WriteLine("|                        Existing User Details                    |");
            Console.WriteLine("-------------------------------------------------------------------");

            Console.WriteLine("Enter your username:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.Clear();
            User_option();
            db.SaveChanges();


        }

        static void RegisterNewUser()
        {
            Console.WriteLine("Enter your username:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();


            var newUser = new User { UserName = username, Password = password };
            db.Users.Add(newUser);
            Console.Clear();
            User_option();
            db.SaveChanges();

            Console.WriteLine("User registered successfully!");
        }


        static void User_option()
        {
            Console.WriteLine("-------------------------------------------------------------------");


            Console.WriteLine("|                            User Options                         |");
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("Welcome User");
            Console.WriteLine("Here ur options");
            bool exist = false;
            while (!exist)
            {

                Console.WriteLine("1. View all trains");
                Console.WriteLine("2. Book a ticket");
                Console.WriteLine("3. Cancel a booking");
                Console.WriteLine("4. show booking tickets");
                Console.WriteLine("5. show cancel tickets");
                Console.WriteLine("6. Exit");
                Console.Write("Choose an option: ");
                Console.WriteLine("-------------------------------------------------------------");
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
                        ShowBookedTickets();
                        break;
                    case "5":
                        ShowCancelTickets();
                        break;
                    case "6":
                        exist = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            Console.WriteLine("---------------------------------------------------------------------------");
        }
        static void ViewAllTrains()
        {
            var trains = db.trains.ToList();


            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Train ID |      Name            | Departure Station | Arrival Station | Class | Total Seats | Available Seats | Fare | Status");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------");


            foreach (var train in trains)
            {
                Console.WriteLine($"| {train.train_No,-9} | {train.train_name,-20} | {train.departure_station,-17} | {train.arrival_station,-15} | {train.Class,-5} | {train.total_seats,-11} | {train.available_seats,-15} | {train.Fare,-5} | {train.status,-10}");
            }


            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
        }




        static void BookTicket()
        {

            try
            {
                Console.WriteLine("Available Trains:");
                ViewAllTrains();
                Console.Write("Enter the Train No of the train you want to book: ");
                int trainNo = int.Parse(Console.ReadLine());

                // Check if the train is active
                var train = db.trains.FirstOrDefault(t => t.train_No == trainNo && t.status == "active");
                if (train == null)
                {
                    Console.WriteLine("Sorry, the train is deactivated. Booking cannot be processed.");
                    return;
                }
                Console.Write("Enter your name: ");
                string customerName = Console.ReadLine();
                Console.Write("Enter the class : ");
                string ticketClass = Console.ReadLine();

                Console.Write("Enter the date of travel : ");
                DateTime dateOfTravel;
                if (!DateTime.TryParse(Console.ReadLine(), out dateOfTravel))
                {
                    Console.WriteLine("Invalid date format. Please enter date in yyyy-MM-dd format.");
                    return;
                }
                var todayDate = DateTime.Today;
                if (dateOfTravel <= todayDate)
                {
                    Console.WriteLine("Error: Date of travel must be greater than today's date.");
                    return;
                }
                Console.Write("Enter the total number of seats: ");
                int totalSeats;
                if (!int.TryParse(Console.ReadLine(), out totalSeats) || totalSeats <= 0)
                {
                    Console.WriteLine("Invalid number of seats. Please enter a valid positive integer.");
                    return;
                }
                var TotalamountParam = new SqlParameter("@Totalamount", SqlDbType.Float);
                TotalamountParam.Direction = ParameterDirection.Output;

                var customerNameParam = new SqlParameter("@CustomerName", customerName);
                var trainNoParam = new SqlParameter("@TrainNo", trainNo);
                var classParam = new SqlParameter("@Class", ticketClass);
                var dateOfTravelParam = new SqlParameter("@DateOfTravel", dateOfTravel);
                var totalSeatsParam = new SqlParameter("@TotalSeats", totalSeats);

                int rowsAffected = db.Database.ExecuteSqlCommand(
                    "EXEC InsertBookingAndUpdateTrainWithDates @CustomerName, @TrainNo, @Class, @DateOfTravel, @TotalSeats,@Totalamount",
                    customerNameParam, trainNoParam, classParam, dateOfTravelParam, totalSeatsParam, TotalamountParam);
                if (rowsAffected > 0)
                {
                    // Decrease available seats in the trains table
                    var bookedTrain = db.trains.FirstOrDefault(t => t.train_No == trainNo);
                    if (bookedTrain != null)
                    {
                        if (ticketClass == "1A")
                            bookedTrain.available_seats -= totalSeats;
                        else if (ticketClass == "2A")
                            bookedTrain.available_seats -= totalSeats;
                        else if (ticketClass == "SL")
                            bookedTrain.available_seats -= totalSeats;
                        else if (ticketClass == "3A")
                            bookedTrain.available_seats -= totalSeats;

                        db.SaveChanges(); 
                    }
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            db.SaveChanges();
            Console.WriteLine("---------------------------------------------------------------------------");
        }
        static void ShowBookedTickets()
        {
            Console.WriteLine("enter your booking id");
            int bookingId = Convert.ToInt32(Console.ReadLine());
            var bookedTickets = db.bookings.Where(bt => bt.booking_id == bookingId).ToList();


            if (bookedTickets.Count == 0)
            {
                Console.WriteLine("No booked tickets found for the specified user.");
                return;
            }

            Console.WriteLine("Booked Tickets:");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine(" Train No        | Train Name           | customer name               |date of travel       | Class         |Booking Date | Price");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------");

            foreach (var ticket in bookedTickets)
            {
                var train = db.trains.FirstOrDefault(sa => sa.train_No == ticket.train_No);

                Console.WriteLine($"|{ticket.train_No,-9} |          {train.train_name,-15} |        {ticket.customer_name}|  {ticket.data_of_travelling} |     {ticket.Class,-6} |    {ticket.data_of_booking,-21}          |{ticket.total_amount} |");
            }
            ViewAllTrains();

        }

        static void CancelBooking()
        {

            try
            {
                Console.WriteLine("enter your booking id");
                int bookingId = Convert.ToInt32(Console.ReadLine());

                var booking = db.bookings.FirstOrDefault(b => b.booking_id == bookingId);

                if (booking == null)
                {

                    Console.WriteLine("No booked ticket for cancel, Please book the tickets");

                    return;
                }
                else
                {

                    decimal refundamount = (decimal)booking.total_amount * 0.75m;
                    var cancelledTicket = new cancellation
                    {
                        booking_id = bookingId,
                        customer_name = booking.customer_name,
                        train_No = booking.train_No,
                        Class = booking.Class,

                        cancel_date = DateTime.Now,
                        no_of_seats = booking.total_seats,
                        refund = (int?)refundamount

                    };
                    db.cancellations.Add(cancelledTicket);
                    db.SaveChanges();



                    //db.bookings.Remove(booking);
                    //db.SaveChanges();
                    Console.WriteLine("Cancallation is done successfully.....");

                    var cancelTrain = db.trains.FirstOrDefault(t => t.train_No == booking.train_No);
                    if (cancelTrain != null)
                    {
                        if (booking.Class == "1A")
                            cancelTrain.available_seats += (int)booking.total_seats;
                        else if (booking.Class == "2A")
                            cancelTrain.available_seats += (int)booking.total_seats;
                        else if (booking.Class == "SL")
                            cancelTrain.available_seats += (int)booking.total_seats;
                        else if (booking.Class == "3A")
                            cancelTrain.available_seats += (int)booking.total_seats;

                        db.SaveChanges();
                    }




                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling booking: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
        static void ShowCancelTickets()
        {
            Console.WriteLine("Enter your booking id:");
            int bookingId = Convert.ToInt32(Console.ReadLine());
            var cancelledTickets = db.cancellations.Where(t => t.booking_id == bookingId).ToList();

            Console.WriteLine($"Found {cancelledTickets.Count} cancelled tickets for booking ID: {bookingId}");

            if (cancelledTickets.Count == 0)
            {
                Console.WriteLine("No cancelled tickets found for the specified user.");
                return;
            }

            Console.WriteLine("Cancelled Tickets:");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("|Booking ID  |  Train No  |   Class |  Total Seats  |Cancellation Date       | Refund  |");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------");

            foreach (var ticket in cancelledTickets)
            {
                Console.WriteLine($"| {ticket.booking_id,-13} | {ticket.train_No,-9} | {ticket.Class,-9} | {ticket.no_of_seats} |    {ticket.cancel_date,-23} | {ticket.refund,-13} |");
            }

            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------");
        }
        static void Admin()
        {
            Console.WriteLine("-------------------------------------------------------------------");


            Console.WriteLine("|                            Admin Options                         |");
            Console.WriteLine("-------------------------------------------------------------------");
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


            Console.WriteLine("1. Add Trains");
            Console.WriteLine("2. Modify Existing Trains");
            Console.WriteLine("3.  deactivate Train");
            Console.WriteLine("4.  activate Train");
            Console.WriteLine("5. Log out");
            Console.WriteLine("--------------------------------------------------------------");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();


            switch (choice)
            {
                case "1":
                    AddTrain();
                    break;
                case "2":
                    ModifyTrain();
                    break;
                case "3":
                    DeleteTrain();
                    break;
                case "4":
                    ReactivateTrain();
                    break;
                case "5":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        private static void AddTrain()
        {

            Console.WriteLine("---------------------------------------------------------------------------");
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

            
            string status = "active";

            
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
            ViewAllTrains();
            Console.WriteLine("---------------------------------------------------------------------------");
        }
        private static void ModifyTrain()
        {
            Console.WriteLine("---------------------------------------------------------------------------");
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
            if (train.train_No == trainNo)
            {
                train.status = "deactive";
            }
            db.SaveChanges();

            Console.WriteLine("Train modified successfully!");
            ViewAllTrains();

        }
        public static void DeleteTrain()
        {
            try
            {
                Console.Write("Enter Train No of the train you want to deactivate: ");
                int trainNo = Convert.ToInt32(Console.ReadLine());
             
                var train = db.trains.FirstOrDefault(t => t.train_No == trainNo && t.status == "active");
                if (train == null)
                {
                    Console.WriteLine("Train not found or already deactivated.");
                    return;
                }

               
                train.status = "deactive";

                
                var classes = db.trains.Where(t => t.train_No == trainNo).ToList();

               
                foreach (var trainClass in classes)
                {
                    trainClass.status = "deactive";
                }


                // Save changes to the database
                db.SaveChanges();
                Console.WriteLine("Train and all its classes deactivated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating train: {ex.Message}");
            }
            ViewAllTrains();
        }

        private static void ReactivateTrain()
        {
            try
            {
                Console.Write("Enter Train No of the train you want to deactivate: ");
                int trainNo = Convert.ToInt32(Console.ReadLine());
                // Find the active train with the provided train number
                var train = db.trains.FirstOrDefault(t => t.train_No == trainNo && t.status == "deactive");
                if (train == null)
                {
                    Console.WriteLine("Train not found or already activated.");
                    return;
                }

             
                train.status = "active";

                
                var classes = db.trains.Where(t => t.train_No == trainNo).ToList();

               
                foreach (var trainClass in classes)
                {
                    trainClass.status = "active";
                }


             
                db.SaveChanges();
                Console.WriteLine("Train and all its classes activated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating train: {ex.Message}");
            }
            ViewAllTrains();
        }

    }

}

