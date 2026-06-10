using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models;

public class Employee
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
}
