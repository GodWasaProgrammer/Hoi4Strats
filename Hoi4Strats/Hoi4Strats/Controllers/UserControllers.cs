﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedProj;

namespace Hoi4Strats.Controllers;
[ApiController]
[Route("api/[controller]")]

public class UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration? configuration) : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IConfiguration? _configuration = configuration;

    [Authorize(Roles = "Admin")]
    [HttpGet("GetUsers")]
    public async Task<ActionResult<List<ApplicationUser>>> GetUsers()
    {
        return Ok(await _userManager.Users.ToListAsync());
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetRoles/{userId}")]
    public async Task<ActionResult<List<string>>> GetRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("UpdateRoles")]
    public async Task<IActionResult> UpdateRoles([FromBody] UpdateRolesDto updateRolesDto)
    {
        if (updateRolesDto != null && updateRolesDto.Roles != null && updateRolesDto.UserId != null)
        {
            var user = await _userManager.FindByIdAsync(updateRolesDto.UserId);
            if (user == null)
            {
                return NotFound();
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, updateRolesDto.Roles);
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAllRoles")]
    public async Task<ActionResult<List<string>>> GetAllRoles()
    {
        var roles = await _roleManager.Roles.Select(role => role.Name).ToListAsync();
        return Ok(roles);
    }
}