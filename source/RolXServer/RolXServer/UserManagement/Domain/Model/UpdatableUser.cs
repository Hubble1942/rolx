﻿// -----------------------------------------------------------------------
// <copyright file="UpdatableUser.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;

using RolXServer.Auth;

namespace RolXServer.UserManagement.Domain.Model
{
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
    }
}
