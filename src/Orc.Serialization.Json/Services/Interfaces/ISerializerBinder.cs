namespace Orc.Serialization.Json;

using System;

/// <summary>
/// Defines a serializer binder that determines whether a type is allowed to be serialized or deserialized.
/// </summary>
public interface ISerializerBinder
{
    /// <summary>
    /// Gets the function that evaluates whether a type is allowed.
    /// </summary>
    Func<Type, bool> IsTypeAllowed { get; }
}
