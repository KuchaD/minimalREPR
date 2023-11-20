using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MinimalREPR.Core.Common;
using MinimalREPR.ExtensionMethods;

namespace MinimalREPR.Core.Endpoints.Validation;

public class ValidationProcessor : IRouteProcessor
{
    public void Process(RouteHandlerBuilder builder, MethodInfo handlerInfo, string route, Type type)
    {
        var validationType = type.IsAssignableToGenericType(typeof(IValidation<>));
        if (validationType)
        {
            var genericType = (
                from iType in type.GetInterfaces()
                where iType.IsGenericType
                      && iType.GetGenericTypeDefinition() == typeof(IValidation<>)
                select iType.GetGenericArguments()[0]).First();
            
            builder.AddEndpointFilterFactory((context, next) =>
            {
                if (context.MethodInfo.GetParameters().Any(p => p.ParameterType == genericType))
                {
                    var filterType = typeof(ValidationFilter<,>);
                    Type[] typeArgs = { genericType, type };
                    var instanceType = filterType.MakeGenericType(typeArgs);
                    var filter = (IEndpointFilter) Activator.CreateInstance(instanceType);
                    
                    return invocationContext => filter!.InvokeAsync(invocationContext, next);
                }

                // pass-thru filter
                return invocationContext => next(invocationContext);
            });
        }
    }
}