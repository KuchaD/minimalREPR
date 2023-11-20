using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using MinimalREPR.Core.Common;
using MinimalREPR.Core.Endpoints;

namespace MinimalREPR.ExtensionMethods;

public static class WebApplicationBuilderExtensions
{
    internal static TypeProcessorFactory TypeProcessorFactory { get; set; } = new();

    public static WebApplicationBuilder SetupMinimalREPR(this WebApplicationBuilder builder)
    {
        return builder.SetupMinimalREPR(Assembly.GetCallingAssembly());
    }

    public static WebApplicationBuilder SetupMinimalREPR(this WebApplicationBuilder builder, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);
        var processors = TypeProcessorFactory.CreateAll();
        var types = assembly.GetExportedTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpoint)) && t.IsClass && !t.IsAbstract);
        foreach (var type in types)
        {
            foreach (var processor in processors)
            {
                processor.Process(builder, type);
            }
        }

        foreach (var processor in processors)
        {
            processor.PostProcessAction(builder);
        }

        return builder;
    }
}

internal class TypeProcessorFactory
    {
        internal virtual IDependenciesProcessor[] CreateAll()
        {
            return GetType().Assembly.GetTypes()
                .Where(t => t.IsClass && t.IsAssignableTo(typeof(IDependenciesProcessor)))
                .Select(t => (IDependenciesProcessor)Activator.CreateInstance(t)!)
                .ToArray();
        }
    }
