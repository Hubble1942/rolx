// -----------------------------------------------------------------------
// <copyright file="VacationBudgetEvaluationTests.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using RolXServer.Common.Util;
using RolXServer.Users.DataAccess;

namespace RolXServer.Records.Domain.Detail.Balances;

public sealed class VacationBudgetEvaluationTests
{
    private const double DayTolerance = 1E-5; // < 1 s
    private static readonly TimeSpan NominalWorkTimePerDay = TimeSpan.FromHours(8);

    [Test]
    public void VacationDays_FullYear()
    {
        new DateRange(new DateOnly(2020, 1, 1), new DateOnly(2021, 1, 1))
            .VacationDays(24)
            .Should().BeApproximately(24, DayTolerance);
    }

    [Test]
    public void VacationDay_FirstHalfOfYear()
    {
        new DateRange(new DateOnly(2020, 1, 1), new DateOnly(2020, 7, 1))
            .VacationDays(24)
            .Should().BeApproximately(12, DayTolerance);
    }

    [Test]
    public void VacationDays_SecondHalfOfYear()
    {
        new DateRange(new DateOnly(2020, 7, 1), new DateOnly(2021, 1, 1))
            .VacationDays(24)
            .Should().BeApproximately(12, DayTolerance);
    }

    [Test]
    public void VacationDays_FullMonth()
    {
        new DateRange(new DateOnly(2020, 7, 14), new DateOnly(2020, 8, 14))
            .VacationDays(24)
            .Should().BeApproximately(2, DayTolerance);
    }

    [Test]
    public void VacationDays_FirstHalfOfMonth()
    {
        new DateRange(new DateOnly(2020, 4, 1), new DateOnly(2020, 4, 16))
            .VacationDays(24)
            .Should().BeApproximately(1, DayTolerance);
    }

    [Test]
    public void VacationDays_SecondHalfOfMonth()
    {
        new DateRange(new DateOnly(2020, 4, 16), new DateOnly(2020, 5, 1))
            .VacationDays(24)
            .Should().BeApproximately(1, DayTolerance);
    }

    [Test]
    public void VacationDays_TenYears()
    {
        new DateRange(new DateOnly(2020, 4, 16), new DateOnly(2030, 4, 16))
            .VacationDays(24)
            .Should().BeApproximately(240, DayTolerance);
    }

    [Test]
    public void VacationDays_OneYearCheck([Range(1, 366)] int middleDays)
    {
        var begin = new DateOnly(2020, 1, 1);
        var middle = begin.AddDays(middleDays);
        var end = new DateOnly(2021, 1, 1);

        var firstRange = new DateRange(begin, middle);
        var secondRange = new DateRange(middle, end);

        (firstRange.VacationDays(24) + secondRange.VacationDays(24))
            .Should().BeApproximately(24, DayTolerance);
    }

    [Test]
    public void VacationBudget_FullTime()
    {
        new User
        {
            EntryDate = new DateOnly(2020, 1, 1),
        }.VacationBudget(2020, 24, NominalWorkTimePerDay)
        .Should().Be(24 * NominalWorkTimePerDay);
    }

    [Test]
    public void VacationBudget_PartTime()
    {
        var user = new User
        {
            EntryDate = new DateOnly(2020, 1, 1),
        };

        user.PartTimeSettings.Add(new UserPartTimeSetting
        {
            StartDate = new DateOnly(2020, 1, 1),
            Factor = 0.5,
        });

        user.VacationBudget(2020, 24, NominalWorkTimePerDay)
            .Should().Be(12 * NominalWorkTimePerDay);
    }

    [Test]
    public void VacationBudget_ChangeToPartTime()
    {
        var user = new User
        {
            EntryDate = new DateOnly(2019, 1, 1),
        };

        user.PartTimeSettings.Add(new UserPartTimeSetting
        {
            StartDate = new DateOnly(2020, 1, 1),
            Factor = 0.5,
        });

        user.VacationBudget(2020, 24, NominalWorkTimePerDay)
            .Should().Be(36 * NominalWorkTimePerDay);
    }

