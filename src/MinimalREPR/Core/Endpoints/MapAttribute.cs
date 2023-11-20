using System;

namespace MinimalREPR.Core.Endpoints;

[AttributeUsage(AttributeTargets.Class)]
public class MapAttribute : Attribute
{
    private static readonly string[] Get = { "GET" };

    private static readonly string[] Post = { "POST" };

    private static readonly string[] Put = { "PUT" };

    private static readonly string[] Patch = { "PATCH" };

    private static readonly string[] Delete = { "DELETE" };

    public string[] Methods { get; init; }

    public string Route { get; init; }

    public MapAttribute(string method, string route) : this(new[] { method }, route)
    {
    }
    
    public MapAttribute(string[] methods, string route)
    {
        Methods = methods;
        Route = route;
    }

    public sealed class GetAttribute : MapAttribute
    {
        public GetAttribute(string route) : base(Get, route)
        {
        }
    }

    public sealed class PostAttribute : MapAttribute
    {
        public PostAttribute(string route) : base(Post, route)
        {
        }
    }
    
    public sealed class PutAttribute : MapAttribute
    {
        public PutAttribute(string route) : base(Put, route)
        {
        }
    }
    
    public sealed class PatchAttribute : MapAttribute
    {
        public PatchAttribute(string route) : base(Patch, route)
        {
        }
    }
    
    public sealed class DeleteAttribute : MapAttribute
    {
        public DeleteAttribute(string route) : base(Delete, route)
        {
        }
    }
}