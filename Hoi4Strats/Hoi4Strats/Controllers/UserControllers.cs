﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedProj;
using Blazored.LocalStorage;

namespace Hoi4Strats.Controllers;
[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration? _configuration;
    private readonly ITokenService _tokenService;
    private readonly ILocalStorageService _localStorage;

    public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration? configuration, ITokenService tokenService, ILocalStorageService localStorage)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _tokenService = tokenService;
        _localStorage = localStorage;
    }

    [Authorize (Roles = "Admin")]
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
            var identity = await _userManager.RemoveFromRolesAsync(user, currentRoles);
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

    [Obsolete] // Login goes via identity pages
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user.Id, roles.ToList());
            // locally store the token
            await _localStorage.SetItemAsync($"authToken_{user.Id}", token);
            Console.WriteLine($"Token written for:{user.Id}");

            return Ok(new { Token = token });
        }

        return Unauthorized("Invalid username or password");
    }

    //[HttpPost("LogOut")]
    //public async Task<IActionResult> Logout()
    //{
    //    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
    //    foreach (var cookie in Request.Cookies.Keys)
    //    {
    //        Response.Cookies.Delete(cookie);
    //    }
    //    return Ok(); // Säkerställ att detta returnerar 200 OK
    //}
}