using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MinimalREPR.Core.Common;
using MinimalREPR.ExtensionMethods;

namespace MinimalREPR.Core.Endpoints;

internal class EndpointsMapper
{
    internal List<Type> EndpointTypes { get; set; } = new();

    internal Dictionary<(string Method, string Route), Type> MappedRoutes { get; set; } = new();

    public void Add(Type type)
    {
        EndpointTypes.Add(type);
    }

    public virtual void MapEndpoints(IEndpointRouteBuilder app, IRouteProcessor[] processors)
    {
        foreach (var endpointType in EndpointTypes)
        {
            var handlerInfo = GetHandlerInfo(endpointType);
            var (methods, route) = GetMethodsAndRoute(endpointType);
            var typeArgs = handlerInfo.GetParameters()
                .Select(p => p.ParameterType)
                .Append(handlerInfo.ReturnType)
                .ToArray();
            var delegateType = Expression.GetDelegateType(typeArgs);
            var handler = Delegate.CreateDelegate(delegateType, handlerInfo);
            var builder = app.MapMethods(route, methods, handler);
            foreach (var processor in processors)
            {
                processor.Process(builder, handlerInfo, route, endpointType);
            }
        }
    }

    private static MethodInfo GetHandlerInfo(Type endpointType)
    {
        MethodInfo? handlerInfo;
        try
        {
            handlerInfo = endpointType.GetMethod("Handle", BindingFlags.Public | BindingFlags.Static);
        }
        catch (AmbiguousMatchException)
        {
            throw new InvalidOperationException($"{endpointType} has more than one public static Handle method.");
        }

        if (handlerInfo == null)
        {
            throw new InvalidOperationException($"{endpointType} has no public static Handle method.");
        }

        return handlerInfo;
    }

    private (string[] Methods, string Route) GetMethodsAndRoute(Type endpointType)
    {
        var mapAttribute = GetMapAttribute(endpointType);
        if (mapAttribute.Route.IsEmpty())
        {
            throw new InvalidOperationException($"{endpointType.FullName} has an empty route.");
        }

        if (mapAttribute.Methods.Any(m => m.IsEmpty()))
        {
            throw new InvalidOperationException($"{endpointType.FullName} has an empty HTTP method.");
        }

        foreach (var method in mapAttribute.Methods)
        {
            var key = (method, mapAttribute.Route);
            if (MappedRoutes.TryGetValue(key, out var existingEndpointType))
            {
                throw new InvalidOperationException(
                    $"{method} {mapAttribute.Route} is handled by both {endpointType.DeclaringType} and {existingEndpointType}.");
            }

            MappedRoutes[key] = endpointType.DeclaringType!;
        }

        return (mapAttribute.Methods, mapAttribute.Route);
    }

    private static MapAttribute GetMapAttribute(Type endpointType)
    {
        var mapAttributes = endpointType.GetCustomAttributes<MapAttribute>().ToList();
        if (mapAttributes.Count == 0)
        {
            throw new InvalidOperationException($"{endpointType.FullName} has no HTTP method and route attribute.");
        }

        if (mapAttributes.Count > 1)
        {
            throw new InvalidOperationException(
                $"{endpointType.FullName} has multiple HTTP method and route attributes.");
        }

        return mapAttributes[0];
    }
}