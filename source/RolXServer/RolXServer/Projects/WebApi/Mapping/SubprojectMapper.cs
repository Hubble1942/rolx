// -----------------------------------------------------------------------
// <copyright file="SubprojectMapper.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using RolXServer.Projects.Domain;
using RolXServer.Users.Domain;

namespace RolXServer.Projects.WebApi.Mapping;

/// <summary>
/// Maps subprojects from / to resource.
/// </summary>
internal static class SubprojectMapper
{
    /// <summary>
    /// Converts to resource.
    /// </summary>
    /// <param name="domain">The domain.</param>
    /// <param name="actualSums">The actual sums.</param>
    /// <returns>The resource.</returns>
    public static Resource.Subproject ToResource(this DataAccess.Subproject domain, IDictionary<int, TimeSpan>? actualSums = null)
        => new Resource.Subproject(
            Id: domain.Id,
            FullNumber: domain.FullNumber(),
            FullName: domain.FullName(),
            CustomerName: domain.CustomerName,
            ProjectNumber: domain.ProjectNumber,
            ProjectName: domain.ProjectName,
            Number: domain.Number,
            Name: domain.Name,
            ManagerId: domain.ManagerId,
            ManagerName: domain.Manager?.FullName() ?? "vakant",
            IsClosed: domain.IsClosed(),
            Activities: domain.Activities.Select(ph => ph.ToResource(actualSums)).ToImmutableList());

    /// <summary>
    /// Converts to shallow resource.
    /// </summary>
    /// <param name="domain">The domain.</param>
    /// <returns>The resource.</returns>
    public static Resource.SubprojectShallow ToShallowResource(this DataAccess.Subproject domain)
        => new Resource.SubprojectShallow(
            Id: domain.Id,
            FullNumber: domain.FullNumber(),
            CustomerName: domain.CustomerName,
            ProjectName: domain.ProjectName,
            Name: domain.Name,
            ManagerName: domain.Manager?.FullName() ?? "vakant",
            IsClosed: domain.IsClosed());

    /// <summary>
    /// Converts to domain.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <returns>
    /// The domain.
    /// </returns>
    public static DataAccess.Subproject ToDomain(this Resource.Subproject resource)
    {
        var domain = new DataAccess.Subproject
        {
            Id = resource.Id,
            Number = resource.Number,
            Name = resource.Name,
            ProjectNumber = resource.ProjectNumber,
            ProjectName = resource.ProjectName,
            CustomerName = resource.CustomerName,
            ManagerId = resource.ManagerId,
        };

        domain.Activities = resource.Activities
                .Select(ph => ph.ToDomain(domain))
                .ToList();

        return domain;
    }
}
