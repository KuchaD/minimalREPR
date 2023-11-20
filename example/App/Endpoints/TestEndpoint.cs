using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MinimalREPR.Core.Endpoints;

namespace App.Endpoints;

public record Test(string? tst);

[MapAttribute.Get("test")] 
public class TestEndpoint : IEndpoint, IValidation<Test>
{
    public static async ValueTask<ValidationResult> Validate(Test request, HttpContext context)
    {
        var x = new TestValidation();
        var xx =await x.ValidateAsync(request);

        return xx;
    }
    
    public static IResult Handle([AsParameters] Test test)
    {
        return Results.Ok(test.tst);
    }
}

public class TestValidation :  AbstractValidator<Test>
{
    public TestValidation()
    {
        RuleFor(x => x.tst).NotEmpty();
    }
}