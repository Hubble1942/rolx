// -----------------------------------------------------------------------
// <copyright file="SubprojectController.cs" company="Christian Ewald">
// Copyright (c) Christian Ewald. All rights reserved.
// Licensed under the MIT license.
// See LICENSE.md in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RolXServer.Projects.Domain;
using RolXServer.Projects.WebApi.Mapping;
using RolXServer.Projects.WebApi.Resource;

namespace RolXServer.Projects.WebApi;

/// <summary>
/// Controller for subproject resources.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "ActiveUser")]
public sealed class SubprojectController : ControllerBase
{
    private readonly ISubprojectService subprojectService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubprojectController" /> class.
    /// </summary>
    /// <param name="subprojectService">The subproject service.</param>
    public SubprojectController(ISubprojectService subprojectService)
    {
        this.subprojectService = subprojectService;
    }

    /// <summary>
    /// Gets all subprojects.
    /// </summary>
    /// <returns>All subprojects.</returns>
    [HttpGet]
    public async Task<IEnumerable<SubprojectShallow>> GetAll()
    {
        return (await this.subprojectService.GetAll()).Select(p => p.ToShallowResource());
    }

    /// <summary>
    /// Gets the subproject with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>
    /// The requested subproject.
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Subproject>> GetById(int id)
    {
        var domain = await this.subprojectService.GetById(id);
        if (domain is null)
        {
            return this.NotFound();
        }

        return domain.ToResource();
    }

    /// <summary>
    /// Creates the specified new subproject.
    /// </summary>
    /// <param name="subproject">The subproject.</param>
    /// <returns>
    /// The created subproject.
    /// </returns>
    [HttpPost]
    [Authorize(Roles = "Administrator, Backoffice, Supervisor")]
    public async Task<ActionResult<Subproject>> Create(Subproject subproject)
    {
        var domain = subproject.ToDomain();
        return (await this.subprojectService.Add(domain)).ToResource();
    }

    /// <summary>
    /// Updates the subproject with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="subproject">The subproject.</param>
    /// <returns>
    /// No content.
    /// </returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator, Backoffice, Supervisor")]
    public async Task<IActionResult> Update(int id, Subproject subproject)
    {
        if (id != subproject.Id)
        {
            return this.BadRequest();
        }

        await this.subprojectService.Update(subproject.ToDomain());

        return this.NoContent();
    }
}
