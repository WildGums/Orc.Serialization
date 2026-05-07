namespace Orc.Serialization.Json;

using System;
using System.Collections.Generic;

/// <summary>
/// Default serializer binder implementation that allows only explicitly configured types.
/// </summary>
public class AllowedTypesSerializerBinder : ISerializerBinder
{
    private readonly HashSet<Type> _allowedTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="AllowedTypesSerializerBinder"/> class.
    /// </summary>
    /// <param name="allowedTypes">The allowed types.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="allowedTypes"/> is <c>null</c>.</exception>
    public AllowedTypesSerializerBinder(IEnumerable<Type> allowedTypes)
    {
        ArgumentNullException.ThrowIfNull(allowedTypes);

        _allowedTypes = new HashSet<Type>(allowedTypes);
        IsTypeAllowed = type => _allowedTypes.Contains(type);
    }

    /// <summary>
    /// Gets the function that evaluates whether a type is allowed.
    /// </summary>
    public Func<Type, bool> IsTypeAllowed { get; }
}
