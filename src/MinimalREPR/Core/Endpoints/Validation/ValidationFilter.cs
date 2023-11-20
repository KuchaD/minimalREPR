using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace MinimalREPR.Core.Endpoints.Validation;

public class ValidationFilter<T, TEndpoint> : IEndpointFilter where T : class
{
    private const string MethodName = "Validate";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T? argToValidate = context.GetArgument<T>(0);

        var method = typeof(TEndpoint).GetMethod(MethodName, BindingFlags.Public | BindingFlags.Static);

        if (method is null)
        {
            throw new InvalidOperationException($"Method {MethodName} missing in endpoint {typeof(TEndpoint)}");
        }

        var validationResult =
            await ((ValueTask<ValidationResult>)method?.Invoke(null,
                new object[] { argToValidate, context.HttpContext })!);
        
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary(),
                statusCode: (int)HttpStatusCode.UnprocessableEntity);
        }


        // Otherwise invoke the next filter in the pipeline
        return await next.Invoke(context);
    }
}