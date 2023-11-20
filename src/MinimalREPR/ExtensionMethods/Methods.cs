using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;

namespace MinimalREPR.ExtensionMethods;

public static class Methods
{
    public static string GetFullName(this MethodInfo methodInfo)
    {
        return $"{methodInfo.DeclaringType}.{methodInfo.Name}";
    }

    public static bool IsEmpty(this string input)
    {
        return string.IsNullOrWhiteSpace(input);
    }

    public static bool TryGetGenericInterfaceType(this Type type, Type unboundGenericType, [NotNullWhen(true)] out Type? interfaceType)
    {
        var implementedInterfaceTypes = type.GetInterfaces();
        foreach (var implementedInterfaceType in implementedInterfaceTypes)
        {
            if (implementedInterfaceType.IsGenericType)
            {
                var genericTypeDefinition = implementedInterfaceType.GetGenericTypeDefinition();
                if (genericTypeDefinition == unboundGenericType)
                {
                    interfaceType = implementedInterfaceType;

                    return true;
                }
            }
        }
        interfaceType = null;

        return false;
    }
    
    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        var baseType = givenType.BaseType;
        return baseType != null && IsAssignableToGenericType(baseType, genericType);
    }
}