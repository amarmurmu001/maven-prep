// RUN WITH: dotnet run --project "C:\Users\iron5\Desktop\maven-prep\LearnCSharp"

// ============ 1. BASICS ============
int age = 25;
double salary = 50000.50;
string name = "Amar Murmu";
bool isEmployed = true;
var department = "IT"; // implicit typing

Console.WriteLine($"Name: {name}, Age: {age}, Salary: {salary}");

string collage = "Roorkee Institute of Technology";
int passingYear = 2026;

Console.WriteLine($"Collage: {collage}, Passing Year: {passingYear}");

// ============ 2. OOP - CLASS ============
var emp = new Employee { Id = 1, Name = "Test", Salary = 30000 };
Console.WriteLine(emp.GetDetails());

// ============ 3. COLLECTIONS & LINQ ============
var numbers = new List<int> { 1, 2, 3, 4, 5 };
numbers.Add(6);
Console.WriteLine($"Count: {numbers.Count}");

var evenNumbers = numbers.Where(n => n % 2 == 0);
Console.WriteLine($"Even: {string.Join(", ", evenNumbers)}");

// ============ 4. NULL CHECKING ============
string? maybeNull = null;
Console.WriteLine(maybeNull ?? "Default value"); // null-coalescing

// ============ 5. ASYNC/AWAIT ============
await DoWorkAsync();
Console.WriteLine("Done!");

// ============ 6. INTERFACE ============
IRepository repo = new EmployeeRepository();
Console.WriteLine(repo.GetById(1));

// ============ 7. EXCEPTION HANDLING ============
try
{
    int x = 10, y = 0;
    var result = x / y;
}
catch (DivideByZeroException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    Console.WriteLine("Cleanup code runs always");
}

// ============ METHODS AFTER TOP-LEVEL CODE ============
async Task DoWorkAsync()
{
    await Task.Delay(100);
    Console.WriteLine("Async work completed");
}

// ============ TYPES MUST BE AT BOTTOM ============
class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Salary { get; set; }
    public string GetDetails() => $"ID: {Id}, Name: {Name}, Salary: {Salary:C}";
}

interface IRepository
{
    string GetById(int id);
}

class EmployeeRepository : IRepository
{
    public string GetById(int id) => $"Employee {id} found";
}
