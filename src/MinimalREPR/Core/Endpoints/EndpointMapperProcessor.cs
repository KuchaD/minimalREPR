using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MinimalREPR.Core.Common;

namespace MinimalREPR.Core.Endpoints;

public class EndpointMapperProcessor : IDependenciesProcessor
{
    internal EndpointsMapper EndpointMapper { get; } = new();

    public void Process(WebApplicationBuilder builder, Type type)
    {
        if (type.IsAssignableTo(typeof(IEndpoint)))
        {
            EndpointMapper.Add(type);
        }
    }
    
    public void PostProcessAction(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(EndpointMapper);
    }
}