using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeePayrollHRSystem
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public decimal MonthlySalary { get; set; }
        public double TaxRate { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; } = string.Empty;

        public decimal NetSalary => MonthlySalary - (MonthlySalary * (decimal)TaxRate);
    }

    class Program
    {
        // Starts with 0 pre-defined records as required
        static List<Employee> employees = new List<Employee>();
        static int nextId = 1001;

        const int TableWidth = 98;

        static void Main(string[] args)
        {
            // Modify this line on both branches later to simulate a merge conflict
            string appBanner = "=== EMPLOYEE PAYROLL & HR MANAGEMENT SYSTEM (MAIN) ===";

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine(appBanner);
                Console.WriteLine("1. Add Employee (Create)");
                Console.WriteLine("2. View All Employees (Read)");
                Console.WriteLine("3. Search / Filter Employees (LINQ)");
                Console.WriteLine("4. Update Employee Details (Update)");
                Console.WriteLine("5. Delete Employee (Delete)");
                Console.WriteLine("6. Exit");
                Console.Write("\nSelect an option (1-6): ");

                switch (Console.ReadLine())
                {
                    case "1":
                        CreateEmployee();
                        break;
                    case "2":
                        ReadAllEmployees();
                        break;
                    case "3":
                        SearchFilterEmployees();
                        break;
                    case "4":
                        UpdateEmployee();
                        break;
                    case "5":
                        DeleteEmployee();
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ---------- Input helpers ----------

        static string ReadRequiredString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(input))
                    return input;
                Console.WriteLine("This field cannot be empty. Try again.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal value) && value >= 0)
                    return value;
                Console.WriteLine("Invalid amount. Enter a number that is 0 or higher.");
            }
        }

        static double ReadTaxRate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double value) && value >= 0 && value <= 1)
                    return value;
                Console.WriteLine("Invalid tax rate. Enter a value between 0 and 1 (e.g., 0.12).");
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParse(Console.ReadLine(), out DateTime value))
                    return value;
                Console.WriteLine("Invalid date. Use the format YYYY-MM-DD.");
            }
        }

        static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (bool.TryParse(Console.ReadLine(), out bool value))
                    return value;
                Console.WriteLine("Invalid input. Type true or false.");
            }
        }

        // ---------- CRUD operations ----------

        static void CreateEmployee()
        {
            Console.Clear();
            Console.WriteLine("--- ADD NEW EMPLOYEE ---");

            Employee emp = new Employee();

            emp.FirstName = ReadRequiredString("First Name: ");
            emp.LastName = ReadRequiredString("Last Name: ");
            emp.Department = ReadRequiredString("Department (e.g., IT, HR, Sales, Finance): ");
            emp.JobTitle = ReadRequiredString("Job Title: ");
            emp.MonthlySalary = ReadDecimal("Monthly Salary: ");
            emp.TaxRate = ReadTaxRate("Tax Rate (e.g., 0.12 for 12%): ");
            emp.HireDate = ReadDate("Hire Date (YYYY-MM-DD): ");
            emp.IsActive = ReadBool("Is Active? (true/false): ");
            emp.Email = ReadRequiredString("Email Address: ");

            // ID is assigned only after every field has been entered successfully
            emp.Id = nextId++;
            employees.Add(emp);

            Console.WriteLine($"\n[SUCCESS] Employee {emp.FirstName} {emp.LastName} added with ID: {emp.Id}!");
            Pause();
        }

        static void ReadAllEmployees()
        {
            Console.Clear();
            Console.WriteLine("--- ALL EMPLOYEE RECORDS ---");

            var allRecords = employees.OrderBy(e => e.Id).ToList();

            if (!allRecords.Any())
            {
                Console.WriteLine("No employee records found.");
            }
            else
            {
                DisplayTable(allRecords);
            }
            Pause();
        }

        static void SearchFilterEmployees()
        {
            Console.Clear();
            Console.WriteLine("--- SEARCH / FILTER EMPLOYEES (LINQ) ---");
            Console.WriteLine("1. Search by Employee ID");
            Console.WriteLine("2. Search by Last Name");
            Console.WriteLine("3. Filter by Department");
            Console.WriteLine("4. Filter by Active Status");
            Console.Write("Choose filter option: ");

            string choice = Console.ReadLine();
            IEnumerable<Employee> results = Enumerable.Empty<Employee>();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter Employee ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        results = employees.Where(e => e.Id == id);
                    }
                    break;
                case "2":
                    Console.Write("Enter Last Name (or partial): ");
                    string lastName = (Console.ReadLine() ?? string.Empty).ToLower();
                    results = employees.Where(e => e.LastName.ToLower().Contains(lastName));
                    break;
                case "3":
                    Console.Write("Enter Department: ");
                    string dept = (Console.ReadLine() ?? string.Empty).Trim().ToLower();
                    results = employees.Where(e => e.Department.ToLower() == dept);
                    break;
                case "4":
                    Console.Write("Show Active (true) or Inactive (false): ");
                    if (bool.TryParse(Console.ReadLine(), out bool status))
                    {
                        results = employees.Where(e => e.IsActive == status);
                    }
                    break;
                default:
                    Console.WriteLine("Invalid search option.");
                    Pause();
                    return;
            }

            var resultList = results.ToList();
            Console.WriteLine($"\nFound {resultList.Count} matching record(s):");
            if (resultList.Any())
            {
                DisplayTable(resultList);
            }
            Pause();
        }

        static void UpdateEmployee()
        {
            Console.Clear();
            Console.WriteLine("--- UPDATE EMPLOYEE DETAILS ---");
            Console.Write("Enter Employee ID to update: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                Pause();
                return;
            }

            Employee emp = employees.FirstOrDefault(e => e.Id == id);

            if (emp == null)
            {
                Console.WriteLine($"Employee with ID {id} not found.");
                Pause();
                return;
            }

            Console.WriteLine($"\nUpdating: {emp.FirstName} {emp.LastName} (Leave blank to keep current value)");

            Console.Write($"First Name [{emp.FirstName}]: ");
            string fName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(fName)) emp.FirstName = fName.Trim();

            Console.Write($"Last Name [{emp.LastName}]: ");
            string lName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(lName)) emp.LastName = lName.Trim();

            Console.Write($"Department [{emp.Department}]: ");
            string dept = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dept)) emp.Department = dept.Trim();

            Console.Write($"Job Title [{emp.JobTitle}]: ");
            string title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title)) emp.JobTitle = title.Trim();

            Console.Write($"Monthly Salary [{emp.MonthlySalary}]: ");
            string sal = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(sal))
            {
                if (decimal.TryParse(sal, out decimal newSal) && newSal >= 0)
                    emp.MonthlySalary = newSal;
                else
                    Console.WriteLine("Invalid salary. Keeping current value.");
            }

            Console.Write($"Tax Rate [{emp.TaxRate}]: ");
            string tax = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(tax))
            {
                if (double.TryParse(tax, out double newTax) && newTax >= 0 && newTax <= 1)
                    emp.TaxRate = newTax;
                else
                    Console.WriteLine("Invalid tax rate (must be 0 to 1). Keeping current value.");
            }

            Console.Write($"Hire Date [{emp.HireDate:yyyy-MM-dd}]: ");
            string hDate = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(hDate))
            {
                if (DateTime.TryParse(hDate, out DateTime newDate))
                    emp.HireDate = newDate;
                else
                    Console.WriteLine("Invalid date. Keeping current value.");
            }

            Console.Write($"Is Active [{emp.IsActive}] (true/false): ");
            string active = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(active))
            {
                if (bool.TryParse(active, out bool newActive))
                    emp.IsActive = newActive;
                else
                    Console.WriteLine("Invalid value. Keeping current value.");
            }

            Console.Write($"Email [{emp.Email}]: ");
            string email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email)) emp.Email = email.Trim();

            Console.WriteLine("\n[SUCCESS] Employee record updated!");
            Pause();
        }

        static void DeleteEmployee()
        {
            Console.Clear();
            Console.WriteLine("--- DELETE EMPLOYEE RECORD ---");
            Console.Write("Enter Employee ID to delete: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                Pause();
                return;
            }

            Employee emp = employees.FirstOrDefault(e => e.Id == id);

            if (emp == null)
            {
                Console.WriteLine($"Employee with ID {id} not found.");
            }
            else
            {
                Console.Write($"Are you sure you want to delete {emp.FirstName} {emp.LastName}? (y/n): ");
                if ((Console.ReadLine() ?? string.Empty).ToLower() == "y")
                {
                    employees.Remove(emp);
                    Console.WriteLine("[SUCCESS] Employee deleted.");
                }
                else
                {
                    Console.WriteLine("Deletion cancelled.");
                }
            }
            Pause();
        }

        static void DisplayTable(List<Employee> list)
        {
            Console.WriteLine(new string('-', TableWidth));
            Console.WriteLine(
                $"{"ID",-4}|{"Name",-14}|{"Dept",-6}|{"Title",-12}|" +
                $"{"Gross",9}|{"Tax",4}|{"Net",9}|" +
                $"{"Hired",-10}|{"Stat",-6}|{"Email",-15}"
            );
            Console.WriteLine(new string('-', TableWidth));

            foreach (var e in list)
            {
                string name = $"{e.FirstName} {e.LastName}";
                if (name.Length > 14) name = name.Substring(0, 14);

                string title = e.JobTitle.Length > 12 ? e.JobTitle.Substring(0, 12) : e.JobTitle;
                string dept = e.Department.Length > 6 ? e.Department.Substring(0, 6) : e.Department;
                string status = e.IsActive ? "Active" : "Inactv";

                Console.WriteLine(
                    $"{e.Id,-4}|{name,-14}|{dept,-6}|{title,-12}|" +
                    $"{e.MonthlySalary,9:N0}|{e.TaxRate,4:P0}|{e.NetSalary,9:N0}|" +
                    $"{e.HireDate:yyyy-MM-dd}|{status,-6}|{e.Email}"
                );
            }
            Console.WriteLine(new string('-', TableWidth));
        }

        static void Pause()
        {
            Console.WriteLine("\nPress any key to return to main menu...");
            Console.ReadKey();
        }
    }
}