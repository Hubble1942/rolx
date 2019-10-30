﻿// -----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace RolXServer.Account
{
    /// <summary>
    /// The settings for the Account package.
    /// </summary>
    public sealed class Settings
    {
        /// <summary>
        /// Gets or sets the pattern for customer numbers.
        /// </summary>
        public string CustomerNumberPattern { get; set; } = @"C\d{3}";
    }
}