// -----------------------------------------------------------------------
// <copyright file="UpdatableUser.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using RolXServer.Users.DataAccess;

namespace RolXServer.Users.Domain.Model;

/// <summary>
/// The updatable part of a user.
/// </summary>
public class UpdatableUser
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the role.
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// Gets or sets the entry date.
    /// </summary>
    public DateOnly EntryDate { get; set; }

    /// <summary>
    /// Gets or sets the date the user has left (exclusive).
    /// </summary>
    /// <remarks>
    /// This marks the first day the user no longer works with us.
    /// </remarks>
    public DateOnly? LeftDate { get; set; }

    /// <summary>
    /// Gets or sets the part time settings for the user.
    /// </summary>
    public IImmutableList<UserPartTimeSetting> PartTimeSettings { get; set; } = ImmutableList<UserPartTimeSetting>.Empty;

    /// <summary>
    /// Gets or sets the vacation days settings for the user.
    /// </summary>
    public IImmutableList<UserVacationDaysSetting> VacationDaysSettings { get; set; } = ImmutableList<UserVacationDaysSetting>.Empty;

    /// <summary>
    /// Gets or sets the vacation days settings for the user.
    /// </summary>
    public IImmutableList<UserBalanceCorrection> BalanceCorrections { get; set; } = ImmutableList<UserBalanceCorrection>.Empty;
}