    [Test]
    public void VacationBudget_OftenChangingPartTime()
    {
        var user = new User
        {
            EntryDate = new DateOnly(2020, 1, 1),
        };

        user.PartTimeSettings.Add(new UserPartTimeSetting
        {
            StartDate = new DateOnly(2020, 2, 1),
            Factor = 0.5,
        });

        user.PartTimeSettings.Add(new UserPartTimeSetting
        {
            StartDate = new DateOnly(2020, 3, 1),
            Factor = 1,
        });

        user.PartTimeSettings.Add(new UserPartTimeSetting
        {
            StartDate = new DateOnly(2020, 8, 1),
            Factor = 0.5,
        });

        user.PartTimeSettings.Add(new UserPartTimeSetting
        {
            StartDate = new DateOnly(2020, 11, 1),
            Factor = 1,
        });

        user.VacationBudget(2020, 24, NominalWorkTimePerDay)
            .Should().Be(20 * NominalWorkTimePerDay);
    }

    [Test]
    public void VacationBudget_MoreVacation()
    {
        int vacationDays = 40;

        var user = new User
        {
            EntryDate = new DateOnly(2020, 1, 1),
        };

        user.VacationDaysSettings.Add(new UserVacationDaysSetting
        {
            StartDate = new DateOnly(2020, 1, 1),
            VacationDaysPerYear = vacationDays,
        });

        user.VacationBudget(2020, 24, NominalWorkTimePerDay)
            .Should().Be(vacationDays * NominalWorkTimePerDay);
    }

    [Test]
    public void VacationBudget_ChangeToVacationDays()
    {
        int defaultVacationDays = 24;
        int newVacationDays = 40;

        var user = new User
        {
            EntryDate = new DateOnly(2019, 1, 1),
        };

        user.VacationDaysSettings.Add(new UserVacationDaysSetting
        {
            StartDate = new DateOnly(2020, 1, 1),
            VacationDaysPerYear = newVacationDays,
        });

        user.VacationBudget(2020, defaultVacationDays, NominalWorkTimePerDay)
            .Should().Be((newVacationDays + defaultVacationDays) * NominalWorkTimePerDay);
    }

    [Test]
    public void VacationBudget_OftenChangingVacationDays()
    {
        int defaultVacationDays = 24;
        int halfVacationDays = 12; // Kinda illegal though...

        var user = new User
        {
            EntryDate = new DateOnly(2020, 1, 1),
        };

        user.VacationDaysSettings.Add(new UserVacationDaysSetting
        {
            StartDate = new DateOnly(2020, 2, 1),
            VacationDaysPerYear = halfVacationDays,
        });

        user.VacationDaysSettings.Add(new UserVacationDaysSetting
        {
            StartDate = new DateOnly(2020, 3, 1),
            VacationDaysPerYear = defaultVacationDays,
        });

        user.VacationDaysSettings.Add(new UserVacationDaysSetting
        {
            StartDate = new DateOnly(2020, 8, 1),
            VacationDaysPerYear = halfVacationDays,
        });

        user.VacationDaysSettings.Add(new UserVacationDaysSetting
        {
            StartDate = new DateOnly(2020, 11, 1),
            VacationDaysPerYear = defaultVacationDays,
        });

        user.VacationBudget(2020, defaultVacationDays, NominalWorkTimePerDay)
            .Should().Be(20 * NominalWorkTimePerDay);
    }

    [Test]
    public void VacationBudget_ChangingPartTimeAndVacationDays()
    {
        int defaultVacationDays = 24;
        int halfVacationDays = 12;

        var user = new User
        {
            EntryDate = new DateOnly(2020, 1, 1),
        };

        user.PartTimeSettings.Add(new UserPartTimeSetting
        {
            StartDate = new DateOnly(2020, 2, 1),
            Factor = 0.5,
        });

        user.VacationDaysSettings.Add(new UserVacationDaysSetting
        {
            StartDate = new DateOnly(2020, 3, 1),
            VacationDaysPerYear = halfVacationDays,
        });

        user.PartTimeSettings.Add(new UserPartTimeSetting
        {
            StartDate = new DateOnly(2020, 8, 1),
            Factor = 1,
        });

        user.VacationDaysSettings.Add(new UserVacationDaysSetting
        {
            StartDate = new DateOnly(2020, 11, 1),
            VacationDaysPerYear = defaultVacationDays,
        });

        user.VacationBudget(2020, defaultVacationDays, NominalWorkTimePerDay)
            .Should().Be(12.5 * NominalWorkTimePerDay);
    }
}
