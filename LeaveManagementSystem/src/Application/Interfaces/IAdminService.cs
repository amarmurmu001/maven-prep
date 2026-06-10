using LeaveManagementSystem.Application.DTOs.Admin;
using LeaveManagementSystem.Application.DTOs.Common;
using LeaveManagementSystem.Application.DTOs.Leave;

namespace LeaveManagementSystem.Application.Interfaces;

public interface IAdminService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<LeaveRequestDto>> GetAllLeavesAsync(PaginationRequest pagination, CancellationToken cancellationToken = default);
    Task<LeaveActionResponse> ApproveLeaveAsync(Guid leaveId, Guid adminId, CancellationToken cancellationToken = default);
    Task<LeaveActionResponse> RejectLeaveAsync(Guid leaveId, Guid adminId, string? comments, CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeDto>> GetAllEmployeesAsync(PaginationRequest pagination, CancellationToken cancellationToken = default);
}
