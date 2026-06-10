using AutoMapper;
using LeaveManagementSystem.Application.DTOs.Common;
using LeaveManagementSystem.Application.DTOs.Leave;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;
using LeaveManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace LeaveManagementSystem.Application.Services;

public sealed class LeaveService : ILeaveService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<LeaveService> _logger;

    public LeaveService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LeaveService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<LeaveRequestDto>> GetMyLeavesAsync(Guid employeeId, PaginationRequest pagination, CancellationToken cancellationToken = default)
    {
        var leaveRepo = _unitOfWork.Repository<LeaveRequest>();
        var query = leaveRepo.Query()
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.CreatedAt);

        var totalCount = await _unitOfWork.Repository<LeaveRequest>().CountAsync(l => l.EmployeeId == employeeId, cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        var leaves = query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<LeaveRequestDto>>(leaves);
        return new PagedResult<LeaveRequestDto>(dtos, totalCount, pagination.Page, pagination.PageSize);
    }

    public async Task<LeaveActionResponse> ApplyLeaveAsync(Guid employeeId, ApplyLeaveRequest request, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<LeaveType>(request.LeaveType, out var leaveType))
        {
            return new LeaveActionResponse(false, "Invalid leave type");
        }

        var overlapping = await _unitOfWork.Repository<LeaveRequest>().FindAsync(
            l => l.EmployeeId == employeeId &&
                 l.Status != LeaveStatus.Cancelled &&
                 l.Status != LeaveStatus.Rejected &&
                 request.StartDate <= l.EndDate &&
                 request.EndDate >= l.StartDate,
            cancellationToken);

        if (overlapping.Any())
        {
            return new LeaveActionResponse(false, "You already have a leave request for this period");
        }

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = employeeId,
            LeaveType = leaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason,
            Status = LeaveStatus.Pending
        };

        await _unitOfWork.Repository<LeaveRequest>().AddAsync(leaveRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Leave applied: {LeaveId} by Employee {EmployeeId}", leaveRequest.Id, employeeId);

        var dto = _mapper.Map<LeaveRequestDto>(leaveRequest);
        return new LeaveActionResponse(true, "Leave application submitted successfully", dto);
    }

    public async Task<LeaveActionResponse> CancelLeaveAsync(Guid employeeId, Guid leaveId, CancellationToken cancellationToken = default)
    {
        var leaveRepo = _unitOfWork.Repository<LeaveRequest>();
        var leave = await leaveRepo.GetByIdAsync(leaveId, cancellationToken);

        if (leave is null)
        {
            return new LeaveActionResponse(false, "Leave request not found");
        }

        if (leave.EmployeeId != employeeId)
        {
            return new LeaveActionResponse(false, "You can only cancel your own leave requests");
        }

        if (leave.Status != LeaveStatus.Pending)
        {
            return new LeaveActionResponse(false, $"Cannot cancel a leave with status {leave.Status}");
        }

        leave.Status = LeaveStatus.Cancelled;
        leave.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Leave cancelled: {LeaveId}", leaveId);

        var dto = _mapper.Map<LeaveRequestDto>(leave);
        return new LeaveActionResponse(true, "Leave request cancelled successfully", dto);
    }
}
