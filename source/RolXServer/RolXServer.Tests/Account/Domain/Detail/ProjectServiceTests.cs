﻿// -----------------------------------------------------------------------
// <copyright file="ProjectServiceTests.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RolXServer.Account.DataAccess;

namespace RolXServer.Account.Domain.Detail
{
    public sealed class ProjectServiceTests
    {
        private static Project DummyProject => new Project
        {
            Id = 1,
            Number = "1",
            Name = "One",
            Phases = new List<Phase>
            {
                new Phase
                {
                    Number = 1,
                    Name = "One",
                },
                new Phase
                {
                    Number = 2,
                    Name = "Two",
                },
            },
        };

        [Test]
        public async Task Update_ExistingPhaseChanged()
        {
            var project = DummyProject;
            var contextFactory = InMemory.ContextFactory(project);

            using (var context = contextFactory())
            {
                var sut = new ProjectService(context);

                project.Phases[0].Name = "Changed";
                await sut.Update(project);
            }

            using (var context = contextFactory())
            {
                context.Projects
                    .Include(p => p.Phases)
                    .Single(p => p.Id == 1)
                    .Phases.Single(ph => ph.Number == 1)
                    .Name.Should().Be("Changed");
            }
        }

        [Test]
        public async Task Update_ExistingPhaseRemoved()
        {
            var project = DummyProject;
            var contextFactory = InMemory.ContextFactory(project);

            using (var context = contextFactory())
            {
                var sut = new ProjectService(context);

                project.Phases.RemoveAt(0);
                await sut.Update(project);
            }

            using (var context = contextFactory())
            {
                context.Projects
                    .Include(p => p.Phases)
                    .Single(p => p.Id == 1)
                    .Phases.Count.Should().Be(1);
            }
        }

        [Test]
        public async Task Update_NewPhaseAdded()
        {
            var project = DummyProject;
            var contextFactory = InMemory.ContextFactory(project);

            using (var context = contextFactory())
            {
                var sut = new ProjectService(context);

                project.Phases.Add(new Phase
                {
                    Number = 3,
                    Name = "Three",
                    Project = project,
                });
                await sut.Update(project);
            }

            using (var context = contextFactory())
            {
                context.Projects
                    .Include(p => p.Phases)
                    .Single(p => p.Id == 1)
                    .Phases.Count.Should().Be(3);
            }
        }
    }
}