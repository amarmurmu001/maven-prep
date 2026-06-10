using FluentAssertions;
using Xunit;
using FluentValidation.TestHelper;
using LeaveManagementSystem.Application.DTOs.Leave;
using LeaveManagementSystem.Application.Validators;

namespace LeaveManagementSystem.Tests.Validators;

public sealed class ApplyLeaveRequestValidatorTests
{
    private readonly ApplyLeaveRequestValidator _sut = new();

    [Fact]
    public void Should_Have_Error_When_LeaveType_Is_Invalid()
    {
        var model = new ApplyLeaveRequest("InvalidType", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), "Reason");
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LeaveType);
    }

    [Fact]
    public void Should_Have_Error_When_StartDate_Is_In_Past()
    {
        var model = new ApplyLeaveRequest("CasualLeave", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), "Reason");
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Should_Have_Error_When_EndDate_Is_Before_StartDate()
    {
        var model = new ApplyLeaveRequest("CasualLeave", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(3), "Reason");
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = new ApplyLeaveRequest("CasualLeave", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), "Family event");
        var result = _sut.TestValidate(model);
        result.IsValid.Should().BeTrue();
    }
}
