namespace LeaveManagementSystem.Application.DTOs.Admin;

public sealed record DashboardDto(
    int TotalEmployees,
    int ActiveEmployees,
    int PendingRequests,
    int ApprovedRequests,
    int RejectedRequests,
    int TotalLeavesThisMonth,
    Dictionary<string, int> LeaveTypeDistribution,
    List<RecentLeaveDto> RecentRequests);

public sealed class RecentLeaveDto
{
    public Guid Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class EmployeeDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int TotalLeaves { get; set; }
    public int PendingLeaves { get; set; }
    public int ApprovedLeaves { get; set; }
    public DateTime CreatedAt { get; set; }
}
