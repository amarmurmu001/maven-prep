using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Data.Seeds;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var admin = new User
        {
            Id = Guid.Parse("A1111111-1111-1111-1111-111111111111"),
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@leavemanagement.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var employee1 = new User
        {
            Id = Guid.Parse("B2222222-2222-2222-2222-222222222222"),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@leavemanagement.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"),
            Role = UserRole.Employee,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var employee2 = new User
        {
            Id = Guid.Parse("C3333333-3333-3333-3333-333333333333"),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@leavemanagement.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@123"),
            Role = UserRole.Employee,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(admin, employee1, employee2);

        context.LeaveRequests.AddRange(
            new LeaveRequest
            {
                EmployeeId = employee1.Id,
                LeaveType = LeaveType.AnnualLeave,
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(15),
                Reason = "Family vacation",
                Status = LeaveStatus.Pending,
                CreatedAt = DateTime.UtcNow
            },
            new LeaveRequest
            {
                EmployeeId = employee2.Id,
                LeaveType = LeaveType.SickLeave,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                Reason = "Medical appointment",
                Status = LeaveStatus.Pending,
                CreatedAt = DateTime.UtcNow
            },
            new LeaveRequest
            {
                EmployeeId = employee1.Id,
                LeaveType = LeaveType.WorkFromHome,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(-5),
                Reason = "Home maintenance",
                Status = LeaveStatus.Approved,
                ApprovedById = admin.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        );

        await context.SaveChangesAsync();
    }
}
