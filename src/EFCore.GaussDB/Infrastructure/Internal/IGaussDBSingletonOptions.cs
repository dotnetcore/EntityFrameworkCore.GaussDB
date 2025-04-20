﻿namespace GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

/// <summary>
///     Represents options for GaussDB that can only be set at the <see cref="IServiceProvider" /> singleton level.
/// </summary>
public interface IGaussDBSingletonOptions : ISingletonOptions
{
    /// <summary>
    ///     The backend version to target.
    /// </summary>
    Version PostgresVersion { get; }

    /// <summary>
    ///     Whether the user has explicitly set the backend version to target.
    /// </summary>
    bool IsPostgresVersionSet { get; }

    /// <summary>
    ///     Whether to target Redshift.
    /// </summary>
    bool UseRedshift { get; }

    /// <summary>
    ///     Whether reverse null ordering is enabled.
    /// </summary>
    bool ReverseNullOrderingEnabled { get; }

    /// <summary>
    ///     The collection of enum mappings.
    /// </summary>
    IReadOnlyList<EnumDefinition> EnumDefinitions { get; }

    /// <summary>
    ///     The collection of range mappings.
    /// </summary>
    IReadOnlyList<UserRangeDefinition> UserRangeDefinitions { get; }
}
