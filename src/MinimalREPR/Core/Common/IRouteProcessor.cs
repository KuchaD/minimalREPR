using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace MinimalREPR.Core.Common;

public interface IRouteProcessor
{
    void Process(RouteHandlerBuilder builder, MethodInfo handlerInfo, string route, Type type);
}
