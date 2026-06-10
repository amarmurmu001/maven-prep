# .NET Core Interview Prep - Quick Crash Course

## 1. C# BASICS (Must Know)

### Data Types
- **Value types**: int, double, float, bool, char, decimal, struct, enum
- **Reference types**: string, class, interface, array, object, delegate

### OOP Concepts
- **Class**: Blueprint for objects
- **Object**: Instance of a class
- **Inheritance**: `class Dog : Animal { }`
- **Polymorphism**: Method overriding (`virtual`/`override`), method overloading
- **Abstraction**: `abstract class`, `interface`
- **Encapsulation**: Properties with `{ get; set; }`, access modifiers

### Access Modifiers
- `public` - anywhere
- `private` - same class only
- `protected` - class + derived classes
- `internal` - same assembly

### Async/Await
```csharp
public async Task<IActionResult> GetData()
{
    var data = await _context.Employees.ToListAsync();
    return Ok(data);
}
```

### Key Keywords
- `var` - implicit typing
- `async`/`await` - asynchronous operations
- `using` - import namespaces or disposable pattern
- `null` - null reference
- `is`/`as` - type checking/casting

---

## 2. .NET CORE ARCHITECTURE

### What is .NET Core?
- Cross-platform, open-source framework
- Modular (uses NuGet packages)
- High performance
- Supports microservices

### Request Pipeline (Middleware)
```
Request → app.UseExceptionHandler() → app.UseStaticFiles()
→ app.UseRouting() → app.UseAuthorization()
→ app.MapControllers() → Response
```

### Dependency Injection (DI)
- Built into .NET Core
- Registers services in `Program.cs`
- Types: AddSingleton, AddScoped, AddTransient
```csharp
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
```

---

## 3. ENTITY FRAMEWORK CORE (EF Core)

### What is EF Core?
- ORM (Object-Relational Mapper) for .NET
- Maps database tables to C# objects

### DbContext
- Main class for interacting with database
- Contains `DbSet<T>` properties for each table

### Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Common Operations
```csharp
// Read
_db.Employees.ToListAsync()
_db.Employees.FindAsync(id)
_db.Employees.FirstOrDefaultAsync(e => e.Name == "John")

// Create
_db.Employees.Add(new Employee { ... });
_db.SaveChangesAsync()

// Update
_db.Entry(emp).State = EntityState.Modified;
_db.SaveChangesAsync()

// Delete
_db.Employees.Remove(emp);
_db.SaveChangesAsync()
```

---

## 4. MVC PATTERN

### Model → View → Controller
- **Model**: Data + business logic (Employee.cs)
- **View**: UI (Razor .cshtml files)
- **Controller**: Handles requests, returns views/data

### Razor Syntax
```razor
@model IEnumerable<Employee>
@foreach (var emp in Model) { <tr>@emp.Name</tr> }
@Html.ActionLink("Create", "Create")
```

---

## 5. REST API CONCEPTS

| HTTP Method | CRUD      | Route              |
|-------------|-----------|--------------------|
| GET         | Read All  | GET /api/employees |
| GET         | Read One  | GET /api/employees/1 |
| POST        | Create    | POST /api/employees |
| PUT         | Update    | PUT /api/employees/1 |
| DELETE      | Delete    | DELETE /api/employees/1 |

### Status Codes
- 200 OK, 201 Created, 204 No Content
- 400 Bad Request, 404 Not Found
- 500 Internal Server Error

---

## 6. SQL BASICS

```sql
-- Create
INSERT INTO Employees (Name, Department, Email, Salary, JoinedDate)
VALUES ('John', 'IT', 'john@email.com', 50000, GETDATE())

-- Read
SELECT * FROM Employees
SELECT * FROM Employees WHERE Department = 'IT'
SELECT COUNT(*) FROM Employees

-- Update
UPDATE Employees SET Salary = 60000 WHERE Id = 1

-- Delete
DELETE FROM Employees WHERE Id = 1

-- Joins
SELECT e.Name, d.Name FROM Employees e
INNER JOIN Departments d ON e.DepartmentId = d.Id
```

---

## 7. YOUR PROJECT - BE READY TO EXPLAIN

**Project**: Employee Management System
- **Tech Stack**: ASP.NET Core MVC, EF Core, SQL Server, Bootstrap
- **Features**: Full CRUD (Create, Read, Update, Delete) for Employees
- **Architecture**: MVC pattern with Controller → DbContext → SQL Server
- **API**: RESTful endpoints at `/api/employees`

### Be ready to answer:
1. How does the request flow from browser to database and back?
   → Browser → Controller → DbContext → SQL Server → Response → View
2. What is Dependency Injection and how did you use it?
   → Injecting AppDbContext into controllers via constructor
3. Why did you choose EF Core?
   → Cross-platform, LINQ queries, migration support, performance
4. How do you handle errors?
   → try-catch, ModelState validation, status codes

---

## 8. COMMON INTERVIEW QUESTIONS

**Q**: What is the difference between `IEnumerable` and `IQueryable`?
**A**: IEnumerable executes in-memory (client-side), IQueryable builds SQL (server-side)

**Q**: What is LINQ?
**A**: Language Integrated Query - query collections/databases with C# syntax

**Q**: What is the difference between MVC and Web API?
**A**: MVC returns Views (HTML), Web API returns data (JSON/XML)

**Q**: What is middleware?
**A**: Components that handle requests/responses in the pipeline

**Q**: Explain `async`/`await`
**A**: Async methods don't block threads. Await releases thread while waiting

**Q**: What is the repository pattern?
**A**: Layer between controllers and data access for abstraction

**Q**: What is SOLID?
**A**: S-Single Responsibility, O-Open/Closed, L-Liskov Substitution, I-Interface Segregation, D-Dependency Inversion

---

## 9. KEY TERMS FOR INTERVIEW

- **DTO** (Data Transfer Object)
- **JWT** (JSON Web Token) - authentication
- **Swagger** - API documentation
- **NuGet** - package manager for .NET
- **IIS** / **Kestrel** - web servers
- **DTO** vs **Entity** - DTO for API, Entity for DB
- **Normalization** - organizing SQL tables
- **Index** - speeds up SQL queries

---

## 10. QUICK TIPS

1. If you don't know something, say "I haven't worked with that yet, but I understand the concept and can learn quickly"
2. Show enthusiasm for .NET Core
3. Mention Braze training - show willingness to learn new tech
4. Ask about the team, tech stack, growth opportunities
5. Practice saying the project out loud 3-4 times before the interview

---

**Good luck Amar! You've got this! 🚀**
