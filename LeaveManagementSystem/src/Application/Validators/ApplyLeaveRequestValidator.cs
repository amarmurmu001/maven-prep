using FluentValidation;
using LeaveManagementSystem.Application.DTOs.Leave;

namespace LeaveManagementSystem.Application.Validators;

public sealed class ApplyLeaveRequestValidator : AbstractValidator<ApplyLeaveRequest>
{
    public ApplyLeaveRequestValidator()
    {
        RuleFor(x => x.LeaveType)
            .NotEmpty().WithMessage("Leave type is required")
            .Must(BeValidLeaveType).WithMessage("Invalid leave type");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be on or after start date");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");
    }

    private static bool BeValidLeaveType(string leaveType)
    {
        return Enum.TryParse<Domain.Enums.LeaveType>(leaveType, out _);
    }
}
