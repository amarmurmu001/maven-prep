using System.Net;
using FluentValidation;

namespace LeaveManagementSystem.API.Filters;

public sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arg = context.Arguments.OfType<T>().FirstOrDefault();
        if (arg is null)
        {
            return Results.Problem(
                statusCode: (int)HttpStatusCode.BadRequest,
                title: "Validation Error",
                detail: $"Request body must not be null");
        }

        var result = await _validator.ValidateAsync(arg);
        if (!result.IsValid)
        {
            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            return Results.Problem(
                statusCode: (int)HttpStatusCode.BadRequest,
                title: "Validation Error",
                detail: "One or more validation errors occurred",
                extensions: new Dictionary<string, object?> { { "errors", errors } });
        }

        return await next(context);
    }
}
