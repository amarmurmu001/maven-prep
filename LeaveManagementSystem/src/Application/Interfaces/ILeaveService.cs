using LeaveManagementSystem.Application.DTOs.Common;
using LeaveManagementSystem.Application.DTOs.Leave;

namespace LeaveManagementSystem.Application.Interfaces;

public interface ILeaveService
{
    Task<PagedResult<LeaveRequestDto>> GetMyLeavesAsync(Guid employeeId, PaginationRequest pagination, CancellationToken cancellationToken = default);
    Task<LeaveActionResponse> ApplyLeaveAsync(Guid employeeId, ApplyLeaveRequest request, CancellationToken cancellationToken = default);
    Task<LeaveActionResponse> CancelLeaveAsync(Guid employeeId, Guid leaveId, CancellationToken cancellationToken = default);
}
