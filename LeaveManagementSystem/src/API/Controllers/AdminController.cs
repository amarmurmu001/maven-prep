using LeaveManagementSystem.API.Extensions;
using LeaveManagementSystem.Application.DTOs.Common;
using LeaveManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public sealed class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _adminService.GetDashboardAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("leaves")]
    public async Task<IActionResult> GetAllLeaves(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationRequest(page, pageSize);
        var result = await _adminService.GetAllLeavesAsync(pagination, cancellationToken);
        return Ok(result);
    }

    [HttpPut("approve/{id:guid}")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var adminId = User.GetUserId();
        var result = await _adminService.ApproveLeaveAsync(id, adminId, cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("reject/{id:guid}")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectRequest request,
        CancellationToken cancellationToken)
    {
        var adminId = User.GetUserId();
        var result = await _adminService.RejectLeaveAsync(id, adminId, request.Comments, cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("employees")]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationRequest(page, pageSize);
        var result = await _adminService.GetAllEmployeesAsync(pagination, cancellationToken);
        return Ok(result);
    }
}

public sealed record RejectRequest(string? Comments);
