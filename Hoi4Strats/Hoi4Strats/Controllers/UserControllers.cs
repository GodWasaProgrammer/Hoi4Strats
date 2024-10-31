namespace Hoi4Strats.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedProj;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//[Authorize]
[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("GetUsers")]
    public async Task<ActionResult<List<ApplicationUser>>> GetUsers()
    {
        return Ok(await _userManager.Users.ToListAsync());
    }

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

    [HttpGet("GetAllRoles")]
    public async Task<ActionResult<List<string>>> GetAllRoles()
    {
        var roles = await _roleManager.Roles.Select(role => role.Name).ToListAsync();
        return Ok(roles);
    }
}