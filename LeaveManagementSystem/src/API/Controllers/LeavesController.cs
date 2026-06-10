using LeaveManagementSystem.API.Extensions;
using LeaveManagementSystem.Application.DTOs.Common;
using LeaveManagementSystem.Application.DTOs.Leave;
using LeaveManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers;

[ApiController]
[Route("api/leaves")]
[Authorize(Roles = "Employee")]
public sealed class LeavesController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeavesController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpGet("my-leaves")]
    public async Task<IActionResult> GetMyLeaves(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        var pagination = new PaginationRequest(page, pageSize);
        var result = await _leaveService.GetMyLeavesAsync(userId, pagination, cancellationToken);
        return Ok(result);
    }

    [HttpPost("apply")]
    public async Task<IActionResult> Apply(
        [FromBody] ApplyLeaveRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _leaveService.ApplyLeaveAsync(userId, request, cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("cancel/{id:guid}")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _leaveService.CancelLeaveAsync(userId, id, cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
