using System.Net.Http;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace MinimalREPR.Core.Endpoints;

public interface IValidation<TRequest>
{
    public static abstract ValueTask<ValidationResult> Validate(TRequest request, HttpContext context);
}