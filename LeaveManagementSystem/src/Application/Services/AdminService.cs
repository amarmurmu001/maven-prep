using AutoMapper;
using LeaveManagementSystem.Application.DTOs.Admin;
using LeaveManagementSystem.Application.DTOs.Common;
using LeaveManagementSystem.Application.DTOs.Leave;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;
using LeaveManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace LeaveManagementSystem.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var leaveRepo = _unitOfWork.Repository<LeaveRequest>();

        var allUsers = await userRepo.GetAllAsync(cancellationToken);
        var allLeaves = await leaveRepo.GetAllAsync(cancellationToken);

        var totalEmployees = allUsers.Count(u => u.Role == UserRole.Employee);
        var activeEmployees = allUsers.Count(u => u.Role == UserRole.Employee && u.IsActive);
        var pendingRequests = allLeaves.Count(l => l.Status == LeaveStatus.Pending);
        var approvedRequests = allLeaves.Count(l => l.Status == LeaveStatus.Approved);
        var rejectedRequests = allLeaves.Count(l => l.Status == LeaveStatus.Rejected);
        var totalLeavesThisMonth = allLeaves.Count(l => l.CreatedAt.Month == DateTime.UtcNow.Month && l.CreatedAt.Year == DateTime.UtcNow.Year);

        var leaveTypeDistribution = allLeaves
            .GroupBy(l => l.LeaveType)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        var recentRequests = allLeaves
            .OrderByDescending(l => l.CreatedAt)
            .Take(10)
            .ToList();

        var recentDtos = _mapper.Map<List<RecentLeaveDto>>(recentRequests);

        return new DashboardDto(
            totalEmployees,
            activeEmployees,
            pendingRequests,
            approvedRequests,
            rejectedRequests,
            totalLeavesThisMonth,
            leaveTypeDistribution,
            recentDtos);
    }

    public async Task<PagedResult<LeaveRequestDto>> GetAllLeavesAsync(PaginationRequest pagination, CancellationToken cancellationToken = default)
    {
        var leaveRepo = _unitOfWork.Repository<LeaveRequest>();
        var query = leaveRepo.Query()
            .OrderByDescending(l => l.CreatedAt);

        var totalCount = await leaveRepo.CountAsync(null, cancellationToken);

        var leaves = query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<LeaveRequestDto>>(leaves);
        return new PagedResult<LeaveRequestDto>(dtos, totalCount, pagination.Page, pagination.PageSize);
    }

    public async Task<LeaveActionResponse> ApproveLeaveAsync(Guid leaveId, Guid adminId, CancellationToken cancellationToken = default)
    {
        var leaveRepo = _unitOfWork.Repository<LeaveRequest>();
        var leave = await leaveRepo.GetByIdAsync(leaveId, cancellationToken);

        if (leave is null)
        {
            return new LeaveActionResponse(false, "Leave request not found");
        }

        if (leave.Status != LeaveStatus.Pending)
        {
            return new LeaveActionResponse(false, $"Cannot approve a leave with status {leave.Status}");
        }

        leave.Status = LeaveStatus.Approved;
        leave.ApprovedById = adminId;
        leave.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Leave approved: {LeaveId} by Admin {AdminId}", leaveId, adminId);

        var dto = _mapper.Map<LeaveRequestDto>(leave);
        return new LeaveActionResponse(true, "Leave request approved successfully", dto);
    }

    public async Task<LeaveActionResponse> RejectLeaveAsync(Guid leaveId, Guid adminId, string? comments, CancellationToken cancellationToken = default)
    {
        var leaveRepo = _unitOfWork.Repository<LeaveRequest>();
        var leave = await leaveRepo.GetByIdAsync(leaveId, cancellationToken);

        if (leave is null)
        {
            return new LeaveActionResponse(false, "Leave request not found");
        }

        if (leave.Status != LeaveStatus.Pending)
        {
            return new LeaveActionResponse(false, $"Cannot reject a leave with status {leave.Status}");
        }

        leave.Status = LeaveStatus.Rejected;
        leave.ApprovedById = adminId;
        leave.Comments = comments;
        leave.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Leave rejected: {LeaveId} by Admin {AdminId}", leaveId, adminId);

        var dto = _mapper.Map<LeaveRequestDto>(leave);
        return new LeaveActionResponse(true, "Leave request rejected", dto);
    }

    public async Task<PagedResult<EmployeeDto>> GetAllEmployeesAsync(PaginationRequest pagination, CancellationToken cancellationToken = default)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var query = userRepo.Query()
            .Where(u => u.Role == UserRole.Employee)
            .OrderBy(u => u.FirstName);

        var totalCount = await userRepo.CountAsync(u => u.Role == UserRole.Employee, cancellationToken);

        var users = query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<EmployeeDto>>(users);

        var allLeaves = await _unitOfWork.Repository<LeaveRequest>().GetAllAsync(cancellationToken);

        foreach (var dto in dtos)
        {
            var userLeaves = allLeaves.Where(l => l.EmployeeId == dto.Id).ToList();
            dto.TotalLeaves = userLeaves.Count;
            dto.PendingLeaves = userLeaves.Count(l => l.Status == LeaveStatus.Pending);
            dto.ApprovedLeaves = userLeaves.Count(l => l.Status == LeaveStatus.Approved);
        }

        return new PagedResult<EmployeeDto>(dtos, totalCount, pagination.Page, pagination.PageSize);
    }
}
