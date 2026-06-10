using AutoMapper;
using Xunit;
using FluentAssertions;
using LeaveManagementSystem.Application.DTOs.Common;
using LeaveManagementSystem.Application.DTOs.Leave;
using LeaveManagementSystem.Application.Mappings;
using LeaveManagementSystem.Application.Services;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;
using LeaveManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace LeaveManagementSystem.Tests.Services;

public sealed class LeaveServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<LeaveService>> _loggerMock;
    private readonly LeaveService _sut;

    public LeaveServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<LeaveService>>();
        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        _sut = new LeaveService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task ApplyLeaveAsync_WithValidRequest_ReturnsSuccess()
    {
        var employeeId = Guid.NewGuid();
        var request = new ApplyLeaveRequest("AnnualLeave", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(12), "Vacation");
        var leaveRepoMock = new Mock<IRepository<LeaveRequest>>();

        leaveRepoMock.Setup(x => x.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<LeaveRequest, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        leaveRepoMock.Setup(x => x.AddAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaveRequest());

        _unitOfWorkMock.Setup(x => x.Repository<LeaveRequest>()).Returns(leaveRepoMock.Object);

        var result = await _sut.ApplyLeaveAsync(employeeId, request);

        result.Success.Should().BeTrue();
        result.Message.Should().Be("Leave application submitted successfully");
    }

    [Fact]
    public async Task ApplyLeaveAsync_WithOverlappingDates_ReturnsFailure()
    {
        var employeeId = Guid.NewGuid();
        var request = new ApplyLeaveRequest("AnnualLeave", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(12), "Vacation");
        var leaveRepoMock = new Mock<IRepository<LeaveRequest>>();

        leaveRepoMock.Setup(x => x.FindAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<LeaveRequest, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([new LeaveRequest()]);

        _unitOfWorkMock.Setup(x => x.Repository<LeaveRequest>()).Returns(leaveRepoMock.Object);

        var result = await _sut.ApplyLeaveAsync(employeeId, request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("You already have a leave request for this period");
    }

    [Fact]
    public async Task CancelLeaveAsync_WithValidRequest_ReturnsSuccess()
    {
        var employeeId = Guid.NewGuid();
        var leaveId = Guid.NewGuid();
        var leave = new LeaveRequest
        {
            Id = leaveId,
            EmployeeId = employeeId,
            Status = LeaveStatus.Pending
        };

        var leaveRepoMock = new Mock<IRepository<LeaveRequest>>();
        leaveRepoMock.Setup(x => x.GetByIdAsync(leaveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(leave);

        _unitOfWorkMock.Setup(x => x.Repository<LeaveRequest>()).Returns(leaveRepoMock.Object);

        var result = await _sut.CancelLeaveAsync(employeeId, leaveId);

        result.Success.Should().BeTrue();
        result.Message.Should().Be("Leave request cancelled successfully");
    }

    [Fact]
    public async Task CancelLeaveAsync_WithApprovedLeave_ReturnsFailure()
    {
        var employeeId = Guid.NewGuid();
        var leaveId = Guid.NewGuid();
        var leave = new LeaveRequest
        {
            Id = leaveId,
            EmployeeId = employeeId,
            Status = LeaveStatus.Approved
        };

        var leaveRepoMock = new Mock<IRepository<LeaveRequest>>();
        leaveRepoMock.Setup(x => x.GetByIdAsync(leaveId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(leave);

        _unitOfWorkMock.Setup(x => x.Repository<LeaveRequest>()).Returns(leaveRepoMock.Object);

        var result = await _sut.CancelLeaveAsync(employeeId, leaveId);

        result.Success.Should().BeFalse();
    }
}
