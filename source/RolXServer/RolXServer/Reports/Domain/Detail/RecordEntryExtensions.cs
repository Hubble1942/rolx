// -----------------------------------------------------------------------
// <copyright file="RecordEntryExtensions.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using RolXServer.Common.Util;
using RolXServer.Records.Domain.Model;
using RolXServer.Reports.Domain.Model;

namespace RolXServer.Reports.Domain.Detail;

/// <summary>
/// Extension methods for <see cref="RecordEntry"/> instances.
/// </summary>
public static class RecordEntryExtensions
{
    /// <summary>
    /// Converts the specified record entries to <see cref="WorkItemGroup"/> instances.
    /// </summary>
    /// <param name="recordEntries">The record entries.</param>
    /// <returns>The groups.</returns>
    public static IEnumerable<WorkItemGroup> ToWorkItemGroups(this IEnumerable<RecordEntry> recordEntries)
        => recordEntries
        .GroupBy(entry => entry.Activity!.Subproject!.FullName)
        .Select(group => new WorkItemGroup(
            group.Key,
            group.ToWorkItems().ToImmutableList()))
        .OrderBy(workItemGroup => workItemGroup.Name);

    /// <summary>
    /// Converts the specified record entries to <see cref="WorkItem"/> instances.
    /// </summary>
    /// <param name="recordEntries">The record entries.</param>
    /// <returns>The items.</returns>
    public static IEnumerable<WorkItem> ToWorkItems(this IEnumerable<RecordEntry> recordEntries)
        => recordEntries
        .GroupBy(entry => entry.Activity!.NumberedName)
        .Select(group => new WorkItem(
            group.Key,
            group.Sum(entry => entry.Duration)))
        .OrderBy(workItem => workItem.Name);
}
