// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace RolXServer.Records;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> instances.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services of the Records package.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>
    /// The service collection.
    /// </returns>
    public static IServiceCollection AddWorkRecord(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Settings>(configuration.GetSection("Records"));

        services.AddScoped<Domain.IBalanceService, Domain.Detail.BalanceService>();
        services.AddScoped<Domain.IEditLockService, Domain.Detail.EditLockService>();
        services.AddScoped<Domain.IRecordService, Domain.Detail.RecordService>();
        services.AddScoped<Domain.IYearInfoService, Domain.Detail.YearInfoService>();

        return services;
    }
}
