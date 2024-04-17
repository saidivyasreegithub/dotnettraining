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
        static AssignmentDbEntities1 db = new AssignmentDbEntities1();
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome To Indian Railway Reservation System");
            Console.WriteLine("1. Admin -> Press 1");
            Console.WriteLine("2. User -> Press 2");
            Console.WriteLine("3. Exit -> Press 3");
            Console.Write("YOUR CHOICE: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Admin();
                    break;
                case "2":
                    UserMenu();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
        static void UserMenu()
        {
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
            Console.WriteLine("Enter your username:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();

            User_option();
        }

        static void RegisterNewUser()
        {
            Console.WriteLine("Enter your username:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();


            var newUser = new User { UserName = username, Password = password };
            db.Users.Add(newUser);
            db.SaveChanges();

            Console.WriteLine("User registered successfully!");
        }
       

        static void User_option()
        {
            bool exist = false;
            while (!exist)
            {
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine("1. View all trains");
                Console.WriteLine("2. Book a ticket");
                Console.WriteLine("3. Cancel a booking");
                Console.WriteLine("4. show booking tickets");
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
                        ShowBookedTickets();
                        break;
                    case "5":
                        exist = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static void ViewAllTrains()
        {
            var trains = db.trains.ToList();

            // Print header
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Train ID |       Name                |          Departure Station |          Arrival Station |       Class   |    Total Seats |    Available Seats |   Fare   |");
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            // Print train details
            foreach (var train in trains)
            {
                Console.WriteLine($"| {train.train_No,-9} |            {train.train_name,-16} |          {train.departure_station,-17} |       {train.arrival_station,-15} |    {train.Class,-9} | {train.total_seats,-11} |   {train.available_seats,-15} |  {train.Fare,-9} |");
            }

            // Print footer
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
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


                //var train2 = db.trains.FirstOrDefault(tc => tc.train_No == trainNo && tc.status == "Active");
                //float totalamount = totalSeats * train.Fare;

                var totalAmountParam = new SqlParameter("@totalamount", SqlDbType.Float);
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
                        Console.WriteLine($"Customer Name: {customerName}, Train No: {trainNo}, Class: {ticketClass}, Date of Travel: {dateOfTravel}, Total Amount: {totalAmountParam.Value}, Number of Seats: {totalSeats}");
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
            Console.WriteLine(" Train No  | Train Name       | customer name |date of travel    | Class  |Booking Date |");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------");

            foreach (var ticket in bookedTickets)
            {
                var train = db.trains.FirstOrDefault(sa => sa.train_No == ticket.train_No);

                Console.WriteLine($"|{ticket.train_No,-9} | {train.train_name,-15} | {ticket.Class,-6}  {ticket.data_of_booking,-21} |");
            }

            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------");
        }






        static void CancelBooking()
        {
            Console.WriteLine("---------------------------------------------------------------------------");
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
                    Console.WriteLine("Booking details:");
                    Console.WriteLine($"Customer Name: {booking.customer_name}");
                    Console.WriteLine($"Booking ID: {booking.booking_id}");
                    Console.WriteLine($"Number of Tickets: {booking.total_seats}");
                    Console.WriteLine($"Date of Travel: {booking.data_of_travelling}");
                    Console.WriteLine($"Class:{booking.Class}");
                    //double refundAmount = (double)0.0m;
                    //decimal refundamount= (decimal)booking.total_amount * 0.75m; // Assuming 75% refund policy
                    //Console.WriteLine($"Refund amount: {refundAmount}");
                    ////double refund = (double)booking.total_amount * 0.75;
                    //Console.WriteLine($"Refund Amount:{refundamount}");

                    // Add cancelled ticket to the database
                    var cancelledTicket = new cancellation
                    {
                        booking_id = bookingId, // Assuming canceledId is PNR
                        customer_name = booking.customer_name,
                        train_No = booking.train_No,
                        cancel_date = DateTime.Now,
                        //refund = (int?)refundAmount
                    };
                   
                    db.cancellations.Add(cancelledTicket);
                    // Retrieve the train entity including the available_seats property
                    
                    
                    db.SaveChanges();

                    // Remove booked ticket from the database
                    db.bookings.Remove(booking);


                    Console.WriteLine("Refund amount will be refunded soon!");
                    Console.WriteLine("Cancellation successful!");
                }   
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling booking: {ex.Message}");
            }
            Console.WriteLine("---------------------------------------------------------------------------");
            
        }
        
        
        static void Admin()
        {
            Console.WriteLine("---------------------------------------------------------------------------");
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
            Console.WriteLine("---------------------------------------------------------------------------");
            AdminMenu();

        }

        private static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("---------------------------------------------------------------------------");
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
                Console.WriteLine("---------------------------------------------------------------------------");
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
            db.SaveChanges();

            Console.WriteLine("Train modified successfully!");
            Console.WriteLine("---------------------------------------------------------------------------");
        }

        public static void DeleteTrain()
        {
            Console.WriteLine("---------------------------------------------------------------------------");
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
                Console.WriteLine("Do you want to deactive the train (y/n)");
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
            Console.WriteLine("---------------------------------------------------------------------------");
        }
    }
}
