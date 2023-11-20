using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

namespace MinimalREPR.Core.Common;

public interface IDependenciesProcessor
{
    public void Process(WebApplicationBuilder builder, Type type);

    public void PostProcessAction(WebApplicationBuilder builder)
    {
        return;
    }
}