using System;

namespace Shared.Contracts.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PackageTypeAttribute : Attribute
{
    public string Name { get; }
    public PackageTypeAttribute(string name) => Name = name;
}
