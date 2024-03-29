// -----------------------------------------------------------------------
// <copyright file="BalanceData.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using RolXServer.Common.Util;
using RolXServer.Users.DataAccess;
using RolXServer.Users.Domain;

namespace RolXServer.Records.Domain.Detail.Balances;

/// <summary>
/// The data required to evaluate a balance.
/// </summary>
internal sealed class BalanceData
{
    /// <summary>
    /// Gets or sets the nominal work time per day.
    /// </summary>
    public TimeSpan NominalWorkTimePerDay { get; set; }

    /// <summary>
    /// Gets or sets the vacation days per year.
    /// </summary>
    public int DefaultVacationDaysPerYear { get; set; }

    /// <summary>
    /// Gets or sets the by-date.
    /// </summary>
    public DateOnly ByDate { get; set; }

    /// <summary>
    /// Gets or sets the user.
    /// </summary>
    public User User { get; set; } = new User();

    /// <summary>
    /// Gets or sets the actual work time in seconds.
    /// </summary>
    public long ActualWorkTimeSeconds { get; set; }

    /// <summary>
    /// Gets or sets the actual work time.
    /// </summary>
    public TimeSpan ActualWorkTime
    {
        get => TimeSpan.FromSeconds(this.ActualWorkTimeSeconds);
        set => this.ActualWorkTimeSeconds = (long)value.TotalSeconds;
    }

    /// <summary>
    /// Gets or sets the paid leave days.
    /// </summary>
    public List<PaidLeaveDay> PaidLeaveDays { get; set; } = new List<PaidLeaveDay>();

    /// <summary>
    /// Gets or sets the vacation days.
    /// </summary>
    public List<PaidLeaveDay> VacationDays { get; set; } = new List<PaidLeaveDay>();

    /// <summary>
    /// Gets or sets the consumed manual vacation in seconds.
    /// </summary>
    public long ManualVacationConsumedSeconds { get; set; }

    /// <summary>
    /// Gets or sets the planned manual vacation in seconds.
    /// </summary>
    public long ManualVacationPlannedSeconds { get; set; }

    private TimeSpan NominalWorkTime => this.User.NominalWorkTime(
        new DateRange(this.User.EntryDate, this.ByDate.AddDays(1)),
        this.NominalWorkTimePerDay);

    private TimeSpan PaidLeaveTime => this.PaidLeaveDays
        .SumOfPaidLeaveTime(this.User, this.NominalWorkTimePerDay);

    private TimeSpan Overtime => this.ActualWorkTime
        + this.BalanceCorrections.Sum(c => c.Overtime)
        + this.PaidLeaveTime
        - this.NominalWorkTime;

    private TimeSpan VacationBudget => this.User.VacationBudget(this.ByDate.Year, this.DefaultVacationDaysPerYear, this.NominalWorkTimePerDay)
            + this.BalanceCorrections.Sum(c => c.Vacation);

    private IEnumerable<UserBalanceCorrection> BalanceCorrections => this.User.BalanceCorrections
        .Where(c => c.Date <= this.ByDate);

    private TimeSpan VacationConsumed => this.VacationDays
        .Where(d => d.Date <= this.ByDate)
        .SumOfPaidLeaveTime(this.User, this.NominalWorkTimePerDay) + this.ManualVacationConsumed;

    private TimeSpan VacationPlanned => this.VacationDays
        .Where(d => d.Date > this.ByDate)
        .SumOfPaidLeaveTime(this.User, this.NominalWorkTimePerDay) + this.ManualVacationPlanned;

    private TimeSpan ManualVacationConsumed => TimeSpan.FromSeconds(this.ManualVacationConsumedSeconds);

    private TimeSpan ManualVacationPlanned => TimeSpan.FromSeconds(this.ManualVacationPlannedSeconds);

    /// <summary>
    /// Converts this instance into a balance.
    /// </summary>
    /// <returns>The balance.</returns>
    public Model.Balance ToBalance()
    {
        if (this.User.EntryDate > this.ByDate)
        {
            return new Model.Balance
            {
                ByDate = this.ByDate,
            };
        }

        var nominalWorkTime = this.NominalWorkTimePerDay * this.User.PartTimeFactorAt(this.ByDate);
        var vacationAvailable = this.VacationBudget - this.VacationConsumed;

        return new Model.Balance
        {
            ByDate = this.ByDate,
            Overtime = this.Overtime,
            VacationAvailable = vacationAvailable,
            VacationAvailableDays = nominalWorkTime > TimeSpan.Zero ? vacationAvailable / nominalWorkTime : null,
            VacationPlanned = this.VacationPlanned,
            VacationPlannedDays = nominalWorkTime > TimeSpan.Zero ? this.VacationPlanned / nominalWorkTime : null,
        };
    }
}
