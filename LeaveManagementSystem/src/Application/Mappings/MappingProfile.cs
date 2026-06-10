using AutoMapper;
using LeaveManagementSystem.Application.DTOs.Admin;
using LeaveManagementSystem.Application.DTOs.Auth;
using LeaveManagementSystem.Application.DTOs.Leave;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Application.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => $"{s.Employee.FirstName} {s.Employee.LastName}"))
            .ForMember(d => d.LeaveType, o => o.MapFrom(s => s.LeaveType.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.DurationDays, o => o.MapFrom(s => (s.EndDate - s.StartDate).Days + 1))
            .ForMember(d => d.ReviewedByName, o => o.MapFrom(s => s.ApprovedBy != null ? $"{s.ApprovedBy.FirstName} {s.ApprovedBy.LastName}" : null));

        CreateMap<LeaveRequest, RecentLeaveDto>()
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => $"{s.Employee.FirstName} {s.Employee.LastName}"))
            .ForMember(d => d.LeaveType, o => o.MapFrom(s => s.LeaveType.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<User, EmployeeDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()))
            .ForMember(d => d.TotalLeaves, o => o.Ignore())
            .ForMember(d => d.PendingLeaves, o => o.Ignore())
            .ForMember(d => d.ApprovedLeaves, o => o.Ignore());
    }
}
