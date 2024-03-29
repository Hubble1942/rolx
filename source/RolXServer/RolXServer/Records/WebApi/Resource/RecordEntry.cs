// -----------------------------------------------------------------------
// <copyright file="RecordEntry.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace RolXServer.Records.WebApi.Resource;

/// <summary>
/// An entry in a <see cref="Record"/>.
/// </summary>
public sealed class RecordEntry
{
    /// <summary>
    /// Gets or sets the activity identifier.
    /// </summary>
    public int ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the full number of the activity.
    /// </summary>
    public string FullActivityNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration in seconds.
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// Gets or sets the begin time in seconds since midnight.
    /// </summary>
    public int? Begin { get; set; }

    /// <summary>
    /// Gets or sets the pause duration in seconds.
    /// </summary>
    public int? Pause { get; set; }

    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    public string? Comment { get; set; }
}
