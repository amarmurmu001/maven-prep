namespace LeaveManagementSystem.Application.DTOs.Leave;

public sealed class LeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ReviewedByName { get; set; }
    public string? Comments { get; set; }
}

public sealed record ApplyLeaveRequest(
    string LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    string Reason);

public sealed record LeaveActionResponse(
    bool Success,
    string Message,
    LeaveRequestDto? LeaveRequest = null);
