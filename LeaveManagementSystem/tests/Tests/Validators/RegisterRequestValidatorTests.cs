using FluentAssertions;
using Xunit;
using FluentValidation.TestHelper;
using LeaveManagementSystem.Application.DTOs.Auth;
using LeaveManagementSystem.Application.Validators;

namespace LeaveManagementSystem.Tests.Validators;

public sealed class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _sut = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var model = new RegisterRequest("John", "Doe", "", "Password123!", "Password123!");
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new RegisterRequest("John", "Doe", "notanemail", "Password123!", "Password123!");
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var model = new RegisterRequest("John", "Doe", "john@test.com", "Short1!", "Short1!");
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Have_Error_When_Passwords_Do_Not_Match()
    {
        var model = new RegisterRequest("John", "Doe", "john@test.com", "Password123!", "DifferentPass1!");
        var result = _sut.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = new RegisterRequest("John", "Doe", "john@test.com", "Password123!", "Password123!");
        var result = _sut.TestValidate(model);
        result.IsValid.Should().BeTrue();
    }
}
