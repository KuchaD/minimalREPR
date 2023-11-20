using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalREPR.Core.Common;
using MinimalREPR.Core.Endpoints;

namespace MinimalREPR.ExtensionMethods;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        var processors = typeof(EndpointRouteBuilderExtensions).Assembly.GetTypes()
            .Where(t => t.IsClass && t.IsAssignableTo(typeof(IRouteProcessor)))
            .Select(t => (IRouteProcessor)Activator.CreateInstance(t)!)
            .ToArray();
        var endpointMapper = app.ServiceProvider.GetRequiredService<EndpointsMapper>();
        endpointMapper.MapEndpoints(app, processors);
        
        return app;
    }
}